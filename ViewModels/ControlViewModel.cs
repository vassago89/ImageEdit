using ImageEdit.Helpers;
using ImageEdit.Helpers.Command;
using ImageEdit.Models;
using ImageEdit.Stores;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageEdit.ViewModels
{
    class ControlViewModel : BindableBase
    {
        public EditStore EditStore => EditStore.Instance;
        public OverlayStore OverlayStore => OverlayStore.Instance;

        public RelayCommand ZoomInCommand { get; }
        public RelayCommand ZoomOutCommand { get; }
        public RelayCommand ZoomFitCommand { get; }

        public RelayCommand UndoCommand { get; }
        public RelayCommand RedoCommand { get; }

        public RelayCommand RemoveCommand { get; }
        public RelayCommand TextCommand { get; }
        public RelayCommand MosaicCommand { get; }

        public ControlViewModel()
        {
            ZoomInCommand = new RelayCommand(() => ImageStore.Instance.ZoomIn());
            ZoomOutCommand = new RelayCommand(() => ImageStore.Instance.ZoomOut());
            ZoomFitCommand = new RelayCommand(() => ImageStore.Instance.ZoomFit());

            RemoveCommand = new RelayCommand(() =>
            {
                if (OverlayStore.Instance.Selected == null)
                    return;

                EditStore.Instance.CommandStack.Push(new Command<Overlay>(
                        OverlayStore.Instance.Selected,
                        overlay =>
                        {
                            OverlayStore.Instance.Overlays.Remove(overlay);
                            OverlayStore.Instance.Selected = null;
                        },
                        overlay =>
                        {
                            OverlayStore.Instance.Selected = overlay;
                            OverlayStore.Instance.Overlays.Add(overlay);
                        }));
            });

            UndoCommand = new RelayCommand(() =>
            {
                EditStore.CommandStack.Undo();
            });

            RedoCommand = new RelayCommand(() =>
            {
                EditStore.CommandStack.Redo();
            });

            TextCommand = new RelayCommand(() =>
            {
                var sourceRect = new Rect(0, 0, ImageStore.Instance.Source.PixelWidth, ImageStore.Instance.Source.PixelHeight);
                var textRect = new Rect(sourceRect.Width / 2 - 50, sourceRect.Height / 2 - 24, 100, 48);
                textRect.Intersect(sourceRect);
                
                var overlay = new TextOverlay(textRect);
                EditStore.CommandStack.Push(new Command<TextOverlay>(
                    overlay,
                    o =>
                    {
                        OverlayStore.Instance.Overlays.Add(overlay);
                    },
                    o =>
                    {
                        OverlayStore.Instance.Overlays.Remove(overlay);
                        OverlayStore.Selected = null;
                    }));

                OverlayStore.Instance.Selected = overlay;
            });

            MosaicCommand = new RelayCommand(() =>
            {
                var results = Algorithms.Algorithms.Mosaic(ImageStore.Instance.Get().ToMat());
                var overlays = new List<ImageOverlay>();

                foreach (var result in results)
                    overlays.Add(
                        new ImageOverlay(
                            new Rect(result.Rect.X, result.Rect.Y, result.Rect.Width, result.Rect.Height),
                            result.Item2.ToBitmapSource()));

                EditStore.CommandStack.Push(new Command<IEnumerable<ImageOverlay>>(
                    overlays,
                    o =>
                    {
                        foreach (var overlay in o)
                        {
                            OverlayStore.Instance.Overlays.Add(overlay);
                        }
                    },
                    o =>
                    {
                        foreach (var overlay in o)
                        {
                            OverlayStore.Instance.Overlays.Remove(overlay);
                        }
                    }));
            });
        }
    }
}
