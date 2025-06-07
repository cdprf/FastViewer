using System.ComponentModel; // Required for INotifyPropertyChanged (optional for now, but good practice)

namespace FilePreviewer.Avalonia.ViewModels
{
    // For now, a simple class. Can implement INotifyPropertyChanged later if needed.
    // public class FilePropertiesViewModel : INotifyPropertyChanged
    public class FilePropertiesViewModel
    {
        // public event PropertyChangedEventHandler? PropertyChanged;

        // protected void OnPropertyChanged(string propertyName)
        // {
        //     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        // }

        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? FileSize { get; set; }
        public string? CreationTime { get; set; }
        public string? LastModifiedTime { get; set; }

        // Constructor for design-time data
        public FilePropertiesViewModel()
        {
            FileName = "example.txt";
            FilePath = "C:\\path\\to\\your\\files\\example.txt";
            FileSize = "123 KB";
            CreationTime = "2023-01-01 10:00:00";
            LastModifiedTime = "2023-01-15 14:30:00";
        }
    }
}
