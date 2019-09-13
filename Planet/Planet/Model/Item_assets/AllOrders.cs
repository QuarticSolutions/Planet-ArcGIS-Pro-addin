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

}


