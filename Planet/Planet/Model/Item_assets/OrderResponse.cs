using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planet.Model.Item_assets
{


    public class OrderResponse
    {
        public string status { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public string download_url { get; set; }
        public Product[] products { get; set; }
        public object downloaded_at { get; set; }
        public string type { get; set; }
        public int id { get; set; }
        public object size { get; set; }
    }


    public class OrderResponse2
    {
        public _Links _links { get; set; }
        public DateTime created_on { get; set; }
        public Delivery2 delivery { get; set; }
        public object[] error_hints { get; set; }
        public string id { get; set; }
        public string last_message { get; set; }
        public DateTime last_modified { get; set; }
        public string name { get; set; }
        public string order_type { get; set; }
        public Product2[] products { get; set; }
        public string state { get; set; }
    }


    public class Product2
    {
        public string[] item_ids { get; set; }
        public string item_type { get; set; }
        public string product_bundle { get; set; }
    }


}
