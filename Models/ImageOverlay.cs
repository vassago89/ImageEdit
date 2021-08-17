using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEdit.Models
{
    [Serializable]
    class ImageOverlay : Overlay
    {
        private BitmapSource _source;
        public BitmapSource Source 
        { 
            get => _source;
            set => SetProperty(ref _source, value);
        }

        public ImageOverlay(Rect rect, BitmapSource source) : base(rect)
        {
            Source = source;
        }
    }
}
