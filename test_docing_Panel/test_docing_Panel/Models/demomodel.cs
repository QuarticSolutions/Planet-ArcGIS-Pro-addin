using ArcGIS.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_docing_Panel.Models
{
    class demomodel
    {
        public Uri Thumbnail { get; set; }
        public List<Coordinate2D> extent { get; set; }
        public string title { get; set; }
    }
}
