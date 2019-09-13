using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planet.Model.Item_assets
{


    public class Order
    {
        public string name { get; set; }
        public Product[] products { get; set; }
        public bool include_metadata_assets { get; set; } = true;
        public string order_type { get; set; } = "partial";
        public Delivery delivery { get; set; }
    }

    public class Delivery
    {
        public bool single_archive { get; set; } = true;
        public string archive_type { get; set; } = "zip";
    }

    public class Product
    {
        public string[] item_ids { get; set; }
        public string item_type { get; set; }
        //public string asset_type { get; set; }
        public string product_bundle { get; set; }
    }

}
