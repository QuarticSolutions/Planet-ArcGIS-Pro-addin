﻿using ArcGIS.Core.CIM;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
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
        public double AreaCover { get; set; } = 0;
        //public int AreaCoverage { get; set; }
        public async Task setAreaCoverageAsync(Geometry queryGeom)
        {
            var vertices = new List<Coordinate2D>();
            object[][][] geom = geometry.coordinates;
            object[][] coords = geom[0];
            try
            {
                // Create a list of coordinates describing the polygon vertices.
                for (int i = 0; i < coords.Length; i++)
                {
                    object[] xy = coords[i];
                    object x = xy[0];
                    object y = xy[1];
                    double _x = Convert.ToDouble(x);
                    double _y = Convert.ToDouble(y);
                    vertices.Add(new Coordinate2D(_x, _y));
                }

                await QueuedTask.Run(() =>
                {
                    var sr = SpatialReferenceBuilder.CreateSpatialReference(4326);
                    Geometry spurcepolygon = PolygonBuilder.CreatePolygon(vertices, sr).Clone();
                    Polygon interse = (Polygon)GeometryEngine.Instance.Intersection(queryGeom, spurcepolygon);
                    //if (interse.Parts.Count > 1)
                    //{

                    //    IDisposable _graphic = null;
                    //    IDisposable _graph2 = null;
                    //    CIMPolygonSymbol _polygonSymbol = SymbolFactory.Instance.ConstructPolygonSymbol(ColorFactory.Instance.BlackRGB, SimpleFillStyle.Cross, SymbolFactory.Instance.ConstructStroke(ColorFactory.Instance.BlackRGB, 1.0, SimpleLineStyle.Solid));
                    //    _graphic = MapView.Active.AddOverlay(interse, _polygonSymbol.MakeSymbolReference());
                    //    _graphic = MapView.Active.AddOverlay(interse, _polygonSymbol.MakeSymbolReference());

                    //}
                    Polygon source = (Polygon)queryGeom;
                    AreaCover = Math.Round((interse.Area / source.Area) * 100,1,MidpointRounding.AwayFromZero);

                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }
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
