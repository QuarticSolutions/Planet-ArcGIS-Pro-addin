using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using test_docing_Panel.Models;

namespace Clean_Tool_and_MV.Model
{
    class Item : INotifyPropertyChanged
    {
        public List<Strip> strips { get; set; }
        public string itemType { get; set; }
        public searchGeometry geometry { get; set; }
        public string thumbnail { get; set; }
        public DateTime acquired { get; set; }
        public AcquiredDateGroup parent { get; set; }
        public int imageCount
        {
            get
            {
                int count = 0;
                foreach(Strip strip in strips)
                {
                    count += strip.imageCount;
                }
                return count;
            }
        }
        public string title
        {
            get
            {
                int count = imageCount;
                return itemType + " (" + count + (count == 1 ? " image" : " images") + ")";
            }
        }
        public IEnumerable<object> Items
        {
            get
            {
                foreach (var strip in strips)
                {
                    yield return strip;
                }
            }
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
            foreach (Strip strip in strips)
            {
                strip.IsChecked = Convert.ToBoolean(value);
            }
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
