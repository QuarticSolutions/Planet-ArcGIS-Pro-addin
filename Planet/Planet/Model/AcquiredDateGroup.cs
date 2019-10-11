using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Planet.Model
{
    class AcquiredDateGroup : INotifyPropertyChanged
    {
        //public List<ItemTypeGroup> itemTypeGroups { get; set; }
        public List<Item> items { get; set; }
        public DateTime acquired { get; set; }
        public string date
        {
            get
            {
                return acquired.Date.ToString("MMM dd, yyyy");
            }
        }

        public IEnumerable<object> Items
        {
            get
            {
                foreach (var item in items)
                {
                    yield return item;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _mapLayerName;
        public string mapLayerName
        {
            get
            {
                return _mapLayerName;
            }
            set
            {
                _mapLayerName = value;
                HasMapLayer = true;
            }
        }

        private bool _HasMapLayer = false;
        public bool HasMapLayer
        {
            get
            {
                return _HasMapLayer;
            }
            set
            {
                { SetHasMapLayer(ref _HasMapLayer, value); }
            }
        }

        private bool _HasPermissions = false;
        public bool HasPermissions
        {
            get
            {
                return _HasPermissions;
            }
            set
            {
                _HasPermissions = value;
            }
        }

        protected bool SetHasMapLayer<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
