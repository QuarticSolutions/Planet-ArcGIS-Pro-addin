using ArcGIS.Desktop.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Planet
{
    public class PlanetConnection : INotifyPropertyChanged
    {
        private API_KEY _api_key;
        public API_KEY API_Key
        {
            get
            {
                if (_api_key == null)
                {
                    _api_key = new API_KEY();
                }
                return _api_key;
            }
            set
            {
                _api_key = value;
                OnPropertyChanged("API_Key");
                Module1.Current.API_KEY.API_KEY_Value = _api_key.API_KEY_Value;
            }
        }

        private string _UserName;
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
                OnPropertyChanged("UserName");
            }
        }
        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }
        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() => getkey(), CanExecute));
            }
        }
        private ICommand _openHyperlinkCommand;
        public ICommand OpenHyperlinkCommand
        {
            get
            {
                if (_openHyperlinkCommand == null)
                    _openHyperlinkCommand = new CommandHandler(() => ExecuteHyperlink(),CanExecute);
                return _openHyperlinkCommand;
            }
        }

        private void ExecuteHyperlink()
        {
            System.Diagnostics.Process.Start("https://www.planet.com/account/#/");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void getkey()
        {
            APIKeyChangedEvent.Publish(new APIKeyChangedEventArgs(API_Key.API_KEY_Value, API_Key.API_KEY_Value));

        }
        public bool CanExecute { get; set; } = true;
    }
    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
