using Artivise.Interfaces_Services;
using Artivise.Model;
using Artivise.ViewModel;
using Artivise.ViewModelGallery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artivise.View
{
    public class WindowFactory : IWindowFactory
    {
        public void CreateAndShowMainWindow()
        {
            var window = new MainWindow();
            ICloseable closeable = window;
            IMessageService message = window;
            window.DataContext = new UserViewModel(message, closeable);
            window.Show();
        }

        public void CreateAndShowRegistrationWindow()
        {
            var window = new Registration();
            ICloseable closeable = window;
            IMessageService message = window; 
            window.DataContext = new RegistrationViewModel(closeable, message);
            window.Show();
        }

        public void CreateAndShowGalleryWindow(UserData userData)
        {
            var window = new GalleryWindow();
            ICloseable closeable = window;
            IMessageService message = window;
            window.DataContext = new Gallery(closeable, message, userData);
            window.Show();
        }
    }


}
