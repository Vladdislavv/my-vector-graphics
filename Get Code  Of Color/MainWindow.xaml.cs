using System;
using System.Collections.Generic;
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

namespace Get_Code__Of_Color
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GetCodeOfColor : Window
    {
        public GetCodeOfColor()
        {
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Color color = Color.FromArgb(Convert.ToByte(transparencySlider.Value), Convert.ToByte(redSlider.Value), Convert.ToByte(greenSlider.Value), Convert.ToByte(blueSlider.Value));
            ColorIndicator.Fill = new SolidColorBrush( color);
            CodeOfColor.Text = color.ToString();
        }
    }
}
