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
            var canvasPos = e.GetPosition(ImageStore.Instance.Image);

            EditStore.Instance.Rect = Rect.Empty;

            foreach (var overlay in OverlayStore.Instance.Overlays)
            {
                if (overlay.Rect.Contains(canvasPos))
                {
                    EditStore.Instance.Rect = overlay.Rect;
                    break;
                }
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (EditStore.Instance.Rect == null)
                return;

            var canvasPos = e.GetPosition(ImageStore.Instance.Image);

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    foreach (var overlay in OverlayStore.Instance.Overlays)
                    {
                        if (overlay.Rect.Contains(canvasPos))
                        {
                            OverlayStore.Instance.Selected = overlay;
                            if (overlay is TextOverlay)
                            {
                                var text = overlay as TextOverlay;
                                text.TextBox?.Focus();
                            }
                            return;
                        }
                    }

                    OverlayStore.Instance.Selected = null;

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