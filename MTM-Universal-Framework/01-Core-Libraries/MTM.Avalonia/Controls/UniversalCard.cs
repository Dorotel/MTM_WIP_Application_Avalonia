using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace MTM.UniversalFramework.Avalonia.Controls;

/// <summary>
/// Universal responsive card control optimized for cross-platform applications.
/// Provides consistent card-based layout with touch and mouse interaction support.
/// </summary>
public class UniversalCard : Border
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<UniversalCard, string>(nameof(Title), string.Empty);

    public static readonly StyledProperty<string> SubtitleProperty =
        AvaloniaProperty.Register<UniversalCard, string>(nameof(Subtitle), string.Empty);

    public static readonly StyledProperty<object?> HeaderContentProperty =
        AvaloniaProperty.Register<UniversalCard, object?>(nameof(HeaderContent));

    public static readonly StyledProperty<object?> ActionContentProperty =
        AvaloniaProperty.Register<UniversalCard, object?>(nameof(ActionContent));

    public static readonly StyledProperty<bool> IsElevatedProperty =
        AvaloniaProperty.Register<UniversalCard, bool>(nameof(IsElevated), true);

    public static readonly StyledProperty<bool> IsSelectableProperty =
        AvaloniaProperty.Register<UniversalCard, bool>(nameof(IsSelectable), false);

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<UniversalCard, bool>(nameof(IsSelected), false, 
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets the card title.
    /// </summary>
    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the card subtitle.
    /// </summary>
    public string Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the header content.
    /// </summary>
    public object? HeaderContent
    {
        get => GetValue(HeaderContentProperty);
        set => SetValue(HeaderContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the action content (buttons, etc.).
    /// </summary>
    public object? ActionContent
    {
        get => GetValue(ActionContentProperty);
        set => SetValue(ActionContentProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the card has elevation/shadow.
    /// </summary>
    public bool IsElevated
    {
        get => GetValue(IsElevatedProperty);
        set => SetValue(IsElevatedProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the card can be selected.
    /// </summary>
    public bool IsSelectable
    {
        get => GetValue(IsSelectableProperty);
        set => SetValue(IsSelectableProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the card is selected.
    /// </summary>
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    static UniversalCard()
    {
        // Apply default card styling
        BackgroundProperty.OverrideDefaultValue<UniversalCard>(Avalonia.Media.Brushes.White);
        CornerRadiusProperty.OverrideDefaultValue<UniversalCard>(new CornerRadius(8));
        PaddingProperty.OverrideDefaultValue<UniversalCard>(new Thickness(16));
        BorderThicknessProperty.OverrideDefaultValue<UniversalCard>(new Thickness(1));
        BorderBrushProperty.OverrideDefaultValue<UniversalCard>(Avalonia.Media.Brushes.LightGray);

        // Watch for property changes to update styling
        IsElevatedProperty.Changed.AddClassHandler<UniversalCard>((card, args) =>
        {
            card.UpdateStyling();
        });

        IsSelectedProperty.Changed.AddClassHandler<UniversalCard>((card, args) =>
        {
            card.UpdateStyling();
        });

        IsSelectableProperty.Changed.AddClassHandler<UniversalCard>((card, args) =>
        {
            card.UpdateStyling();
        });
    }

    public UniversalCard()
    {
        Classes.Add("universal-card");
        UpdateStyling();

        // Add pointer interaction for selectable cards
        PointerPressed += (sender, e) =>
        {
            if (IsSelectable)
            {
                IsSelected = !IsSelected;
            }
        };
    }

    private void UpdateStyling()
    {
        Classes.Set("elevated", IsElevated);
        Classes.Set("selectable", IsSelectable);
        Classes.Set("selected", IsSelected);
    }
}