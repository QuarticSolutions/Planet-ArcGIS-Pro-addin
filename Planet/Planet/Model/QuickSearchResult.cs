using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_docing_Panel.Models
{


        public class QuickSearchResult
        {
            public _Links _links { get; set; }
            public Feature[] features { get; set; }
            public string type { get; set; }
        }

        public class _Links
        {
            public string _first { get; set; }
            public string _next { get; set; }
            public string _self { get; set; }
        }

        public class Feature
        {

            public _Links1 _links { get; set; }
            public object[] _permissions { get; set; }
            public searchGeometry geometry { get; set; }
            public string id { get; set; }
            public Properties properties { get; set; }
            public string type { get; set; }
            public bool IsSelected { get; set; } = false;
        }

        public class _Links1
        {
            public string _self { get; set; }
            public string assets { get; set; }
            public string thumbnail { get; set; }
        }

        public class searchGeometry
        {
            public object[][][] coordinates { get; set; }
            public string type { get; set; }
        }

        public class Properties
        {
            public DateTime acquired { get; set; }
            public float anomalous_pixels { get; set; }
            public float black_fill { get; set; }
            public float cloud_cover { get; set; }
            public int columns { get; set; }
            public int epsg_code { get; set; }
            public string grid_cell { get; set; }
            public bool ground_control { get; set; }
            public float gsd { get; set; }
            public string instrument { get; set; }
            public string item_type { get; set; }
            public int origin_x { get; set; }
            public int origin_y { get; set; }
            public float pixel_resolution { get; set; }
            public string provider { get; set; }
            public DateTime published { get; set; }
            public string quality_category { get; set; }
            public int rows { get; set; }
            public string satellite_id { get; set; }
            public string strip_id { get; set; }
            public float sun_azimuth { get; set; }
            public float sun_elevation { get; set; }
            public DateTime updated { get; set; }
            public float usable_data { get; set; }
            public float view_angle { get; set; }
            public int clear_confidence_percent { get; set; }
            public int clear_percent { get; set; }
            public int cloud_percent { get; set; }
            public int heavy_haze_percent { get; set; }
            public int light_haze_percent { get; set; }
            public int shadow_percent { get; set; }
            public int snow_ice_percent { get; set; }
            public int visible_confidence_percent { get; set; }
            public int visible_percent { get; set; }
            public float ground_control_ratio { get; set; }
            public float satellite_azimuth { get; set; }
        }


}
