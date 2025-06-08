using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using FilePreviewer.Avalonia.Core; // Required for FileTypeDetector
using FilePreviewer.Avalonia.Views;
using FilePreviewer.Avalonia.WindowsIntegration; // Required for SpacebarHook
using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives; // Required for ToggleButton

namespace FilePreviewer.Avalonia
{
    public partial class MainWindow : Window
    {
        private ToggleButton? _alwaysOnTopToggleButton;
        private ContentControl? _mainContentArea;

        public MainWindow()
        {
            InitializeComponent();
            InitializeCustomControls();
            SetupSpacebarHook(); // Placeholder for spacebar hook integration
            SetInitialContentMessage(); // Set initial message in MainContentArea

            this.Closing += MainWindow_Closing; // Handle window closing for cleanup
        }

        private void InitializeCustomControls()
        {
            _alwaysOnTopToggleButton = this.FindControl<ToggleButton>("AlwaysOnTopToggleButton");
            _mainContentArea = this.FindControl<ContentControl>("MainContentArea");

            if (_alwaysOnTopToggleButton != null)
            {
                _alwaysOnTopToggleButton.IsCheckedChanged += AlwaysOnTopToggleButton_IsCheckedChanged;
                _alwaysOnTopToggleButton.IsChecked = this.Topmost;
            }
            else
            {
                Console.WriteLine("MainWindow: AlwaysOnTopToggleButton control not found.");
            }

            if (_mainContentArea == null)
            {
                Console.WriteLine("MainWindow: MainContentArea control not found. Cannot display content.");
                // Fallback if main content area is somehow missing
                this.Content = new TextBlock { Text = "Error: MainContentArea not found. UI cannot load.", Margin = new Avalonia.Thickness(10) };
            }
        }

        private void SetInitialContentMessage()
        {
            if (_mainContentArea != null)
            {
                _mainContentArea.Content = new TextBlock
                {
                    Text = "Select a file in Explorer and press Spacebar to preview.\n(Note: Spacebar hook not yet implemented.)",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Avalonia.Thickness(20)
                };
            }
        }

        public async Task ProcessFileAsync(string filePath)
        {
            if (_mainContentArea == null)
            {
                Console.WriteLine("MainWindow: MainContentArea is null. Cannot process file.");
                return;
            }

            // Clear previous content except for dialogs
            if (!(_mainContentArea.Content is Window)) // Don't clear if a dialog is somehow in content
            {
                _mainContentArea.Content = null;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                _mainContentArea.Content = new TextBlock { Text = "Error: No file path provided.", Foreground = Brushes.Red };
                return;
            }
            if (!File.Exists(filePath))
            {
                _mainContentArea.Content = new TextBlock { Text = $"Error: File not found at '{filePath}'.", Foreground = Brushes.Red, TextWrapping = TextWrapping.Wrap };
                return;
            }

            try
            {
                FileTypeDetector.FileType fileType = await FileTypeDetector.GetFileTypeAsync(filePath);

                switch (fileType)
                {
                    case FileTypeDetector.FileType.Image:
                        var imageView = new ImageDisplayView();
                        _mainContentArea.Content = imageView;
                        await imageView.LoadImageAsync(filePath);
                        break;

                    case FileTypeDetector.FileType.Text:
                        var textView = new TextDisplayView();
                        _mainContentArea.Content = textView;
                        await textView.LoadTextAsync(filePath);
                        break;

                    case FileTypeDetector.FileType.Binary:
                        // For binary files, we show a dialog, so we don't replace MainContentArea directly.
                        // We might want to show some indication in MainContentArea that a dialog is open, or leave it as is.
                        _mainContentArea.Content = new TextBlock { Text = $"Displaying properties for binary file: {Path.GetFileName(filePath)}", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                        var propertiesWindow = new FilePropertiesView(filePath);
                        await propertiesWindow.ShowDialog(this);
                        // After dialog closes, we could revert MainContentArea or leave the message.
                        // For now, leave the message. If user selects another file, it will update.
                        break;

                    case FileTypeDetector.FileType.Unknown:
                    default:
                        _mainContentArea.Content = new TextBlock { Text = $"Error: Cannot preview file. Unknown or unsupported file type at '{filePath}'.", Foreground = Brushes.Red, TextWrapping = TextWrapping.Wrap };
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file '{filePath}': {ex.Message}");
                _mainContentArea.Content = new TextBlock { Text = $"An unexpected error occurred while processing '{Path.GetFileName(filePath)}':\n{ex.Message}", Foreground = Brushes.Red, TextWrapping = TextWrapping.Wrap };
            }
        }

        private void SetupSpacebarHook()
        {
            // Placeholder for actual hook logic
            try
            {
                SpacebarHook.Start(); // This is a static placeholder call
                SpacebarHook.SpacebarPressedWithFile += OnSpacebarPressedWithFile;
                Console.WriteLine("MainWindow: SpacebarHook.Start() called and event subscribed (placeholder).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MainWindow: Failed to start SpacebarHook (placeholder) - {ex.Message}");
                if (_mainContentArea != null && _mainContentArea.Content is TextBlock currentTextBlock && currentTextBlock.Text.Contains("Spacebar hook not yet implemented"))
                {
                    currentTextBlock.Text += $"\nError initializing hook: {ex.Message}";
                }
            }
        }

        private async void OnSpacebarPressedWithFile(string filePathFromEvent)
        {
            Console.WriteLine($"MainWindow: SpacebarPressedWithFile event received for: {filePathFromEvent}");
            // Bring window to front and focus before processing
            this.Activate();
            await ProcessFileAsync(filePathFromEvent);
        }

        private void AlwaysOnTopToggleButton_IsCheckedChanged(object? sender, RoutedEventArgs e)
        {
            if (_alwaysOnTopToggleButton != null)
            {
                this.Topmost = _alwaysOnTopToggleButton.IsChecked == true;
            }
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // Cleanup spacebar hook
            try
            {
                SpacebarHook.SpacebarPressedWithFile -= OnSpacebarPressedWithFile;
                SpacebarHook.Stop(); // This is a static placeholder call
                Console.WriteLine("MainWindow: SpacebarHook.Stop() called and event unsubscribed (placeholder).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MainWindow: Error stopping SpacebarHook (placeholder) - {ex.Message}");
            }
        }
    }
}
