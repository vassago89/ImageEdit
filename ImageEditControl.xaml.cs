using FontAwesome.WPF;
using ImageEdit.Stores;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageEdit
{
    

    /// <summary>
    /// ImageView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageEditControl : UserControl
    {
        public ImageEditControl()
        {
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
            //이거 안하면 dll 복사가 안되서ㄴ... 수정할 부분
            var dllFont = FontAwesomeIcon.Font;
            var dllToolkit = Xceed.Wpf.Toolkit.AllowedSpecialValues.Any;

            InitializeComponent();
        }

        public void Set(Bitmap bitmap)
        {
            ImageStore.Instance.Set(bitmap.ToMat());
        }

        public void Set(Uri url)
        {
            ImageStore.Instance.Set(url);
        }

        public (Bitmap All, Bitmap Overlay) Get()
        {
            return ImageStore.Instance.Get();
        }

        public void SetBackground(Bitmap bitmap)
        {
            ImageStore.Instance.SetBackground(bitmap.ToMat());
        }

        public void SetBackground(Uri url)
        {
            ImageStore.Instance.SetBackground(url);
        }
    }
}
