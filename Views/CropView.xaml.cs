using ImageEdit.Helpers.Command;
using ImageEdit.Models;
using ImageEdit.Stores;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageEdit.Views
{
    /// <summary>
    /// CropView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CropView : UserControl
    {
        bool _isPressed;
        Point _pos;

        public CropView()
        {
            InitializeComponent();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            _isPressed = true;
            _pos = e.GetPosition(Canvas);
            EditStore.Instance.CropRect = new Rect(_pos.X, _pos.Y, 0, 0);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPressed == false)
                return;

            var pos = e.GetPosition(Canvas);

            EditStore.Instance.CropRect 
                = new Rect(Math.Min(_pos.X, pos.X), Math.Min(_pos.Y, pos.Y), Math.Abs(_pos.X - pos.X), Math.Abs(_pos.Y - pos.Y));
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            _isPressed = false;
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPressed = false;

            var rect = EditStore.Instance.CropRect;
            var sourceRect = new Rect(0, 0, ImageStore.Instance.Source.PixelWidth, ImageStore.Instance.Source.PixelHeight);
            rect.Intersect(sourceRect);

            var opencvRect = new OpenCvSharp.Rect((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

            EditStore.Instance.CropRect = Rect.Empty;

            if (rect.Width <= 0 || rect.Height <= 0 || opencvRect.Width <= 0 || opencvRect.Height <= 0)
                return;

            var sub = ImageStore.Instance.Mat.SubMat(new OpenCvSharp.Rect((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height));

            var overlay = new ImageOverlay(rect, sub.ToBitmapSource());
            EditStore.Instance.CommandStack.Push(new Command<Overlay>(
                    overlay,
                    o =>
                    {
                        OverlayStore.Instance.Overlays.Add(o);
                    },
                    o =>
                    {
                        OverlayStore.Instance.Overlays.Remove(o);
                        OverlayStore.Instance.Selected = null;
                    }));

            OverlayStore.Instance.Selected = overlay;
            EditStore.Instance.IsNone = true;
        }
    }
}
