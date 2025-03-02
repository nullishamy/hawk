using System.Diagnostics;

namespace Hawk.Utilities;

public static class Utils {
    public static bool RunPowerShellCommand(string command) {
        try {
            var psi = new ProcessStartInfo {
                FileName = "powershell",
                Arguments = $"-Command \"{command}\"",
                Verb = "RunAs", // Require Admin Privileges
                UseShellExecute = true,
            };

            using var process = Process.Start(psi);
            
            Debug.Assert(process != null);
            
            process.WaitForExit();

            return process.ExitCode == 0;
        }
        catch {
            return false;
        }
    }

    public static async Task<bool> RunPowerShellCommandAsync(string command, CancellationToken cancellationToken = default) {
        try {
            var psi = new ProcessStartInfo {
                FileName = "powershell",
                Arguments = $"-Command \"{command}\"",
                Verb = "RunAs", // Require Admin Privileges
                UseShellExecute = true,
            };

            using var process = Process.Start(psi);
            
            Debug.Assert(process != null);

            await process.WaitForExitAsync(cancellationToken);

            return process.ExitCode == 0;
        }
        catch {
            return false;
        }
    }
}
