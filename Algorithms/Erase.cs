using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEdit.Algorithms
{
    static class Algorithms
    {
        public static Mat Erase(int centerX, int centerY, int size, Mat source)
        {
            var element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(size, size));

            var sourceRect = new Rect(0, 0, source.Width, source.Height);
            var inflateRect = new Rect(centerX, centerY, 1, 1);

            inflateRect.Inflate(size / 2, size / 2);
            inflateRect = inflateRect.Intersect(sourceRect);

            if (inflateRect.Width == 0 || inflateRect.Height == 0)
                return source;

            var sub = source.SubMat(inflateRect);
            Mat result = new Mat();

            Cv2.MorphologyEx(sub, result, MorphTypes.Dilate, element);

            result.CopyTo(sub);

            return source;
        }
    }
}
