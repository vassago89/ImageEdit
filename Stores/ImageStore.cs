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
            OverlayStore.Instance.Backgrounds.Clear();
            EditStore.Instance.Rect = System.Windows.Rect.Empty;
            EditStore.Instance.CommandStack.Clear();

            _mat = mat;

            var source = _mat.ToBitmapSource();
            source.Freeze();
            Source = source;
            ZoomFit();
        }

        internal void SetMat(Mat mat)
        {
            _mat = mat;
            var source = mat.ToBitmapSource();
            source.Freeze();
            Source = source;
        }

        public void Set(Uri url)
        {
            OverlayStore.Instance.Selected = null;
            OverlayStore.Instance.Overlays.Clear();
            OverlayStore.Instance.Backgrounds.Clear();
            EditStore.Instance.Rect = System.Windows.Rect.Empty;
            EditStore.Instance.CommandStack.Clear();

            var source = (BitmapSource)new BitmapImage(url);
            if (source.IsDownloading)
            {
                source.DownloadCompleted += (sender, e) =>
                {
                    if (source.DpiX != 96 || source.DpiY != 96)
                    {
                        int stride = source.PixelWidth * source.Format.BitsPerPixel;
                        var data = new byte[stride * source.PixelHeight];
                        source.CopyPixels(data, stride, 0);

                        source = BitmapSource.Create(
                            source.PixelWidth,
                            source.PixelHeight,
                            96, 96,
                            source.Format, null,
                            data,
                            source.PixelWidth * source.Format.BitsPerPixel);
                    }

                    _mat = source.ToMat();
                    source.Freeze();
                    Source = source;
                    ZoomFit();
                };
            }
            else
            {
                if (source.DpiX != 96 || source.DpiY != 96)
                {
                    int stride = source.PixelWidth * source.Format.BitsPerPixel;
                    var data = new byte[stride * source.PixelHeight];
                    source.CopyPixels(data, stride, 0);

                    source = BitmapSource.Create(
                        source.PixelWidth,
                        source.PixelHeight,
                        96, 96,
                        source.Format, null,
                        data,
                        source.PixelWidth * source.Format.BitsPerPixel);
                }

                _mat = source.ToMat();
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

        private void Render(RenderTargetBitmap target, Canvas canvas, System.Windows.Rect targetRect, System.Windows.Rect sourceRect)
        {
            var brush = new VisualBrush(canvas);
            brush.ViewboxUnits = BrushMappingMode.Absolute;
            brush.Viewbox = sourceRect;

            var visual = new DrawingVisual();
            var drawingContext = visual.RenderOpen();

            drawingContext.DrawRectangle(
                brush,
                null,
                targetRect);

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
            OverlayStore.Instance.Backgrounds.Clear();

            return bitmap.ToBitmap();
        }

        public Mat GetMat(System.Windows.Rect targetRect, System.Windows.Rect sourceRect)
        {
            if (_source == null || Image == null || Overlay == null)
                return null;

            //var bitmap = new RenderTargetBitmap((int)(_source.Width), (int)(_source.Height), _source.DpiX, _source.DpiY, PixelFormats.Pbgra32);
            var bitmap = new RenderTargetBitmap((int)targetRect.Width, (int)targetRect.Height, _source.DpiX, _source.DpiY, PixelFormats.Pbgra32);
            //var scaleRect = new System.Windows.Rect(rect.X * ZoomService.Scale, rect.Y * ZoomService.Scale, rect.Width * ZoomService.Scale, rect.Height * ZoomService.Scale);
            Render(bitmap, Image, new System.Windows.Rect(0, 0, targetRect.Width, targetRect.Height), sourceRect);
            Render(bitmap, Overlay, new System.Windows.Rect(0, 0, targetRect.Width, targetRect.Height), sourceRect);

            Mat result = new Mat();
            result.Create(bitmap.PixelHeight, bitmap.PixelWidth, MatType.CV_8UC4);
            bitmap.CopyPixels(Int32Rect.Empty, result.Data, (int)result.Step() * result.Rows, (int)result.Step());
            return result;
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
