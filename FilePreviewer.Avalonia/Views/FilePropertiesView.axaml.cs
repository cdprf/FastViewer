using Avalonia.Controls;
using Avalonia.Interactivity; // For RoutedEventArgs
using FilePreviewer.Avalonia.ViewModels;
using System.Threading.Tasks; // For Task
// System.IO and System are already in ViewModel or not directly needed here now


namespace FilePreviewer.Avalonia.Views
{
    public partial class FilePropertiesView : Window
    {

        private readonly FilePropertiesViewModel _viewModel;

        // Constructor for XAML previewer design-time data
        public FilePropertiesView()
        {
            InitializeComponent();
            _viewModel = new FilePropertiesViewModel(); // Use the default constructor for design data
            DataContext = _viewModel;

        }

        public FilePropertiesView(string filePath)
        {
            InitializeComponent();
            _viewModel = new FilePropertiesViewModel();
            DataContext = _viewModel;
            // Asynchronously load properties. Fire and forget with error handling within LoadPropertiesAsync.
            // Or, if this window's lifecycle allows, await it in an event handler like 'Loaded'.
            // For now, calling it like this means the window might show "Loading..." briefly.
            _ = LoadPropertiesInternalAsync(filePath);
        }

        private async Task LoadPropertiesInternalAsync(string filePath)
        {
            // This method is called from the constructor context that cannot be async itself directly.
            // The actual async work is in the ViewModel.
            if (_viewModel != null)
            {
                await _viewModel.LoadPropertiesAsync(filePath);
            }
        }

        // The Loaded event can also be used to trigger async loading if preferred:
        // protected override async void OnLoaded(RoutedEventArgs e)
        // {
        //     base.OnLoaded(e);
        //     // If filePath was passed to constructor and stored in a field:
        //     // if (!string.IsNullOrEmpty(_filePathToLoad) && _viewModel != null)
        //     // {
        //     //     await _viewModel.LoadPropertiesAsync(_filePathToLoad);
        //     // }
        // }

        // FormatFileSize is now in the ViewModel
        // private string FormatFileSize(long bytes) { ... }
   

        private void OkButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
