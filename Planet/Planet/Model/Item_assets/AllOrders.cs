using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planet.Model.Item_assets
{
    public class AllOrders
    {
        public PastOrder[] Property1 { get; set; }
    }

    public class PastOrder
    {
        public string status { get; set; }
        public string name { get; set; }
        public DateTime created_at { get; set; }
        public string download_url { get; set; }
        public ProductOrdered[] products { get; set; }
        public DateTime? downloaded_at { get; set; }
        public string type { get; set; }
        public int id { get; set; }
        public int size { get; set; }
    }

    public class ProductOrdered
    {
        public string item_id { get; set; }
        public string item_type { get; set; }
        public string asset_type { get; set; }
    }





    public class AllOrders2
    {
        //public _Links2 _links { get; set; }
        public Order2[] orders { get; set; }
    }

    public class _Links2
    {
        public string _self { get; set; }
    }

    public class Order2
    {
        public _Links1 _links { get; set; }
        public DateTime created_on { get; set; }
        public Delivery2 delivery { get; set; }
        public object[] error_hints { get; set; }
        public string id { get; set; }
        public string last_message { get; set; }
        public DateTime last_modified { get; set; }
        public string name { get; set; }
        public string order_type { get; set; }
        public Product[] products { get; set; }
        public string state { get; set; }
    }

    public class _Links1
    {
        public string _self { get; set; }
    }

    public class Delivery2
    {
        public string archive_filename { get; set; }
        public string archive_type { get; set; }
        public bool single_archive { get; set; }
    }
}


