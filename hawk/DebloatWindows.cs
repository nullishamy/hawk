using System.Diagnostics;
using Hawk.Utilities;
using Microsoft.Win32;

namespace Hawk;

public static class DebloatWindows {
    public static async Task ApplyMiscRegistryChangesAsync(CancellationToken token = default) {
        try {
            WindowsUtils.SetCurrentUserRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAl", 0, RegistryValueKind.DWord);// Align Taskbar to the left
            WindowsUtils.SetCurrentUserRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0, RegistryValueKind.DWord); // Set Windows to dark theme
            WindowsUtils.SetCurrentUserRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0, RegistryValueKind.DWord); // Set Windows to dark theme
            WindowsUtils.SetCurrentUserRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent", "AccentColorMenu", 1, RegistryValueKind.DWord); // Makes accent color the color of the taskbar and start menu
            WindowsUtils.SetCurrentUserRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", 1, RegistryValueKind.DWord); // Makes accent color the color of the taskbar and start menu
            WindowsUtils.SetCurrentUserRegistryValue(@"Software\Microsoft\Windows\DWM", "AccentColorInStartAndTaskbar", 1, RegistryValueKind.DWord); // Makes accent color the color of the taskbar and start menu
            //WindowsUtils.SetCurrentUserRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent", "AccentPalette", 0x00 * 32, RegistryValueKind.Binary); // Makes the taskbar black
            WindowsUtils.SetCurrentUserRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", 0, RegistryValueKind.DWord); // Fix the "Get an app for 'ms-gamingoverlay'" popup
            WindowsUtils.SetLocalMachineRegistryValue(@"SOFTWARE\Microsoft\PolicyManager\default\ApplicationManagement\AllowGameDVR", "Value", 0, RegistryValueKind.DWord); // Disable Game DVR (Reduces FPS Drops)
            WindowsUtils.SetCurrentUserRegistryValue(@"Control Panel\Desktop", "MenuShowDelay", "0", RegistryValueKind.String); // Reduce menu delay for snappier UI
            WindowsUtils.SetCurrentUserRegistryValue(@"Control Panel\Desktop\WindowMetrics", "MinAnimate", 0, RegistryValueKind.DWord); // Disable minimize/maximize animations
            WindowsUtils.SetCurrentUserRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ExtendedUIHoverTime", 1, RegistryValueKind.DWord); // Reduce hover time for tooltips and UI elements
            WindowsUtils.SetCurrentUserRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", 0, RegistryValueKind.DWord); // Show file extensions in Explorer (useful for security and organization)
            //WindowsUtils.SetCurrentUserRegistryValue(@"Control Panel\Colors", "Hilight", "0 0 0", RegistryValueKind.String); // Sets highlight color to black
            //WindowsUtils.SetCurrentUserRegistryValue(@"Control Panel\Colors", "HotTrackingColor", "0 0 0", RegistryValueKind.String); // Sets the click-and-drag box color to black
            
            await KillExplorerAsync(token);
            RestartExplorer();
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }
    
    private static async Task KillExplorerAsync(CancellationToken token = default) {
        var psi = new ProcessStartInfo {
            FileName = "taskkill",
            Arguments = "/F /IM explorer.exe",
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        using var process = Process.Start(psi);
        
        await process?.WaitForExitAsync(token)!;
    }

    private static void RestartExplorer() {
        var psi = new ProcessStartInfo {
            FileName = "explorer.exe",
            UseShellExecute = true
        };

        // No need to wait before continuing
        using var _ = Process.Start(psi);
    }
}
