using ImageEdit.Stores;
using ImageEdit.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using ImageEdit.Models;
using System.ComponentModel;

namespace ImageEdit.Views
{
    class OverlaySelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var fe = container as FrameworkElement;

            if (item is TextOverlay)
                return fe.FindResource("Text") as DataTemplate;

            if (item is ImageOverlay)
                return fe.FindResource("Image") as DataTemplate;

            return null;
        }
    }

    /// <summary>
    /// OverlayView.xaml에 대한 상호 작용 논리
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class OverlayView : UserControl
    {
        public OverlayView()
        {
            //OverlayStore.Instance.Overlays.CollectionChanged += Texts_CollectionChanged;
            InitializeComponent();
        }

        //private void Texts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.NewItems == null)
        //        return;

        //    if (e.NewItems[0] is ImageOverlay)
        //        return;

        //    Task.Run(() =>
        //    {
        //        TextBox textBox = null;
        //        while (textBox == null)
        //        {
        //            Dispatcher.Invoke(() =>
        //            {
        //                var obj = Texts.ItemContainerGenerator.ContainerFromIndex(0);
        //                if (e.NewItems.Count > 0)
        //                {
        //                    textBox = Texts.ItemContainerGenerator.ContainerFromIndex(e.NewStartingIndex).FindVisualChild<TextBox>();
        //                    textBox?.Focus();
        //                    //FocusManager.SetFocusedElement(this, textBox);
        //                }
        //            });
        //        }
        //    });
        //}

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            OverlayStore.Instance.Selected = null;
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            OverlayStore.Instance.Selected = textBox.DataContext as Overlay;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            OverlayStore.Instance.Selected = grid.DataContext as Overlay;
        }
    }
}
