using Artivise.Interfaces_Services;
using Artivise.Model;
using Artivise.Services;
using Artivise.View;
using Newtonsoft.Json;
using ResumeDatabase.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Artivise.ViewModel
{
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        public UserData User { get; private set; }

        private readonly UserDataService _userDataService;
        public ICommand RegisterCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        private readonly IMessageService messageService;

        private readonly ICloseable closeable;
        
        public RegistrationViewModel(ICloseable closeable, IMessageService messageService)
        {
            User = new UserData();
            _userDataService = new UserDataService("path/to/local/user.json", "user.json");
            RegisterCommand = new DelegateCommand(ExecuteRegister, CanExecuteRegister);
            CloseCommand = new DelegateCommand(CloseWindow, CanExecuteClose);
            this.closeable = closeable;
            this.messageService = messageService;
        }

        private bool CanExecuteClose(object obj)
        {
            return true;
        }

        private void CloseWindow(object obj)
        {
            closeable?.Close();
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

        public string Email
        {
            get => User.Email;
            set
            {
                if (User.Email != value)
                {
                    User.Email = value;
                    OnPropertyChanged(nameof(Email));
                    UpdateCanExecuteRegister();
                }
            }
        }

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

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
                UpdateCanExecuteRegister();
            }
        }

        public string FirstName
        {
            get => User.FirstName;
            set
            {
                if (User.FirstName != value)
                {
                    User.FirstName = value;
                    OnPropertyChanged(nameof(FirstName));
                    UpdateCanExecuteRegister();
                }
            }
        }

        public string LastName
        {
            get => User.LastName;
            set
            {
                if (User.LastName != value)
                {
                    User.LastName = value;
                    OnPropertyChanged(nameof(LastName));
                    UpdateCanExecuteRegister();
                }
            }
        }
       

        public bool DialogResult { get; private set; }

        private void ExecuteRegister(object parameter)
        {
            if (Password != ConfirmPassword)
            {
                messageService.ShowMessage("Passwords do not match.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidEmail(User.Email))
            {
                messageService.ShowMessage("Invalid email address.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var users = _userDataService.ReadUsers();

            if (users.Any(u => u.UserName.Equals(UserName, StringComparison.OrdinalIgnoreCase)))
            {
                messageService.ShowMessage("Username is already taken.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (users.Any(u => u.Email.Equals(Email, StringComparison.OrdinalIgnoreCase)))
            {
                messageService.ShowMessage("A user with the same email already exists.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            users.Add(new UserData(UserName, Email, Password, FirstName, LastName));
            _userDataService.WriteUsers(users);

            messageService.ShowMessage("Registration successful.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            ClearRegistrationFields();
            CloseWindow(parameter);
        }

        private void ClearRegistrationFields()
        {
            UserName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool CanExecuteRegister(object parameter)
        {
            return !string.IsNullOrWhiteSpace(User.UserName) &&
                   !string.IsNullOrWhiteSpace(User.Email) &&
                   !string.IsNullOrWhiteSpace(User.Password) &&
                   !string.IsNullOrWhiteSpace(User.FirstName) &&
                   !string.IsNullOrWhiteSpace(User.LastName);
        }
        private void UpdateCanExecuteRegister()
        {
            ((DelegateCommand)RegisterCommand).RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            ((DelegateCommand)RegisterCommand).RaiseCanExecuteChanged();
        }
    }

}
