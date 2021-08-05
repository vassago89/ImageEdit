using ImageEdit.Helpers;
using ImageEdit.Models;
using ImageEdit.Stores;
using ImageEdit.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageEdit.ViewModels
{
    class TextViewModel : BindableBase
    {
        public ImageStore ImageStore => ImageStore.Instance;
        public OverlayStore OverlayStore => OverlayStore.Instance;
        public EditStore EditStore => EditStore.Instance;

        public TextView TextView { get; set; }

        private Thickness _margin;
        public Thickness Margin 
        { 
            get => _margin; 
            set => SetProperty(ref _margin, value); 
        }

        public TextViewModel()
        {
            OverlayStore.TextChanged += TextChanged;
        }

        private void TextChanged(object sender, EventArgs e)
        {
            if (OverlayStore.Selected == null)
                return;

            var point = ImageStore.Image.TranslatePoint(
                new Point(OverlayStore.Selected.Rect.Left, OverlayStore.Selected.Rect.Bottom),
                TextView);

            Margin = new Thickness(point.X, point.Y, 0, 0);
        }
    }
}
