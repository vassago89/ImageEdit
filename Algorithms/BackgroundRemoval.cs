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
    class BackgroundRemovalONNX
    {
        private byte[] _data;
        public byte[] Data => _data;

        public BackgroundRemovalONNX(byte[] data)
        {
            _data = data;
        }
    }

    static partial class Algorithms
    {
        private const string _bgFileName = "Algorithms/BG.bin";

        private static Net _bgNet;

        private static void InitilizeBG()
        {
            if (File.Exists("1.onnx"))
            {
                var formatter = new BinaryFormatter();

                var data = File.ReadAllBytes("1.onnx");
                var bg = new BackgroundRemovalONNX(data);

                using (var fs = new FileStream(_bgFileName, FileMode.Create))
                    formatter.Serialize(fs, bg);
            }
        }

        private static Net OpenBG()
        {
            if (File.Exists(_bgFileName))
            {
                var formatter = new BinaryFormatter();
                using (var fs = new FileStream(_bgFileName, FileMode.Open))
                {
                    var bg = formatter.Deserialize(fs) as BackgroundRemovalONNX;
                    return Net.ReadNetFromONNX(bg.Data);
                }
            }

            return null;
        }

        public static Mat BackgroundRemoval(Mat mat)
        {
            InitilizeBG();

            if (_bgNet == null)
            {
                _bgNet = OpenBG();

                if (_bgNet == null)
                    return null;
            }

            var converted = new Mat();
            if (mat.Channels() == 4)
                converted = mat.CvtColor(ColorConversionCodes.BGRA2BGR);
            else if (mat.Channels() == 3)
                converted = mat;
            else if (mat.Channels() == 1)
                converted = mat.CvtColor(ColorConversionCodes.GRAY2BGR);
            else
                return null;

            var resized = converted.Resize(new Size(320, 320));
            
            Scalar mean = new Scalar(0.485 * 255.0, 0.456 * 255.0, 0.406 * 255.0);
            Scalar std = new Scalar(0.229, 0.224, 0.225);

            using (var blob = CvDnn.BlobFromImage(resized, 1 / 255.0, new Size(320, 320), mean, swapRB: true))
            {
                Cv2.Divide(blob, std, blob);

                _bgNet.SetInput(blob);
                string[] names = _bgNet.GetUnconnectedOutLayersNames();
                List<Mat> outputs = new List<Mat>();
                foreach (var name in names)
                    outputs.Add(new Mat());

                _bgNet.Forward(outputs, names);

                Mat result = outputs[0].Reshape(1, 320);
                result = result.Resize(mat.Size());
                converted = converted.CvtColor(ColorConversionCodes.BGR2BGRA);
                result.GetArray(out float[] resultData);
                converted.GetArray(out Vec4b[] sourceData);
                for (int i = 0; i < resultData.Length; i++)
                {
                    var vector = sourceData[i];
                    sourceData[i] *= resultData[i];
                }

                //for (int i = 0; i < outputs.Count; i++)
                //{
                //    Mat mul = outputs[i].Reshape(1, 320) * 255.0;
                //    mul.SaveImage($"{i}.bmp");
                //}

                converted.SetArray(sourceData);
                return converted;
            }
        }
    }
}
