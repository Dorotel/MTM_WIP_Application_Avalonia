using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using Material.Icons;

namespace MTM_WIP_Application_Avalonia.Views;

public partial class ConfirmationOverlayView : UserControl
{
    public ConfirmationOverlayView()
    {
        InitializeComponent();
        
        // Set up property change handler to update icon
        DataContextChanged += OnDataContextChanged;
    }
    
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is ConfirmationOverlayViewModel viewModel)
        {
            // Update icon based on overlay type
            UpdateHeaderIcon(viewModel.OverlayType, viewModel.IconKind);
            
            // Subscribe to property changes
            viewModel.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(ConfirmationOverlayViewModel.OverlayType) ||
                    args.PropertyName == nameof(ConfirmationOverlayViewModel.IconKind))
                {
                    UpdateHeaderIcon(viewModel.OverlayType, viewModel.IconKind);
                }
            };
        }
    }
    
    private void UpdateHeaderIcon(OverlayType overlayType, string iconKind)
    {
        var headerIcon = this.FindControl<Material.Icons.Avalonia.MaterialIcon>("HeaderIcon");
        if (headerIcon != null)
        {
            // Set icon based on type or custom icon kind
            headerIcon.Kind = overlayType switch
            {
                OverlayType.Error => MaterialIconKind.AlertCircle,
                OverlayType.Warning => MaterialIconKind.Warning,
                OverlayType.Success => MaterialIconKind.CheckCircle,
                OverlayType.Information => MaterialIconKind.Information,
                OverlayType.Confirmation => MaterialIconKind.QuestionMarkCircle,
                _ => MaterialIconKind.QuestionMarkCircle
            };
            
            // Override with custom icon if specified
            if (!string.IsNullOrEmpty(iconKind) && Enum.TryParse<MaterialIconKind>(iconKind, out var customIcon))
            {
                headerIcon.Kind = customIcon;
            }
        }
        
        // Update header background based on overlay type
        UpdateHeaderStyling(overlayType);
        
        // Update primary button styling based on overlay type
        UpdatePrimaryButtonStyling(overlayType);
    }
    
    private void UpdateHeaderStyling(OverlayType overlayType)
    {
        var headerBorder = this.FindControl<Border>("HeaderPanel");
        if (headerBorder != null)
        {
            // Clear existing classes
            headerBorder.Classes.Clear();
            
            // Add appropriate class based on overlay type
            var headerClass = overlayType switch
            {
                OverlayType.Error => "header-error",
                OverlayType.Warning => "header-warning", 
                OverlayType.Success => "header-success",
                _ => "header-confirmation"
            };
            
            headerBorder.Classes.Add(headerClass);
        }
    }
    
    private void UpdatePrimaryButtonStyling(OverlayType overlayType)
    {
        var primaryButton = this.FindControl<Button>("PrimaryActionButton");
        if (primaryButton != null)
        {
            // Clear existing action classes
            primaryButton.Classes.Remove("primary-action");
            primaryButton.Classes.Remove("warning-action");
            
            // Add base class
            if (!primaryButton.Classes.Contains("overlay-button"))
            {
                primaryButton.Classes.Add("overlay-button");
            }
            
            // Add appropriate action class
            var actionClass = overlayType switch
            {
                OverlayType.Warning when primaryButton.Content?.ToString()?.Contains("Delete") == true => "warning-action",
                OverlayType.Error => "warning-action",
                _ => "primary-action"
            };
            
            primaryButton.Classes.Add(actionClass);
        }
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DataContextChanged -= OnDataContextChanged;
        base.OnDetachedFromVisualTree(e);
    }
    
    private void OnUserControlKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not ConfirmationOverlayViewModel viewModel) return;
        
        switch (e.Key)
        {
            case Key.Escape:
                viewModel.CancelCommand.Execute(null);
                e.Handled = true;
                break;
                
            case Key.Enter when e.KeyModifiers == KeyModifiers.None:
                viewModel.ConfirmCommand.Execute(null);
                e.Handled = true;
                break;
        }
    }
    
    private void OnBackgroundClicked(object? sender, PointerPressedEventArgs e)
    {
        // Close overlay when clicking on background (outside dialog)
        if (DataContext is ConfirmationOverlayViewModel viewModel)
        {
            viewModel.CancelCommand.Execute(null);
        }
        e.Handled = true;
    }
}
