using ImageEdit.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEdit.ViewModels
{
    class ImageControlViewModel
    {
        public OverlayStore OverlayStore => OverlayStore.Instance;
        public ImageStore ImageStore => ImageStore.Instance;
        public EditStore EditStore => EditStore.Instance;

        public ImageControlViewModel()
        {

        }
    }
}
