using ImageEdit.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEdit.ViewModels
{
    class CropViewModel
    {
        public ImageStore ImageStore => ImageStore.Instance;
        public EditStore EditStore => EditStore.Instance;
        public OverlayStore OverlayStore => OverlayStore.Instance;
    }
}
