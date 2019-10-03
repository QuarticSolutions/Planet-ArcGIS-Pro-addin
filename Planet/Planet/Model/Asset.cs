using ArcGIS.Core.CIM;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
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

namespace Planet.Model
{
    class Asset : Feature, INotifyPropertyChanged
    {
        public static string RootGroup = "Planet Daily Imagery";
        public static string BasemapsGroup = "Planet Basemaps";
        public static string[] ValidTypes = { "TiledServiceLayer", "VectorTileLayer", "RasterLayer" };
        public Strip parent { get; set; }
        private string _image;
        public string image
        {
            get
            {
                string url = _links.thumbnail + "?api_key=" + Module1.Current.API_KEY.API_KEY_Value;
                return url;
            }
            set
            {
                _image = value + "?api_key=" + Module1.Current.API_KEY.API_KEY_Value; ;
            }
        }
        public string title
        {
            get
            {
                return properties.acquired.ToLongTimeString();
            }
        }
        private bool canToggleExisting()
        {
            if (mapLayerName == null)
            {
                return false;
            } else
            {
                Task<bool> task1 = Task.Run<bool>(() =>
                {
                    bool result = false;
                    if (mapLayerName != "")
                    {
                        IReadOnlyList<Layer> layers = MapView.Active.Map.FindLayers(mapLayerName, true);
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
        }
        /// <summary>
        /// adds the sected  item to the map. uses the _mapLayerName prperty to know what scene to use
        /// </summary>
        public async void doAddToMap()
        {
            if(_permissions.Length == 0)
            {
                return;
            }
            //IsSelected = true;
            if (canToggleExisting())
            {
                return;
            }
            string targets = properties.item_type + ":" + id.ToString();
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
            var byteArray = Encoding.ASCII.GetBytes(Module1.Current.API_KEY.API_KEY_Value +":hgvhgv");
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
                    customwmts.wmtsURL = new Uri("https://tiles.planet.com/data/v1/layers/wmts/" + customwmts.name + "?api_key=" + Module1.Current.API_KEY.API_KEY_Value);
                    //Geometry geometry2 = GeometryEngine.Instance.ImportFromJSON(JSONImportFlags.jsonImportDefaults, JsonConvert.SerializeObject( quickSearchResult.features[5].geometry));
                    var serverConnection = new CIMProjectServerConnection { URL = customwmts.wmtsURL.ToString() };// "1fe575980e78467f9c28b552294ea410"
                    var connection = new CIMWMTSServiceConnection { ServerConnection = serverConnection };
                    string layerName = title + " (" + id + ")";
                    await QueuedTask.Run(() =>
                    {
                        GroupLayer group = GetGroupLayer();
                        BasicRasterLayer layer2 = LayerFactory.Instance.CreateRasterLayer(connection, group as ILayerContainerEdit, 0, layerName);
                    });
                    mapLayerName = layerName;
                    CheckParents(true);
                }
            }
        }

        public void CheckParents(bool visible)
        {
            if (parent.IsChecked != visible)
            {
                parent.IsChecked = visible;
            }
            if (parent.parent.IsChecked != visible)
            {
                parent.parent.IsChecked = visible;
            }
        }

        public static GroupLayer GetGroup(string name, string[] parents)
        {
            IReadOnlyList<Layer> layers = MapView.Active.Map.FindLayers(name, true);
            if (layers.Count > 0)
            {
                foreach (Layer layer in layers)
                {
                    List<Layer> parentGroups = new List<Layer>();
                    Layer currentLayer = layer.Parent as Layer;
                    bool isGroup = true;
                    for(int i = 0; i < parents.Length; i++)
                    {
                        if (currentLayer != null && currentLayer.Name != parents[i])
                        {
                            isGroup = false;
                            break;
                        }
                        currentLayer = currentLayer.Parent as Layer;
                    }
                    if (isGroup)
                    {
                        return layer as GroupLayer;
                    }
                }
            }
            return null;
        }
        private GroupLayer GetGroupLayer()
        {
            //check strip, then check item, then check date, then check root
            string rootGroup = RootGroup;
            string dateGroup = parent.parent.parent.date;
            string itemGroup = parent.parent.itemType;
            string stripGroup = parent.acquired.ToShortTimeString() + " (" + parent.stripId + ")";
            string[] stripParents = { itemGroup, dateGroup, rootGroup };
            var stripLayerGroup = GetGroup(stripGroup, stripParents);
            if (stripLayerGroup != null)
            {
                return stripLayerGroup;
            } else
            {
                string[] itemParents = { dateGroup, rootGroup };
                var itemLayerGroup = GetGroup(itemGroup, itemParents);
                if (itemLayerGroup != null)
                {
                    GroupLayer stripGroupLayer = LayerFactory.Instance.CreateGroupLayer(itemLayerGroup as ILayerContainerEdit, 0, stripGroup);
                    parent.mapLayerName = stripGroup;
                    return stripGroupLayer;
                } else
                {
                    string[] dateParents = { rootGroup };
                    var dateLayerGroup = GetGroup(dateGroup, dateParents);
                    if (dateLayerGroup != null)
                    {
                        GroupLayer itemGroupLayer = LayerFactory.Instance.CreateGroupLayer(dateLayerGroup as ILayerContainerEdit, 0, itemGroup);
                        GroupLayer stripGroupLayer = LayerFactory.Instance.CreateGroupLayer(itemGroupLayer as ILayerContainerEdit, 0, stripGroup);
                        parent.parent.mapLayerName = itemGroup;
                        parent.mapLayerName = stripGroup;
                        return stripGroupLayer;
                    } else
                    {
                        string[] rootParents = { };
                        var rootLayerGroup = GetGroup(rootGroup, rootParents);
                        if (rootLayerGroup != null)
                        {
                            GroupLayer dateGroupLayer = LayerFactory.Instance.CreateGroupLayer(rootLayerGroup as ILayerContainerEdit, 0, dateGroup);
                            GroupLayer itemGroupLayer = LayerFactory.Instance.CreateGroupLayer(dateGroupLayer as ILayerContainerEdit, 0, itemGroup);
                            GroupLayer stripGroupLayer = LayerFactory.Instance.CreateGroupLayer(itemGroupLayer as ILayerContainerEdit, 0, stripGroup);
                            parent.parent.parent.mapLayerName = dateGroup;
                            parent.parent.mapLayerName = itemGroup;
                            parent.mapLayerName = stripGroup;
                            return stripGroupLayer;
                        }
                        else {
                            int index = FindRootIndex();
                            GroupLayer rootGroupLayer = LayerFactory.Instance.CreateGroupLayer(MapView.Active.Map, index, rootGroup);
                            GroupLayer dateGroupLayer = LayerFactory.Instance.CreateGroupLayer(rootGroupLayer as ILayerContainerEdit, 0, dateGroup);
                            GroupLayer itemGroupLayer = LayerFactory.Instance.CreateGroupLayer(dateGroupLayer as ILayerContainerEdit, 0, itemGroup);
                            GroupLayer stripGroupLayer = LayerFactory.Instance.CreateGroupLayer(itemGroupLayer as ILayerContainerEdit, 0, stripGroup);
                            parent.parent.parent.mapLayerName = dateGroup;
                            parent.parent.mapLayerName = itemGroup;
                            parent.mapLayerName = stripGroup;
                            return stripGroupLayer;
                        }
                    }
                }
            }
        }

        public static int FindRootIndex()
        {
            IEnumerable<Layer> layers = MapView.Active.Map.Layers;
            int targetIndex = 0;
            int lowestIndex = MapView.Active.Map.Layers.Count;
            if (lowestIndex > 0)
            {
                lowestIndex = lowestIndex - 1;
                foreach (Layer layer in layers)
                {
                    if (layer.Name == BasemapsGroup)
                    {
                        targetIndex = MapView.Active.Map.Layers.IndexOf(layer);
                        if (targetIndex > 0)
                        {
                            targetIndex = targetIndex - 1;
                        }
                        return targetIndex;
                    }
                    if (layer is GroupLayer group)
                    {
                        IEnumerable<Layer> children = group.GetLayersAsFlattenedList();
                        foreach (Layer child in children)
                        {
                            string childType = child.GetType().Name;
                            if (ValidTypes.Contains(childType))
                            {
                                int layerIndex = MapView.Active.Map.Layers.IndexOf(group);
                                if (layerIndex < lowestIndex)
                                {
                                    lowestIndex = layerIndex;
                                    break;
                                }
                            }
                        }
                    }
                    string type = layer.GetType().Name;
                    if (ValidTypes.Contains(type))
                    {
                        int layerIndex = MapView.Active.Map.Layers.IndexOf(layer);
                        if (layerIndex < lowestIndex)
                        {
                            lowestIndex = layerIndex;
                        }
                    }
                }
            }

            return lowestIndex == 0 ? lowestIndex : lowestIndex - 1; ;
        }

        public async void doRemoveFromMap()
        {
            //IsSelected = false;
            await QueuedTask.Run(() =>
            {
                IReadOnlyList<Layer> layers = MapView.Active.Map.FindLayers(mapLayerName, true);
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _mapLayerName;
        public string mapLayerName
        {
            get
            {
                return _mapLayerName;
            }
            set
            {
                _mapLayerName = value;
                HasMapLayer = true;
            }
        }

        private bool _HasMapLayer = false;
        public bool HasMapLayer
        {
            get
            {
                return _HasMapLayer;
            }
            set
            {
                { SetHasMapLayer(ref _HasMapLayer, value); }
            }
        }

        protected bool SetHasMapLayer<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private bool _IsChecked;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set { SetIsChecked(ref _IsChecked, value); }
        }

        protected bool SetIsChecked<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            bool isChecked = Convert.ToBoolean(value);
            DockPane pane = FrameworkApplication.DockPaneManager.Find("Planet_Data_DocPane");
            Data_DocPaneViewModel data_DocPaneViewModel = (Data_DocPaneViewModel)pane;
            if (isChecked)
            {
                doAddToMap();

                data_DocPaneViewModel.SelectAssets.Add(this);
            }
            else
            {
                doRemoveFromMap();
                data_DocPaneViewModel.SelectAssets.Remove(this);
            }
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
