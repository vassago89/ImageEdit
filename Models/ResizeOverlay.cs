using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEdit.Models
{
    class ResizeOverlay : ImageOverlay
    {
        public ResizeOverlay(Rect rect, BitmapSource source) : base(rect, source)
        {

        }
    }
}
