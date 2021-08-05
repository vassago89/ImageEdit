using ImageEdit.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEdit.ViewModels
{
    class OverlayViewModel
    {
        public ImageStore ImageStore => ImageStore.Instance;
        public OverlayStore OverlayStore => OverlayStore.Instance;
        public EditStore EditStore => EditStore.Instance;

        public OverlayViewModel()
        {

        }
    }
}
