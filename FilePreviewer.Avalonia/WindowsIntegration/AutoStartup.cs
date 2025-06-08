using System;
using System.Diagnostics; // For Debug.WriteLine
// using Microsoft.Win32; // Would be required for actual registry operations on Windows

namespace FilePreviewer.Avalonia.WindowsIntegration
{
    public static class AutoStartup
    {
        private const string AppName = "FilePreviewer";
        // Registry path for Current User startup programs
        private const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// Enables automatic startup for the application on Windows.
        /// This method would add the application's executable path to the
        /// 'HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run' registry key.
        /// </summary>
        public static void EnableAutoStartup()
        {
            Debug.WriteLine($"AutoStartup: Attempting to enable auto-startup for '{AppName}'.");

            // --- Actual Windows Implementation Comments ---
            // This operation requires Microsoft.Win32.Registry (add using Microsoft.Win32;).
            // It may also require appropriate permissions. Error handling for registry access is crucial.
            //
            // try
            // {
            //     string? executablePath = System.Reflection.Assembly.GetEntryAssembly()?.Location;
            //     // For .NET Core/5+ applications, Assembly.Location might point to a .dll.
            //     // If so, it needs to be the path to the host executable (e.g., AppName.exe).
            //     // This can be tricky, especially for single-file deployments or different publishing models.
            //     // A common approach is `AppContext.BaseDirectory + AppName + ".exe"` but needs verification.
            //     // Or `Environment.ProcessPath` on .NET 6+
            //
            //     if (string.IsNullOrEmpty(executablePath))
            //     {
            //         Debug.WriteLine($"AutoStartup: Could not determine executable path.");
            //         return;
            //     }
            //
            //     // Ensure the path is to the .exe for startup, especially if GetEntryAssembly().Location returns a .dll
            //     if (Path.GetExtension(executablePath).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            //     {
            //          executablePath = Path.Combine(AppContext.BaseDirectory, Path.GetFileNameWithoutExtension(executablePath) + ".exe");
            //     }
            //
            //     if (!File.Exists(executablePath)) // Verify the .exe path exists before adding to registry
            //     {
            //          Debug.WriteLine($"AutoStartup: Executable not found at presumed path: {executablePath}");
            //          // Potentially try Environment.ProcessPath if available and different
            //          executablePath = Environment.ProcessPath;
            //          if (string.IsNullOrEmpty(executablePath) || !File.Exists(executablePath)) {
            //              Debug.WriteLine($"AutoStartup: Environment.ProcessPath also invalid or not found.");
            //              return;
            //          }
            //     }
            //
            //     using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true))
            //     {
            //         if (key == null)
            //         {
            //             Debug.WriteLine($"AutoStartup: Could not open registry key '{RegistryKeyPath}'.");
            //             return;
            //         }
            //         key.SetValue(AppName, $"\"{executablePath}\""); // Quote path in case of spaces
            //         Debug.WriteLine($"AutoStartup: Enabled for '{AppName}' with path '{executablePath}'.");
            //     }
            // }
            // catch (Exception ex)
            // {
            //     Debug.WriteLine($"AutoStartup: Error enabling auto-startup - {ex.Message}");
            //     // Handle exceptions, e.g., SecurityException if permissions are insufficient.
            // }
            // --- End Windows Implementation Comments ---
        }

        /// <summary>
        /// Disables automatic startup for the application on Windows.
        /// This method would remove the application's entry from the
        /// 'HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run' registry key.
        /// </summary>
        public static void DisableAutoStartup()
        {
            Debug.WriteLine($"AutoStartup: Attempting to disable auto-startup for '{AppName}'.");

            // --- Actual Windows Implementation Comments ---
            // This operation requires Microsoft.Win32.Registry.
            // It may also require appropriate permissions. Error handling is important.
            //
            // try
            // {
            //     using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true))
            //     {
            //         if (key == null)
            //         {
            //             Debug.WriteLine($"AutoStartup: Could not open registry key '{RegistryKeyPath}'.");
            //             return;
            //         }
            //         // Check if value exists before trying to delete
            //         if (key.GetValue(AppName) != null)
            //         {
            //              key.DeleteValue(AppName, false); // false: do not throw if not found (though we checked)
            //              Debug.WriteLine($"AutoStartup: Disabled for '{AppName}'.");
            //         }
            //         else
            //         {
            //              Debug.WriteLine($"AutoStartup: Entry for '{AppName}' not found, no action needed.");
            //         }
            //     }
            // }
            // catch (Exception ex)
            // {
            //     Debug.WriteLine($"AutoStartup: Error disabling auto-startup - {ex.Message}");
            //     // Handle exceptions, e.g., SecurityException.
            // }
            // --- End Windows Implementation Comments ---
        }

        /// <summary>
        /// Checks if automatic startup is currently enabled for the application on Windows.
        /// This method would check for the application's entry in the
        /// 'HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run' registry key.
        /// </summary>
        /// <returns>True if auto-startup is enabled, false otherwise.</returns>
        public static bool IsAutoStartupEnabled()
        {
            Debug.WriteLine($"AutoStartup: Checking auto-startup status for '{AppName}'.");

            // --- Actual Windows Implementation Comments ---
            // This operation requires Microsoft.Win32.Registry.
            //
            // try
            // {
            //     using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false)) // false: read-only
            //     {
            //         if (key == null)
            //         {
            //             Debug.WriteLine($"AutoStartup: Could not open registry key '{RegistryKeyPath}' for reading.");
            //             return false;
            //         }
            //         object? value = key.GetValue(AppName);
            //         if (value != null)
            //         {
            //             // Optionally, verify the path in the registry value matches the current executable's expected path.
            //             // string? registeredPath = value.ToString();
            //             // string? currentExecutablePath = ... ; // Determine current exe path
            //             // if (registeredPath.Equals($"\"{currentExecutablePath}\"", StringComparison.OrdinalIgnoreCase))
            //             // {
            //             //    Debug.WriteLine($"AutoStartup: Enabled for '{AppName}' with path '{registeredPath}'.");
            //             //    return true;
            //             // }
            //             // else {
            //             //    Debug.WriteLine($"AutoStartup: Entry found for '{AppName}' but path mismatch: '{registeredPath}'.");
            //             //    return false; // Or handle as appropriate
            //             // }
            //             Debug.WriteLine($"AutoStartup: Entry found for '{AppName}'. Value: {value}");
            //             return true;
            //         }
            //     }
            // }
            // catch (Exception ex)
            // {
            //     Debug.WriteLine($"AutoStartup: Error checking auto-startup status - {ex.Message}");
            //     // Handle exceptions.
            // }
            // Debug.WriteLine($"AutoStartup: Entry for '{AppName}' not found.");
            // return false;
            // --- End Windows Implementation Comments ---

            return false; // Placeholder default return value
        }
    }
}
