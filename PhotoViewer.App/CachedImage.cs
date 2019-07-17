using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace PhotoViewer.App {
    public class CachedImage : IDisposable {        
        private FileStream _fileStream;
        private BitmapImage _bitmap;

        public CachedImage(string filepath) {
            FilePath = filepath;
        }

        public void Load() {
            if (Image == null) {
                _bitmap = new BitmapImage();
                _fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, false);
                try {
                    _bitmap.BeginInit();
                    _bitmap.CreateOptions = BitmapCreateOptions.None;
                    _bitmap.CacheOption = BitmapCacheOption.None;
                    _bitmap.StreamSource = _fileStream;
                    _bitmap.DecodePixelWidth = 1920;
                    _bitmap.EndInit();
                    _bitmap.Freeze();
                }catch (Exception ex) {
                    Console.WriteLine(ex);
                    Failed = true;
                }
                Image = _bitmap;
            }
        }

        public void Unload() {
            if (_fileStream != null) {
                _fileStream.Close();
                _fileStream.Dispose();
                _fileStream = null;
                _bitmap = null;
                Image = null;
                Failed = false;
                GC.Collect();
            }
        }

        public void Dispose() {
            Unload();
        }

        public BitmapImage Image { get; set; }
        public bool Failed { get; set; }
        public string FilePath { get; set; }
    }
}
