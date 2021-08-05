using ImageEdit.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageEdit.ViewModels
{
    class ImageViewModel
    {
        public ImageStore ImageStore => ImageStore.Instance;
        public OverlayStore OverlayStore => OverlayStore.Instance;
        public EditStore EditStore => EditStore.Instance;

        public ImageViewModel()
        {
            
        }
    }
}
