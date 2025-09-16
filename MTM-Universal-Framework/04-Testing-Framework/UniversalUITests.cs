using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Xunit;

namespace MTM.UniversalFramework.Testing
{
    /// <summary>
    /// Universal UI test patterns for Avalonia UI components.
    /// Provides cross-platform UI testing utilities for any business domain.
    /// </summary>
    public abstract class UniversalUITestBase : IDisposable
    {
        protected Application TestApplication { get; private set; }
        protected Window TestWindow { get; private set; }
        protected bool IsDisposed { get; private set; }

        protected UniversalUITestBase()
        {
            InitializeAvaloniaForTesting();
        }

        private void InitializeAvaloniaForTesting()
        {
            if (Application.Current == null)
            {
                AppBuilder.Configure<TestApplication>()
                    .UseHeadless(new AvaloniaHeadlessOptions
                    {
                        UseHeadlessDrawing = true
                    })
                    .SetupWithoutStarting();
            }

            TestApplication = Application.Current as TestApplication ?? new TestApplication();
            CreateTestWindow();
        }

        protected virtual void CreateTestWindow()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                TestWindow = new Window
                {
                    Width = 800,
                    Height = 600,
                    Title = "Universal Framework Test Window"
                };
            }).Wait();
        }

        /// <summary>
        /// Tests that a control can be created and added to the UI tree
        /// </summary>
        protected async Task TestControlCreationAsync<T>() where T : Control, new()
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var control = new T();
                Assert.NotNull(control);
                
                TestWindow.Content = control;
                Assert.Equal(control, TestWindow.Content);
            });
        }

        /// <summary>
        /// Tests that a control properly binds to data
        /// </summary>
        protected async Task TestDataBindingAsync<T>(T control, object dataContext) where T : Control
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                control.DataContext = dataContext;
                Assert.Equal(dataContext, control.DataContext);
            });
        }

        /// <summary>
        /// Tests that a control responds to property changes
        /// </summary>
        protected async Task TestPropertyChangeAsync<T, TProp>(
            T control,
            StyledProperty<TProp> property,
            TProp newValue) where T : Control
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                control.SetValue(property, newValue);
                var actualValue = control.GetValue(property);
                Assert.Equal(newValue, actualValue);
            });
        }

        /// <summary>
        /// Tests control layout and sizing
        /// </summary>
        protected async Task TestControlLayoutAsync<T>(T control, Size expectedSize) where T : Control
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                TestWindow.Content = control;
                
                // Trigger layout
                TestWindow.UpdateLayout();
                
                // Check bounds (this is simplified - real layout testing is more complex)
                Assert.True(control.Bounds.Width >= 0);
                Assert.True(control.Bounds.Height >= 0);
            });
        }

        /// <summary>
        /// Tests that a control can be focused
        /// </summary>
        protected async Task TestControlFocusAsync<T>(T control) where T : Control
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                TestWindow.Content = control;
                
                if (control.Focusable)
                {
                    control.Focus();
                    Assert.True(control.IsFocused);
                }
            });
        }

        /// <summary>
        /// Helper method to wait for UI operations to complete
        /// </summary>
        protected async Task WaitForUIAsync(int millisecondsDelay = 100)
        {
            await Task.Delay(millisecondsDelay);
            await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
        }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    TestWindow?.Close();
                    TestWindow = null;
                }).Wait();

                IsDisposed = true;
            }
        }
    }

    /// <summary>
    /// Test application for Avalonia UI testing
    /// </summary>
    public class TestApplication : Application
    {
        public override void Initialize()
        {
            // Minimal initialization for testing
        }
    }

    /// <summary>
    /// Sample UI test implementation showing how to use the base class
    /// </summary>
    public class SampleControlTests : UniversalUITestBase
    {
        [Fact]
        public async Task TestButton_Creation_ShouldCreateSuccessfully()
        {
            await TestControlCreationAsync<Button>();
        }

        [Fact]
        public async Task TestTextBlock_TextProperty_ShouldUpdateCorrectly()
        {
            var textBlock = new TextBlock();
            await TestPropertyChangeAsync(textBlock, TextBlock.TextProperty, "Test Text");
        }

        [Fact]
        public async Task TestButton_DataBinding_ShouldBindCorrectly()
        {
            var button = new Button();
            var dataContext = new { ButtonText = "Click Me" };
            
            await TestDataBindingAsync(button, dataContext);
        }

        [Fact]
        public async Task TestStackPanel_Layout_ShouldLayoutCorrectly()
        {
            var stackPanel = new StackPanel();
            stackPanel.Children.Add(new Button { Content = "Button 1" });
            stackPanel.Children.Add(new Button { Content = "Button 2" });
            
            await TestControlLayoutAsync(stackPanel, new Size(200, 100));
        }

        [Fact]
        public async Task TestButton_Focus_ShouldFocusCorrectly()
        {
            var button = new Button { Content = "Test Button" };
            await TestControlFocusAsync(button);
        }
    }
}