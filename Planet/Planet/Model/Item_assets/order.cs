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


    public class OrderDownload
    {
        public _DDLinks _links { get; set; }
        public DateTime created_on { get; set; }
        public Delivery delivery { get; set; }
        public object[] error_hints { get; set; }
        public string id { get; set; }
        public string last_message { get; set; }
        public DateTime last_modified { get; set; }
        public string name { get; set; }
        public string order_type { get; set; }
        public Product[] products { get; set; }
        public string state { get; set; }
    }

    public class _DDLinks
    {
        public string _self { get; set; }
        public Result[] results { get; set; }
    }

    public class Result
    {
        public string delivery { get; set; }
        public DateTime expires_at { get; set; }
        public string location { get; set; }
        public string name { get; set; }
    }

    public class Delivery
    {
        public string archive_filename { get; set; }
        public string archive_type { get; set; }
        public bool single_archive { get; set; }
    }

    public class Product
    {
        public string[] item_ids { get; set; }
        public string item_type { get; set; }
        public string product_bundle { get; set; }
    }


}
