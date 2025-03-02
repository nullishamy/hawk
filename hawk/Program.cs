using Hawk.Utilities;

namespace Hawk;

internal static class Program {
    private static async Task Main() {
        var cts = new CancellationTokenSource();
        
        // Capture Ctrl + C and trigger cancellation
        Console.CancelKeyPress += (sender, e) => {
            e.Cancel = true;
            
            cts.Cancel();
            cts.Dispose();
        };

        AppDomain.CurrentDomain.ProcessExit += (sender, e) => {
            cts.Cancel();
            cts.Dispose();
        };

        try {
            await RunApplicationAsync(cts.Token);
        }
        catch (OperationCanceledException) {
            Console.WriteLine("Closing...");
        }
    }

    private static async Task RunApplicationAsync(CancellationToken token) {
        if (WindowsUtils.IsWindowsDefenderActive()) {
            Console.WriteLine("Windows Defender Real-Time Protection is still active! Attempting to disable...");

            if (WindowsUtils.IsTamperProtectionActive()) {
                Console.WriteLine("Tamper Protection is enabled! You must disable Real-Time Protection yourself before running. Quitting...");

                return;
            }

            if (await WindowsUtils.DisableWindowsDefender(token)) {
                Console.WriteLine("Successfully disabled Windows Defender.");
            }
            else {
                Console.WriteLine("Failed to disable Windows Defender! You need to disable it yourself! Aborting...");

                return;
            }
        }
        else {
            Console.WriteLine("Windows Defender Real-Time Protection is not active...");
        }

        await DebloatWindows.ApplyMiscRegistryChangesAsync(token);
        
        // Reenable Windows Real-time Protection
        if (await WindowsUtils.EnableWindowsDefender(token)) {
            Console.WriteLine("Successfully enabled Windows Defender.");
        }
        else {
            Console.WriteLine("Failed to enable windows defender! You need to enable it yourself!");
        }
        
        Console.WriteLine("Tamper Protection cannot be enabled from code! Please be sure to re-enable it yourself!");
    }
}
