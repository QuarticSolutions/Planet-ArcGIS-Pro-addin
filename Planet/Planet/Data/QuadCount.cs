using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planet.Data
{
    class QuadCount : INotifyPropertyChanged

    {
        private string _quadCount = "";
        public string QuadTotal
        {
            get
            {
                return _quadCount;
            }
            set
            {
                _quadCount = value;
                OnPropertyChanged(QuadTotal);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
