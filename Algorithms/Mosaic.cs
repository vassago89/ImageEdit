using OpenCvSharp;
using OpenCvSharp.Dnn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEdit.Algorithms
{
    static partial class Algorithms
    {
        private static Net _net = Net.ReadNetFromCaffe("Algorithms/deploy.prototxt", "Algorithms/res10_300x300_ssd_iter_140000.caffemodel");

        public static Mat Mosaic(Mat source)
        {
            var converted = new Mat();
            if (source.Channels() == 4)
                converted = source.CvtColor(ColorConversionCodes.BGRA2BGR);
            else if (source.Channels() == 1)
                converted = source.CvtColor(ColorConversionCodes.GRAY2BGR);
            else
                return source;

            converted = converted.Resize(new Size(300, 300));

            var sourceRect = new Rect(0, 0, source.Width, source.Height);
            
            using (var blob = CvDnn.BlobFromImage(converted, 1, new Size(300, 300), new Scalar(104.0, 177.0, 123.0)))
            {
                _net.SetInput(blob);
                using (var detection = _net.Forward())
                {
                    using (var result = new Mat(detection.Size(2), detection.Size(3), MatType.CV_32F, detection.Ptr(0)))
                    {
                        //result.GetArray(out float[] array);
                        //var a = array[2];
                        double confidence = result.At<float>(0, 2);
                        if (confidence > 0.5)
                        {
                            var x = (int)Math.Round(result.At<float>(0, 3) * 300);
                            var y = (int)Math.Round(result.At<float>(0, 4) * 300);
                            var width = (int)Math.Round(result.At<float>(0, 5) * 300) - x;
                            var height = (int)Math.Round(result.At<float>(0, 6) * 300) - y;

                            var xRatio = source.Width / 300;
                            var yRatio = source.Height / 300;

                            var rect = new Rect(x * xRatio, y * yRatio, width * xRatio, height * yRatio);
                            rect.Intersect(sourceRect);
                            if (rect.Width <= 0 || rect.Height <= 0)
                                return source;

                            var sub = source.SubMat(rect);
                            var resized = sub.Resize(new Size(Math.Max(1, rect.Width / 5), Math.Max(1, rect.Height / 5)));
                            resized = resized.Resize(new Size(rect.Width, rect.Height));

                            resized.CopyTo(sub);
                        }
                    }
                }
            }

            return source;
        }
    }
}
