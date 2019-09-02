using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Tool_and_MV.Model
{
    class Strip : INotifyPropertyChanged
    {
        public Item parent { get; set; }
        public string stripId { get; set; }
        public List<Asset> assets { get; set; }
        public DateTime acquired { get; set; }
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
            foreach (Asset asset in assets)
            {
                asset.IsChecked = isChecked;
            }
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
