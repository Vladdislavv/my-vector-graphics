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
using Microsoft.Win32;

namespace Export_window
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ExportDiologWindow : Window
    {
        public ExportDiologWindow()
        {
            InitializeComponent();
        }
        public string NameOfFile
        {
            get
            {
                return name_of_file.Text;
            }
        }
        public string PathOfFile
        {
            get
            {
                return path.Text;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void export_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}
