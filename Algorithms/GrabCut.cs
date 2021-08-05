using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEdit.Algorithms
{
    class GrabCut
    {
        private void PreProcess(Mat source, out Mat rgb, out Mat result)
        {
            rgb = new Mat(source.Size(), MatType.CV_8UC3);
            result = new Mat(source.Size(), MatType.CV_8UC1, new Scalar((double)GrabCutClasses.PR_FGD));
            Cv2.Rectangle(result, new Rect(0, 0, 1, source.Height), (byte)GrabCutClasses.PR_BGD);
            var matType = source.Type();
            if (matType == MatType.CV_8UC4)
                Cv2.CvtColor(source, rgb, ColorConversionCodes.RGBA2RGB);
            else if (matType == MatType.CV_8UC3)
                source.CopyTo(rgb);
            else if (matType == MatType.CV_8UC1)
                Cv2.CvtColor(source, rgb, ColorConversionCodes.GRAY2RGB);
            else
                throw new NotImplementedException();
        }

        public Mat PostProcess(Mat source, Mat result)
        {
            var merged = new Mat(result.Size(), MatType.CV_8UC3);
            var temp = (result & 1) * 255;
            Cv2.Merge(new Mat[]
            {
                temp, temp, temp
            }, merged);

            return merged & source;
        }

        public Mat Cut(
            Mat source, 
            IEnumerable<System.Windows.Point> foreground,
            IEnumerable<System.Windows.Point> background, 
            int size)
        {
            PreProcess(source, out Mat rgb, out Mat result);

            foreach (var point in foreground)
                Cv2.Ellipse(result, new Point(point.X, point.Y), new Size(size, size), 0, 0, 0, (byte)GrabCutClasses.FGD);

            foreach (var point in background)
                Cv2.Ellipse(result, new Point(point.X, point.Y), new Size(size, size), 0, 0, 0, (byte)GrabCutClasses.BGD);

            rgb.GrabCut(result,
                Rect.Empty,
                new Mat(), new Mat(),
                5, GrabCutModes.InitWithMask);

            return PostProcess(rgb, result);
        }

        public Mat Cut(Mat source, Mat fgd, Mat bgd)
        {
            PreProcess(source, out Mat rgb, out Mat result);
            Cv2.GrabCut(rgb, result, new Rect(), bgd, fgd, 1, GrabCutModes.Eval);
            return PostProcess(rgb, result);
        }

        public Mat Cut(Mat source)
        {
            PreProcess(source, out Mat rgb, out Mat result);
            Cv2.GrabCut(rgb, result, new Rect(1, 0, source.Width - 1, source.Height), new Mat(), new Mat(), 5, GrabCutModes.InitWithRect);
            
            return PostProcess(rgb, result);
        }
    }
}
