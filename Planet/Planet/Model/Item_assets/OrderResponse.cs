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

}
