using ImageEdit.Helpers;
using ImageEdit.Helpers.Command;
using ImageEdit.Models;
using ImageEdit.Stores;
using ImageEdit.ViewModels;
using OpenCvSharp.Extensions;
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
    public partial class BlurView : UserControl
    {
        bool _isPressed;
        Point _pos;

        List<ImageOverlay> _curOverlays;

        BlurViewModel _viewModel => this.DataContext as BlurViewModel;

        public BlurView()
        {
            InitializeComponent();

            _curOverlays = new List<ImageOverlay>();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            _isPressed = true;
            OverlayStore.Instance.TextFocusLost?.Invoke();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(Canvas);
            _viewModel.CurPos = new Point(pos.X - _viewModel.Radius / 2, pos.Y - _viewModel.Radius / 2);
            Cursor = Cursors.None;

            if (_isPressed)
            {
                var sourcePos = e.GetPosition(Grid);
                var sourceRect = new Rect(sourcePos.X - _viewModel.Radius / 2, sourcePos.Y - _viewModel.Radius / 2, _viewModel.Radius, _viewModel.Radius);

                var targetRect = new Rect(_viewModel.CurPos.X, _viewModel.CurPos.Y, _viewModel.Radius, _viewModel.Radius);
                    var processed = Algorithms.Algorithms.Blur(
                    ImageStore.Instance.GetMat(targetRect, sourceRect), new OpenCvSharp.Rect((int)_viewModel.CurPos.X, (int)_viewModel.CurPos.Y, (int)_viewModel.Radius, (int)_viewModel.Radius));

                if (processed == null)
                    return;

                var overlay = new ImageOverlay(targetRect, processed.ToBitmapSource());
                OverlayStore.Instance.Backgrounds.Add(overlay);
                _curOverlays.Add(overlay);
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPressed = false;
            var overlays = new List<ImageOverlay>();
            overlays.AddRange(_curOverlays);
            _curOverlays.Clear();

            EditStore.Instance.CommandStack.Push(new Command<IEnumerable<ImageOverlay>>(
                    overlays,
                    os =>
                    {
                        foreach (var o in os)
                            OverlayStore.Instance.Backgrounds.Add(o);
                    },
                    os =>
                    {
                        foreach (var o in os)
                            OverlayStore.Instance.Backgrounds.Remove(o);
                    }), false);
        }

        private void NumericButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                OverlayStore.Instance.TextFocusLost?.Invoke();
            }
        }
    }
}
