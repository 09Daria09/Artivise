using Artivise.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artivise.View
{
    public interface IWindowFactory
    {
        void CreateAndShowMainWindow();
        void CreateAndShowRegistrationWindow();
        void CreateAndShowGalleryWindow(UserData user);
    }


}
