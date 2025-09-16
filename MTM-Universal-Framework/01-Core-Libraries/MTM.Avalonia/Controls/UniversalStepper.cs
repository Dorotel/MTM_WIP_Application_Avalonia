using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTM.UniversalFramework.Avalonia.Controls;

/// <summary>
/// Universal stepper control for multi-step processes and wizards.
/// Provides visual progress indication and navigation.
/// </summary>
public class UniversalStepper : UserControl
{
    public static readonly StyledProperty<ObservableCollection<StepItem>> StepsProperty =
        AvaloniaProperty.Register<UniversalStepper, ObservableCollection<StepItem>>(nameof(Steps));

    public static readonly StyledProperty<int> CurrentStepProperty =
        AvaloniaProperty.Register<UniversalStepper, int>(nameof(CurrentStep), 0, 
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<UniversalStepper, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<bool> AllowNavigationProperty =
        AvaloniaProperty.Register<UniversalStepper, bool>(nameof(AllowNavigation), true);

    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<UniversalStepper, bool>(nameof(ShowLabels), true);

    /// <summary>
    /// Gets or sets the collection of steps.
    /// </summary>
    public ObservableCollection<StepItem> Steps
    {
        get => GetValue(StepsProperty);
        set => SetValue(StepsProperty, value);
    }

    /// <summary>
    /// Gets or sets the current step index (0-based).
    /// </summary>
    public int CurrentStep
    {
        get => GetValue(CurrentStepProperty);
        set => SetValue(CurrentStepProperty, value);
    }

    /// <summary>
    /// Gets or sets the orientation of the stepper.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets whether users can navigate by clicking steps.
    /// </summary>
    public bool AllowNavigation
    {
        get => GetValue(AllowNavigationProperty);
        set => SetValue(AllowNavigationProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show step labels.
    /// </summary>
    public bool ShowLabels
    {
        get => GetValue(ShowLabelsProperty);
        set => SetValue(ShowLabelsProperty, value);
    }

    static UniversalStepper()
    {
        StepsProperty.Changed.AddClassHandler<UniversalStepper>((stepper, args) =>
        {
            stepper.BuildStepperUI();
        });

        CurrentStepProperty.Changed.AddClassHandler<UniversalStepper>((stepper, args) =>
        {
            stepper.UpdateStepStates();
        });

        OrientationProperty.Changed.AddClassHandler<UniversalStepper>((stepper, args) =>
        {
            stepper.BuildStepperUI();
        });
    }

    public UniversalStepper()
    {
        Classes.Add("universal-stepper");
        Steps = new ObservableCollection<StepItem>();
        BuildStepperUI();
    }

    private void BuildStepperUI()
    {
        var panel = Orientation == Orientation.Horizontal
            ? new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16 }
            : new StackPanel { Orientation = Orientation.Vertical, Spacing = 8 };

        if (Steps?.Any() == true)
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                var step = Steps[i];
                var stepControl = CreateStepControl(step, i);
                panel.Children.Add(stepControl);

                // Add connector line (except for last step)
                if (i < Steps.Count - 1)
                {
                    var connector = CreateConnector();
                    panel.Children.Add(connector);
                }
            }
        }

        Content = new ScrollViewer
        {
            Content = panel,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };

        UpdateStepStates();
    }

    private Control CreateStepControl(StepItem step, int index)
    {
        var stepPanel = new StackPanel
        {
            Orientation = Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 4
        };

        // Step number/icon circle
        var circle = new Border
        {
            Width = 32,
            Height = 32,
            CornerRadius = new CornerRadius(16),
            Background = Avalonia.Media.Brushes.LightGray,
            BorderBrush = Avalonia.Media.Brushes.Gray,
            BorderThickness = new Thickness(1),
            Child = new TextBlock
            {
                Text = step.IsCompleted ? "✓" : (index + 1).ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                Foreground = Avalonia.Media.Brushes.White
            }
        };

        stepPanel.Children.Add(circle);

        // Step label
        if (ShowLabels && !string.IsNullOrEmpty(step.Label))
        {
            var label = new TextBlock
            {
                Text = step.Label,
                FontSize = 12,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 100
            };
            stepPanel.Children.Add(label);
        }

        // Make clickable if navigation is allowed
        if (AllowNavigation)
        {
            var button = new Button
            {
                Content = stepPanel,
                Background = Avalonia.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(4),
                [!Button.CommandProperty] = new Avalonia.Data.Binding
                {
                    Source = this,
                    Path = nameof(NavigateToStepCommand)
                },
                CommandParameter = index
            };
            return button;
        }

        return stepPanel;
    }

    private Control CreateConnector()
    {
        return Orientation == Orientation.Horizontal
            ? new Rectangle
            {
                Width = 40,
                Height = 2,
                Fill = Avalonia.Media.Brushes.LightGray,
                VerticalAlignment = VerticalAlignment.Center
            }
            : new Rectangle
            {
                Width = 2,
                Height = 40,
                Fill = Avalonia.Media.Brushes.LightGray,
                HorizontalAlignment = HorizontalAlignment.Center
            };
    }

    private void UpdateStepStates()
    {
        if (Steps?.Any() != true || Content is not ScrollViewer scrollViewer ||
            scrollViewer.Content is not StackPanel panel)
            return;

        for (int i = 0; i < Steps.Count; i++)
        {
            var step = Steps[i];
            var stepIndex = i * 2; // Account for connectors

            if (stepIndex < panel.Children.Count)
            {
                var stepControl = panel.Children[stepIndex];
                UpdateStepVisualState(stepControl, i);
            }
        }
    }

    private void UpdateStepVisualState(Control stepControl, int stepIndex)
    {
        var step = Steps[stepIndex];
        var isActive = stepIndex == CurrentStep;
        var isCompleted = step.IsCompleted || stepIndex < CurrentStep;

        // Update step styling based on state
        stepControl.Classes.Set("active", isActive);
        stepControl.Classes.Set("completed", isCompleted);
    }

    public void NavigateToStep(int stepIndex)
    {
        if (stepIndex >= 0 && stepIndex < (Steps?.Count ?? 0))
        {
            CurrentStep = stepIndex;
        }
    }

    // Command for navigation (would typically be implemented with MVVM Community Toolkit)
    public System.Windows.Input.ICommand NavigateToStepCommand => 
        new SimpleCommand<int>(NavigateToStep);
}

/// <summary>
/// Represents a single step in the stepper.
/// </summary>
public class StepItem
{
    public string Label { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public bool IsEnabled { get; set; } = true;
    public object? Icon { get; set; }
}

/// <summary>
/// Simple command implementation for demo purposes.
/// In real usage, use MVVM Community Toolkit RelayCommand.
/// </summary>
internal class SimpleCommand<T> : System.Windows.Input.ICommand
{
    private readonly System.Action<T> _execute;

    public SimpleCommand(System.Action<T> execute)
    {
        _execute = execute;
    }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        if (parameter is T typedParam)
            _execute(typedParam);
    }

    public event System.EventHandler? CanExecuteChanged;
}