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
        public static void Mosaic(Mat mat)
        {
            var net = Net.ReadNetFromCaffe("Algorithms/deploy.prototxt", "Algorithms/res10_300x300_ssd_iter_140000.caffemodel");
            if (mat.Channels() == 4)
                mat = mat.CvtColor(ColorConversionCodes.BGRA2BGR);

            mat = mat.Resize(new Size(500, 500));

            var blob = CvDnn.BlobFromImage(mat, 1, new Size(300, 300), new Scalar(104.0, 177.0, 123.0), true, false);
            //new OpenCvSharp.Size(widthBlob, heightBlob), crop: false
            net.SetInput(blob);
            string[] outLayers = net.GetUnconnectedOutLayersNames();

            //Initialize all blobs for the all out layers
            Mat[] result = new Mat[outLayers.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = new Mat();

            ///Execute all out layers
            net.Forward(result, outLayers);

            //List<NetResult> netResults = new List<NetResult>();
            foreach (var item in result)
            {
                for (int i = 0; i < item.Rows; i++)
                {
                    //Get the max loc and max of the col range by prefix result
                    //Cv2.MinMaxLoc(item.Row[i].ColRange(5, item.Cols), out double min, out double max, out OpenCvSharp.Point minLoc, out OpenCvSharp.Point maxLoc);

                    //Validate the min probability
                    //if (max >= minProbability)
                    //{
                    //    //The label is the max Loc
                    //    string label = Labels[maxLoc.X];
                    //    if (labelsFilters != null)
                    //    {
                    //        if (!labelsFilters.Contains(label))
                    //        {
                    //            continue;
                    //        }
                    //    }

                    //    //The probability is the max value
                    //    double probability = max;

                    //    //Center BoundingBox X is the 0 index result
                    //    int centerX = Convert.ToInt32(item.At<float>(i, 0) * (float)mat.Width);
                    //    //Center BoundingBox X is the 1 index result
                    //    int centerY = Convert.ToInt32(item.At<float>(i, 1) * (float)mat.Height);
                    //    //Width BoundingBox is the 2 index result
                    //    int width = Convert.ToInt32(item.At<float>(i, 2) * (float)mat.Width);
                    //    //Height BoundingBox is the 2 index result
                    //    int height = Convert.ToInt32(item.At<float>(i, 3) * (float)mat.Height);

                    //    //Build NetResult
                    //    netResults.Add(NetResult.Build(centerX, centerY, width, height, label, probability));

                    //}
                }
            }
        }
    }
}
