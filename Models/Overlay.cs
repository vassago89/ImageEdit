using ImageEdit.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageEdit.Models
{
    abstract class Overlay : BindableBase
    {
        private event EventHandler _rectChanged;
        public event EventHandler RectChanged
        {
            add => _rectChanged += value;
            remove => _rectChanged -= value;
        }

        private Rect _rect;
        public Rect Rect
        {
            get => _rect;
            set
            {
                SetProperty(ref _rect, value);
                OnPropertyChanged("TopLeft");
                OnPropertyChanged("Top");
                OnPropertyChanged("TopRight");

                OnPropertyChanged("Left");
                OnPropertyChanged("Right");

                OnPropertyChanged("BottomLeft");
                OnPropertyChanged("Bottom");
                OnPropertyChanged("BottomRight");

                OnPropertyChanged("Translate");
                _rectChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private double _opacity;
        public double Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, value);
        }

        private double _opacityPercent;
        public double OpacityPercent
        {
            get => _opacityPercent;
            set
            {
                SetProperty(ref _opacityPercent, value);
                Opacity = _opacityPercent / 100.0;
            }
        }

        public Point TopLeft => new Point(_rect.X, _rect.Y);
        public Point Top => new Point(_rect.X + (_rect.Width / 2), _rect.Y);
        public Point TopRight => new Point(_rect.Right, _rect.Y);

        public Point Left => new Point(_rect.X, _rect.Y + (_rect.Height / 2));
        public Point Right => new Point(_rect.Right, _rect.Y + (_rect.Height / 2));

        public Point BottomLeft => new Point(_rect.X, _rect.Bottom);
        public Point Bottom => new Point(_rect.X + (_rect.Width / 2), _rect.Bottom);
        public Point BottomRight => new Point(_rect.Right, _rect.Bottom);

        public Point Translate => new Point(_rect.X + (_rect.Width / 2), _rect.Top - 40);

        public Overlay(Rect rect)
        {
            Rect = rect;
            OpacityPercent = 100;
            Opacity = 1;
        }
    }
}
