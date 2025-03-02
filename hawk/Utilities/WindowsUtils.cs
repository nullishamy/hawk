using System.Management;
using Microsoft.Win32;

namespace Hawk.Utilities;

public static class WindowsUtils {
    public static bool IsWindowsDefenderActive() {
        try {
            using var searcher = new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntivirusProduct");

            foreach (var obj in searcher.Get()) {
                var displayName = obj["DisplayName"]?.ToString() ?? string.Empty;

                if (!displayName.Contains("Windows Defender", StringComparison.OrdinalIgnoreCase))
                    continue;

                var productState = (uint)(obj["productState"] ?? 0);

                return (productState & 0x1000) != 0; // 0x1000 indicates realtime protection is enabled
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return false;
    }

    public static async Task<bool> DisableWindowsDefender(CancellationToken cancellationToken = default) {
        // You can't disable Windows Defender via C#, so using PS `Set-MpPreference` command
        return await Utils.RunPowerShellCommandAsync("Set-MpPreference -DisableRealtimeMonitoring $true", cancellationToken);
    }

    public static async Task<bool> EnableWindowsDefender(CancellationToken cancellationToken = default) {
        // You can't enable Windows Defender via C#, so using PS `Set-MpPreference` command
        return await Utils.RunPowerShellCommandAsync("Set-MpPreference -DisableRealtimeMonitoring $false", cancellationToken);
    }
    
    public static bool IsTamperProtectionActive() {
        try {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows Defender\Features");

            var value = key?.GetValue("TamperProtection");

            if (value == null) return false;

            var tamperProtectionValue = (int)value;

            return tamperProtectionValue == 5;
        }
        catch (UnauthorizedAccessException) {
            return true;
        }
    }

    public static bool SetCurrentUserRegistryValue(string subKey, string name, object value, RegistryValueKind valueKind) {
        try {
            using var key = Registry.CurrentUser.OpenSubKey(subKey, true);

            key?.SetValue(name, value, valueKind);

            return true;
        }
        catch {
            return false;
        }
    }

    public static bool SetLocalMachineRegistryValue(string subKey, string name, object value, RegistryValueKind valueKind) {
        try {
            using var key = Registry.LocalMachine.OpenSubKey(subKey, true);

            key?.SetValue(name, value, valueKind);

            return true;
        }
        catch {
            return false;
        }
    }
}
