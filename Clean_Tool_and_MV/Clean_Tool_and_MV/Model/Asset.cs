using ArcGIS.Core.CIM;
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
        private bool _IsSelected = false;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                OnPropertyChanged("IsSelected");
                if (_IsSelected)
                {

                    //Task<bool> task1 = Task<bool>.Factory.StartNew(() => true);
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
                                    Task<bool> task2  = QueuedTask.Run<bool>(() =>
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

                    if (!i)
                    {
                        doAddToMap();
                    }
 
                    
                }
                else
                {
                    doRemoveFromMap();
                }
            }
        }

        private async void doRemoveFromMap()
        {
            await QueuedTask.Run(() =>
            {
                IReadOnlyList<Layer> layers = MapView.Active.Map.FindLayers(_mapLayerName, true);
                foreach (Layer layer in layers)
                {
                    layer.SetVisibility(false);
                }
            });

        }

        //private   Task<bool> ToggleMapLayer()
        //{
        //    bool result = false;
        //    if (_mapLayerName != "")
        //    {
        //        IReadOnlyList<Layer> layers = MapView.Active.Map.FindLayers(_mapLayerName, true);
        //        if (layers.Count > 0)
        //        {

        //            foreach (Layer layer in layers)
        //            {
        //                QueuedTask.Run(() =>
        //                {
        //                    layer.SetVisibility(true);
        //                    result = true;
        //                });

        //            }
        //        }
        //    }
        //    return result;
        //}
        private string _mapLayerName = "";
        private async void doAddToMap()
        {

            string targets =  properties.item_type + ":" + id.ToString();
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }




    }
}
