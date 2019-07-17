using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace PhotoViewer.App {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private string _folder = @"D:\Temp\ImageTest";
        private string _goodFolderName = @"!Gode";
        private CachedImageList _imageList;        

        public MainWindow() {
            InitializeComponent();
            _imageList = new CachedImageList(_folder);
            SetCurrentImage(_imageList.Current);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Right) {
                SetCurrentImage(_imageList.Next);
            }
            if (e.Key == Key.Left) {
                SetCurrentImage(_imageList.Previous);
            }
            if (e.Key == Key.Enter) {
                CopyImage(_imageList.Current);
            }
            if (e.Key == Key.Delete) {
                DeleteImage(_imageList.Current);
            }
        }

        private void SetCurrentImage(CachedImage image) {
            Title = image.FilePath;
            StatusText.Text = "";
            if (!image.Failed) {
                MainImage.Source = image.Image;
            }
            CachedImage previousImage = _imageList.PeekPrevious;
            if (!previousImage.Failed) {
                PreviousImage.Source = previousImage.Image;
            }
            CachedImage nextImage = _imageList.PeekNext;
            if (!nextImage.Failed) {
                NextImage.Source = nextImage.Image;
            }
            CachedImage nextNextImage = _imageList.PeekNextNext;
            if (!nextNextImage.Failed) {
                NextNextImage.Source = nextNextImage.Image;
            }
        }

        private void DeleteImage(CachedImage image) {
            if (!image.Failed) {                
                string nameWithoutExt = Path.GetFileNameWithoutExtension(image.FilePath);
                string wildcardName = $"{nameWithoutExt}.*";
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {wildcardName}?", "Delete Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes) {
                    image.Unload();
                    foreach (string searchFilepath in Directory.EnumerateFiles(_folder, nameWithoutExt + ".*")) {
                        FileSystem.DeleteFile(searchFilepath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                    }
                    _imageList.RemoveCurrent();
                    SetCurrentImage(_imageList.Current);
                    StatusText.Text = $"{wildcardName} was successfully deleted to recycle bin";
                }
            }
        }

        private void CopyImage(CachedImage image) {
            if (!image.Failed) {
                FileInfo sourceFileInfo = new FileInfo(image.FilePath);
                string destinationFolder = Path.Combine(sourceFileInfo.DirectoryName, _goodFolderName);
                string nameWithoutExt = Path.GetFileNameWithoutExtension(image.FilePath);
                foreach (string searchFilepath in Directory.EnumerateFiles(_folder, nameWithoutExt + ".*")) {
                    FileInfo searchFileInfo = new FileInfo(searchFilepath);
                    string destinationFilePath = Path.Combine(destinationFolder, searchFileInfo.Name);
                    File.Copy(searchFilepath, destinationFilePath, true);
                }
                StatusText.Text = $"{sourceFileInfo.Name} was copied to {_goodFolderName}";
            }
        }

        private void Window_Drop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {                
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0) {
                    _imageList.SetIndex(files[0]);
                    SetCurrentImage(_imageList.Current);
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) {
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e) {
        }
    }
}
