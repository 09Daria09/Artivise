using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Artivise.Interfaces_Services
{
    public interface IMessageService
    {
        void ShowMessage(string text, string title, MessageBoxButton buttons, MessageBoxImage icon);
    }

}
