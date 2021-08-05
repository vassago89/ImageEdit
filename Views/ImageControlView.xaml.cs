using ImageEdit.Helpers.Command;
using ImageEdit.Models;
using ImageEdit.Stores;
using ImageEdit.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// ImageControlView.xaml에 대한 상호 작용 논리
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class ImageControlView : UserControl
    {
        Point _rectPos;

        public ImageControlView()
        {
            InitializeComponent();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (EditStore.Instance.IsPressed == false)
                return;

            var canvasPos = e.GetPosition(ImageStore.Instance.Image);

            switch (EditStore.Instance.EditMode)
            {
                case Enums.EditMode.None:
                    break;
                case Enums.EditMode.Text:
                    var sourceRect = new Rect(0, 0, ImageStore.Instance.Source.PixelWidth, ImageStore.Instance.Source.PixelHeight);
                    var rect = new Rect(
                        Math.Min(canvasPos.X, _rectPos.X),
                        Math.Min(canvasPos.Y, _rectPos.Y),
                        Math.Abs(canvasPos.X - _rectPos.X),
                        Math.Abs(canvasPos.Y - _rectPos.Y));

                    rect.Intersect(sourceRect);

                    EditStore.Instance.Rect = rect;
                    break;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    switch (EditStore.Instance.EditMode)
                    {
                        case Enums.EditMode.None:
                            OverlayStore.Instance.Selected = null;
                            break;
                        case Enums.EditMode.Text:
                            _rectPos = e.GetPosition(ImageStore.Instance.Image);
                            if (_rectPos.X < 0 || _rectPos.Y < 0
                                || _rectPos.X >= ImageStore.Instance.Source.PixelWidth
                                || _rectPos.Y >= ImageStore.Instance.Source.PixelHeight)
                                return;

                            EditStore.Instance.IsPressed = true;
                            EditStore.Instance.Rect = new Rect(_rectPos.X, _rectPos.Y, 1, 1);
                            break;
                    }
                    break;
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (EditStore.Instance.IsPressed == false)
                return;

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    switch (EditStore.Instance.EditMode)
                    {
                        case Enums.EditMode.None:
                            break;
                        case Enums.EditMode.Text:
                            if (EditStore.Instance.Rect == null)
                                break;

                            EditStore.Instance.CommandStack.Push(new Command<Overlay>(
                                new Models.TextOverlay(EditStore.Instance.Rect),
                                overlay =>
                                {
                                    OverlayStore.Instance.Selected = overlay;
                                    OverlayStore.Instance.Overlays.Add(overlay);
                                },
                                overlay =>
                                {
                                    OverlayStore.Instance.Overlays.Remove(overlay);
                                    OverlayStore.Instance.Selected = null;
                                }));
                            
                            EditStore.Instance.Rect = Rect.Empty;
                            EditStore.Instance.IsNone = true;
                            break;
                    }
                    break;
            }

            EditStore.Instance.IsPressed = false;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            EditStore.Instance.IsPressed = false;
            EditStore.Instance.Rect = Rect.Empty;
        }
    }
}