using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planet
{

    public class Mosaics
    {
        public _Links _links { get; set; }
        public Mosaic[] mosaics { get; set; }
    }

    public class _Links
    {
        public string _next { get; set; }
        public string _self { get; set; }
    }

    public class Mosaic
    {
        public _Links1 _links { get; set; }
        public float[] bbox { get; set; }
        public string coordinate_system { get; set; }
        public string datatype { get; set; }
        public DateTime first_acquired { get; set; }
        public Grid grid { get; set; }
        public string id { get; set; }
        public string interval { get; set; }
        public string[] item_types { get; set; }
        public DateTime last_acquired { get; set; }
        public int level { get; set; }
        public string name { get; set; }
        public string product_type { get; set; }
        public bool quad_download { get; set; }
        public string Thumbnail { get; set; }
    }

    public class _Links1
    {
        public string _self { get; set; }
        public string quads { get; set; }
        public string tiles { get; set; }
        public string download { get; set; }
        public string items { get; set; }
        public string thumbnail { get; set; }
    }

    public class Grid
    {
        public int quad_size { get; set; }
        public float resolution { get; set; }
        public string quad_pattern { get; set; }
    }

}
