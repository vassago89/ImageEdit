using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEdit.Extensions
{
    static class BitmapSourceExtensions
    {
        public static Bitmap ToBitmap(this BitmapSource source)
        {
            var bitmap = new Bitmap(
                source.PixelWidth,
                source.PixelHeight,
                PixelFormat.Format32bppPArgb);

            var data = bitmap.LockBits(
                new Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppPArgb);

            source.CopyPixels(
                Int32Rect.Empty,
                data.Scan0,data.Height * data.Stride,
                data.Stride);

            bitmap.UnlockBits(data);
            return bitmap;
        }
    }
}
