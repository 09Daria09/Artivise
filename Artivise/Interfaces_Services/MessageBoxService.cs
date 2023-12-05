using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Artivise.Interfaces_Services
{
    public class MessageBoxService : IMessageService
    {
        public void ShowMessage(string text, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            MessageBox.Show(text, title, buttons, icon);
        }
    }

}
