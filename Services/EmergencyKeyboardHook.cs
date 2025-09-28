using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Emergency keyboard hook service for handling global shortcuts when UI is locked
/// Uses low-level Windows API calls to detect key combinations even when application is unresponsive
/// </summary>
public class EmergencyKeyboardHook : IDisposable
{
    private readonly ILogger<EmergencyKeyboardHook> _logger;
    private LowLevelKeyboardProc? _proc;
    private IntPtr _hookID = IntPtr.Zero;
    private bool _disposed = false;
    private volatile bool _ctrlPressed = false;
    private volatile bool _altPressed = false;

    // Windows API constants
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;

    // Virtual key codes
    private const int VK_CONTROL = 0x11;
    private const int VK_MENU = 0x12; // Alt key
    private const int VK_Q = 0x51;
    private const int VK_C = 0x43;

    // Events
    public event Action? EmergencyExitRequested;
    public event Action? EmergencyContinueRequested;

    public EmergencyKeyboardHook(ILogger<EmergencyKeyboardHook> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Starts the emergency keyboard hook
    /// </summary>
    public void StartHook()
    {
        try
        {
            if (_hookID != IntPtr.Zero)
            {
                _logger.LogWarning("Emergency keyboard hook already started");
                return;
            }

            _proc = HookCallback;
            _hookID = SetHook(_proc);
            
            if (_hookID == IntPtr.Zero)
            {
                _logger.LogError("Failed to install emergency keyboard hook");
            }
            else
            {
                _logger.LogInformation("Emergency keyboard hook installed successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting emergency keyboard hook");
        }
    }

    /// <summary>
    /// Stops the emergency keyboard hook
    /// </summary>
    public void StopHook()
    {
        try
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
                _logger.LogInformation("Emergency keyboard hook removed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping emergency keyboard hook");
        }
    }

    /// <summary>
    /// Sets up the low-level keyboard hook
    /// </summary>
    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        
        if (curModule?.ModuleName == null)
            return IntPtr.Zero;

        return SetWindowsHookEx(
            WH_KEYBOARD_LL,
            proc,
            GetModuleHandle(curModule.ModuleName),
            0
        );
    }

    /// <summary>
    /// Keyboard hook callback - processes all keyboard events
    /// </summary>
    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        try
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                bool isKeyDown = wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN;
                bool isKeyUp = wParam == WM_KEYUP || wParam == WM_SYSKEYUP;

                // Track modifier keys
                if (vkCode == VK_CONTROL)
                {
                    _ctrlPressed = isKeyDown;
                }
                else if (vkCode == VK_MENU)
                {
                    _altPressed = isKeyDown;
                }
                else if (isKeyDown && _ctrlPressed && _altPressed)
                {
                    // Check for emergency key combinations
                    if (vkCode == VK_Q)
                    {
                        _logger.LogWarning("Emergency exit shortcut detected (Ctrl+Alt+Q)");
                        
                        // Trigger emergency exit on background thread to avoid UI blocking
                        Task.Run(() =>
                        {
                            try
                            {
                                EmergencyExitRequested?.Invoke();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error invoking emergency exit");
                                // Force exit as last resort
                                Environment.Exit(1);
                            }
                        });
                        
                        return (IntPtr)1; // Suppress the key
                    }
                    else if (vkCode == VK_C)
                    {
                        _logger.LogWarning("Emergency continue shortcut detected (Ctrl+Alt+C)");
                        
                        // Trigger emergency continue on background thread
                        Task.Run(() =>
                        {
                            try
                            {
                                EmergencyContinueRequested?.Invoke();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error invoking emergency continue");
                            }
                        });
                        
                        return (IntPtr)1; // Suppress the key
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Don't let hook callback exceptions crash the app
            _logger.LogError(ex, "Error in keyboard hook callback");
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            StopHook();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    ~EmergencyKeyboardHook()
    {
        Dispose();
    }

    // Windows API P/Invoke declarations
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
