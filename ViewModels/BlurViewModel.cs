using ImageEdit.Helpers;
using ImageEdit.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageEdit.ViewModels
{
    class BlurViewModel : BindableBase
    {
        private Point _curPos;
        public Point CurPos
        {
            get => _curPos;
            set => SetProperty(ref _curPos, value);
        }

        private double _radius = 15;
        public double Radius
        {
            get => _radius;
            set => SetProperty(ref _radius, value);
        }

        public OverlayStore OverlayStore => OverlayStore.Instance;
        public ImageStore ImageStore => ImageStore.Instance;
    }
}
