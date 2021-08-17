using ImageEdit.Helpers;
using ImageEdit.Models;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        public Action TextFocusLost { get; set; }
        public Action<NotifyCollectionChangedEventArgs> OverlayChanged { get; set; }

        private EventHandler _textChanged;
        public event EventHandler TextChanged 
        {
            add => _textChanged += value;
            remove => _textChanged -= value;
        }

        private ObservableCollection<ImageOverlay> _backgrounds;
        public ObservableCollection<ImageOverlay> Backgrounds
        {
            get => _backgrounds;
            set => SetProperty(ref _backgrounds, value);
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

        private bool _isTextFocused;
        public bool IsTextFocused
        {
            get => _isTextFocused;
            set
            {
                SetProperty(ref _isTextFocused, value);
                if (_isTextFocused == false)
                    TextFocusLost?.Invoke();
            }
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

            _backgrounds = new ObservableCollection<ImageOverlay>();
            BindingOperations.EnableCollectionSynchronization(_backgrounds, new object());

            _overlays.CollectionChanged += _overlays_CollectionChanged;
        }

        private void _overlays_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OverlayChanged?.Invoke(e);
        }

        public void ZoomChange() => _textChanged?.Invoke(this, EventArgs.Empty);

        public Rect GetRegion()
        {
            var merged = new List<Overlay>();
            merged.AddRange(_overlays);
            merged.AddRange(_backgrounds);

            if (merged.Count == 0)
                return Rect.Empty;

            var left = merged.Min(t => t.Rect.Left);
            var right = merged.Max(t => t.Rect.Right);

            var top = merged.Min(t => t.Rect.Top);
            var bottom = merged.Max(t => t.Rect.Bottom);

            return new Rect(new Point(left, top), new Point(right, bottom));
        }
    }
}
