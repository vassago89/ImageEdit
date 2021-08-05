using ImageEdit.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ImageEdit.Controls
{
    /// <summary>
    /// NumericButton.xaml에 대한 상호 작용 논리
    /// </summary>
    [DesignTimeVisible(false)]
    partial class NumericButton : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
          DependencyProperty.Register(
              "Value",
              typeof(decimal),
              typeof(NumericButton),
              new FrameworkPropertyMetadata((decimal)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public decimal Value
        {
            get => (decimal)GetValue(ValueProperty);
            set => SetValue(ValueProperty, Math.Round(value, 2));
        }

        public static readonly DependencyProperty MinProperty =
          DependencyProperty.Register(
              "Min",
              typeof(decimal),
              typeof(NumericButton),
              new FrameworkPropertyMetadata((decimal)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public decimal Min
        {
            get => (decimal)GetValue(MinProperty);
            set => SetValue(MinProperty, Math.Round(value, 2));
        }

        public static readonly DependencyProperty MaxProperty =
          DependencyProperty.Register(
              "Max",
              typeof(decimal),
              typeof(NumericButton),
              new FrameworkPropertyMetadata((decimal)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public decimal Max
        {
            get => (decimal)GetValue(MaxProperty);
            set => SetValue(MaxProperty, Math.Round(value, 2));
        }

        public RelayCommand UpCommand { get; }
        public RelayCommand DownCommand { get; }

        public NumericButton()
        {
            UpCommand = new RelayCommand(() =>
            {
                Value = Math.Min(Math.Max(Value + 1, Min), Max);
            });

            DownCommand = new RelayCommand(() =>
            {
                Value = Math.Min(Math.Max(Value - 1, Min), Max);
            });

            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            if (decimal.TryParse(textBox.Text, out decimal value))
            {
                Value = Math.Min(Math.Max(value, Min), Max);
                textBox.Text = Value.ToString();
            }
            else
            {
                textBox.Text = Value.ToString();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            if (e.Key == Key.Enter)
            {
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}
