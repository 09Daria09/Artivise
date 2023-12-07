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
using System.Windows.Shapes;

namespace Artivise
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window, ICloseable, IMessageService
    {
        public Registration()
        {
            InitializeComponent();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) // Использую прямое обращение к PasswordBox, так как WPF не позволяет нам привязать свойство Password напрямую.
                                                                                   // Это не совсем по правилам MVVM, но так часто делают для работы с паролями в WPF :)

        {
            if (DataContext is RegistrationViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password; 
            }
        }
        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegistrationViewModel viewModel)
            {
                viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
            }
        }
        public void Close()
        {
            base.Close();
        }

        public void ShowMessage(string text, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            MessageBox.Show(text, title, buttons, icon);
        }
    }
}
