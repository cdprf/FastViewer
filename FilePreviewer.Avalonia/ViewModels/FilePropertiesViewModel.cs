using System.ComponentModel; // Required for INotifyPropertyChanged (optional for now, but good practice)
using System.IO;
using System.Threading.Tasks;
using System; // For DateTime and Math

namespace FilePreviewer.Avalonia.ViewModels
{
    // If we were to implement INotifyPropertyChanged:
    // public class FilePropertiesViewModel : INotifyPropertyChanged
    // {
    //     public event PropertyChangedEventHandler? PropertyChanged;
    //     protected virtual void OnPropertyChanged(string propertyName)
    //     {
    //         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //     }

    //     private string? _fileName;
    //     public string? FileName { get => _fileName; set { _fileName = value; OnPropertyChanged(nameof(FileName)); } }

    //     private string? _filePath;
    //     public string? FilePath { get => _filePath; set { _filePath = value; OnPropertyChanged(nameof(FilePath)); } }

    //     private string? _fileSize;
    //     public string? FileSize { get => _fileSize; set { _fileSize = value; OnPropertyChanged(nameof(FileSize)); } }

    //     private string? _creationTime;
    //     public string? CreationTime { get => _creationTime; set { _creationTime = value; OnPropertyChanged(nameof(CreationTime)); } }

    //     private string? _lastModifiedTime;
    //     public string? LastModifiedTime { get => _lastModifiedTime; set { _lastModifiedTime = value; OnPropertyChanged(nameof(LastModifiedTime)); } }

    //     private bool _isLoading;
    //     public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); } }

    //     private string? _errorMessage;
    //     public string? ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); } }

    // For simplicity as per subtask (direct properties, INotifyPropertyChanged not strictly required yet):
    public class FilePropertiesViewModel : INotifyPropertyChanged // Added INotifyPropertyChanged for IsLoading binding
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string? _fileName;
        public string? FileName { get => _fileName; set { _fileName = value; OnPropertyChanged(nameof(FileName)); } }

        private string? _filePath;
        public string? FilePath { get => _filePath; set { _filePath = value; OnPropertyChanged(nameof(FilePath)); } }

        private string? _fileSize;
        public string? FileSize { get => _fileSize; set { _fileSize = value; OnPropertyChanged(nameof(FileSize)); } }

        private string? _creationTime;
        public string? CreationTime { get => _creationTime; set { _creationTime = value; OnPropertyChanged(nameof(CreationTime)); } }

        private string? _lastModifiedTime;
        public string? LastModifiedTime { get => _lastModifiedTime; set { _lastModifiedTime = value; OnPropertyChanged(nameof(LastModifiedTime)); } }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
                OnPropertyChanged(nameof(IsNotLoading)); // For button IsEnabled
            }
        }
        public bool IsNotLoading => !IsLoading;


        private string? _errorMessage;
        public string? ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); OnPropertyChanged(nameof(HasErrorMessage)); } }
        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);


        // Constructor for design-time data or initial state
        public FilePropertiesViewModel()
        {
            // Design-time data or default values if needed by XAML previewer
            FileName = "Loading...";
            FilePath = string.Empty;
            FileSize = string.Empty;
            CreationTime = string.Empty;
            LastModifiedTime = string.Empty;
            IsLoading = true; // Initial state often is loading
        }

        public async Task LoadPropertiesAsync(string filePathParam)
        {
            IsLoading = true;
            ErrorMessage = null;
            try
            {
                // Simulate async file access
                await Task.Delay(10); // Small delay to ensure it's truly async and allow UI to update IsLoading

                if (string.IsNullOrEmpty(filePathParam) || !File.Exists(filePathParam))
                {
                    FileName = "Error";
                    FilePath = "File not found or path is invalid.";
                    FileSize = "N/A";
                    CreationTime = "N/A";
                    LastModifiedTime = "N/A";
                    ErrorMessage = FilePath;
                }
                else
                {
                    var fileInfo = new FileInfo(filePathParam);
                    FileName = fileInfo.Name;
                    FilePath = fileInfo.FullName;
                    FileSize = FormatFileSize(fileInfo.Length);
                    CreationTime = fileInfo.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ss UTC");
                    LastModifiedTime = fileInfo.LastWriteTimeUtc.ToString("yyyy-MM-dd HH:mm:ss UTC");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading file properties: {ex.Message}");
                FileName = "Error";
                FilePath = $"An error occurred: {ex.Message}";
                FileSize = "N/A";
                CreationTime = "N/A";
                LastModifiedTime = "N/A";
                ErrorMessage = FilePath;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private string FormatFileSize(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes >= max) // Use >= for the largest unit
                    return string.Format("{0:0.##} {1}", decimal.Divide(bytes, max), order);
                if (max > 1) // Avoid dividing by zero or negative if something went wrong with scale
                    max /= scale;
            }
            return "0 Bytes";
        }
    }
}
