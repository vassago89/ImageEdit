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

        public static IEnumerable<(Rect Rect, Mat Mat)> Mosaic(Mat source)
        {
            var results = new List<(Rect, Mat)>();

            var converted = new Mat();
            if (source.Channels() == 4)
                converted = source.CvtColor(ColorConversionCodes.BGRA2BGR);
            else if (source.Channels() == 1)
                converted = source.CvtColor(ColorConversionCodes.GRAY2BGR);
            else
                return results;

            converted = converted.Resize(new Size(300, 300));

            var sourceRect = new Rect(0, 0, source.Width, source.Height);

            using (var blob = CvDnn.BlobFromImage(converted, 1, new Size(300, 300), new Scalar(104.0, 177.0, 123.0)))
            {
                _net.SetInput(blob);
                using (var detection = _net.Forward())
                {
                    using (var result = new Mat(detection.Size(2), detection.Size(3), MatType.CV_32F, detection.Ptr(0)))
                    {
                        for (int i = 0; i < result.Rows; i++)
                        {
                            double confidence = result.At<float>(i, 2);
                            if (confidence > 0.2)
                            {
                                var x = result.At<float>(i, 3) * 300;
                                var y = result.At<float>(i, 4) * 300;
                                var width = result.At<float>(i, 5) * 300 - x;
                                var height = result.At<float>(i, 6) * 300 - y;

                                var xRatio = source.Width / 300.0;
                                var yRatio = source.Height / 300.0;

                                var rect = new Rect(
                                    (int)Math.Round(x * xRatio),
                                    (int)Math.Round(y * yRatio),
                                    (int)Math.Round(width * xRatio),
                                    (int)Math.Round(height * yRatio));
                                rect.Intersect(sourceRect);
                                if (rect.Width <= 0 || rect.Height <= 0)
                                    continue; ;

                                var sub = source.Clone(rect);
                                sub = sub.Resize(new Size(Math.Max(1, rect.Width / 5), Math.Max(1, rect.Height / 5)));

                                results.Add((rect, sub.Resize(new Size(rect.Width, rect.Height))));
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}
