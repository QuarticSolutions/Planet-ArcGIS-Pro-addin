using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Tool_and_MV.Model
{
    class Strip
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
    }
}
