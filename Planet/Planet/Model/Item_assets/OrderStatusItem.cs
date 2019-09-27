using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Planet.Model.Item_assets
{
    class OrderStatusItem : INotifyPropertyChanged
    {
        public string name { get; set; }
        public string path { get; set; }
        public string id { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _status;
        public string status
        {
            get
            {
                return _status;
            }
            set
            {
                { SetStatus(ref _status, value); }
            }
        }
        protected bool SetStatus<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
