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

namespace Clean_Tool_and_MV.Model
{
    class Strip : INotifyPropertyChanged
    {
        public Item parent { get; set; }
        public string stripId { get; set; }
        public List<Asset> assets { get; set; }
        public DateTime acquired { get; set; }
        public string mapLayerName { get; set; }
        public int imageCount
        {
            get
            {
                return assets.Count;
            }
        }
        public string title
        {
            get
            {
                int count = imageCount;
                return acquired.ToShortTimeString() + " (" + count + (count == 1 ? " image" : " images") + ")";
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

        private bool? _IsChecked = false;

        public bool? IsChecked
        {
            get { return _IsChecked; }
            set { SetField(ref _IsChecked, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
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
                string rootGroup = "Planet API";
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
            if (targets == "")
            {
                return;
            }
            targets = targets.TrimEnd(',');
            string name = parent.parent.date + " " + parent.itemType + " - Strip: " + stripId + " Selection";
            Item.AddLayer(targets, name);
        }
    }
}
