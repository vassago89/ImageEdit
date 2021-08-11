using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using ImageEdit.Helpers;
using ImageEdit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using OpenCvSharp.Extensions;
using System.Runtime.InteropServices;
using System.Windows;

namespace ImageEdit.Stores
{
    class ImageStore : BindableSingleTon<ImageStore>
    {
        public ZoomService ZoomService { get; } = new ZoomService();

        private BitmapSource _source;
        public BitmapSource Source
        {
            get => _source;
            private set => SetProperty(ref _source, value);
        }

        private BitmapSource _background;
        public BitmapSource Background
        {
            get => _background;
            private set => SetProperty(ref _background, value);
        }

        public Canvas Image { get; private set; }
        public Canvas Overlay { get; private set; }

        private Mat _mat;
        public Mat Mat => _mat;

        public void Set(Mat mat)
        {
            OverlayStore.Instance.Selected = null;
            OverlayStore.Instance.Overlays.Clear();
            EditStore.Instance.Rect = System.Windows.Rect.Empty;
            EditStore.Instance.CommandStack.Clear();

            _mat = mat;

            var source = _mat.ToBitmapSource();
            source.Freeze();
            Source = source;
            ZoomFit();
        }

        public void Set(Uri url)
        {
            OverlayStore.Instance.Selected = null;
            OverlayStore.Instance.Overlays.Clear();
            EditStore.Instance.Rect = System.Windows.Rect.Empty;
            EditStore.Instance.CommandStack.Clear();

            var source = new BitmapImage(url);
            if (source.IsDownloading)
            {
                source.DownloadCompleted += (sender, e) =>
                {
                    source.Freeze();
                    Source = source;
                    ZoomFit();
                };
            }
            else
            {
                source.Freeze();
                Source = source;
                ZoomFit();
            }
        }

        public void SetBackground(Mat mat)
        {
            if (_source == null)
                return;

            var background = mat.Resize(new OpenCvSharp.Size(_source.Width, _source.Height)).ToBitmapSource();
            background.Freeze();

            Background = background;
        }

        public void SetBackground(Uri url)
        {
            if (_source == null)
                return;

            var source = new BitmapImage(url);
            source.DownloadCompleted += (sender, e) =>
            {
                source.Freeze();

                var background = source.ToMat().Resize(new OpenCvSharp.Size(_source.Width, _source.Height)).ToBitmapSource();
                background.Freeze();

                Background = background;
            };
        }

        private void Render(RenderTargetBitmap target, Canvas canvas, System.Windows.Rect rect)
        {
            var brush = new VisualBrush(canvas);

            var visual = new DrawingVisual();
            var drawingContext = visual.RenderOpen();

            drawingContext.DrawRectangle(
                brush,
                null,
                rect);

            drawingContext.Close();

            target.Render(visual);
        }

        public System.Drawing.Bitmap Get()
        {
            if (_source == null || Image == null || Overlay == null)
                return null;

            var bitmap = new RenderTargetBitmap((int)(_source.Width), (int)(_source.Height), _source.DpiX, _source.DpiY, PixelFormats.Pbgra32);
            Render(bitmap, Image, new System.Windows.Rect(0, 0, _source.Width, _source.Height));
            Render(bitmap, Overlay, OverlayStore.Instance.GetRegion());

            OverlayStore.Instance.Selected = null;
            OverlayStore.Instance.Overlays.Clear();

            return bitmap.ToBitmap();
        }

        public void ZoomFit(BitmapSource source = null)
        {
            if (Source == null || Image == null)
                return;

            if (source == null)
                source = Source;

            ZoomService.ZoomFit(
                Image.ActualWidth,
                Image.ActualHeight,
                source.PixelWidth,
                source.PixelHeight);

            ZoomOut();
        }

        public void ZoomIn()
        {
            if (Source == null || Image == null)
                return;

            ZoomService.ZoomIn(
                Image.ActualWidth,
                Image.ActualHeight);
        }

        public void ZoomOut()
        {
            if (Source == null || Image == null)
                return;

            ZoomService.ZoomOut(
                Image.ActualWidth,
                Image.ActualHeight);
        }

        public void SetCanvas(Canvas image, Canvas overlay)
        {
            Image = image;
            Overlay = overlay;
        }
    }
}
