using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Artivise.Interfaces_Services;
using Artivise.Model;
using Artivise.Services;
using Artivise.View;
using ResumeDatabase.Commands;

namespace Artivise.ViewModel
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private UserData User;
        private readonly UserDataService _userDataService;
        private readonly IWindowFactory windowFactory;
        private readonly ICloseable closeable;
        public ICommand CloseCommand { get; private set; }
        public string Password
        {
            get => User.Password;
            set
            {
                User.Password = value;
                OnPropertyChanged(nameof(Password));
                UpdateCanExecuteRegister();
            }
        }
        public string UserName
        {
            get => User.UserName;
            set
            {
                if (User.UserName != value)
                {
                    User.UserName = value;
                    OnPropertyChanged(nameof(UserName));
                    UpdateCanExecuteRegister();
                }
            }
        }

        public ICommand RegisterCommand { get; private set; }
        public ICommand LoginCommand { get; private set; }

        private readonly IMessageService messageService;
        public UserViewModel(IMessageService messageService, ICloseable closeable)
        {
            User = new UserData();
            RegisterCommand = new DelegateCommand(ExecuteRegister, CanExecuteRegister);
            LoginCommand = new DelegateCommand(ExecuteLogin, CanExecuteLogin);
            _userDataService = new UserDataService("path/to/local/user.json", "user.json");
            windowFactory = new WindowFactory();
            CloseCommand = new DelegateCommand(CloseWindow, CanExecuteClose);
            this.messageService = messageService;
            this.closeable = closeable;

            
        }
        private bool CanExecuteClose(object obj)
        {
            return true;
        }

        private void CloseWindow(object obj)
        {
            closeable?.Close();
        }
        private void ExecuteRegister(object parameter)
        {
            windowFactory.CreateAndShowRegistrationWindow();
        }

        private bool CanExecuteRegister(object parameter)
        {
            return true;
        }

        private void ExecuteLogin(object parameter)
        {
            var users = _userDataService.ReadUsers();
            var user = users.FirstOrDefault(u => u.UserName.Equals(User.UserName) && u.Password.Equals(Password));

            if (user != null)
            {
                messageService.ShowMessage($"Dear {user.FirstName}, you have successfully logged in.", "Login", MessageBoxButton.OK, MessageBoxImage.Information);

                windowFactory.CreateAndShowGalleryWindow(user);

            }
            else
            {
                messageService.ShowMessage("Invalid username or password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void UpdateCanExecuteRegister()
        {
            ((DelegateCommand)RegisterCommand).RaiseCanExecuteChanged();
        }

        private bool CanExecuteLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(User.UserName) &&
                    !string.IsNullOrWhiteSpace(User.Password);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
