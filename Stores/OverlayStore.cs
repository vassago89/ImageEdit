using ImageEdit.Helpers;
using ImageEdit.Models;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEdit.Stores
{
    class OverlayStore : BindableSingleTon<OverlayStore>
    {
        private EventHandler _textChanged;
        public event EventHandler TextChanged 
        {
            add => _textChanged += value;
            remove => _textChanged -= value;
        }

        private ObservableCollection<Overlay> _overlays;
        public ObservableCollection<Overlay> Overlays
        {
            get => _overlays;
            set => SetProperty(ref _overlays, value);
        }

        private bool _isTextSelected;
        public bool IsTextSelected
        {
            get => _isTextSelected;
            private set => SetProperty(ref _isTextSelected, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            private set => SetProperty(ref _isSelected, value);
        }

        private bool _isDeselected;
        public bool IsDeselected
        {
            get => _isDeselected;
            private set => SetProperty(ref _isDeselected, value);
        }

        private Overlay _selected;
        public Overlay Selected
        {
            get => _selected;
            set
            {
                if (_selected != null)
                    _selected.RectChanged -= _textChanged;

                SetProperty(ref _selected, value);

                IsSelected = _selected != null;
                IsDeselected = !IsSelected;
                IsTextSelected = _selected != null && _selected is TextOverlay;

                if (_selected != null)
                    _selected.RectChanged += _textChanged;

                _textChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public OverlayStore()
        {
            _overlays = new ObservableCollection<Overlay>();
            BindingOperations.EnableCollectionSynchronization(_overlays, new object());
        }

        public void ZoomChange() => _textChanged?.Invoke(this, EventArgs.Empty);

        public Rect GetRegion()
        {
            if (_overlays.Count == 0)
                return Rect.Empty;

            var left = _overlays.Min(t => t.Rect.Left);
            var right = _overlays.Max(t => t.Rect.Right);

            var top = _overlays.Min(t => t.Rect.Top);
            var bottom = _overlays.Max(t => t.Rect.Bottom);

            return new Rect(new Point(left, top), new Point(right, bottom));
        }
    }
}
