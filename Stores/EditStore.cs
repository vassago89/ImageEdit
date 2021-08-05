using ImageEdit.Enums;
using ImageEdit.Helpers;
using ImageEdit.Helpers.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEdit.Stores
{
    class EditStore : BindableSingleTon<EditStore>
    {
        public CommandStack CommandStack { get; } = new CommandStack();

        private bool _isNone;
        public bool IsNone
        {
            get => _isNone;
            set
            {
                SetProperty(ref _isNone, value);
                if (value)
                    EditMode = Enums.EditMode.None;
            }
        }

        private bool _isText;
        public bool IsText
        {
            get => _isText;
            set
            {
                SetProperty(ref _isText, value);
                if (value)
                    EditMode = Enums.EditMode.Text;
            }
        }

        private EditMode _editMode;
        public EditMode EditMode
        {
            get => _editMode;
            set => SetProperty(ref _editMode, value);
        }

        private Rect _rect;
        public Rect Rect
        {
            get => _rect;
            set => SetProperty(ref _rect, value);
        }

        private bool _isPressed;
        public bool IsPressed
        {
            get => _isPressed;
            set => SetProperty(ref _isPressed, value);
        }

        public EditStore()
        {
            IsNone = true;
        }
    }
}