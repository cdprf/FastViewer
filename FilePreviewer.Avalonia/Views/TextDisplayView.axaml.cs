using Avalonia.Controls;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FilePreviewer.Avalonia.Views
{
    public partial class TextDisplayView : UserControl
    {
        public TextDisplayView()
        {
            InitializeComponent();
        }

        public async Task LoadTextAsync(string filePath, int maxLines = 100)
        {
            var errorTextBlock = this.FindControl<TextBlock>("ErrorTextBlock");
            var textContentBox = this.FindControl<TextBox>("TextContent");

            if (errorTextBlock == null || textContentBox == null)
            {
                Console.WriteLine("TextDisplayView: Critical error - UI controls not found.");
                // Cannot display error in UI if ErrorTextBlock is null
                return;
            }

            errorTextBlock.IsVisible = false;
            textContentBox.Text = string.Empty;

            if (string.IsNullOrEmpty(filePath))
            {
                errorTextBlock.Text = "Error: File path is null or empty.";
                errorTextBlock.IsVisible = true;
                return;
            }

            try
            {
                if (!File.Exists(filePath))
                {
                    errorTextBlock.Text = $"Error: File not found at '{filePath}'.";
                    errorTextBlock.IsVisible = true;
                    return;
                }

                var sb = new StringBuilder();
                int linesRead = 0;

                // Read file line by line asynchronously
                using (var reader = new StreamReader(filePath))
                {
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null && linesRead < maxLines)
                    {
                        sb.AppendLine(line);
                        linesRead++;
                    }
                }

                textContentBox.Text = sb.ToString();

                if (linesRead == maxLines && !string.IsNullOrEmpty(await File.ReadAllLinesAsync(filePath).ContinueWith(t => t.Result.Length > maxLines ? "more" : null)))
                {
                    // Optional: Add a note if content was truncated, though this check is a bit inefficient.
                    // A better way would be to check if reader.EndOfStream was false after the loop.
                    // For now, the spec is just to load maxLines.
                    // errorTextBlock.Text = $"Displayed first {maxLines} lines.";
                    // errorTextBlock.IsVisible = true; // Could be an info message rather than error
                }

            }
            catch (IOException ex)
            {
                errorTextBlock.Text = $"IO Error reading file '{filePath}': {ex.Message}";
                errorTextBlock.IsVisible = true;
                Console.WriteLine($"TextDisplayView: IOException - {ex.Message}");
            }
            catch (Exception ex)
            {
                errorTextBlock.Text = $"An unexpected error occurred while reading file '{filePath}': {ex.Message}";
                errorTextBlock.IsVisible = true;
                Console.WriteLine($"TextDisplayView: Exception - {ex.Message}");
            }
        }
    }
}
