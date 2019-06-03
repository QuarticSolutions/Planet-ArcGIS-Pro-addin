using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Planet.Data
{
    public class Geotiffs: INotifyPropertyChanged, INotifyCollectionChanged
    {
        private ObservableCollection<string> _items;

        public ObservableCollection<string> GeoTIFFS
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("GeoTIFFS");
            }
        }
        public void Add(string newtiff)
        {
            if (_items is null)
            {
                _items = new ObservableCollection<string>
                {
                    "Drew Test"
                };
            }
            _items.Add(newtiff);
            OnPropertyChanged("GeoTIFFS");
        }

        public override string ToString()
        {
            return base.ToString();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged

        #region INotifyCollectionChanged
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        }
        #endregion INotifyCollectionChanged
    }


}
