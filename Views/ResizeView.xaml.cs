using ImageEdit.Helpers.Command;
using ImageEdit.Models;
using ImageEdit.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    enum Direction
    {
        None, Move, Left, Right, Top, Bottom, TopLeft, TopRight, BottomLeft, BottomRight
    }

    /// <summary>
    /// ResizeView.xaml에 대한 상호 작용 논리
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class ResizeView : UserControl
    {
        private Direction _direction;
        private Point _pos;
        public ResizeView()
        {
            InitializeComponent();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(Canvas);

            if (_direction == Direction.None)
            {
                switch (GetDirection(pos))
                {
                    case Direction.None:
                        Mouse.OverrideCursor = Cursors.Arrow;
                        break;
                    case Direction.Move:
                        Mouse.OverrideCursor = Cursors.SizeAll;
                        break;
                    case Direction.Left:
                    case Direction.Right:
                        Mouse.OverrideCursor = Cursors.SizeWE;
                        break;
                    case Direction.Top:
                    case Direction.Bottom:
                        Mouse.OverrideCursor = Cursors.SizeNS;
                        break;
                    case Direction.TopLeft:
                    case Direction.BottomRight:
                        Mouse.OverrideCursor = Cursors.SizeNWSE;
                        break;
                    case Direction.TopRight:
                    case Direction.BottomLeft:
                        Mouse.OverrideCursor = Cursors.SizeNESW;
                        break;
                }
            }
            else
            {
                if (ImageStore.Instance.Source == null)
                    return;

                var rect = OverlayStore.Instance.Selected.Rect;
                var leftTop = rect.TopLeft;
                var rightBottom = rect.BottomRight;
                var sourceRect = new Rect(0, 0, ImageStore.Instance.Source.Width, ImageStore.Instance.Source.Height);
                switch (_direction)
                {
                    case Direction.Move:
                        var temp = new Rect(leftTop, rightBottom);
                        temp.Offset(
                            pos.X - _pos.X,
                            pos.Y - _pos.Y);
                        
                        if (temp.Left < 0)
                            temp.Offset(-temp.Left, 0);

                        if (temp.Top < 0)
                            temp.Offset(0, -temp.Top);

                        if (temp.Right > sourceRect.Right)
                            temp.Offset(sourceRect.Right - temp.Right, 0);

                        if (temp.Bottom > sourceRect.Bottom)
                            temp.Offset(0, sourceRect.Bottom - temp.Bottom);

                        leftTop = temp.TopLeft;
                        rightBottom = temp.BottomRight;
                        break;
                    case Direction.Left:
                        leftTop = new Point(pos.X, rect.Top);
                        break;
                    case Direction.Right:
                        rightBottom = new Point(pos.X, rect.Bottom);
                        break;
                    case Direction.Top:
                        leftTop = new Point(rect.Left, pos.Y);
                        break;
                    case Direction.Bottom:
                        rightBottom = new Point(rect.Right, pos.Y);
                        break;
                    case Direction.TopLeft:
                        leftTop = new Point(pos.X, pos.Y);
                        break;
                    case Direction.BottomRight:
                        rightBottom = new Point(pos.X, pos.Y);
                        break;
                    case Direction.TopRight:
                        leftTop = new Point(rect.X, pos.Y);
                        rightBottom = new Point(pos.X, rect.Bottom);
                        break;
                    case Direction.BottomLeft:
                        leftTop = new Point(pos.X, rect.Top);
                        rightBottom = new Point(rect.Right, pos.Y);
                        break;
                }

                if (leftTop.X > rightBottom.X || leftTop.Y > rightBottom.Y)
                    return;

                var newRect = new Rect(leftTop, rightBottom);
                newRect.Intersect(sourceRect);
                if (newRect.Width < 15 || newRect.Height < 15)
                    return;

                EditStore.Instance.Rect = newRect;
            }
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            //_direction = Direction.None;
            //Border.Background = null;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(Canvas);

            _direction = GetDirection(pos);

            if (_direction != Direction.None)
            {
                _pos = pos;
                Border.Background = Brushes.Transparent;
                EditStore.Instance.Rect = OverlayStore.Instance.Selected.Rect;
                EditStore.Instance.IsPressed = true;
            }
        }
            
        private Direction GetDirection(Point pos)
        {
            if (OverlayStore.Instance.Selected.Rect.Contains(pos))
                return Direction.Move;
            else if (IsContain(pos, OverlayStore.Instance.Selected.TopLeft))
                return Direction.TopLeft;
            else if (IsContain(pos, OverlayStore.Instance.Selected.Top))
                return Direction.Top;
            else if (IsContain(pos, OverlayStore.Instance.Selected.TopRight))
                return Direction.TopRight;
            else if (IsContain(pos, OverlayStore.Instance.Selected.Left))
                return Direction.Left;
            else if (IsContain(pos, OverlayStore.Instance.Selected.Right))
                return Direction.Right;
            else if (IsContain(pos, OverlayStore.Instance.Selected.BottomLeft))
                return Direction.BottomLeft;
            else if (IsContain(pos, OverlayStore.Instance.Selected.Bottom))
                return Direction.Bottom;
            else if (IsContain(pos, OverlayStore.Instance.Selected.BottomRight))
                return Direction.BottomRight;

            return Direction.None;
        }

        private bool IsContain(Point canvas, Point point)
        {
            var rect = new Rect(point.X, point.Y, 0, 0);
            rect.Inflate(ImageStore.Instance.ZoomService.FontThickness / 2, ImageStore.Instance.ZoomService.FontThickness / 2);
            return rect.Contains(canvas);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            _direction = Direction.None;
            Border.Background = null;
            
            if (EditStore.Instance.IsPressed)
            {
                EditStore.Instance.IsPressed = false;
                var prevRect = OverlayStore.Instance.Selected.Rect;
                var nextRect = EditStore.Instance.Rect;
                var command = new Command<Overlay>(
                    OverlayStore.Instance.Selected,
                    overlay =>
                    {
                        overlay.Rect = nextRect;
                    },
                    overlay =>
                    {
                        overlay.Rect = prevRect;
                    });

                EditStore.Instance.CommandStack.Push(command);
            }
        }
    }
}
