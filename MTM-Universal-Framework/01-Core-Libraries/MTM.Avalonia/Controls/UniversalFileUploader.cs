using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;

namespace MTM.Universal.Controls
{
    /// <summary>
    /// Universal file uploader control with drag-and-drop support and progress tracking.
    /// </summary>
    public class UniversalFileUploader : UserControl
    {
        public static readonly StyledProperty<string> PlaceholderTextProperty =
            AvaloniaProperty.Register<UniversalFileUploader, string>(
                nameof(PlaceholderText), "Click or drag files here");

        public static readonly StyledProperty<string> AcceptedFileTypesProperty =
            AvaloniaProperty.Register<UniversalFileUploader, string>(
                nameof(AcceptedFileTypes), "*.*");

        public static readonly StyledProperty<bool> AllowMultipleFilesProperty =
            AvaloniaProperty.Register<UniversalFileUploader, bool>(
                nameof(AllowMultipleFiles), false);

        public static readonly StyledProperty<long> MaxFileSizeProperty =
            AvaloniaProperty.Register<UniversalFileUploader, long>(
                nameof(MaxFileSize), 10 * 1024 * 1024); // 10MB default

        public static readonly StyledProperty<bool> IsUploadingProperty =
            AvaloniaProperty.Register<UniversalFileUploader, bool>(
                nameof(IsUploading), false, defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<double> ProgressProperty =
            AvaloniaProperty.Register<UniversalFileUploader, double>(
                nameof(Progress), 0.0, defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<ICommand?> FileSelectedCommandProperty =
            AvaloniaProperty.Register<UniversalFileUploader, ICommand?>(
                nameof(FileSelectedCommand));

        public string PlaceholderText
        {
            get => GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public string AcceptedFileTypes
        {
            get => GetValue(AcceptedFileTypesProperty);
            set => SetValue(AcceptedFileTypesProperty, value);
        }

        public bool AllowMultipleFiles
        {
            get => GetValue(AllowMultipleFilesProperty);
            set => SetValue(AllowMultipleFilesProperty, value);
        }

        public long MaxFileSize
        {
            get => GetValue(MaxFileSizeProperty);
            set => SetValue(MaxFileSizeProperty, value);
        }

        public bool IsUploading
        {
            get => GetValue(IsUploadingProperty);
            set => SetValue(IsUploadingProperty, value);
        }

        public double Progress
        {
            get => GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public ICommand? FileSelectedCommand
        {
            get => GetValue(FileSelectedCommandProperty);
            set => SetValue(FileSelectedCommandProperty, value);
        }

        public event EventHandler<FileSelectedEventArgs>? FilesSelected;

        public UniversalFileUploader()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var border = new Border
            {
                CornerRadius = new CornerRadius(8),
                BorderThickness = new Thickness(2),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                Background = new SolidColorBrush(Colors.White),
                MinHeight = 120,
                Padding = new Thickness(20)
            };

            var stackPanel = new StackPanel
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Spacing = 10
            };

            // Icon
            var icon = new TextBlock
            {
                Text = "📁",
                FontSize = 32,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };

            // Placeholder text
            var placeholderBinding = new Binding(nameof(PlaceholderText)) { Source = this };
            var placeholder = new TextBlock
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray)
            };
            placeholder.Bind(TextBlock.TextProperty, placeholderBinding);

            // Progress bar
            var progressBinding = new Binding(nameof(Progress)) { Source = this };
            var isUploadingBinding = new Binding(nameof(IsUploading)) { Source = this };
            var progressBar = new ProgressBar
            {
                Height = 4,
                Margin = new Thickness(0, 10, 0, 0)
            };
            progressBar.Bind(ProgressBar.ValueProperty, progressBinding);
            progressBar.Bind(ProgressBar.IsVisibleProperty, isUploadingBinding);

            stackPanel.Children.Add(icon);
            stackPanel.Children.Add(placeholder);
            stackPanel.Children.Add(progressBar);

            border.Child = stackPanel;
            Content = border;

            // Handle click events
            border.PointerPressed += OnPointerPressed;
            
            // Handle drag and drop
            AddHandler(DragDrop.DropEvent, OnDrop);
            AddHandler(DragDrop.DragOverEvent, OnDragOver);
        }

        private async void OnPointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (IsUploading) return;

            await OpenFileDialogAsync();
        }

        private async Task OpenFileDialogAsync()
        {
            var dialog = new OpenFileDialog
            {
                AllowMultiple = AllowMultipleFiles,
                Title = "Select File(s)"
            };

            // Set file filters based on AcceptedFileTypes
            if (!string.IsNullOrEmpty(AcceptedFileTypes) && AcceptedFileTypes != "*.*")
            {
                var filters = AcceptedFileTypes.Split(',', StringSplitOptions.RemoveEmptyEntries);
                dialog.Filters = new System.Collections.Generic.List<FileDialogFilter>
                {
                    new() { Name = "Allowed Files", Extensions = new System.Collections.Generic.List<string>(filters) }
                };
            }

            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel is Window window)
            {
                var files = await dialog.ShowAsync(window);
                if (files != null && files.Length > 0)
                {
                    await ProcessSelectedFilesAsync(files);
                }
            }
        }

        private void OnDragOver(object? sender, DragEventArgs e)
        {
            if (IsUploading)
            {
                e.DragEffects = DragDropEffects.None;
                return;
            }

            if (e.Data.Contains(DataFormats.Files))
            {
                e.DragEffects = DragDropEffects.Copy;
            }
            else
            {
                e.DragEffects = DragDropEffects.None;
            }
        }

        private async void OnDrop(object? sender, DragEventArgs e)
        {
            if (IsUploading) return;

            if (e.Data.Contains(DataFormats.Files))
            {
                var files = e.Data.GetFiles();
                if (files != null)
                {
                    var filePaths = new System.Collections.Generic.List<string>();
                    foreach (var file in files)
                    {
                        if (file.TryGetLocalPath() is string localPath)
                        {
                            filePaths.Add(localPath);
                        }
                    }

                    if (filePaths.Count > 0)
                    {
                        await ProcessSelectedFilesAsync(filePaths.ToArray());
                    }
                }
            }
        }

        private async Task ProcessSelectedFilesAsync(string[] files)
        {
            // Validate files
            var validFiles = new System.Collections.Generic.List<string>();
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.Exists && fileInfo.Length <= MaxFileSize)
                {
                    validFiles.Add(file);
                }
            }

            if (validFiles.Count == 0) return;

            // Limit to single file if multiple not allowed
            if (!AllowMultipleFiles && validFiles.Count > 1)
            {
                validFiles = new System.Collections.Generic.List<string> { validFiles[0] };
            }

            // Simulate upload progress
            IsUploading = true;
            Progress = 0;

            try
            {
                for (int i = 0; i <= 100; i += 10)
                {
                    Progress = i;
                    await Task.Delay(50); // Simulate upload time
                }

                // Notify about selected files
                var eventArgs = new FileSelectedEventArgs(validFiles.ToArray());
                FilesSelected?.Invoke(this, eventArgs);
                
                // Execute command if bound
                FileSelectedCommand?.Execute(validFiles.ToArray());
            }
            finally
            {
                IsUploading = false;
                Progress = 0;
            }
        }
    }

    public class FileSelectedEventArgs : EventArgs
    {
        public string[] Files { get; }

        public FileSelectedEventArgs(string[] files)
        {
            Files = files;
        }
    }
}