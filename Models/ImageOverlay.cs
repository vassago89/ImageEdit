using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEdit.Models
{
    class ImageOverlay : Overlay
    {
        public BitmapSource Source { get; }

        public ImageOverlay(Rect rect, BitmapSource source) : base(rect)
        {
            Source = source;
        }
    }
}
