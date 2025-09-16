using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using NUnit.Framework;

namespace MTM.Universal.Testing
{
    /// <summary>
    /// Base class for Avalonia UI tests using headless testing.
    /// </summary>
    public abstract class UITestBase : TestBase
    {
        protected Application TestApp { get; private set; } = null!;
        protected Window TestWindow { get; private set; } = null!;

        protected override async Task InitializeAsync()
        {
            // Initialize Avalonia for headless testing
            TestApp = BuildAvaloniaApp()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions
                {
                    UseHeadlessDrawing = true
                })
                .StartWithClassicDesktopLifetime(Array.Empty<string>());

            await base.InitializeAsync();
        }

        /// <summary>
        /// Build Avalonia application for testing. Override in derived classes.
        /// </summary>
        protected virtual AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<TestApplication>()
                .UsePlatformDetect()
                .LogToTrace();
        }

        /// <summary>
        /// Create test window with specified content.
        /// </summary>
        protected async Task<Window> CreateTestWindowAsync(Control content = null!)
        {
            Window window = null!;
            
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                window = new Window
                {
                    Width = 800,
                    Height = 600,
                    Content = content ?? new StackPanel()
                };
            });

            return window;
        }

        /// <summary>
        /// Show window and wait for it to be visible.
        /// </summary>
        protected async Task ShowWindowAsync(Window window)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                window.Show();
            });

            // Wait for window to be rendered
            await Task.Delay(100);
        }

        /// <summary>
        /// Close window safely.
        /// </summary>
        protected async Task CloseWindowAsync(Window window)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                window?.Close();
            });
        }

        /// <summary>
        /// Find control in visual tree.
        /// </summary>
        protected T? FindControl<T>(Control parent, string name) where T : Control
        {
            T? result = null;
            
            Dispatcher.UIThread.Invoke(() =>
            {
                result = parent.FindControl<T>(name);
            });

            return result;
        }

        /// <summary>
        /// Set text in TextBox control.
        /// </summary>
        protected async Task SetTextAsync(TextBox textBox, string text)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                textBox.Text = text;
            });
        }

        /// <summary>
        /// Click button control.
        /// </summary>
        protected async Task ClickButtonAsync(Button button)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                button.Command?.Execute(button.CommandParameter);
            });
        }

        /// <summary>
        /// Wait for condition to be true.
        /// </summary>
        protected async Task WaitForConditionAsync(Func<bool> condition, TimeSpan timeout = default)
        {
            if (timeout == default)
                timeout = TimeSpan.FromSeconds(5);

            var start = DateTime.Now;
            while (!condition() && DateTime.Now - start < timeout)
            {
                await Task.Delay(50);
            }

            if (!condition())
            {
                throw new TimeoutException($"Condition was not met within {timeout}");
            }
        }

        protected override async Task AfterEachTestAsync()
        {
            if (TestWindow != null)
            {
                await CloseWindowAsync(TestWindow);
                TestWindow = null!;
            }
            await base.AfterEachTestAsync();
        }

        /// <summary>
        /// Test application for headless testing.
        /// </summary>
        private class TestApplication : Application
        {
            public override void Initialize()
            {
                // Initialize test application
            }
        }
    }
}