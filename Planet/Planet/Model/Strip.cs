using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Planet.Model
{
    class Strip : INotifyPropertyChanged
    {
        public Item parent { get; set; }
        public string stripId { get; set; }
        public List<Asset> assets { get; set; }
        public DateTime acquired { get; set; }
        public string imageCount
        {
            get
            {
                int count = assets.Count;
                return count + (count == 1 ? " image" : " images");
            }
        }
        public string title
        {
            get
            {
                return acquired.ToShortTimeString();
            }
        }
        public IEnumerable<object> Items
        {
            get
            {
                foreach (var asset in assets)
                {
                    yield return asset;
                }
            }
        }

        public static Strip FindStrip(ObservableCollection<AcquiredDateGroup> items, string id)
        {
            foreach (Model.AcquiredDateGroup group in items)
            {
                foreach (Model.Item item in group.items)
                {
                    foreach (Model.Strip strip in item.strips)
                    {
                        if (strip.stripId == id)
                        {
                            return strip;
                        }
                    }
                }
            }
            return null;
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

        protected bool SetHasMapLayer<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private bool? _IsChecked = false;
        public bool? IsChecked
        {
            get { return _IsChecked; }
            set { SetIsChecked(ref _IsChecked, value); }
        }

        protected bool SetIsChecked<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            bool isChecked = Convert.ToBoolean(value);
            if (mapLayerName != null)
            {
                toggleOnMap(isChecked);
            }
            else
            {
                for (int i = assets.Count-1; i >= 0; i--)
                {
                    Asset asset = assets[i];
                    asset.IsChecked = isChecked;
                }
            }
            OnPropertyChanged(propertyName);
            return true;
        }

        public async void toggleOnMap(bool value)
        {
            await QueuedTask.Run(() =>
            {
                string rootGroup = Asset.RootGroup;
                string dateGroup = parent.parent.mapLayerName;
                string itemGroup = parent.mapLayerName;
                string[] stripParents = { itemGroup, dateGroup, rootGroup };
                GroupLayer group = Asset.GetGroup(mapLayerName, stripParents);
                group.SetVisibility(value);
            });
        }

        private ICommand _SelectAll;
        public ICommand SelectAll
        {
            get
            {
                if (_SelectAll == null)
                    _SelectAll = new ArcGIS.Desktop.Framework.RelayCommand(() => doSelectAll());
                return _SelectAll;
            }
        }

        private void doSelectAll()
        {
            IsChecked = true;
            if (mapLayerName != null)
            {
                for (int i = assets.Count - 1; i >= 0; i--)
                {
                    Asset asset = assets[i];
                    asset.IsChecked = true;
                }
            }
        }

        private ICommand _AddAllAsLayer;
        public ICommand AddAllAsLayer
        {
            get
            {
                if (_AddAllAsLayer == null)
                    _AddAllAsLayer = new ArcGIS.Desktop.Framework.RelayCommand(() => doAddAll());
                return _AddAllAsLayer;
            }
        }

        private void doAddAll()
        {
            string targets = string.Empty;
            foreach (Asset asset in assets)
            {
                targets = targets + asset.properties.item_type + ":" + asset.id.ToString() + ",";
            }
            targets = targets.TrimEnd(',');
            string name = parent.parent.date + " " + parent.itemType + " - Strip: " + stripId + " All";
            Item.AddLayer(targets, name);
        }

        private ICommand _AddSelectedAsLayer;
        public ICommand AddSelectedAsLayer
        {
            get
            {
                if (_AddSelectedAsLayer == null)
                    _AddSelectedAsLayer = new ArcGIS.Desktop.Framework.RelayCommand(() => doAddSelected());
                return _AddSelectedAsLayer;
            }
        }

        private void doAddSelected()
        {
            string targets = string.Empty;
            foreach (Asset asset in assets)
            {
                if (asset.IsChecked)
                {
                    targets = targets + asset.properties.item_type + ":" + asset.id.ToString() + ",";
                }
            }
            if (targets == string.Empty)
            {
                return;
            }
            targets = targets.TrimEnd(',');
            string name = parent.parent.date + " " + parent.itemType + " - Strip: " + stripId + " Selection";
            Item.AddLayer(targets, name);
        }
    }
}
