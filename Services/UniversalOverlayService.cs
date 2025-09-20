using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services.Interfaces;
using Avalonia.Threading;

namespace MTM_WIP_Application_Avalonia.Services.Universal
{
    /// <summary>
    /// Universal Overlay Service implementation providing centralized overlay management for the MTM application.
    /// Handles overlay lifecycle, registration, memory pooling, and cross-platform compatibility.
    /// </summary>
    public class UniversalOverlayService : IUniversalOverlayService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UniversalOverlayService> _logger;
        private readonly ConcurrentDictionary<string, OverlayRegistration> _registeredOverlays;
        private readonly ConcurrentDictionary<string, ActiveOverlay> _activeOverlays;
        private readonly object _eventLock = new();

        public UniversalOverlayService(IServiceProvider serviceProvider, ILogger<UniversalOverlayService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _registeredOverlays = new ConcurrentDictionary<string, OverlayRegistration>();
            _activeOverlays = new ConcurrentDictionary<string, ActiveOverlay>();

            _logger.LogInformation("Universal Overlay Service initialized");
        }

        #region Events

        public event EventHandler<OverlayEventArgs>? OverlayShowing;
        public event EventHandler<OverlayEventArgs>? OverlayShown;
        public event EventHandler<OverlayEventArgs>? OverlayHiding;
        public event EventHandler<OverlayEventArgs>? OverlayHidden;

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public async Task<OverlayResult<T>> ShowOverlayAsync<T>(T request) where T : class
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var overlayId = Guid.NewGuid().ToString("N")[..8];
            var requestType = typeof(T);
            
            try
            {
                _logger.LogDebug("Showing overlay for type {OverlayType} with ID {OverlayId}", requestType.Name, overlayId);

                // Fire showing event
                var showingArgs = new OverlayEventArgs(overlayId, requestType, canCancel: true);
                await FireEventSafelyAsync(() => OverlayShowing?.Invoke(this, showingArgs));

                if (showingArgs.Cancel)
                {
                    _logger.LogDebug("Overlay {OverlayId} cancelled during showing event", overlayId);
                    return OverlayResult<T>.Cancelled(overlayId);
                }

                // Get or create overlay registration
                var registration = GetOrCreateRegistration<T>();
                if (registration == null)
                {
                    var errorMessage = $"No overlay registered for type {requestType.Name}";
                    _logger.LogError(errorMessage);
                    return OverlayResult<T>.Failed(errorMessage, overlayId);
                }

                // Create and show overlay
                var result = await ShowOverlayInternalAsync<T>(overlayId, request, registration);

                // Fire shown event
                var shownArgs = new OverlayEventArgs(overlayId, requestType);
                await FireEventSafelyAsync(() => OverlayShown?.Invoke(this, shownArgs));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing overlay {OverlayId} for type {OverlayType}", overlayId, requestType.Name);
                return OverlayResult<T>.Failed(ex.Message, overlayId);
            }
        }

        /// <inheritdoc />
        public async Task<bool> HideOverlayAsync(string overlayId)
        {
            if (string.IsNullOrEmpty(overlayId))
                throw new ArgumentException("Overlay ID cannot be null or empty", nameof(overlayId));

            try
            {
                if (!_activeOverlays.TryGetValue(overlayId, out var activeOverlay))
                {
                    _logger.LogWarning("Attempted to hide non-existent overlay {OverlayId}", overlayId);
                    return false;
                }

                _logger.LogDebug("Hiding overlay {OverlayId} of type {OverlayType}", overlayId, activeOverlay.OverlayType.Name);

                // Fire hiding event
                var hidingArgs = new OverlayEventArgs(overlayId, activeOverlay.OverlayType, canCancel: true);
                await FireEventSafelyAsync(() => OverlayHiding?.Invoke(this, hidingArgs));

                if (hidingArgs.Cancel)
                {
                    _logger.LogDebug("Overlay {OverlayId} hide cancelled during hiding event", overlayId);
                    return false;
                }

                // Hide overlay (implementation depends on UI framework integration)
                await HideOverlayInternalAsync(overlayId, activeOverlay);

                // Remove from active overlays
                _activeOverlays.TryRemove(overlayId, out _);

                // Fire hidden event
                var hiddenArgs = new OverlayEventArgs(overlayId, activeOverlay.OverlayType);
                await FireEventSafelyAsync(() => OverlayHidden?.Invoke(this, hiddenArgs));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error hiding overlay {OverlayId}", overlayId);
                return false;
            }
        }

        /// <inheritdoc />
        public void RegisterOverlay<TViewModel, TView>()
            where TViewModel : class
            where TView : class
        {
            var viewModelType = typeof(TViewModel);
            var viewType = typeof(TView);
            var key = viewModelType.FullName ?? viewModelType.Name;

            var registration = new OverlayRegistration
            {
                ViewModelType = viewModelType,
                ViewType = viewType,
                Key = key
            };

            _registeredOverlays.AddOrUpdate(key, registration, (_, _) => registration);
            
            _logger.LogDebug("Registered overlay: ViewModel={ViewModelType}, View={ViewType}", 
                viewModelType.Name, viewType.Name);
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetActiveOverlayIds()
        {
            return _activeOverlays.Keys.ToList().AsReadOnly();
        }

        #endregion

        #region Private Methods

        private OverlayRegistration? GetOrCreateRegistration<T>() where T : class
        {
            var requestType = typeof(T);
            var key = requestType.FullName ?? requestType.Name;

            if (_registeredOverlays.TryGetValue(key, out var registration))
            {
                return registration;
            }

            // Try to auto-register based on naming conventions
            // Look for TViewModel and TView types in the same assembly/namespace
            var assembly = requestType.Assembly;
            var namespaceName = requestType.Namespace;

            var viewModelTypeName = $"{namespaceName}.ViewModels.{requestType.Name}ViewModel";
            var viewTypeName = $"{namespaceName}.Views.{requestType.Name}View";

            var viewModelType = assembly.GetType(viewModelTypeName);
            var viewType = assembly.GetType(viewTypeName);

            if (viewModelType != null && viewType != null)
            {
                var autoRegistration = new OverlayRegistration
                {
                    ViewModelType = viewModelType,
                    ViewType = viewType,
                    Key = key
                };

                _registeredOverlays.TryAdd(key, autoRegistration);
                _logger.LogDebug("Auto-registered overlay for {RequestType}: ViewModel={ViewModelType}, View={ViewType}",
                    requestType.Name, viewModelType.Name, viewType.Name);
                
                return autoRegistration;
            }

            _logger.LogWarning("Could not find overlay registration for type {RequestType}", requestType.Name);
            return null;
        }

        private async Task<OverlayResult<T>> ShowOverlayInternalAsync<T>(string overlayId, T request, OverlayRegistration registration) where T : class
        {
            // Create ViewModel instance
            var viewModel = _serviceProvider.GetRequiredService(registration.ViewModelType);
            var view = _serviceProvider.GetRequiredService(registration.ViewType);

            // Create active overlay tracking
            var activeOverlay = new ActiveOverlay
            {
                Id = overlayId,
                ViewModel = viewModel,
                View = view,
                OverlayType = typeof(T),
                CreatedAt = DateTime.UtcNow
            };

            _activeOverlays.TryAdd(overlayId, activeOverlay);

            // TODO: Implement actual UI showing logic here
            // This will depend on the Avalonia integration with MainWindow or overlay host
            // For now, simulate async operation
            await Task.Delay(100); // Placeholder for actual UI operation

            // TODO: Get actual result from overlay interaction
            // This would come from user interaction with the overlay
            var result = OverlayResult<T>.Success(request, overlayId);

            _logger.LogDebug("Overlay {OverlayId} shown successfully", overlayId);
            return result;
        }

        private async Task HideOverlayInternalAsync(string overlayId, ActiveOverlay activeOverlay)
        {
            // TODO: Implement actual UI hiding logic here
            // This will depend on the Avalonia integration with MainWindow or overlay host
            await Task.Delay(50); // Placeholder for actual UI operation

            _logger.LogDebug("Overlay {OverlayId} hidden successfully", overlayId);
        }

        private async Task FireEventSafelyAsync(Action eventAction)
        {
            try
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    lock (_eventLock)
                    {
                        eventAction.Invoke();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error firing overlay event");
            }
        }

        #endregion

        #region Private Classes

        private class OverlayRegistration
        {
            public Type ViewModelType { get; set; } = null!;
            public Type ViewType { get; set; } = null!;
            public string Key { get; set; } = string.Empty;
        }

        private class ActiveOverlay
        {
            public string Id { get; set; } = string.Empty;
            public object ViewModel { get; set; } = null!;
            public object View { get; set; } = null!;
            public Type OverlayType { get; set; } = null!;
            public DateTime CreatedAt { get; set; }
        }

        #endregion

        #region IDisposable Implementation

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Hide all active overlays
                    var activeIds = GetActiveOverlayIds();
                    foreach (var overlayId in activeIds)
                    {
                        try
                        {
                            HideOverlayAsync(overlayId).Wait(TimeSpan.FromSeconds(5));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error disposing overlay {OverlayId}", overlayId);
                        }
                    }

                    _activeOverlays.Clear();
                    _registeredOverlays.Clear();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}