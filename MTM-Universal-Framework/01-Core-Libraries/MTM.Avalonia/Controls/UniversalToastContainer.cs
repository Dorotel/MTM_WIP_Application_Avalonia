using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTM.UniversalFramework.Avalonia.Controls;

/// <summary>
/// Universal toast notification control for displaying temporary messages.
/// Supports multiple notification types with auto-dismiss functionality.
/// </summary>
public class UniversalToastContainer : UserControl
{
    public static readonly StyledProperty<ObservableCollection<ToastItem>> ToastsProperty =
        AvaloniaProperty.Register<UniversalToastContainer, ObservableCollection<ToastItem>>(nameof(Toasts));

    public static readonly StyledProperty<ToastPosition> PositionProperty =
        AvaloniaProperty.Register<UniversalToastContainer, ToastPosition>(nameof(Position), ToastPosition.TopRight);

    public static readonly StyledProperty<int> MaxToastsProperty =
        AvaloniaProperty.Register<UniversalToastContainer, int>(nameof(MaxToasts), 5);

    public static readonly StyledProperty<TimeSpan> DefaultTimeoutProperty =
        AvaloniaProperty.Register<UniversalToastContainer, TimeSpan>(nameof(DefaultTimeout), TimeSpan.FromSeconds(5));

    /// <summary>
    /// Gets or sets the collection of toast notifications.
    /// </summary>
    public ObservableCollection<ToastItem> Toasts
    {
        get => GetValue(ToastsProperty);
        set => SetValue(ToastsProperty, value);
    }

    /// <summary>
    /// Gets or sets the position of toast notifications.
    /// </summary>
    public ToastPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of visible toasts.
    /// </summary>
    public int MaxToasts
    {
        get => GetValue(MaxToastsProperty);
        set => SetValue(MaxToastsProperty, value);
    }

    /// <summary>
    /// Gets or sets the default timeout for toast notifications.
    /// </summary>
    public TimeSpan DefaultTimeout
    {
        get => GetValue(DefaultTimeoutProperty);
        set => SetValue(DefaultTimeoutProperty, value);
    }

    private StackPanel? _toastPanel;

    static UniversalToastContainer()
    {
        ToastsProperty.Changed.AddClassHandler<UniversalToastContainer>((container, args) =>
        {
            container.UpdateToasts();
        });

        PositionProperty.Changed.AddClassHandler<UniversalToastContainer>((container, args) =>
        {
            container.UpdatePosition();
        });
    }

    public UniversalToastContainer()
    {
        Classes.Add("universal-toast-container");
        Toasts = new ObservableCollection<ToastItem>();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        _toastPanel = new StackPanel
        {
            Spacing = 8,
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Right
        };

        Content = _toastPanel;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (_toastPanel == null) return;

        switch (Position)
        {
            case ToastPosition.TopLeft:
                _toastPanel.VerticalAlignment = VerticalAlignment.Top;
                _toastPanel.HorizontalAlignment = HorizontalAlignment.Left;
                break;
            case ToastPosition.TopCenter:
                _toastPanel.VerticalAlignment = VerticalAlignment.Top;
                _toastPanel.HorizontalAlignment = HorizontalAlignment.Center;
                break;
            case ToastPosition.TopRight:
                _toastPanel.VerticalAlignment = VerticalAlignment.Top;
                _toastPanel.HorizontalAlignment = HorizontalAlignment.Right;
                break;
            case ToastPosition.BottomLeft:
                _toastPanel.VerticalAlignment = VerticalAlignment.Bottom;
                _toastPanel.HorizontalAlignment = HorizontalAlignment.Left;
                break;
            case ToastPosition.BottomCenter:
                _toastPanel.VerticalAlignment = VerticalAlignment.Bottom;
                _toastPanel.HorizontalAlignment = HorizontalAlignment.Center;
                break;
            case ToastPosition.BottomRight:
                _toastPanel.VerticalAlignment = VerticalAlignment.Bottom;
                _toastPanel.HorizontalAlignment = HorizontalAlignment.Right;
                break;
        }
    }

    private void UpdateToasts()
    {
        if (_toastPanel == null || Toasts == null) return;

        _toastPanel.Children.Clear();

        var visibleToasts = Toasts.Take(MaxToasts);
        foreach (var toast in visibleToasts)
        {
            var toastControl = CreateToastControl(toast);
            _toastPanel.Children.Add(toastControl);
        }
    }

    private Control CreateToastControl(ToastItem toast)
    {
        var toastBorder = new Border
        {
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(16, 12),
            MinWidth = 250,
            MaxWidth = 400,
            Margin = new Thickness(4),
            BoxShadow = new BoxShadows(BoxShadow.Parse("0 4 12 0 #40000000"))
        };

        // Set background and border based on toast type
        SetToastAppearance(toastBorder, toast.Type);

        var contentGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto")
        };

        // Icon
        var icon = CreateToastIcon(toast.Type);
        if (icon != null)
        {
            Grid.SetColumn(icon, 0);
            contentGrid.Children.Add(icon);
        }

        // Content panel
        var contentPanel = new StackPanel
        {
            Spacing = 4,
            Margin = new Thickness(icon != null ? 12 : 0, 0, 12, 0)
        };

        // Title
        if (!string.IsNullOrEmpty(toast.Title))
        {
            contentPanel.Children.Add(new TextBlock
            {
                Text = toast.Title,
                FontWeight = FontWeight.SemiBold,
                FontSize = 14,
                Foreground = GetTextBrush(toast.Type),
                TextWrapping = TextWrapping.Wrap
            });
        }

        // Message
        if (!string.IsNullOrEmpty(toast.Message))
        {
            contentPanel.Children.Add(new TextBlock
            {
                Text = toast.Message,
                FontSize = 12,
                Foreground = GetTextBrush(toast.Type),
                TextWrapping = TextWrapping.Wrap,
                Opacity = 0.9
            });
        }

        Grid.SetColumn(contentPanel, 1);
        contentGrid.Children.Add(contentPanel);

        // Close button
        var closeButton = new Button
        {
            Content = "×",
            FontSize = 16,
            Width = 24,
            Height = 24,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Foreground = GetTextBrush(toast.Type),
            Padding = new Thickness(0),
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
        };

        closeButton.Click += (s, e) => RemoveToast(toast);
        Grid.SetColumn(closeButton, 2);
        contentGrid.Children.Add(closeButton);

        toastBorder.Child = contentGrid;

        // Auto-dismiss timer
        if (toast.AutoDismiss && toast.Timeout > TimeSpan.Zero)
        {
            var timer = new System.Threading.Timer(
                _ => Avalonia.Threading.Dispatcher.UIThread.Post(() => RemoveToast(toast)),
                null, toast.Timeout, Timeout.InfiniteTimeSpan);

            toast.Timer = timer;
        }

        return toastBorder;
    }

    private void SetToastAppearance(Border border, ToastType type)
    {
        switch (type)
        {
            case ToastType.Success:
                border.Background = new SolidColorBrush(Color.FromRgb(34, 197, 94));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(22, 163, 74));
                break;
            case ToastType.Warning:
                border.Background = new SolidColorBrush(Color.FromRgb(251, 191, 36));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(245, 158, 11));
                break;
            case ToastType.Error:
                border.Background = new SolidColorBrush(Color.FromRgb(239, 68, 68));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 38, 38));
                break;
            case ToastType.Info:
                border.Background = new SolidColorBrush(Color.FromRgb(59, 130, 246));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(37, 99, 235));
                break;
            default:
                border.Background = new SolidColorBrush(Color.FromRgb(107, 114, 128));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(75, 85, 99));
                break;
        }

        border.BorderThickness = new Thickness(1);
    }

    private TextBlock? CreateToastIcon(ToastType type)
    {
        string iconText = type switch
        {
            ToastType.Success => "✓",
            ToastType.Warning => "⚠",
            ToastType.Error => "✕",
            ToastType.Info => "ℹ",
            _ => null
        };

        if (string.IsNullOrEmpty(iconText)) return null;

        return new TextBlock
        {
            Text = iconText,
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.White,
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    private IBrush GetTextBrush(ToastType type)
    {
        return Brushes.White; // All toast types use white text for contrast
    }

    public void ShowToast(string message, ToastType type = ToastType.Info, string? title = null, 
                         TimeSpan? timeout = null, bool autoDismiss = true)
    {
        var toast = new ToastItem
        {
            Title = title ?? string.Empty,
            Message = message,
            Type = type,
            AutoDismiss = autoDismiss,
            Timeout = timeout ?? DefaultTimeout
        };

        AddToast(toast);
    }

    public void ShowSuccess(string message, string? title = null) =>
        ShowToast(message, ToastType.Success, title);

    public void ShowWarning(string message, string? title = null) =>
        ShowToast(message, ToastType.Warning, title);

    public void ShowError(string message, string? title = null) =>
        ShowToast(message, ToastType.Error, title);

    public void ShowInfo(string message, string? title = null) =>
        ShowToast(message, ToastType.Info, title);

    private void AddToast(ToastItem toast)
    {
        Toasts.Insert(0, toast); // Add to top

        // Remove excess toasts
        while (Toasts.Count > MaxToasts)
        {
            var oldToast = Toasts.Last();
            RemoveToast(oldToast);
        }

        UpdateToasts();
    }

    private void RemoveToast(ToastItem toast)
    {
        toast.Timer?.Dispose();
        Toasts.Remove(toast);
        UpdateToasts();
    }

    public void ClearAllToasts()
    {
        foreach (var toast in Toasts.ToList())
        {
            RemoveToast(toast);
        }
    }
}

/// <summary>
/// Represents a toast notification item.
/// </summary>
public class ToastItem
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ToastType Type { get; set; } = ToastType.Info;
    public bool AutoDismiss { get; set; } = true;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
    internal System.Threading.Timer? Timer { get; set; }
}

/// <summary>
/// Defines the position of toast notifications.
/// </summary>
public enum ToastPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}

/// <summary>
/// Defines the type of toast notification.
/// </summary>
public enum ToastType
{
    Info,
    Success,
    Warning,
    Error
}