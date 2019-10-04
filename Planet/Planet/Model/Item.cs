using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using test_docing_Panel.Models;

namespace Planet.Model
{
    class Item : INotifyPropertyChanged
    {
        public List<Strip> strips { get; set; }
        public string itemType { get; set; }
        public searchGeometry geometry { get; set; }
        public string thumbnail { get; set; }
        public DateTime acquired { get; set; }
        public AcquiredDateGroup parent { get; set; }
        public string imageCount
        {
            get
            {
                int count = 0;
                foreach(Strip strip in strips)
                {
                    count += strip.assets.Count;
                }
                return count + (count == 1 ? " image" : " images");
            }
        }
        public string stripCount
        {
            get
            {
                int count = strips.Count;
                return count + (count == 1 ? " strip" : " strips");
            }
        }
        public string title
        {
            get
            {
                return itemType;
            }
        }
        public IEnumerable<object> Items
        {
            get
            {
                foreach (var strip in strips)
                {
                    yield return strip;
                }
            }
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

        private bool? _IsChecked = false;

        public bool? IsChecked
        {
            get { return _IsChecked; }
            set { SetIsChecked(ref _IsChecked, value); }
        }

        protected bool SetIsChecked<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            bool isChecked = Convert.ToBoolean(value);
            if (mapLayerName != null)
            {
                toggleOnMap(isChecked);
            }
            else
            {
                for (int i = strips.Count-1; i >= 0; i--)
                {
                    Strip strip = strips[i];
                    strip.IsChecked = isChecked;
                }
            }
            OnPropertyChanged(propertyName);
            return true;
        }

        public async void toggleOnMap(bool value)
        {
            await QueuedTask.Run(() =>
            {
                try
                {
                    string rootGroup = Asset.RootGroup;
                    string dateGroup = parent.mapLayerName;
                    string[] stripParents = { dateGroup, rootGroup };
                    GroupLayer group = Asset.GetGroup(mapLayerName, stripParents);
                    group.SetVisibility(value);
                }
                catch (Exception)
                {

                    Console.WriteLine("Groupm not found to toggle");
                }

            });
        }

        private ICommand _SelectAll;
        public ICommand SelectAll
        {
            get
            {
                if (_SelectAll == null)
                    _SelectAll = new ArcGIS.Desktop.Framework.RelayCommand(() => doSelectAll());
                return _SelectAll;
            }
        }

        private void doSelectAll()
        {
            IsChecked = true;
            if (mapLayerName != null)
            {
                for (int i = strips.Count - 1; i >= 0; i--)
                {
                    Strip strip = strips[i];
                    if (strip.IsChecked != true)
                    {
                        if (strip.mapLayerName != null)
                        {
                            strip.IsChecked = true;
                            List<Asset> assets = strip.assets;
                            for (int j = assets.Count - 1; j >= 0; j--)
                            {
                                Asset asset = assets[j];
                                asset.IsChecked = true;
                            }
                        } else
                        {
                            strip.IsChecked = true;
                        }
                    }
                    else
                    {
                        List<Asset> assets = strip.assets;
                        for (int j = assets.Count - 1; j >= 0; j--)
                        {
                            Asset asset = assets[j];
                            asset.IsChecked = true;
                        }
                    }
                }
            }
        }

        private ICommand _AddAllAsLayer;
        public ICommand AddAllAsLayer
        {
            get
            {
                if (_AddAllAsLayer == null)
                    _AddAllAsLayer = new ArcGIS.Desktop.Framework.RelayCommand(() => doAddAll());
                return _AddAllAsLayer;
            }
        }

        private void doAddAll()
        {
            string targets = string.Empty;
            foreach (Strip strip in strips)
            {
                foreach (Asset asset in strip.assets)
                {
                    targets = targets + asset.properties.item_type + ":" + asset.id.ToString() + ",";
                }
            }
            targets = targets.TrimEnd(',');
            string name = parent.date + " " + itemType + " - All";
            AddLayer(targets, name);
        }

        private ICommand _AddSelectedAsLayer;
        public ICommand AddSelectedAsLayer
        {
            get
            {
                if (_AddSelectedAsLayer == null)
                    _AddSelectedAsLayer = new ArcGIS.Desktop.Framework.RelayCommand(() => doAddSelected());
                return _AddSelectedAsLayer;
            }
        }

        private void doAddSelected()
        {
            string targets = string.Empty;
            foreach (Strip strip in strips)
            {
                foreach (Asset asset in strip.assets)
                {
                    if (asset.IsChecked)
                    {
                        targets = targets + asset.properties.item_type + ":" + asset.id.ToString() + ",";
                    }
                }
            }
            if (targets == "")
            {
                return;
            }
            targets = targets.TrimEnd(',');
            string name = parent.date + " " + itemType + " - Selection";
            AddLayer(targets, name);
        }

        public static async void AddLayer(string targets, string name)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                HttpClient client = new HttpClient(handler)
                {

                    BaseAddress = new Uri("https://api.planet.com")
                };
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
                var byteArray = Encoding.ASCII.GetBytes(Module1.Current.API_KEY.API_KEY_Value + ":hgvhgv");
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
                        var serverConnection = new CIMProjectServerConnection { URL = customwmts.wmtsURL.ToString() };
                        var connection = new CIMWMTSServiceConnection { ServerConnection = serverConnection };
                        await QueuedTask.Run(() =>
                        {
                            Layer group = MapView.Active.Map.FindLayer(Asset.RootGroup);
                            GroupLayer groupLayer = null;
                            if (group != null)
                            {
                                groupLayer = group as GroupLayer;
                            }
                            else
                            {
                                int index = Asset.FindRootIndex();
                                groupLayer = LayerFactory.Instance.CreateGroupLayer(MapView.Active.Map, index, Asset.RootGroup);
                            }
                            BasicRasterLayer layer2 = LayerFactory.Instance.CreateRasterLayer(connection, groupLayer, 0, name);
                        });
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error adding to Map", "Add to Map");
            }
            
        }
    }
}
