using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEdit.Algorithms
{
    static partial class Algorithms
    {
        public static Mat Blur(Mat source, Rect rect)
        {
            var sourceRect = new Rect(0, 0, source.Width, source.Height);

            if (sourceRect.Contains(rect) == false)
                return null;
            
            var size = new Size(Math.Min(5, rect.Width), Math.Min(5, rect.Height));
            if (size.Width % 2 == 0)
                size.Width--;

            if (size.Height % 2 == 0)
                size.Height--;

            var sub = source.SubMat(rect);
            return sub.GaussianBlur(size, 1.5);
        }
    }
}
