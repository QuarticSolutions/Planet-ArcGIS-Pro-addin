using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using test_docing_Panel.Models;

namespace Clean_Tool_and_MV.Model
{
    class Item
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
    }
}
