using ImageEdit.Helpers;
using ImageEdit.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageEdit.ViewModels
{
    class ImageControlViewModel : BindableBase
    {
        public OverlayStore OverlayStore => OverlayStore.Instance;
        public ImageStore ImageStore => ImageStore.Instance;
        public EditStore EditStore => EditStore.Instance;

        private Rect _rect;
        public Rect Rect
        {
            get => _rect;
            set => SetProperty(ref _rect, value);
        }

        public ImageControlViewModel()
        {

        }
    }
}
