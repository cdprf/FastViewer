using Avalonia.Controls;
using Avalonia.Interactivity; // Required for RoutedEventArgs
using FilePreviewer.Avalonia.Views; // Required for ImageDisplayView
using System.IO; // Required for Path
using System; // Required for AppContext
using Avalonia.Layout; // Required for HorizontalAlignment/VerticalAlignment
using Avalonia.Media; // Required for TextWrapping

namespace FilePreviewer.Avalonia
{
    public partial class MainWindow : Window
    {
        private Button? _testLoadImageButton;
        private Button? _testShowPropertiesButton; // Added
        private ContentControl? _mainContentArea;

        public MainWindow()
        {
            InitializeComponent(); // This method is auto-generated and calls FindControls if needed by source gen
                                   // For explicit control finding, it's better to do it after InitializeComponent
            InitializeCustomControls();
        }

        private void InitializeCustomControls()
        {
            // Find the controls by their names from the AXAML
            _testLoadImageButton = this.FindControl<Button>("TestLoadImageButton");
            _testShowPropertiesButton = this.FindControl<Button>("TestShowPropertiesButton"); // Added
            _mainContentArea = this.FindControl<ContentControl>("MainContentArea");

            if (_testLoadImageButton != null)
            {
                _testLoadImageButton.Click += TestLoadImageButton_Click;
            }
            else
            {
                Console.WriteLine("MainWindow: TestLoadImageButton control not found. Check AXAML x:Name.");
                // Display an error in the window if critical controls are missing
                this.Content = new TextBlock { Text = "Error: TestLoadImageButton not found during initialization.", Margin = new Avalonia.Thickness(10) };
            }

            if (_testShowPropertiesButton != null) // Added
            {
                _testShowPropertiesButton.Click += TestShowPropertiesButton_Click; // Added
            }
            else // Added
            {
                Console.WriteLine("MainWindow: TestShowPropertiesButton control not found. Check AXAML x:Name."); // Added
                // Optionally, display an error if this button is critical, though less so than the content area
            }

            if (_mainContentArea == null)
            {
                Console.WriteLine("MainWindow: MainContentArea control not found. Check AXAML x:Name.");
                // If _testLoadImageButton was found, this part of UI might still be usable to show the error
                if (this.Content is not TextBlock) // Avoid overwriting previous error
                {
                     this.Content = new TextBlock { Text = "Error: MainContentArea not found during initialization.", Margin = new Avalonia.Thickness(10) };
                }
            }
        }

        private async void TestLoadImageButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_mainContentArea == null)
            {
                Console.WriteLine("MainWindow: MainContentArea is null, cannot load image view.");
                // Display this message in the UI as well if possible
                var errorTextBlock = new TextBlock
                {
                    Text = "Critical Error: MainContentArea reference is null. Cannot display content.",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };
                // If the _mainContentArea itself is null, we might need to set the window's content directly
                this.Content = errorTextBlock;
                return;
            }

            var imageView = new ImageDisplayView();
            _mainContentArea.Content = imageView;

            // The dummy PNG was created at 'FilePreviewer.Avalonia/test.png'.
            // When running from the project directory (e.g. /app/FilePreviewer.Avalonia),
            // a relative path should work.
            string imagePath = "test.png";

            // Absolute path for checking, in case relative path fails due to working directory issues.
            string absoluteImagePath = Path.GetFullPath(imagePath);

            if (!File.Exists(imagePath)) // Check relative path first
            {
                if (!File.Exists(absoluteImagePath)) // Then check absolute path
                {
                    string errorMessage = $"Test image not found. \nRelative path checked: '{imagePath}' \nAbsolute path checked: '{absoluteImagePath}'. \nEnsure 'test.png' is in the project root directory ('FilePreviewer.Avalonia') and the application's working directory is set correctly, or that the file is copied to the output folder.";
                    Console.WriteLine(errorMessage);
                    var textBlock = new TextBlock
                    {
                        Text = errorMessage,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Avalonia.Thickness(10)
                    };
                    _mainContentArea.Content = textBlock;
                    return;
                }
                // If absolute path exists, use it (means working directory was not the project root)
                imagePath = absoluteImagePath;
            }

            await imageView.LoadImageAsync(imagePath);
        }

        private async void TestShowPropertiesButton_Click(object? sender, RoutedEventArgs e) // Added method
        {
            string filePath = "test.bin"; // Use the dummy binary file
            string absoluteFilePath = Path.GetFullPath(filePath);

            if (!File.Exists(filePath))
            {
                if (!File.Exists(absoluteFilePath))
                {
                    string errorMessage = $"Test file not found for properties window. \nRelative path checked: '{filePath}' \nAbsolute path checked: '{absoluteFilePath}'. \nEnsure 'test.bin' is in the project root directory ('FilePreviewer.Avalonia') and accessible.";
                    Console.WriteLine(errorMessage);
                    if (_mainContentArea != null)
                    {
                        _mainContentArea.Content = new TextBlock
                        {
                            Text = errorMessage,
                            TextWrapping = TextWrapping.Wrap,
                            Margin = new Avalonia.Thickness(10)
                        };
                    }
                    else
                    {
                        this.Content = new TextBlock { Text = errorMessage, TextWrapping = TextWrapping.Wrap, Margin = new Avalonia.Thickness(10) };
                    }
                    return;
                }
                filePath = absoluteFilePath;
            }

            var propertiesWindow = new FilePropertiesView(filePath);
            // Show the window; since it's a dialog, we might want to show it as such,
            // but for now, just Show(). If we want to ShowDialog, it needs a parent window.
            // propertiesWindow.Show();
            await propertiesWindow.ShowDialog(this); // Show as a dialog ancentered on this MainWindow
        }
    }
}
