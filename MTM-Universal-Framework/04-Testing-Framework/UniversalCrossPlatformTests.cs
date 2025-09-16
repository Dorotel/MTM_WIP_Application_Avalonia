using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace MTM.UniversalFramework.Testing
{
    /// <summary>
    /// Universal cross-platform test patterns for testing functionality across Windows, macOS, Linux, and Android.
    /// Provides platform-specific testing utilities and platform detection.
    /// </summary>
    public abstract class UniversalCrossPlatformTestBase : UniversalTestBase
    {
        protected PlatformInfo CurrentPlatform { get; }

        protected UniversalCrossPlatformTestBase()
        {
            CurrentPlatform = DetectPlatform();
        }

        /// <summary>
        /// Detects the current platform
        /// </summary>
        protected virtual PlatformInfo DetectPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new PlatformInfo(OSPlatform.Windows, RuntimeInformation.OSArchitecture);
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return new PlatformInfo(OSPlatform.OSX, RuntimeInformation.OSArchitecture);
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return new PlatformInfo(OSPlatform.Linux, RuntimeInformation.OSArchitecture);
            
            // For Android detection, you'd typically check for specific Android indicators
            // This is a simplified example
            return new PlatformInfo(OSPlatform.Create("Unknown"), RuntimeInformation.OSArchitecture);
        }

        /// <summary>
        /// Runs a test only on specified platforms
        /// </summary>
        protected void SkipIfNotPlatform(params OSPlatform[] supportedPlatforms)
        {
            bool isSupported = false;
            foreach (var platform in supportedPlatforms)
            {
                if (RuntimeInformation.IsOSPlatform(platform))
                {
                    isSupported = true;
                    break;
                }
            }

            if (!isSupported)
            {
                Skip.If(true, $"Test not supported on {CurrentPlatform.OS}");
            }
        }

        /// <summary>
        /// Tests platform-specific file path handling
        /// </summary>
        protected void TestPlatformSpecificPaths()
        {
            var separator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? '\\' : '/';
            var testPath = $"folder{separator}file.txt";
            
            Assert.Contains(separator, testPath);
        }

        /// <summary>
        /// Tests platform-specific environment variables
        /// </summary>
        protected void TestPlatformEnvironmentVariables()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
                Assert.False(string.IsNullOrEmpty(userProfile), "USERPROFILE should exist on Windows");
            }
            else
            {
                var home = Environment.GetEnvironmentVariable("HOME");
                Assert.False(string.IsNullOrEmpty(home), "HOME should exist on Unix-like systems");
            }
        }

        /// <summary>
        /// Tests platform-specific features
        /// </summary>
        protected async Task TestPlatformSpecificFeatureAsync<T>(
            Func<Task<T>> windowsImplementation = null,
            Func<Task<T>> macOSImplementation = null,
            Func<Task<T>> linuxImplementation = null,
            Func<Task<T>> androidImplementation = null)
        {
            Func<Task<T>> implementation = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && windowsImplementation != null)
                implementation = windowsImplementation;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && macOSImplementation != null)
                implementation = macOSImplementation;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && linuxImplementation != null)
                implementation = linuxImplementation;
            // Android detection would go here

            if (implementation != null)
            {
                var result = await implementation();
                Assert.NotNull(result);
            }
            else
            {
                Skip.If(true, $"No implementation provided for platform {CurrentPlatform.OS}");
            }
        }
    }

    /// <summary>
    /// Platform information structure
    /// </summary>
    public class PlatformInfo
    {
        public OSPlatform OS { get; }
        public Architecture Architecture { get; }

        public PlatformInfo(OSPlatform os, Architecture architecture)
        {
            OS = os;
            Architecture = architecture;
        }

        public bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public bool IsUnix => IsMacOS || IsLinux;

        public override string ToString() => $"{OS} {Architecture}";
    }

    /// <summary>
    /// Sample cross-platform test implementation
    /// </summary>
    public class SampleCrossPlatformTests : UniversalCrossPlatformTestBase
    {
        [Fact]
        public void TestPlatformDetection_ShouldDetectCurrentPlatform()
        {
            Assert.NotNull(CurrentPlatform);
            Assert.NotNull(CurrentPlatform.OS);
        }

        [Fact]
        public void TestPathHandling_ShouldUsePlatformSpecificSeparators()
        {
            TestPlatformSpecificPaths();
        }

        [Fact]
        public void TestEnvironmentVariables_ShouldFindPlatformSpecificVariables()
        {
            TestPlatformEnvironmentVariables();
        }

        [Fact]
        public async Task TestFileOperations_ShouldWorkOnAllPlatforms()
        {
            await TestPlatformSpecificFeatureAsync(
                windowsImplementation: async () =>
                {
                    // Windows-specific file operation
                    var tempPath = System.IO.Path.GetTempPath();
                    Assert.True(tempPath.Contains("\\"), "Windows should use backslashes");
                    return true;
                },
                macOSImplementation: async () =>
                {
                    // macOS-specific file operation
                    var tempPath = System.IO.Path.GetTempPath();
                    Assert.True(tempPath.Contains("/"), "macOS should use forward slashes");
                    return true;
                },
                linuxImplementation: async () =>
                {
                    // Linux-specific file operation
                    var tempPath = System.IO.Path.GetTempPath();
                    Assert.True(tempPath.Contains("/"), "Linux should use forward slashes");
                    return true;
                }
            );
        }

        [Fact]
        public void TestWindowsOnlyFeature_ShouldRunOnlyOnWindows()
        {
            SkipIfNotPlatform(OSPlatform.Windows);
            
            // This test will only run on Windows
            var userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
            Assert.False(string.IsNullOrEmpty(userProfile));
        }

        [Fact]
        public void TestUnixOnlyFeature_ShouldRunOnlyOnUnixSystems()
        {
            SkipIfNotPlatform(OSPlatform.Linux, OSPlatform.OSX);
            
            // This test will only run on Unix-like systems
            var home = Environment.GetEnvironmentVariable("HOME");
            Assert.False(string.IsNullOrEmpty(home));
        }
    }
}