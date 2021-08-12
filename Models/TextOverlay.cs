using ImageEdit.Helpers;
using ImageEdit.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEdit.Models
{
    class TextOverlay : Overlay
    {
        public string Text { get; set; }
        public TextBox TextBox { get; set; }

        private string _foreground;
        public string Foreground
        {
            get => _foreground;
            set => SetProperty(ref _foreground, value);
        }

        public string _background;
        public string Background
        {
            get => _background;
            set => SetProperty(ref _background, value);
        }

        private bool _isBold;
        public bool IsBold
        {
            get => _isBold;
            set
            {
                SetProperty(ref _isBold, value);
                FontWeight = _isBold ? FontWeights.Bold : FontWeights.Normal;
            }
        }

        private bool _isUnderLine;
        public bool IsUnderLine
        {
            get => _isUnderLine;
            set
            {
                SetProperty(ref _isUnderLine, value);
                TextDecoration = _isUnderLine ? TextDecorations.Baseline : null;
            }
        }

        private object _textDecoration;
        public object TextDecoration
        {
            get => _textDecoration;
            set => SetProperty(ref _textDecoration, value);
        }

        private FontWeight _fontWeight;
        public FontWeight FontWeight
        {
            get => _fontWeight;
            set => SetProperty(ref _fontWeight, value);
        }

        private double _fontSize;
        public double FontSize 
        { 
            get => _fontSize; 
            set => SetProperty(ref _fontSize, value); 
        }

        private double _radius;
        public double Radius
        {
            get => _radius;
            set => SetProperty(ref _radius, value);
        }

        public FontFamily _fontFamily;
        public FontFamily FontFamily
        {
            get => _fontFamily;
            set => SetProperty(ref _fontFamily, value);
        }

        private bool _isAlignLeft;
        public bool IsAlignLeft
        {
            get => _isAlignLeft;
            set
            {
                SetProperty(ref _isAlignLeft, value);
                if (value)
                    Alignment = TextAlignment.Left;
            }
        }

        private bool _isAlignRight;
        public bool IsAlignRight
        {
            get => _isAlignRight;
            set
            {
                SetProperty(ref _isAlignRight, value);
                if (value)
                    Alignment = TextAlignment.Right;
            }
        }

        private bool _isAlignCenter;
        public bool IsAlignCenter
        {
            get => _isAlignCenter;
            set
            {
                SetProperty(ref _isAlignCenter, value);
                if (value)
                    Alignment = TextAlignment.Center;
            }
        }

        private TextAlignment _alignment;
        public TextAlignment Alignment 
        { 
            get => _alignment; 
            set => SetProperty(ref _alignment, value); 
        }

        public TextOverlay(Rect rect, string text = null) : base(rect)
        {
            Text = text;
            Alignment = TextAlignment.Center;
            IsAlignCenter = true;
            Foreground = "Black";
            Background = "Transparent";
            FontSize = 32;
            FontFamily = Fonts.SystemFontFamilies.First(b => b.Source == "Arial");
            FontWeight = FontWeights.Normal;
            TextDecoration = null;
        }
    }
}
