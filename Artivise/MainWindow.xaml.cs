using Artivise.Interfaces_Services;
using Artivise.View;
using Artivise.ViewModel;
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

namespace Artivise
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMessageService, ICloseable
    {
        public MainWindow()
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

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) // Использую прямое обращение к PasswordBox, так как WPF не позволяет нам привязать свойство Password напрямую.
                                                                                   // Это не совсем по правилам MVVM, но так часто делают для работы с паролями в WPF :)

        {
            if (DataContext is UserViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;
            }
        }
    }
}
