using OpenCvSharp;
using OpenCvSharp.Dnn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ImageEdit.Algorithms
{
    [Serializable]
    class DectectionFaceCaffe
    {
        private byte[] _proto;
        public byte[] Proto => _proto;

        private byte[] _model;
        public byte[] Model => _model;

        public DectectionFaceCaffe(byte[] proto, byte[] model)
        {
            _proto = proto;
            _model = model;
        }
    }


    static partial class Algorithms
    {
        private const string _faceFileName = "Algorithms/Face.bin";

        private static readonly int _resize = 20;
        private static readonly double _confidence = 0.5;

        private static void InitilizeFace()
        {
            if (File.Exists("Algorithms/deploy.prototxt") && File.Exists("Algorithms/deploy.prototxt"))
            {
                var formatter = new BinaryFormatter();

                var proto = File.ReadAllBytes("Algorithms/deploy.prototxt");
                var model = File.ReadAllBytes("Algorithms/res10_300x300_ssd_iter_140000.caffemodel");
                var detectionFace = new DectectionFaceCaffe(proto, model);

                using (var fs = new FileStream(_faceFileName, FileMode.Create))
                    formatter.Serialize(fs, detectionFace);
            }
        }

        private static Net Open()
        {
            if (File.Exists(_faceFileName))
            {
                var formatter = new BinaryFormatter();
                using (var fs = new FileStream(_faceFileName, FileMode.Open))
                {
                    var dectectionFaceCaffe = formatter.Deserialize(fs) as DectectionFaceCaffe;
                    return Net.ReadNetFromCaffe(dectectionFaceCaffe.Proto, dectectionFaceCaffe.Model);
                }
            }

            return null;
        }

        public static Mat Mosaic(Mat source, Rect rect)
        {
            var sourceRect = new Rect(0, 0, source.Width, source.Height);

            rect = rect.Intersect(sourceRect);
            if (rect.Width <= 0 || rect.Height <= 0)
                return null;

            var sub = source.SubMat(rect);
            var resized = sub.Resize(new Size(20, 20));
            return resized.Resize(new Size(source.Width, source.Height));
        }

        private static IEnumerable<Rect> GetFaceRects(Mat converted, Rect sourceRect)
        {
            var rects = new List<Rect>();

            InitilizeFace();
            var net = Open();
            if (net == null)
                return rects;

            converted = converted.Resize(new Size(300, 300));
            using (var blob = CvDnn.BlobFromImage(converted, 1, new Size(300, 300), new Scalar(104.0, 177.0, 123.0)))
            {
                net.SetInput(blob);
                using (var detection = net.Forward())
                {
                    using (var result = new Mat(detection.Size(2), detection.Size(3), MatType.CV_32F, detection.Ptr(0)))
                    {
                        for (int i = 0; i < result.Rows; i++)
                        {
                            double confidence = result.At<float>(i, 2);
                            if (confidence > _confidence)
                            {
                                var x = result.At<float>(i, 3) * 300.0;
                                var y = result.At<float>(i, 4) * 300.0;
                                var width = result.At<float>(i, 5) * 300.0 - x;
                                var height = result.At<float>(i, 6) * 300.0 - y;

                                var xRatio = sourceRect.Width / 300.0;
                                var yRatio = sourceRect.Height / 300.0;

                                var rect = new Rect(
                                    (int)Math.Round(x * xRatio),
                                    (int)Math.Round(y * yRatio),
                                    (int)Math.Round(width * xRatio),
                                    (int)Math.Round(height * yRatio));

                                rect = rect.Intersect(sourceRect);

                                if (rect.Width <= 0 || rect.Height <= 0)
                                    continue;

                                rects.Add(rect);
                            }
                        }
                    }
                }
            }

            return rects;
        }

        public static IEnumerable<(Rect Rect, Mat Mat)> Mosaic(Mat source)
        {
            var results = new List<(Rect, Mat)>();

            var converted = new Mat();
            if (source.Channels() == 4)
                converted = source.CvtColor(ColorConversionCodes.BGRA2BGR);
            else if (source.Channels() == 3)
                converted = source;
            else if (source.Channels() == 1)
                converted = source.CvtColor(ColorConversionCodes.GRAY2BGR);
            else
                return results;

            var sourceRect = new Rect(0, 0, source.Width, source.Height);

            var rects = GetFaceRects(converted, sourceRect);
            foreach (var rect in rects)
            {
                results.Add((rect, Mosaic(converted, rect)));
            }

            return results;
        }
    }
}
