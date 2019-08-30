using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Tool_and_MV.Model
{
    class AcquiredDateGroup
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
    }
}
