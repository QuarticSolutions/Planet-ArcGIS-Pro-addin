using ArcGIS.Core.CIM;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using test_docing_Panel.Models;

namespace Clean_Tool_and_MV.Model
{
    class Asset : Feature, INotifyPropertyChanged
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

        private string _mapLayerName = "";
        private bool canToggleExisting()
        {
            Task<bool> task1 = Task.Run<bool>(() =>
            {
                bool result = false;
                if (_mapLayerName != "")
                {
                    IReadOnlyList<Layer> layers = MapView.Active.Map.FindLayers(_mapLayerName, true);
                    if (layers.Count > 0)
                    {
                        foreach (Layer layer in layers)
                        {
                            Task<bool> task2 = QueuedTask.Run<bool>(() =>
                            {
                                layer.SetVisibility(true);
                                result = true;
                                return result;
                            });
                            result = task2.Result;
                        }
                    }

                }
                return result;
            });
            bool i = task1.Result;
            if (i)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// adds the sected  item to the map. uses the _mapLayerName prperty to know what scene to use
        /// </summary>
        public async void doAddToMap()
        {
            //IsSelected = true;
            if (canToggleExisting())
            {
                return;
            }

            string targets = properties.item_type + ":" + id.ToString();
            IReadOnlyList<Layer> layers = MapView.Active.Map.FindLayers(_mapLayerName, true);
            if (layers.Count > 0)
            {

                foreach (Layer layer in layers)
                {
                    await QueuedTask.Run(() =>
                    {
                        layer.SetVisibility(true);
                    });

                }
                return;
            }
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            HttpClient client = new HttpClient(handler)
            {

                BaseAddress = new Uri("https://api.planet.com")
            };
            targets = targets.TrimEnd(',');
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "data/v1/layers");
            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            request.Headers.Host = "tiles2.planet.com";
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            var nvc = new List<KeyValuePair<string, string>>();
            //nvc.Add(new KeyValuePair<string, string>("ids", "PSScene4Band:20190603_205042_1042,PSScene4Band:20190528_205949_43_1061,PSScene4Band:20190818_205116_1009"));
            nvc.Add(new KeyValuePair<string, string>("ids", targets));
            //var content = new StringContent(json, Encoding.UTF8, "application/json");
            var content = new FormUrlEncodedContent(nvc);
            request.Content = content;
            var byteArray = Encoding.ASCII.GetBytes("1fe575980e78467f9c28b552294ea410:hgvhgv");
            client.DefaultRequestHeaders.Host = "api.planet.com";
            //_client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("User-Agent", "ArcGISProC#");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            using (HttpResponseMessage httpResponse = client.SendAsync(request).Result)
            {
                using (HttpContent content2 = httpResponse.Content)
                {
                    var json2 = content2.ReadAsStringAsync().Result;
                    customwmts customwmts = JsonConvert.DeserializeObject<customwmts>(json2);
                    customwmts.wmtsURL = new Uri("https://tiles.planet.com/data/v1/layers/wmts/" + customwmts.name + "?api_key=1fe575980e78467f9c28b552294ea410");
                    //Geometry geometry2 = GeometryEngine.Instance.ImportFromJSON(JSONImportFlags.jsonImportDefaults, JsonConvert.SerializeObject( quickSearchResult.features[5].geometry));
                    var serverConnection = new CIMProjectServerConnection { URL = customwmts.wmtsURL.ToString() };// "1fe575980e78467f9c28b552294ea410"
                    var connection = new CIMWMTSServiceConnection { ServerConnection = serverConnection };
                    await QueuedTask.Run(() =>
                    {
                        BasicRasterLayer layer2 = LayerFactory.Instance.CreateRasterLayer(connection, MapView.Active.Map, 0, customwmts.items[0]);
                    });
                    _mapLayerName = customwmts.items[0];
                }
            }
        }

        public async void doRemoveFromMap()
        {
            //IsSelected = false;
            await QueuedTask.Run(() =>
            {
                IReadOnlyList<Layer> layers = MapView.Active.Map.FindLayers(_mapLayerName, true);
                foreach (Layer layer in layers)
                {
                    layer.SetVisibility(false);
                }
            });
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

        public static Model.Asset FindAsset(ObservableCollection<AcquiredDateGroup> items, string id)
        {
            foreach (Model.AcquiredDateGroup group in items)
            {
                foreach (Model.Item item in group.items)
                {
                    foreach (Model.Strip strip in item.strips)
                    {
                        foreach (Model.Asset asset in strip.assets)
                        {
                            if (asset.id == id)
                            {
                                return asset;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private bool _IsChecked;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set { SetField(ref _IsChecked, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            bool isChecked = Convert.ToBoolean(value);
            if (isChecked)
            {
                doAddToMap();
            }
            else
            {
                doRemoveFromMap();
            }
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
