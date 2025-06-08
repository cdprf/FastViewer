# File Previewer

## Description

File Previewer is a desktop application designed to provide quick previews of files directly from Windows Explorer. When a file is selected and the spacebar is pressed, the application aims to display a relevant preview or information about the file.

It is built using Avalonia UI, a cross-platform UI framework, allowing for potential future compatibility with other operating systems. However, core features like Windows Explorer integration and spacebar hooking are Windows-specific.

## Features

*   **Spacebar Triggered Preview:** Opens a preview window when a file is selected in Windows Explorer and the spacebar is pressed (Windows-specific, requires hook implementation).
*   **Image Preview:** Displays previews for common image formats (JPG, PNG, GIF, BMP).
*   **File Properties Display:** Shows metadata for binary files (e.g., file name, size, creation/modification dates).
*   **Plain Text Preview:** Shows the first 100 lines of text-based files.
*   **(Planned) Always on Top:** Option to keep the preview window always on top of other windows.
*   **(Planned) Automatic Startup:** Option to automatically start with Windows (Windows-specific).

## Setup Instructions

### Prerequisites

*   **.NET SDK:** .NET 8.0 or later.
*   **For Windows Development (Recommended):**
    *   Visual Studio 2022 (or later) with the ".NET desktop development" workload installed.
    *   A PowerShell script (`setup_dev_env.ps1` - *to be created in a subsequent step*) can assist with ensuring the correct Visual Studio components are installed.

### Building the Project

1.  **Clone the repository:**
    ```bash
    git clone <repository-url>
    cd <repository-directory>
    ```
2.  **Open in Visual Studio:**
    *   Open `FilePreviewer.sln` with Visual Studio.
    *   Build the solution (usually by pressing F6 or Ctrl+Shift+B).
3.  **Build from Command Line:**
    *   Navigate to the repository root.
    *   Run the command:
        ```bash
        dotnet build
        ```

## Usage

1.  Run the `FilePreviewer.Avalonia.exe` application (typically found in `FilePreviewer.Avalonia/bin/Debug/net8.0/` after building).
2.  Once the application is running (it may be in the system tray or as a background process depending on future implementation), navigate to Windows Explorer.
3.  Select a file.
4.  Press the **Spacebar**.
5.  The preview window should appear, displaying content based on the file type.

*(Note: The spacebar hook functionality is a core planned feature and requires platform-specific implementation.)*

## Current Limitations / Future Work

*   **Windows-Specific Features:** The spacebar hook for Windows Explorer integration and automatic startup with Windows are Windows-specific features that require platform-dependent implementations (e.g., using P/Invoke or similar techniques). These are not yet fully implemented.
*   **Syntax Highlighting:** The planned syntax highlighting for text files (using AvaloniaEdit) was not implemented during the current development phase due to NuGet package resolution issues in the development environment. This can be revisited.
*   **Basic Previews:** Current previews are functional but can be enhanced (e.g., more file types, more interactive previews).
*   **Application Lifecycle:** Full application lifecycle management (tray icon, background process management, settings) is yet to be developed.
*   **Project Status:** The project is currently under development. Features may be incomplete or subject to change.

This README provides a general guide. As development progresses, specific instructions and features will be updated.
