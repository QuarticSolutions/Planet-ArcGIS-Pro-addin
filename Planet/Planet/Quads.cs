using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planet
{
        public class Quads
        {
            public _Links _links { get; set; }
            public Item[] items { get; set; }
        }


        public class Item
        {
            public _Links1 _links { get; set; }
            public float[] bbox { get; set; }
            public string id { get; set; }
            public float percent_covered { get; set; }
        }


}
