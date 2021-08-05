using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace ImageEdit.Helpers
{
    static class StoryboardExtensions
    {
        public static Task BeginAsync(this Storyboard storyboard)
        {
            var tcs = new TaskCompletionSource<bool>();
            if (storyboard == null)
            {
                tcs.SetException(new ArgumentNullException());
            }
            else
            {
                EventHandler onComplete = null;
                onComplete = (s, e) => {
                    storyboard.Completed -= onComplete;
                    tcs.SetResult(true);
                };

                storyboard.Completed += onComplete;
                storyboard.Begin();
            }

            return tcs.Task;
        }
    }

    class ZoomService : DependencyObject
    {
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(
                "Scale",
                typeof(double),
                typeof(ZoomService));

        public static readonly DependencyProperty InverseScaleProperty =
            DependencyProperty.Register(
                "InverseScale",
                typeof(double),
                typeof(ZoomService));

        public static readonly DependencyProperty TranslateXProperty =
            DependencyProperty.Register(
                "TranslateX",
                typeof(double),
                typeof(ZoomService));

        public static readonly DependencyProperty TranslateYProperty =
            DependencyProperty.Register(
                "TranslateY",
                typeof(double),
                typeof(ZoomService));

        public static readonly DependencyProperty OverlayThicknessProperty =
            DependencyProperty.Register(
                "OverlayThickness",
                typeof(double),
                typeof(ZoomService),
                new PropertyMetadata(_defaultOverlayThickness));

        public static readonly DependencyProperty FontThicknessProperty =
            DependencyProperty.Register(
                "FontThickness",
                typeof(double),
                typeof(ZoomService));

        public static readonly DependencyProperty FontSmallThicknessProperty =
            DependencyProperty.Register(
                "FontSmallThickness",
                typeof(double),
                typeof(ZoomService));

        const double _defaultOverlayThickness = 2;
        const double _defaultFontThickness = 8;

        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set
            {
                SetValue(OverlayThicknessProperty, _defaultOverlayThickness / Math.Abs(value));
                SetValue(FontThicknessProperty, _defaultFontThickness / Math.Abs(value));

                SetValue(ScaleProperty, value);
                if (Scale > 0)
                    SetValue(InverseScaleProperty, 1.0 / Scale);
            }
        }

        public double InverseScale
        {
            get => (double)GetValue(InverseScaleProperty);
            set => SetValue(InverseScaleProperty, value);
        }

        public double TranslateX
        {
            get => (double)GetValue(TranslateXProperty);
            set
            {
                SetValue(TranslateXProperty, value);
                System.Diagnostics.Debug.WriteLine($"{value}");
            }
        }

        public double TranslateY
        {
            get => (double)GetValue(TranslateYProperty);
            set
            {
                SetValue(TranslateYProperty, value);
            }
        }

        public double OverlayThickness
        {
            get => (double)GetValue(OverlayThicknessProperty);
            set
            {
                if (value < 1)
                    return;

                if (value > 35791.3940666666)
                    return;

                SetValue(OverlayThicknessProperty, value);
            }
        }

        public double FontThickness
        {
            get
            {
                var value = (double)GetValue(FontThicknessProperty);
                if (value < 1)
                    return 1;
                if (value > 35791.3940666666)
                    return 35791.3940666666;

                return value;
            }
            set
            {
                if (value < 1)
                    return;

                if (value > 35791.3940666666)
                    return;

                SetValue(FontThicknessProperty, value);
            }
        }

        public ZoomService()
        {
            Scale = 1;
        }

        private void CheckValues(ref double scale, ref double translateX, ref double translateY, ref double overlayThickness, ref double fontThickness)
        {
            if (overlayThickness < 1)
                overlayThickness = 1;
            else if (overlayThickness > 35791.3940666666)
                overlayThickness = 35791.3940666665;

            if (fontThickness < 1)
                fontThickness = 1;
            else if (fontThickness > 35791.3940666666)
                fontThickness = 35791.3940666665;
        }

        private async Task UpdateValue(double scale, double translateX, double translateY, bool isAnimated, int duration)
        {
            if (Dispatcher.CheckAccess())
            {
                var overlayThickness = _defaultOverlayThickness / Math.Abs(scale);
                var fontThickness = _defaultFontThickness / Math.Abs(scale);

                CheckValues(ref scale, ref translateX, ref translateY, ref overlayThickness, ref fontThickness);

                if (isAnimated)
                {
                    TimeSpan timeSpan = TimeSpan.FromMilliseconds(duration);

                    Storyboard storyboard = new Storyboard();

                    var scaleAnimation = new DoubleAnimation(scale, timeSpan);
                    Storyboard.SetTarget(scaleAnimation, this);
                    Storyboard.SetTargetProperty(scaleAnimation, new PropertyPath(ScaleProperty));
                    storyboard.Children.Add(scaleAnimation);

                    var translateXAnimation = new DoubleAnimation(translateX, timeSpan);
                    Storyboard.SetTarget(translateXAnimation, this);
                    Storyboard.SetTargetProperty(translateXAnimation, new PropertyPath(TranslateXProperty));
                    storyboard.Children.Add(translateXAnimation);

                    var translateYAnimation = new DoubleAnimation(translateY, timeSpan);
                    Storyboard.SetTarget(translateYAnimation, this);
                    Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath(TranslateYProperty));
                    storyboard.Children.Add(translateYAnimation);

                    var overlayThicknessAnimation = new DoubleAnimation(overlayThickness, timeSpan);
                    Storyboard.SetTarget(overlayThicknessAnimation, this);
                    Storyboard.SetTargetProperty(overlayThicknessAnimation, new PropertyPath(OverlayThicknessProperty));
                    storyboard.Children.Add(overlayThicknessAnimation);

                    var fontThicknessAnimation = new DoubleAnimation(fontThickness, timeSpan);
                    Storyboard.SetTarget(fontThicknessAnimation, this);
                    Storyboard.SetTargetProperty(fontThicknessAnimation, new PropertyPath(FontThicknessProperty));
                    storyboard.Children.Add(fontThicknessAnimation);

                    await storyboard.BeginAsync();
                }
                else
                {
                    Scale = scale;

                    TranslateX = translateX;
                    TranslateY = translateY;

                    OverlayThickness = overlayThickness;
                    FontThickness = fontThickness;
                }
            }
            else
            {
                await Dispatcher.InvokeAsync(async () =>
                {
                    await UpdateValue(scale, translateX, translateY, isAnimated, duration);
                });
            }
        }

        public async void ZoomIn(double presentorWidth, double presentorHeight)
        {
            await ExecuteZoom(presentorWidth / 2, presentorHeight / 2, 1.1f);
        }

        public async void ZoomOut(double presentorWidth, double presentorHeight)
        {
            await ExecuteZoom(presentorWidth / 2, presentorHeight / 2, 0.9f);
        }

        public async Task ExecuteZoom(double centerX, double centerY, double zoomFactor, bool isAnimated = false, int duration = 500)
        {
            double prevX = (centerX - TranslateX);
            double prevY = (centerY - TranslateY);

            var translateX = TranslateX + prevX - (prevX * zoomFactor);
            var translateY = TranslateY + prevY - (prevY * zoomFactor);

            var scale = Scale * zoomFactor;

            await UpdateValue(scale, translateX, translateY, isAnimated, duration);
        }

        public void ZoomFit(double presentorWidth, double presentorHeight, int imageWidth, int imageHeight)
        {
            var scaleX = presentorWidth / imageWidth;
            var scaleY = presentorHeight / imageHeight;

            Scale = Math.Min(scaleX, scaleY);

            if (scaleX > scaleY)
            {
                TranslateX = presentorWidth / 2 - (imageWidth / 2 * Scale);
                TranslateY = 0;
            }
            else
            {
                TranslateX = 0;
                TranslateY = presentorHeight / 2 - (imageHeight / 2 * Scale);
            }
        }
    }
}
