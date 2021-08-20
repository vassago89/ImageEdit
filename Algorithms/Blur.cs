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

            //var size = new Size(rect.Width, rect.Height);
            if (source.Cols < 3 || source.Rows < 3)
                return null;

            source.SaveImage("source.bmp");

            return source.GaussianBlur(new Size(3, 3), 1);
        }
    }
}
