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
                {
                    EditMode = EditMode.None;
                    Opacity = 1;
                }
            }
        }

        private bool _isCrop;
        public bool IsCrop
        {
            get => _isCrop;
            set
            {
                SetProperty(ref _isCrop, value);
                if (value)
                {
                    EditMode = EditMode.Crop;
                    Opacity = 0.25;
                }
            }
        }

        private EditMode _editMode;
        public EditMode EditMode
        {
            get => _editMode;
            set => SetProperty(ref _editMode, value);
        }

        private double _opacity;
        public double Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, value);
        }

        private Rect _rect;
        public Rect Rect
        {
            get => _rect;
            set => SetProperty(ref _rect, value);
        }

        private Rect _cropRect;
        public Rect CropRect
        {
            get => _cropRect;
            set
            {
                SetProperty(ref _cropRect, value);
                InvertCropPoint = new Point(-_cropRect.X, -_cropRect.Y);
            }
        }

        private Point _invertCropPoint;
        public Point InvertCropPoint
        {
            get => _invertCropPoint;
            set => SetProperty(ref _invertCropPoint, value);
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