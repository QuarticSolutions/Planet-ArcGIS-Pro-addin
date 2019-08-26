using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using Geometry = ArcGIS.Core.Geometry.Geometry;
using Polygon = ArcGIS.Core.Geometry.Polygon;

namespace test_docing_Panel
{
    /// <summary>
    /// Interaction logic for DemoView.xaml
    /// </summary>
    public partial class DemoView : UserControl
    {
        private IDisposable _graphic = null;
        CIMPolygonSymbol _polygonSymbol = null;
        public DemoView()
        {
            InitializeComponent();
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {

        }

        async private void   Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Polygon polygon;


            var blackSolidLineSymbol = SymbolFactory.Instance.ConstructLineSymbol(ColorFactory.Instance.BlueRGB, 5, SimpleLineStyle.Solid);

            Geometry geometry = null;
            await ArcGIS.Desktop.Framework.Threading.Tasks.QueuedTask.Run(() =>
            {
                SpatialReference inSR = SpatialReferenceBuilder.CreateSpatialReference(32604);
                SpatialReference sr4326 = SpatialReferences.WGS84;
                SpatialReference sr3857 = SpatialReferences.WebMercator;
                ProjectionTransformation projTransFromSRs = ArcGIS.Core.Geometry.ProjectionTransformation.Create(inSR, sr3857);
                List<Coordinate2D> coordinates = new List<Coordinate2D>()
                {
                  //new Coordinate2D(-159.20168702818188, 21.876487211082708),
                  //new Coordinate2D(-159.42653907783114, 21.838951660451173),
                  //new Coordinate2D(-159.44077880308507, 21.94718691051718),
                  //new Coordinate2D(-159.21630329750306, 21.94718691051718),
                  //new Coordinate2D(-159.21413990271841, 21.9365008022738),
                  //new Coordinate2D(-159.21383956606297, 21.93655454291286),
                  //new Coordinate2D(-159.20168702818188, 21.876487211082708),
                  new Coordinate2D(-17773406.8675, 2478583.7239999995),
                  new Coordinate2D(-17773406.8675, 2578583.7239999995),
                  new Coordinate2D(-16773406.8675, 2578583.7239999995),
                  new Coordinate2D(-17773406.8675, 2478583.7239999995)
                };
            //MapPoint point = new MapPointBuilder.FromGeoCoordinateString()
            //List<MapPoint> mapPoints = new List<MapPoint>();
            //foreach (Coordinate2D item in coordinates)
            //{
            //    MapPoint point = new MapPointBuilder()
            //}
            //mapPoints.Add( coordinates[0].ToMapPoint());
            MapPointBuilder asas = new MapPointBuilder(new Coordinate2D(-159.20168702818188, 21.876487211082708), MapView.Active.Extent.SpatialReference);
                _polygonSymbol = SymbolFactory.Instance.ConstructPolygonSymbol(ColorFactory.Instance.BlackRGB, SimpleFillStyle.Null, SymbolFactory.Instance.ConstructStroke(ColorFactory.Instance.BlackRGB, 2.0, SimpleLineStyle.Solid));
                MapPoint point = asas.ToGeometry();
                MapPoint point2 = MapPointBuilder.FromGeoCoordinateString(point.ToGeoCoordinateString(new ToGeoCoordinateParameter(GeoCoordinateType.DD)), MapView.Active.Extent.SpatialReference, GeoCoordinateType.DD);
                using (PolygonBuilder polygonBuilder = new PolygonBuilder(coordinates, inSR))
                {
                    polygonBuilder.SpatialReference = inSR;
                    polygon = polygonBuilder.ToGeometry();
                    geometry = polygonBuilder.ToGeometry();
                    Geometry geometry2= GeometryEngine.Instance.ProjectEx(geometry, projTransFromSRs);
                    _graphic = MapView.Active.AddOverlayAsync(geometry, _polygonSymbol.MakeSymbolReference());
                    //Application.Current.
                }
            });

            //await  QueuedTask.Run(() =>
            //{
            //    MapView.Active.UpdateOverlay(_graphic, point, SymbolFactory.Instance.ConstructPointSymbol(
            //                            ColorFactory.Instance.BlueRGB, 20.0, SimpleMarkerStyle.Circle).MakeSymbolReference());
            //});
            //_graphic = await this.AddOverlayAsync(geometry, _lineSymbol.MakeSymbolReference());
            Console.WriteLine(sender.ToString());
            //_graphic = MapView.Active.AddOverlay(geometry, _lineSymbol.MakeSymbolReference());


            //Geometry geometry = new PolygonBuilder.CreatePolygon(coordinates, inSR); ;

        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_graphic != null)
            {
                //_graphic.Dispose();
            }
        }
    }
}
