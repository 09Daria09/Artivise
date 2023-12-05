using Artivise.Interfaces_Services;
using Artivise.View;
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
using System.Windows.Shapes;

namespace Artivise
{
    /// <summary>
    /// Interaction logic for GalleryWindow.xaml
    /// </summary>
    public partial class GalleryWindow : Window, ICloseable, IMessageService
    {
        public GalleryWindow()
        {
            InitializeComponent();
        }

        public void ShowMessage(string text, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            MessageBox.Show(text, title, buttons, icon);
        }
        public void Close()
        {
            base.Close();
        }
    }
}
