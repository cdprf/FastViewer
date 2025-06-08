# PowerShell Script to Check and Help Setup Visual Studio Development Environment for File Previewer
# Author: AI Agent
# Version: 1.0

# --- Introduction and Warning ---
Write-Host "----------------------------------------------------------------------"
Write-Host "File Previewer - Visual Studio Environment Setup Helper"
Write-Host "----------------------------------------------------------------------"
Write-Host "This script will attempt to check for required .NET SDK and Visual Studio components."
Write-Host "INFO: Operations involving the Visual Studio Installer may require Administrator privileges."
Write-Host "You might be prompted for elevation by the Visual Studio Installer if modifications are needed."
Write-Host "----------------------------------------------------------------------"
Read-Host "Press Enter to continue..."
Write-Host ""

# --- Configuration ---
$requiredSdkMajorVersion = "8.0"
$managedDesktopWorkload = "Microsoft.VisualStudio.Workload.ManagedDesktop"
$vsInstallerPath = "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vs_installer.exe"
$vsWherePath = "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe"

# --- Check for .NET SDK Version ---
Write-Host "Checking for .NET SDK Version..."
$sdkInstalled = $false
$installedSdks = dotnet --list-sdks
if ($LASTEXITCODE -ne 0) {
    Write-Warning ".NET SDK check failed. 'dotnet --list-sdks' command failed."
    Write-Host "Please ensure the .NET SDK is installed and accessible in your PATH."
    Write-Host "Download .NET SDK from: https://dotnet.microsoft.com/download"
} else {
    foreach ($sdk in $installedSdks) {
        if ($sdk -match "^$requiredSdkMajorVersion\.") {
            Write-Host "Found compatible .NET SDK: $sdk"
            $sdkInstalled = $true
            break
        }
    }
    if (-not $sdkInstalled) {
        Write-Warning ".NET SDK version $requiredSdkMajorVersion (or later major version) not found."
        Write-Host "Installed SDKs found:"
        $installedSdks | ForEach-Object { Write-Host "  $_" }
        Write-Host "Please download and install the required .NET SDK from: https://dotnet.microsoft.com/download"
    } else {
        Write-Host ".NET SDK check: OK"
    }
}
Write-Host "----------------------------------------------------------------------"
Write-Host ""

# --- Check for Visual Studio Installation and Workloads ---
Write-Host "Checking for Visual Studio Installation and Workloads..."

if (-not (Test-Path $vsWherePath)) {
    Write-Warning "Visual Studio Installer query tool (vswhere.exe) not found at '$vsWherePath'."
    Write-Host "Visual Studio might not be installed, or it's installed in a non-standard location."
    Write-Host "Please download and install Visual Studio from: https://visualstudio.microsoft.com/downloads/"
} else {
    Write-Host "vswhere.exe found."

    # Get the path to the latest Visual Studio installation
    $vsInstallationPath = & $vsWherePath -latest -property installationPath -nologo

    if ([string]::IsNullOrWhiteSpace($vsInstallationPath)) {
        Write-Warning "No Visual Studio installation found via vswhere.exe."
        Write-Host "Please ensure Visual Studio is installed correctly."
        Write-Host "Download Visual Studio from: https://visualstudio.microsoft.com/downloads/"
    } else {
        Write-Host "Latest Visual Studio installation found at: $vsInstallationPath"

        # Check if the .NET desktop development workload is installed
        Write-Host "Checking for '.NET desktop development' workload ($managedDesktopWorkload)..."
        $workloadInstalledResult = & $vsWherePath -latest -requires $managedDesktopWorkload -property installationPath -nologo

        if ([string]::IsNullOrWhiteSpace($workloadInstalledResult)) {
            Write-Warning "The '.NET desktop development' workload ($managedDesktopWorkload) appears to be MISSING."
            Write-Host "This workload is required to build the project in Visual Studio."
            Write-Host ""
            Write-Host "To install the missing workload, you can:"
            Write-Host "1. Open Visual Studio Installer (search for it in the Start Menu)."
            Write-Host "   Find your Visual Studio installation, click 'Modify'."
            Write-Host "   Go to the 'Workloads' tab, select '.NET desktop development', and click 'Modify'."
            Write-Host ""
            Write-Host "2. Or, run the following command in PowerShell (as Administrator):"

            # Construct the command carefully, ensuring paths are quoted
            $vsInstallerCommand = "& `"$vsInstallerPath`" modify --installPath `"$vsInstallationPath`" --add $managedDesktopWorkload --quiet --norestart"
            Write-Host "   Command: $vsInstallerCommand"
            Write-Host "   (Note: --quiet will suppress UI, --norestart will prevent automatic restart if not needed immediately)"
            Write-Host "   You might be prompted for Administrator privileges."
        } else {
            Write-Host "'.NET desktop development' workload ($managedDesktopWorkload) seems to be installed."
            Write-Host "Visual Studio check: OK"
        }
    }
}
Write-Host "----------------------------------------------------------------------"
Write-Host ""

# --- Conclusion ---
Write-Host "Setup check finished."
Write-Host "Please review any warnings above and take appropriate action if necessary."
Write-Host "If all checks are OK, your environment should be ready for developing the File Previewer."
Write-Host "Remember to clone the repository and open FilePreviewer.sln in Visual Studio, or use 'dotnet build'."
Write-Host "----------------------------------------------------------------------"

# End of script
