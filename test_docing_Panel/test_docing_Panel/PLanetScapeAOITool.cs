using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Serialization;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using test_docing_Panel.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net;
using Geometry = ArcGIS.Core.Geometry.Geometry;

namespace test_docing_Panel
{
    internal class PLanetScapeAOITool : MapTool
    {
        private IDisposable _graphic = null;
        private CIMLineSymbol _lineSymbol = null;
        HttpClient _client = new HttpClient();
        public PLanetScapeAOITool()
        {
            IsSketchTool = true;
            SketchType = SketchGeometryType.Polygon;
            SketchOutputMode = SketchOutputMode.Map;
        }

        protected override Task OnToolActivateAsync(bool active)
        {
            return base.OnToolActivateAsync(active);
        }

        protected override async Task<bool> OnSketchCompleteAsync(ArcGIS.Core.Geometry.Geometry geometry)
        {
            try
            {
                if ( _graphic != null)
                {
                    _graphic.Dispose();
                }
                Polygon polygon;
                await ArcGIS.Desktop.Framework.Threading.Tasks.QueuedTask.Run(() =>
                {
                    List<Coordinate2D> coordinates2 = new List<Coordinate2D>()
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
                    CIMPolygonSymbol _polygonSymbol = null;
                    _polygonSymbol = SymbolFactory.Instance.ConstructPolygonSymbol(ColorFactory.Instance.BlackRGB, SimpleFillStyle.Null, SymbolFactory.Instance.ConstructStroke(ColorFactory.Instance.BlackRGB, 2.0, SimpleLineStyle.Solid));
                    using (PolygonBuilder polygonBuilder = new PolygonBuilder(coordinates2, MapView.Active.Extent.SpatialReference))
                    {
                        polygonBuilder.SpatialReference = MapView.Active.Extent.SpatialReference;
                        polygon = polygonBuilder.ToGeometry();
                        geometry = polygonBuilder.ToGeometry();
                        //Geometry geometry2 = GeometryEngine.Instance.ProjectEx(geometry, projTransFromSRs);
                        _graphic = MapView.Active.AddOverlayAsync(geometry, _polygonSymbol.MakeSymbolReference());
                    }
                });

                //return true;
                //DockPane pane = FrameworkApplication.DockPaneManager.Find("test_docing_Panel_PlanetDocPane");
                DockPane pane = FrameworkApplication.DockPaneManager.Find("test_docing_Panel_Demo");
                //PlanetDocPaneViewModel planetDocPaneViewModel = (PlanetDocPaneViewModel)pane;
                //planetDocPaneViewModel.Users = "New collection"
                pane.Enabled = true;
                //Add an overlay graphic to the map view
                _graphic = await this.AddOverlayAsync(geometry, _lineSymbol.MakeSymbolReference());

                //define the text symbol
                var textSymbol = new CIMTextSymbol();
                //define the text graphic
                var textGraphic = new CIMTextGraphic();

                //await QueuedTask.Run(() =>
                //{
                //    //Create a simple text symbol
                //    textSymbol = SymbolFactory.Instance.ConstructTextSymbol(ColorFactory.Instance.BlackRGB, 8.5, "Corbel", "Regular");
                //    //Sets the geometry of the text graphic
                //    textGraphic.Shape = geometry;
                //    //Sets the text string to use in the text graphic
                //    //textGraphic.Text = "This is my line";
                //    //Sets symbol to use to draw the text graphic
                //    textGraphic.Symbol = textSymbol.MakeSymbolReference();
                //    //Draw the overlay text graphic
                //    _graphic = this.ActiveMapView.AddOverlay(textGraphic);

                //});
                string ejson = geometry.ToJson();
                Polygon poly = (Polygon)geometry;
                IReadOnlyList<Coordinate2D> coordinates = poly.Copy2DCoordinatesToList();
                ToGeoCoordinateParameter ddParam = new ToGeoCoordinateParameter(GeoCoordinateType.DD);
                List<string> geocoords = new List<string>();
                List<Tuple<double, double>> AllPts = new List<Tuple<double, double>>();
                double x;
                double y;
                foreach (Coordinate2D item in coordinates)
                {
                    MapPoint mapPoint = MapPointBuilder.CreateMapPoint(item, MapView.Active.Extent.SpatialReference);
                    List<Tuple<string, string>> pts = new List<Tuple<string, string>>();
                    string dd1 = mapPoint.ToGeoCoordinateString(ddParam).Split(' ')[0];
                    pts.Add(new Tuple<string, string>(mapPoint.ToGeoCoordinateString(ddParam).Split(' ')[1], mapPoint.ToGeoCoordinateString(ddParam).Split(' ')[0]));
                    if (pts[0].Item1.Contains("W"))
                    {
                        x = double.Parse("-" + pts[0].Item1.Substring(0, pts[0].Item1.Length - 1));
                        y = double.Parse(pts[0].Item2.Substring(0, pts[0].Item2.Length - 1));
                        //AllPts.Add(new Tuple<int, int>(int.Parse("-" + pts[0].Item1.Substring(0, pts[0].Item1.Length - 1)), int.Parse(pts[0].Item2.Substring(0, pts[0].Item2.Length -1))));
                    }
                    else if (pts[1].Item2.Contains("S"))
                    {
                        x = double.Parse(pts[0].Item1.Substring(0, pts[0].Item1.Length - 1));
                        y = double.Parse("-" + pts[0].Item2.Substring(0, pts[1].Item2.Length - 1));
                        //AllPts.Add(new Tuple<int, int>(int.Parse(pts[0].Item1.Substring(0, pts[0].Item1.Length - 1)), int.Parse("-" + pts[0].Item2.Substring(0, pts[1].Item2.Length - 1))));
                    }
                    else
                    {
                        x = double.Parse(pts[0].Item1.Substring(0, pts[0].Item1.Length - 1));
                        y = double.Parse(pts[0].Item2.Substring(0, pts[0].Item2.Length - 1));
                        //AllPts.Add(new Tuple<int, int>(int.Parse(pts[0].Item1.Substring(0, pts[0].Item1.Length - 1)), int.Parse(pts[0].Item2.Substring(0, pts[1].Item2.Length - 1))));
                    }
                    AllPts.Add(new Tuple<double, double>(x, y));
                    geocoords.Add(mapPoint.ToGeoCoordinateString(ddParam));
                }

                double[,] sd = new double[AllPts.Count,2];
                for (int i = 0; i < AllPts.Count; i++)
                {
                    sd[i, 0] = AllPts[i].Item1; //+ "," + AllPts[i].Item2;
                    sd[i, 1] = AllPts[i].Item2;
                }
                List<double[,]> ss = new List<double[,]>();
                ss.Add(sd);
                Config configPoints = new Config
                {
                    type = "Polygon",
                    coordinates = ss.ToArray()
                };
                Config configGeom = new Config
                {
                    type = "GeometryFilter",
                    field_name = "geometry",
                    config = configPoints
                };



                //DateFilter
                Config dateconfigconfig = new Config
                {
                    type = "DateRangeFilter",
                    field_name = "acquired",
                    gte = "2019-05-19T16:51:19.926Z",
                    lte = "2019-08-19T16:51:19.926Z"
                };
                Config dateconfig = new Config
                {
                    type = "OrFilter",
                    config = dateconfigconfig
                };

                SearchFilter searchFilter = new SearchFilter();
                List<string> typoes = new List<string>();
                typoes.Add("PSScene4Band");
                typoes.Add("SkySatCollect");
                typoes.Add("REOrthoTile");


                List<Config> mainconfigs = new List<Config>();
                mainconfigs.Add(dateconfig);
                mainconfigs.Add(configGeom);
                searchFilter.item_types = typoes.ToArray();
                Filter topfilter = new Filter();
                topfilter.type = "AndFilter";
                searchFilter.filter = topfilter;
                Config mainConfig = new Config();
                searchFilter.filter.config = mainconfigs.ToArray();


                string json = JsonConvert.SerializeObject(searchFilter);
                string asas = "{\"filter\":{\"type\":\"AndFilter\",\"config\":[{\"type\":\"GeometryFilter\",\"field_name\":\"geometry\",\"config\":{\"type\":\"Polygon\",\"coordinates\":[[[-159.44149017333984,21.877787931279187],[-159.44998741149902,21.87679231243837],[-159.45372104644778,21.872769941600623],[-159.45217609405518,21.866835742000745],[-159.44372177124023,21.864207091531895],[-159.43561077117923,21.86930503623256],[-159.44149017333984,21.877787931279187]]]}},{\"type\":\"OrFilter\",\"config\":[{\"type\":\"DateRangeFilter\",\"field_name\":\"acquired\",\"config\":{\"gte\":\"2019-05-22T16:36:32.254Z\",\"lte\":\"2019-08-22T16:36:32.254Z\"}}]}]},\"item_types\":[\"PSScene4Band\",\"REOrthoTile\",\"SkySatCollect\"]}";
                //var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.somewhere.com/v2/cases");
                HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                HttpClient client = new HttpClient(handler) {

                    BaseAddress = new Uri("https://api.planet.com")
                };
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "data/v1/quick-search");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.CacheControl = new CacheControlHeaderValue();

                request.Headers.CacheControl.NoCache = true;
                request.Headers.Host = "api.planet.com";
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                //request.Headers.Remove("Content-Type");
                //request.Headers.Add("Content-Type", "application/json");
                var content = new StringContent(asas, Encoding.UTF8, "application/json");
                request.Content = content;
                var byteArray = Encoding.ASCII.GetBytes("1fe575980e78467f9c28b552294ea410:hgvhgv");
                client.DefaultRequestHeaders.Host = "api.planet.com";
                //_client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                content.Headers.Remove("Content-Type");
                content.Headers.Add("Content-Type", "application/json");
                //client.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("gzip"));
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                client.DefaultRequestHeaders.Add("User-Agent", "ArcGISProC#");
                //content.Headers.TryAddWithoutValidation("Authorization", "Basic " + Convert.ToBase64String(byteArray));
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "MWZlNTc1OTgwZTc4NDY3ZjljMjhiNTUyMjk0ZWE0MTA6");//Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                using (HttpResponseMessage httpResponse = client.SendAsync(request).Result)
                {
                    using (HttpContent content2 = httpResponse.Content)
                    {
                        var json2 = content2.ReadAsStringAsync().Result;
                        QuickSearchResult quickSearchResult = JsonConvert.DeserializeObject<QuickSearchResult>(json2);
                        //Geometry geometry2 = GeometryEngine.Instance.ImportFromJSON(JSONImportFlags.jsonImportDefaults, JsonConvert.SerializeObject( quickSearchResult.features[5].geometry));
                    }
                }
                



                pane.Activate();
                return true;
            }
            catch (Exception  exe)
            {
                if (_graphic != null)
                {
                    _graphic.Dispose();
                }
                return false ;
            }

        }
    }
}
