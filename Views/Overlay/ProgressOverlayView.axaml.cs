using System;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace MTM_WIP_Application_Avalonia.Views.Overlay;

/// <summary>
/// ProgressOverlayView provides a universal progress indication overlay
/// following MTM design patterns. Supports both determinate and indeterminate
/// progress display with proper theming and animations.
/// </summary>
public partial class ProgressOverlayView : UserControl
{
    public ProgressOverlayView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Clean up resources when the control is detached from the visual tree
    /// </summary>
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        // Dispose ViewModel if it implements IDisposable
        if (DataContext is IDisposable disposableViewModel)
        {
            disposableViewModel.Dispose();
        }

        base.OnDetachedFromLogicalTree(e);
    }
}
