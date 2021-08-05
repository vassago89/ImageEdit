﻿using ImageEdit.Helpers.Command;
using ImageEdit.Models;
using ImageEdit.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
    class NullToEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// MainView.xaml에 대한 상호 작용 논리
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class MainView : UserControl
    {
        bool _isPressed;

        Point _translate;
        Point _pos;

        public MainView()
        {
            InitializeComponent();

            ImageStore.Instance.SetCanvas(Image, Overlay);
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length <= 0)
                    return;

                try
                {
                    var source = new BitmapImage(new System.Uri(files[0]));

                    source.Freeze();
                    var sourceRect = new Rect(0, 0, ImageStore.Instance.Source.PixelWidth, ImageStore.Instance.Source.PixelHeight);

                    double width = source.PixelWidth;
                    double height = source.PixelHeight;

                    if (width > ImageStore.Instance.Source.PixelWidth)
                    {
                        var ratio = ImageStore.Instance.Source.PixelWidth / width;

                        width *= ratio;
                        height *= ratio;
                    }

                    if (height > ImageStore.Instance.Source.PixelHeight)
                    {
                        var ratio = ImageStore.Instance.Source.PixelHeight / height;

                        width *= ratio;
                        height *= ratio;
                    }

                    var x = ImageStore.Instance.Source.PixelWidth / 2 - width / 2;
                    var y = ImageStore.Instance.Source.PixelHeight / 2 - height / 2;

                    var rect = new Rect(x, y, width, height);

                    rect.Intersect(sourceRect);

                    EditStore.Instance.CommandStack.Push(new Command<Overlay>(
                            new ImageOverlay(rect, source),
                            o =>
                            {
                                OverlayStore.Instance.Selected = o;
                                OverlayStore.Instance.Overlays.Add(o);
                            },
                            o =>
                            {
                                OverlayStore.Instance.Overlays.Remove(o);
                                OverlayStore.Instance.Selected = null;
                            }));
                }
                catch (Exception exception)
                {

                }
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPressed == false)
                return;

            var controlPos = e.GetPosition(this);

            ImageStore.Instance.ZoomService.TranslateX = _translate.X + (controlPos.X - _pos.X);
            ImageStore.Instance.ZoomService.TranslateY = _translate.Y + (controlPos.Y - _pos.Y);
            OverlayStore.Instance.ZoomChange();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (OverlayStore.Instance.IsSelected == false)
                grid.Focus();

            var controlPos = e.GetPosition(this);
            var canvasPos = e.GetPosition(ImageStore.Instance.Image);

            switch (e.ChangedButton)
            {
                case MouseButton.Right:
                    _isPressed = true;
                    _translate = new Point(ImageStore.Instance.ZoomService.TranslateX, ImageStore.Instance.ZoomService.TranslateY);
                    _pos = controlPos;
                    break;
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPressed = false;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            _isPressed = false;
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var pos = e.GetPosition(this);
            if (e.Delta > 0)
                ImageStore.Instance.ZoomService.ExecuteZoom(pos.X, pos.Y, 1.1);
            else
                ImageStore.Instance.ZoomService.ExecuteZoom(pos.X, pos.Y, 0.9);

            OverlayStore.Instance.ZoomChange();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) == false)
                return;

            if (e.Key == Key.V)
            {
                var textContains = Clipboard.ContainsText();
                var imageContains = Clipboard.ContainsImage();
                    
                if (textContains == false && imageContains == false)
                    return;

                var sourceRect = new Rect(0, 0, ImageStore.Instance.Source.PixelWidth, ImageStore.Instance.Source.PixelHeight);
                var center = new Point(sourceRect.Width / 2, sourceRect.Height / 2);

                Overlay overlay = null;
                if (textContains)
                {
                    var text = Clipboard.GetText();
                    var width = text.Length * 24;
                    var height = 24 * 2;
                    var rect = new Rect(center.X - width / 2, center.Y - height / 2, width, height);
                    rect.Intersect(sourceRect);
                    overlay = new TextOverlay(rect, text);

                        
                }

                if (imageContains)
                {
                    var image = Clipboard.GetImage();
                    image.Freeze();

                    var width = image.PixelWidth;
                    var height = image.PixelHeight;
                    var rect = new Rect(center.X - width / 2, center.Y - height / 2, width, height);
                    rect.Intersect(sourceRect);
                    overlay = new ImageOverlay(rect, image);
                }

                EditStore.Instance.CommandStack.Push(new Command<Overlay>(
                        overlay,
                        o =>
                        {
                            OverlayStore.Instance.Overlays.Add(o);
                        },
                        o =>
                        {
                            OverlayStore.Instance.Overlays.Remove(o);
                        }));
            }
            else if (e.Key == Key.R)
            {
                EditStore.Instance.CommandStack.Redo();
            }
            else if (e.Key == Key.Z)
            {
                EditStore.Instance.CommandStack.Undo();
            }
        }
    }
}
