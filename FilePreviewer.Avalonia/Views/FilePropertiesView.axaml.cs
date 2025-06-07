using Avalonia.Controls;
using Avalonia.Interactivity; // For RoutedEventArgs
using FilePreviewer.Avalonia.ViewModels;
using System;
using System.IO; // For FileInfo

namespace FilePreviewer.Avalonia.Views
{
    public partial class FilePropertiesView : Window
    {
        // Default constructor for XAML previewer and potential parameterless instantiation
        public FilePropertiesView()
        {
            InitializeComponent();
            // Provide design-time or default data if filePath is not available
            DataContext = new FilePropertiesViewModel
            {
                FileName = "N/A (Design Mode)",
                FilePath = "No file selected",
                FileSize = "0 bytes",
                CreationTime = DateTime.MinValue.ToString("g"),
                LastModifiedTime = DateTime.MinValue.ToString("g")
            };
        }

        public FilePropertiesView(string filePath)
        {
            InitializeComponent();
            LoadFileProperties(filePath);
        }

        private void LoadFileProperties(string filePath)
        {
            var viewModel = new FilePropertiesViewModel();

            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    viewModel.FileName = "Error";
                    viewModel.FilePath = "File not found or path is invalid.";
                    viewModel.FileSize = "N/A";
                    viewModel.CreationTime = "N/A";
                    viewModel.LastModifiedTime = "N/A";
                }
                else
                {
                    var fileInfo = new FileInfo(filePath);
                    viewModel.FileName = fileInfo.Name;
                    viewModel.FilePath = fileInfo.FullName;
                    viewModel.FileSize = FormatFileSize(fileInfo.Length);
                    viewModel.CreationTime = fileInfo.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ss UTC");
                    viewModel.LastModifiedTime = fileInfo.LastWriteTimeUtc.ToString("yyyy-MM-dd HH:mm:ss UTC");
                }
            }
            catch (Exception ex)
            {
                // Log exception ex.Message
                viewModel.FileName = "Error";
                viewModel.FilePath = $"An error occurred: {ex.Message}";
                viewModel.FileSize = "N/A";
                viewModel.CreationTime = "N/A";
                viewModel.LastModifiedTime = "N/A";
            }

            DataContext = viewModel;
        }

        private string FormatFileSize(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);
                max /= scale;
            }
            return "0 Bytes";
        }

        private void OkButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
