using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ImageEdit.Extensions
{
    static class DependencyObjectExtensions
    {
        public static T FindVisualChild<T>(this DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    var child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is T)
                        return (T)child;

                    T t = FindVisualChild<T>(child);
                    if (t != null)
                        return t;
                }
            }

            return null;
        }
    }
}
