using ImageEdit.Helpers;
using ImageEdit.Helpers.Command;
using ImageEdit.Models;
using ImageEdit.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
