using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Modal Dialog control for cross-platform modal presentations.
    /// Provides consistent modal behavior across Windows, macOS, Linux, and Android.
    /// </summary>
    public class UniversalModal : ContentControl
    {
        public static readonly StyledProperty<bool> IsOpenProperty =
            AvaloniaProperty.Register<UniversalModal, bool>(nameof(IsOpen), false);

        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<UniversalModal, string>(nameof(Title), string.Empty);

        public static readonly StyledProperty<bool> CanCloseProperty =
            AvaloniaProperty.Register<UniversalModal, bool>(nameof(CanClose), true);

        public static readonly StyledProperty<ICommand> CloseCommandProperty =
            AvaloniaProperty.Register<UniversalModal, ICommand>(nameof(CloseCommand));

        public static readonly StyledProperty<bool> ShowCloseButtonProperty =
            AvaloniaProperty.Register<UniversalModal, bool>(nameof(ShowCloseButton), true);

        public static readonly StyledProperty<ModalSize> SizeProperty =
            AvaloniaProperty.Register<UniversalModal, ModalSize>(nameof(Size), ModalSize.Medium);

        /// <summary>
        /// Whether the modal is currently open
        /// </summary>
        public bool IsOpen
        {
            get => GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        /// <summary>
        /// Title displayed in the modal header
        /// </summary>
        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Whether the modal can be closed by the user
        /// </summary>
        public bool CanClose
        {
            get => GetValue(CanCloseProperty);
            set => SetValue(CanCloseProperty, value);
        }

        /// <summary>
        /// Command executed when the modal is closed
        /// </summary>
        public ICommand CloseCommand
        {
            get => GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        /// <summary>
        /// Whether to show the close button in the header
        /// </summary>
        public bool ShowCloseButton
        {
            get => GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, value);
        }

        /// <summary>
        /// Size of the modal dialog
        /// </summary>
        public ModalSize Size
        {
            get => GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        static UniversalModal()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UniversalModal),
                new StyledPropertyMetadata(typeof(UniversalModal)));
        }

        public UniversalModal()
        {
            UpdateModalClasses();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsOpenProperty)
            {
                UpdateModalClasses();
                
                if (IsOpen)
                    OnOpened();
                else
                    OnClosed();
            }
            else if (change.Property == SizeProperty)
            {
                UpdateModalClasses();
            }
        }

        private void UpdateModalClasses()
        {
            Classes.Set("open", IsOpen);
            Classes.Set("closed", !IsOpen);
            Classes.Set("small", Size == ModalSize.Small);
            Classes.Set("medium", Size == ModalSize.Medium);
            Classes.Set("large", Size == ModalSize.Large);
            Classes.Set("fullscreen", Size == ModalSize.FullScreen);
        }

        protected virtual void OnOpened()
        {
            // Focus management when modal opens
            Focus();
        }

        protected virtual void OnClosed()
        {
            // Cleanup when modal closes
            CloseCommand?.Execute(null);
        }

        public void Close()
        {
            if (CanClose)
            {
                IsOpen = false;
            }
        }

        public void Open()
        {
            IsOpen = true;
        }
    }

    /// <summary>
    /// Modal size options
    /// </summary>
    public enum ModalSize
    {
        Small,
        Medium,
        Large,
        FullScreen
    }
}