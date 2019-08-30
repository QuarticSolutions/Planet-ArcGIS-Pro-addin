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
    }
}
