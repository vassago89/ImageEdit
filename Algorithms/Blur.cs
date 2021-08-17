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
            //var sourceRect = new Rect(0, 0, source.Width, source.Height);

            //if (sourceRect.Contains(rect) == false)
            //    return null;

            var size = new Size(rect.Width, rect.Height);
            if (size.Width % 2 == 0)
                size.Width--;

            if (size.Height % 2 == 0)
                size.Height--;

            return source.GaussianBlur(size, 1.5);
        }
    }
}
