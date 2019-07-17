using System;
using System.Collections.Generic;
using System.IO;

namespace PhotoViewer.App {
    public class CachedImageList {
        private List<CachedImage> _images = new List<CachedImage>();
        private int _index = 0;

        public CachedImageList(string folder) {
            foreach (string filepath in Directory.EnumerateFiles(folder, "*.jpg")) {
                _images.Add(new CachedImage(filepath));
            }
        }

        public void SetIndex(string filePath) {
            for (int i = 0; i < _images.Count; i++) {
                CachedImage image = _images[i];
                image.Unload();
                if (string.Equals(image.FilePath, filePath, StringComparison.OrdinalIgnoreCase)) {
                    _index = i;
                }
            }
            PopulateIndex();
        }

        public void RemoveCurrent() {
            _images.RemoveAt(_index);
        }

        private int getNextIndex(int index, int offset = 1) {
            while (offset > 0) {
                index++;
                if (index > _images.Count - 1) {
                    index = 0;
                }
                offset--;
            }
            return index;
        }

        private int getPreviousIndex(int index, int offset = 1) {
            while (offset > 0) {
                index--;
                if (index < 0) {
                    index = _images.Count - 1;
                }
                offset--;
            }
            return index;
        }

        private void PopulateIndex() {
            _images[getPreviousIndex(_index)].Load();
            _images[getNextIndex(_index)].Load();
        }

        private CachedImage GetImage(int index) {
            CachedImage image = _images[index];
            image.Load();
            return image;
        }

        public IReadOnlyList<CachedImage> Images {
            get {
                return _images;
            }
        }

        public CachedImage Current {
            get {
                if (_index > _images.Count - 1) {
                    return Next;
                }
                return GetImage(_index);
            }
        }

        public CachedImage Next {
            get {
                _index = getNextIndex(_index);
                _images[getNextIndex(_index)].Load();
                _images[getPreviousIndex(_index, 2)].Unload();
                return Current;
            }
        }

        public CachedImage Previous {
            get {
                _index = getPreviousIndex(_index);                
                _images[getPreviousIndex(_index)].Load();
                _images[getNextIndex(_index, 2)].Unload();
                return Current;
            }
        }

        public CachedImage PeekPrevious {
            get {
                int index = getPreviousIndex(_index);
                return GetImage(index);
            }
        }

        public CachedImage PeekNext {
            get {
                int index = getNextIndex(_index);
                return GetImage(index);
            }
        }

        public CachedImage PeekNextNext {
            get {
                int index = getNextIndex(_index, 2);
                return GetImage(index);
            }
        }
    }
}
