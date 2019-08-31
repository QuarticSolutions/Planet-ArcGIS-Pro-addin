using ArcGIS.Core.CIM;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using test_docing_Panel.Models;

namespace Clean_Tool_and_MV.Model
{
    class Asset : Feature
    {
        public Strip parent { get; set; }
        public string image
        {
            get
            {
                string url = _links.thumbnail + "?api_key=" + "1fe575980e78467f9c28b552294ea410";
                return url;
            }
        }
        public string title
        {
            get
            {
                return properties.acquired.ToLongTimeString();
            }
        }
        public CIMSymbolReference footprintSymbol { get; set; }
        public void setFootprintSymbol()
        {
            CIMColor outlineColor = ColorFactory.Instance.CreateRGBColor(0, 157, 165, 100);
            CIMColor fillColor = ColorFactory.Instance.CreateRGBColor(0, 157, 165, 25);
            CIMStroke outline = SymbolFactory.Instance.ConstructStroke(
                outlineColor, 2.0, SimpleLineStyle.Solid);
            CIMPolygonSymbol polygonSymbol = SymbolFactory.Instance.ConstructPolygonSymbol(
                fillColor, SimpleFillStyle.Solid, outline);
            footprintSymbol = polygonSymbol.MakeSymbolReference();
        }
        public List<Coordinate2D> footprintVertices { get; set; }
        public void setFootprintVertices()
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
            }
            catch (Exception e)
            {
                //
            }
            footprintVertices = vertices;
        }
        private Geometry polygon { get; set; }
        public void setPolygon()
        {
            QueuedTask.Run(() =>
            {
                var sr = SpatialReferenceBuilder.CreateSpatialReference(4326);
                polygon = PolygonBuilder.CreatePolygon(footprintVertices, sr).Clone();
            });
        }
        public void drawFootprint()
        {
            QueuedTask.Run(() =>
            {
                footprint = MapView.Active.AddOverlay(polygon, footprintSymbol);
            });
        }
        private IDisposable _footprint;
        public IDisposable footprint
        {
            get
            {
                return _footprint;
            }
            set
            {
                if (_footprint != null)
                {
                    _footprint.Dispose();
                    _footprint = null;
                }
                _footprint = value;
            }
        }
        public void disposeFootprint()
        {
            if (footprint != null)
            {
                footprint.Dispose();
            }
        }
    }
}
