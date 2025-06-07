using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FilePreviewer.Avalonia.Views
{
    public partial class ImageDisplayView : UserControl
    {
        public ImageDisplayView()
        {
            InitializeComponent();
        }

        public async Task LoadImageAsync(string imagePath)
        {
            // Ensure controls are available
            var imageControl = this.FindControl<Image>("DisplayedImage");
            var errorTextBlock = this.FindControl<TextBlock>("ErrorTextBlock");

            if (imageControl == null || errorTextBlock == null)
            {
                // This might happen if called before controls are initialized, though unlikely for a public method.
                // Consider logging or a specific exception.
                Console.WriteLine("ImageDisplayView: Controls not found. Has InitializeComponent been called and completed?");
                return;
            }

            errorTextBlock.IsVisible = false;
            imageControl.Source = null; // Clear previous image

            if (string.IsNullOrEmpty(imagePath))
            {
                errorTextBlock.Text = "Error: Image path is null or empty.";
                errorTextBlock.IsVisible = true;
                return;
            }

            try
            {
                // Check if file exists before attempting to load
                if (!File.Exists(imagePath))
                {
                    errorTextBlock.Text = $"Error: Image file not found at '{imagePath}'.";
                    errorTextBlock.IsVisible = true;
                    return;
                }

                // Load the bitmap in a background thread
                Bitmap? bitmap = null;
                await Task.Run(() =>
                {
                    try
                    {
                        bitmap = new Bitmap(imagePath);
                    }
                    catch (Exception ex) // Catch exceptions during Bitmap creation
                    {
                        Console.WriteLine($"ImageDisplayView: Error creating Bitmap: {ex.Message}");
                        // errorTextBlock.Text is set on UI thread later
                    }
                });

                if (bitmap != null)
                {
                    imageControl.Source = bitmap;
                }
                else
                {
                    errorTextBlock.Text = $"Error: Could not load image from '{imagePath}'. File might be corrupted or not a valid image format.";
                    errorTextBlock.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                // Catch other potential exceptions (e.g., related to UI updates if any were here)
                errorTextBlock.Text = $"An unexpected error occurred: {ex.Message}";
                errorTextBlock.IsVisible = true;
                Console.WriteLine($"ImageDisplayView: Unexpected error in LoadImageAsync: {ex.Message}");
            }
        }
    }
}
