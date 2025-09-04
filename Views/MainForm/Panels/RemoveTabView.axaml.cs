using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels;
using System;
using Avalonia.Threading;

namespace MTM_WIP_Application_Avalonia.Views;

public partial class RemoveTabView : UserControl
{
    private readonly ILogger<RemoveTabView>? _logger;
    private RemoveItemViewModel? _viewModel;

    public RemoveTabView()
    {
        try
        {
            InitializeComponent();
            SetupEventHandlers();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RemoveTabView initialization error: {ex.Message}");
            throw;
        }
    }

    public RemoveTabView(ILogger<RemoveTabView> logger) : this()
    {
        _logger = logger;
        _logger?.LogInformation("RemoveTabView initialized with dependency injection");
    }

    private void SetupEventHandlers()
    {
        try
        {
            this.DataContextChanged += OnDataContextChanged;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to setup event handlers");
        }
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        try
        {
            // Unwire previous ViewModel events
            if (_viewModel != null)
            {
                UnwireViewModelEvents();
            }

            // Wire up new ViewModel events
            if (DataContext is RemoveItemViewModel viewModel)
            {
                _viewModel = viewModel;
                WireViewModelEvents(viewModel);
                _logger?.LogInformation("RemoveItemViewModel connected successfully");
            }
            else if (DataContext != null)
            {
                _logger?.LogWarning("DataContext is not RemoveItemViewModel. Type: {Type}", DataContext.GetType().Name);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to handle DataContext change in RemoveTabView");
        }
    }

    private void WireViewModelEvents(RemoveItemViewModel viewModel)
    {
        try
        {
            // Command error handling is now managed within the ViewModels themselves
            // using standard .NET error handling patterns
            _logger?.LogDebug("RemoveTabView ViewModel events wired successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error wiring ViewModel events in RemoveTabView");
        }
    }

    private void HandleCommandException(string commandName, Exception ex)
    {
        try
        {
            _logger?.LogError(ex, "Command {CommandName} encountered an error in RemoveTabView: {Message}", commandName, ex.Message);
            
            // Handle specific exception types
            switch (ex)
            {
                case FormatException formatEx:
                    _logger?.LogError(formatEx, "Format exception in RemoveTabView command {CommandName}. Check data binding formats", commandName);
                    break;
                case InvalidCastException castEx:
                    _logger?.LogError(castEx, "Invalid cast exception in RemoveTabView command {CommandName}. Check data type conversions", commandName);
                    break;
                case ArgumentException argEx:
                    _logger?.LogError(argEx, "Argument exception in RemoveTabView command {CommandName}. Check parameter values", commandName);
                    break;
                default:
                    _logger?.LogError(ex, "Unhandled exception in RemoveTabView command {CommandName}", commandName);
                    break;
            }

            // Update ViewModel if safe to do so
            if (_viewModel != null)
            {
                try
                {
                    // Use Avalonia's Dispatcher instead of RxApp.MainThreadScheduler
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        try
                        {
                            // Set loading to false if an error occurred
                            _viewModel.IsLoading = false;
                        }
                        catch (Exception statusEx)
                        {
                            _logger?.LogError(statusEx, "Error updating ViewModel status in RemoveTabView");
                        }
                    });
                }
                catch (Exception schedulerEx)
                {
                    _logger?.LogError(schedulerEx, "Error scheduling status update in RemoveTabView");
                }
            }
        }
        catch (Exception handlerEx)
        {
            _logger?.LogCritical(handlerEx, "Critical error in RemoveTabView exception handler for command {CommandName}", commandName);
            System.Diagnostics.Debug.WriteLine($"Critical RemoveTabView exception handling error for {commandName}: {handlerEx.Message}");
        }
    }

    private void UnwireViewModelEvents()
    {
        try
        {
            // No subscriptions to clear in standard .NET patterns
            _logger?.LogDebug("RemoveTabView ViewModel events unwired successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error unwiring ViewModel events in RemoveTabView");
        }
    }

    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        try
        {
            if (_viewModel != null)
            {
                UnwireViewModelEvents();
            }

            this.DataContextChanged -= OnDataContextChanged;
            
            _logger?.LogInformation("RemoveTabView cleanup completed");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during RemoveTabView cleanup");
        }
        finally
        {
            base.OnDetachedFromVisualTree(e);
        }
    }
}
