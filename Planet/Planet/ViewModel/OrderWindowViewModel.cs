using Newtonsoft.Json;
using Planet.Model;
using Planet.Model.Item_assets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Planet.ViewModel
{
    class OrderWindowViewModel : INotifyPropertyChanged
    {
        private string _orderName;
        private static HttpClientHandler _handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        private HttpClient _client = new HttpClient(_handler)
        {
            BaseAddress = new Uri("https://api.planet.com")
        };


        public string OrderName
        {
            get
            {
                return _orderName;
            }
            set
            {
                _orderName = value;
                OnPropertyChanged("OrderName");
            }
        }
        private ObservableCollection<Asset> _selectedAssets = new ObservableCollection<Asset>();
        public OrderWindowViewModel()
        {
            var byteArray = Encoding.ASCII.GetBytes(Module1.Current.API_KEY.API_KEY_Value + ":hgvhgv");
            _client.DefaultRequestHeaders.Host = "api.planet.com";
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _client.DefaultRequestHeaders.Add("User-Agent", "ArcGISProC#");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            _client.DefaultRequestHeaders.ExpectContinue = false;

            Bundles2 bundles2 = new Bundles2();
            foreach (Bundles2.BundelItem item in bundles2.bundles)
            {
                foreach (string sdsd in item.bundleassets)
                {
                    switch (sdsd)
                    {
                        case "Landsat8L1G":
                            if (lstLandsat8L1G == null)
                            {
                                lstLandsat8L1G = new List<string>();
                                lstLandsat8L1G.Add(" ");
                            }
                            lstLandsat8L1G.Add(item.BundleName);
                            break;
                        case "PSScene4Band":
                            if (lstPSScene4Band == null)
                            {
                                lstPSScene4Band = new List<string>();
                                lstPSScene4Band.Add(" ");
                            }
                            lstPSScene4Band.Add(item.BundleName);
                            break;
                        case "PSScene3Band":
                            if (lstPSScene3Band == null)
                            {
                                lstPSScene3Band = new List<string>();
                            }
                            lstPSScene3Band.Add(item.BundleName);
                            break;
                        case "PSOrthoTile":
                            if (lstPSOrthoTile == null)
                            {
                                lstPSOrthoTile = new List<string>();
                            }
                            lstPSOrthoTile.Add(item.BundleName);
                            break;
                        case "REOrthoTile":
                            if (lstREOrthoTile == null)
                            {
                                lstREOrthoTile = new List<string>();
                            }
                            lstREOrthoTile.Add(item.BundleName);
                            break;
                        case "REScene":
                            if (lstREScene == null)
                            {
                                lstREScene = new List<string>();
                            }
                            lstREScene.Add(item.BundleName);
                            break;
                        case "SkySatScene":
                            if (lstSkySatScene == null)
                            {
                                lstSkySatScene = new List<string>();
                            }
                            lstSkySatScene.Add(item.BundleName);
                            break;
                        case "SkySatCollect":
                            if (lstSkySatCollect == null)
                            {
                                lstSkySatCollect = new List<string>();
                            }
                            lstSkySatCollect.Add(item.BundleName);
                            break;
                        case "Sentinel2L1C":
                            if (lstSentinel2L1C == null)
                            {
                                lstSentinel2L1C = new List<string>();
                                lstSentinel2L1C.Add(" ");
                            }
                            lstSentinel2L1C.Add(item.BundleName);
                            break;
                        default:
                            break;
                    }
                }
            }


        }

        private ObservableCollection<Item> _items;
        public ObservableCollection<Item> Items
        {
            get { return _items; }
            set
            {
                if (_items is null)
                {
                    _items = new ObservableCollection<Item>();
                }

                _items = value;
                OnPropertyChanged("Items");
            }
        }
        public ObservableCollection<Asset> SelectAssets
        {
            get { return _selectedAssets; }
            set
            {
                _selectedAssets = value;
                OnPropertyChanged("SelectAssets");
                doUpdateItems();
                PSScene3BandListView = CollectionViewSource.GetDefaultView(SelectAssets);
                PSScene3BandListView.Filter = new Predicate<object>(FilterPSScene3Band);
                PSScene3BandListView.Refresh();
                PSScene4BandListView = CollectionViewSource.GetDefaultView(SelectAssets);
                PSScene4BandListView.Filter = new Predicate<object>(FilterPSScene4Band);
                SkySatSceneListView = CollectionViewSource.GetDefaultView(SelectAssets);
                SkySatSceneListView.Filter = new Predicate<object>(FilterMessageList);
                PSOrthoTileListView = CollectionViewSource.GetDefaultView(SelectAssets);
                PSOrthoTileListView.Filter = new Predicate<object>(FilterMessageList2);
                SkySatCollectListView = CollectionViewSource.GetDefaultView(SelectAssets);
                SkySatCollectListView.Filter = new Predicate<object>(FilterSkySatCollect);
                RESceneListView = CollectionViewSource.GetDefaultView(SelectAssets);
                RESceneListView.Filter = new Predicate<object>(FilterREScene);
                REOrthoTileListView = CollectionViewSource.GetDefaultView(SelectAssets);
                REOrthoTileListView.Filter = new Predicate<object>(FilterREOrthoTile);
                Landsat8L1GListView = CollectionViewSource.GetDefaultView(SelectAssets);
                Landsat8L1GListView.Filter = new Predicate<object>(FilterLandsat8L1G);
                Sentinel2L1CListView = CollectionViewSource.GetDefaultView(SelectAssets);
                Sentinel2L1CListView.Filter = new Predicate<object>(FilterSentinel2L1C);

            }
        }


        #region listboxvisibility

        
        private string _psscene4Bandvis = "Hidden";
        public string PSScene4BandVis { get { return _psscene4Bandvis; } set { _psscene4Bandvis = value; OnPropertyChanged("PSScene4BandVis"); } }

        private string _psscene3Bandvis = "Hidden";
        public string PSScene3BandVis { get { return _psscene3Bandvis; } set { _psscene3Bandvis = value; OnPropertyChanged("PSScene3BandVis"); } }

        private string _PSOrthoTilevis = "Hidden";
        public string PSOrthoTileVis { get { return _PSOrthoTilevis; } set { _PSOrthoTilevis = value; OnPropertyChanged("PSOrthoTileVis"); } }

        private string _REOrthoTilevis = "Hidden";
        public string REOrthoTilevis { get { return _REOrthoTilevis; } set { _REOrthoTilevis = value; OnPropertyChanged("REOrthoTilevis"); } }

        private string _REScenevis = "Hidden";
        public string REScenevis { get { return _REScenevis; } set { _REScenevis = value; OnPropertyChanged("REScenevis"); } }

        private string _SkySatScenevis = "Hidden";
        public string SkySatScenevis { get { return _SkySatScenevis; } set { _SkySatScenevis = value; OnPropertyChanged("SkySatScenevis"); } }

        private string _SkySatCollectvis = "Hidden";
        public string SkySatCollectvis { get { return _SkySatCollectvis; } set { _SkySatCollectvis = value; OnPropertyChanged("SkySatCollectvis"); } }

        private string _Landsat8L1Gvis = "Hidden";
        public string Landsat8L1Gvis { get { return _Landsat8L1Gvis; } set { _Landsat8L1Gvis = value; OnPropertyChanged("Landsat8L1Gvis"); } }

        private string _Sentinel2L1Cvis = "Hidden";
        public string Sentinel2L1Cvis { get { return _Sentinel2L1Cvis; } set { _Sentinel2L1Cvis = value; OnPropertyChanged("Sentinel2L1Cvis"); } }
        #endregion

        private ObservableCollection<PSScene4Band> _psscene4Band;
        public ObservableCollection<PSScene4Band> PSScene4Band
        {
            get
            {

                if (_psscene4Band == null)
                {
                    _psscene4Band = new ObservableCollection<PSScene4Band>();
                }
                return _psscene4Band;

            }
            set
            {
                _psscene4Band = value;
                OnPropertyChanged("PSScene4Band");
            }
        }
        private ObservableCollection<PSScene4Band> _psscene3Band;
        public ObservableCollection<PSScene4Band> PSScene3Band
        {
            get
            {

                if (_psscene3Band == null)
                {
                    _psscene3Band = new ObservableCollection<PSScene4Band>();
                }
                return _psscene3Band;

            }
            set
            {
                _psscene3Band = value;
                OnPropertyChanged("PSScene3Band");
            }
        }
        private ObservableCollection<PSScene4Band> _PSOrthoTile;
        public ObservableCollection<PSScene4Band> PSOrthoTile
        {
            get
            {

                if (_PSOrthoTile == null)
                {
                    _PSOrthoTile = new ObservableCollection<PSScene4Band>();
                }
                return _PSOrthoTile;

            }
            set
            {
                _PSOrthoTile = value;
                OnPropertyChanged("PSOrthoTile");
            }
        }

        private ObservableCollection<PSScene4Band> _REOrthoTile;
        public ObservableCollection<PSScene4Band> REOrthoTile
        {
            get
            {

                if (_REOrthoTile == null)
                {
                    _REOrthoTile = new ObservableCollection<PSScene4Band>();
                }
                return _REOrthoTile;

            }
            set
            {
                _REOrthoTile = value;
                OnPropertyChanged("REOrthoTile");
            }
        }
        private ObservableCollection<PSScene4Band> _SkySatScene;
        public ObservableCollection<PSScene4Band> SkySatScene
        {
            get
            {

                if (_SkySatScene == null)
                {
                    _SkySatScene = new ObservableCollection<PSScene4Band>();
                }
                return _SkySatScene;

            }
            set
            {
                _SkySatScene = value;
                OnPropertyChanged("SkySatScene");
            }
        }
        private ObservableCollection<PSScene4Band> _SkySatCollect;
        public ObservableCollection<PSScene4Band> SkySatCollect
        {
            get
            {

                if (_SkySatCollect == null)
                {
                    _SkySatCollect = new ObservableCollection<PSScene4Band>();
                }
                return _SkySatCollect;

            }
            set
            {
                _SkySatCollect = value;
                OnPropertyChanged("SkySatCollect");
            }
        }
        private ObservableCollection<PSScene4Band> _REScene;
        public ObservableCollection<PSScene4Band> REScene
        {
            get
            {

                if (_REScene == null)
                {
                    _REScene = new ObservableCollection<PSScene4Band>();
                }
                return _REScene;

            }
            set
            {
                _REScene = value;
                OnPropertyChanged("REScene");
            }
        }
        private ObservableCollection<PSScene4Band> _Landsat8L1G;
        public ObservableCollection<PSScene4Band> Landsat8L1G
        {
            get
            {

                if (_Landsat8L1G == null)
                {
                    _Landsat8L1G = new ObservableCollection<PSScene4Band>();
                }
                return _Landsat8L1G;

            }
            set
            {
                _Landsat8L1G = value;
                OnPropertyChanged("Landsat8L1G");
            }
        }
        private ObservableCollection<PSScene4Band> _Sentinel2L1C;
        public ObservableCollection<PSScene4Band> Sentinel2L1C
        {
            get
            {

                if (_Sentinel2L1C == null)
                {
                    _Sentinel2L1C = new ObservableCollection<PSScene4Band>();
                }
                return _Sentinel2L1C;

            }
            set
            {
                _Sentinel2L1C = value;
                OnPropertyChanged("Sentinel2L1C");
            }
        }
        private void doUpdateItems()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            HttpClient client = new HttpClient(handler)
            {

                BaseAddress = new Uri("https://api.planet.com")
            };
            var byteArray = Encoding.ASCII.GetBytes(Module1.Current.API_KEY.API_KEY_Value + ":hgvhgv");
            client.DefaultRequestHeaders.Host = "api.planet.com";
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("User-Agent", "ArcGISProC#");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            foreach (Asset asset in _selectedAssets)
            {
                HttpRequestMessage request = new HttpRequestMessage();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true
                };
                request.Headers.Host = "api.planet.com";
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri("https://api.planet.com/data/v1/item-types/" + asset.properties.item_type + "/items/" + asset.id + "/assets");
                try
                {
                    using (HttpResponseMessage httpResponse = _client.SendAsync(request).Result)
                    {
                        using (HttpContent content2 = httpResponse.Content)
                        {
                            var json2 = content2.ReadAsStringAsync().Result;
                            PSScene4Band psscene4Band = JsonConvert.DeserializeObject<PSScene4Band>(json2);
                            psscene4Band.properties = asset.properties;
                            psscene4Band.id = asset.id;
                            psscene4Band._links = asset._links;
                            psscene4Band._permissions = asset._permissions;
                            switch (psscene4Band.properties.item_type)
                            {
                                case "PSScene4Band":
                                    PSScene4Band.Add(psscene4Band);
                                    if (PSScene4BandVis != "Visible")
                                    {
                                        PSScene4BandVis = "Visible";
                                    }
                                    break;
                                case "PSScene3Band":
                                    PSScene3Band.Add(psscene4Band);
                                    if (PSScene3BandVis != "Visible")
                                    {
                                        PSScene3BandVis = "Visible";
                                    }
                                    break;
                                case "PSOrthoTile":
                                    PSOrthoTile.Add(psscene4Band);
                                    if (PSOrthoTileVis != "Visible")
                                    {
                                        PSOrthoTileVis = "Visible";
                                    }
                                    break;
                                case "REOrthoTile":
                                    REOrthoTile.Add(psscene4Band);
                                    if (REOrthoTilevis != "Visible")
                                    {
                                        REOrthoTilevis = "Visible";
                                    }
                                    break;
                                case "REScene":
                                    REScene.Add(psscene4Band);
                                    if (REScenevis != "Visible")
                                    {
                                        REScenevis = "Visible";
                                    }
                                    break;
                                case "SkySatScene":
                                    SkySatScene.Add(psscene4Band);
                                    if (SkySatScenevis != "Visible")
                                    {
                                        SkySatScenevis = "Visible";
                                    }
                                    break;
                                case "SkySatCollect":
                                    SkySatCollect.Add(psscene4Band);
                                    if (SkySatCollectvis != "Visible")
                                    {
                                        SkySatCollectvis = "Visible";
                                    }
                                    break;
                                case "Landsat8L1G":
                                    Landsat8L1G.Add(psscene4Band);
                                    if (Landsat8L1Gvis != "Visible")
                                    {
                                        Landsat8L1Gvis = "Visible";
                                    }
                                    break;
                                case "Sentinel2L1C":
                                    Sentinel2L1C.Add(psscene4Band);
                                    if (Sentinel2L1Cvis != "Visible")
                                    {
                                        Sentinel2L1Cvis = "Visible";
                                    }
                                    break;
                                default:
                                    break;
                            }
                            //if (psscene4Band.properties.item_type == "PSScene4Band")
                            //{
                            //    PSScene4Band.Add(psscene4Band);
                            //}
                            
                            //if (psscene4Band.properties.item_type == "PSScene3Band")
                            //{
                            //    PSScene3Band.Add(psscene4Band);
                            //}


                        }
                    }
                }
                catch (Exception ex)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool CanExecuteOrder { get; set; } = true;
        private ICommand _ordercommand;
        public ICommand OrderCommand
        {
            get
            {
                if (_ordercommand == null)
                    _ordercommand = new CommandHandler3(() => DoOrder(), CanExecuteOrder);
                return _ordercommand;
            }
        }

        private void DoOrder()
        {
            Order order = new Order();
            order.name = _orderName;
            Delivery delivery = new Delivery();
            order.delivery = delivery;
            order.delivery.single_archive = true;
            List<Product> productlist = new List<Product>();
            List<string> analyticitem_ids = null;
            List<string> visualitem_ids = null;
            List<string> basicitem_ids = null;
            List<string> selectteditem_types = new List<string>();
            List<string> selecttedproduct_bundles = new List<string>();
            //Dictionary<string, string> pppp = new Dictionary<string, string>;
            List<Tuple<string, string>> pppp = new List<Tuple<string, string>>();
            var combined = PSScene4Band.Concat(PSScene3Band).Concat(PSOrthoTile).Concat(REOrthoTile).Concat(REScene).Concat(SkySatScene).Concat(SkySatCollect).Concat(Landsat8L1G).Concat(Sentinel2L1C) ;
            foreach (PSScene4Band pSScene4Band in combined)
            {
                if (pSScene4Band.selectedBundle == " ")
                {
                    break;
                }
                Tuple<string, string> tuple = Tuple.Create(pSScene4Band.properties.item_type, pSScene4Band.selectedBundle);
                if (!selecttedproduct_bundles.Contains(pSScene4Band.selectedBundle))
                {
                    selecttedproduct_bundles.Add(pSScene4Band.selectedBundle);
                }
                if (!selectteditem_types.Contains(pSScene4Band.properties.item_type))
                {
                    selectteditem_types.Add(pSScene4Band.properties.item_type);
                }
            }
            foreach (var item in selecttedproduct_bundles)
            {
                foreach (PSScene4Band pSScene4Band in combined)
                {
                    if (pSScene4Band.selectedBundle == item)
                    {
                        pppp.Add(Tuple.Create(item, pSScene4Band.properties.item_type));
                    }
                }
                

            }
            var asdasd = pppp.Distinct();
            foreach (var item in asdasd)
            {
                Product product = new Product();
                product.product_bundle = item.Item1;
                product.item_type = item.Item2;
                List<string> ids = new List<string>();
                foreach (PSScene4Band pSScene4Band in combined)
                {
                    if (pSScene4Band.properties.item_type == item.Item2 && pSScene4Band.selectedBundle == item.Item1)
                    {

                        ids.Add(pSScene4Band.id);
                    }
                }
                if (ids.Count > 0)
                {
                    product.item_ids = ids.ToArray();
                    productlist.Add(product);
                }

            }

            //foreach (PSScene4Band pSScene4Band in PSScene4Band)
            //{
            //    if (!selectteditem_types.Contains(pSScene4Band.properties.item_type))
            //    {
            //        selectteditem_types.Add(pSScene4Band.properties.item_type);
            //    }
            //    if (!selecttedproduct_bundles.Contains(pSScene4Band.selectedBundle))
            //    {
            //        selecttedproduct_bundles.Add(pSScene4Band.selectedBundle);
            //    }
            //    if (pSScene4Band.oAnalytic)
            //    {
            //        if (!selecttedproduct_bundles.Contains("analytic"))
            //        {
            //            selecttedproduct_bundles.Add("analytic");
            //        }
            //        if (analyticitem_ids == null)
            //        {
            //            analyticitem_ids = new List<string>();
            //        }
            //        analyticitem_ids.Add(pSScene4Band.id);
            //    }
            //    if (pSScene4Band.oVisual)
            //    {
            //        if (!selecttedproduct_bundles.Contains("visual"))
            //        {
            //            selecttedproduct_bundles.Add("visual");
            //        }
            //        if (visualitem_ids == null)
            //        {
            //            visualitem_ids = new List<string>();
            //        }
            //        visualitem_ids.Add(pSScene4Band.id);
            //    }
            //    if (pSScene4Band.oBasic)
            //    {
            //        if (!selecttedproduct_bundles.Contains("basic_analytic"))
            //        {
            //            selecttedproduct_bundles.Add("basic_analytic");
            //        }
            //        if (!selecttedproduct_bundles.Contains("basic_uncalibrated_dn"))
            //        {
            //            selecttedproduct_bundles.Add("basic_uncalibrated_dn");
            //        }
            //        if (basicitem_ids == null)
            //        {
            //            basicitem_ids = new List<string>();
            //        }
            //        basicitem_ids.Add(pSScene4Band.id);
            //    }
            //}

            //foreach (string  item in selecttedproduct_bundles)
            //{
            //    foreach (string selected_item in selectteditem_types)
            //    {
            //        Product product = new Product();
            //        product.product_bundle = item;
            //        product.item_type = selected_item;
            //        switch (item)
            //        {
            //            case "analytic":
            //                product.item_ids = analyticitem_ids.ToArray();
            //                break;
            //            case "basic_analytic":
            //                product.item_ids = basicitem_ids.ToArray();
            //                break;
            //            case "basic_uncalibrated_dn":
            //                product.item_ids = basicitem_ids.ToArray();
            //                break;
            //            case "visual":
            //                product.item_ids = visualitem_ids.ToArray();
            //                break;
            //            default:
            //                break;
            //        }
            //        productlist.Add(product);

            //    }
            //}
            order.products = productlist.ToArray();
            order.delivery.archive_filename = _orderName + ".zip";
            order.delivery.archive_type = "zip";

            if (order.products.Length == 0 )
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("No bundles selected. Please choose at least one bundle for one Item to download");
                return;
            }
            string json = JsonConvert.SerializeObject(order, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore

            });

            HttpClientHandler _handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            HttpClient _client = new HttpClient(_handler)
            {
                BaseAddress = new Uri("https://api.planet.com")

            };
            var byteArray = Encoding.ASCII.GetBytes("1fe575980e78467f9c28b552294ea410:");
            _client.DefaultRequestHeaders.Host = "api.planet.com";
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.16.3");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            //_client.DefaultRequestHeaders.ExpectContinue = false;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "compute/ops/orders/v2");
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "v0/orders/");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            request.Headers.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
            request.Headers.Host = "api.planet.com";
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            request.Headers.CacheControl = new CacheControlHeaderValue();
            request.Headers.CacheControl.NoCache = true;
            //string json = "{ \"name\":\"Prod5\",\"products\":[{\"item_ids\":[\"20190914_195736_0f2b\",\"20190914_195737_0f2b\"],\"item_type\":\"PSScene4Band\",\"product_bundle\":\"analytic\"}],\"include_metadata_assets\":true,\"order_type\":\"partial\",\"delivery\":{\"single_archive\":true,\"archive_type\":\"zip\"}}";
            //string json = "{\"name\":\"Pro4\",\"products\":[{\"item_ids\":[\"20190910_205244_101b\",\"20190908_195741_1048\"],\"item_type\":\"PSScene4Band\",\"product_bundle\":\"analytic\"}],\"include_metadata_assets\":true,\"order_type\":\"partial\",\"delivery\":{\"single_archive\":true,\"archive_type\":\"zip\"}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            request.Content = content;
            try
            {
                using (HttpResponseMessage httpResponse = _client.SendAsync(request).Result)
                {
                    using (HttpContent content2 = httpResponse.Content)
                    {
                        var json2 = content2.ReadAsStringAsync().Result;
                        OrderResponse2 orderResponse2 = JsonConvert.DeserializeObject<OrderResponse2>(json2);
                        if (orderResponse2.state == "failed")
                        {
                            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("There was an error placing the order. Possible problems are:" + Environment.NewLine + orderResponse2.error_hints.ToString());
                        }
                        else if (orderResponse2.state == "initializing" || orderResponse2.state == "queued")
                        {
                            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("The order has been placed and is being processed." + Environment.NewLine + "Please saee the Orders tab for details." + Environment.NewLine + "Order Name:" + orderResponse2.name.ToString(),"Order Success",System.Windows.MessageBoxButton.OK);
                        }
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse resp = e.Response;
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        string resps = sr.ReadToEnd();
                        //Response.Write(sr.ReadToEnd());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "compute/ops/orders/v2");
            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            //request.Headers.CacheControl = new CacheControlHeaderValue
            //{
            //    NoCache = true
            //};
            //request.Headers.Host = "api.planet.com";
            //request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            //request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            //request.Headers.CacheControl = new CacheControlHeaderValue();
            //request.Headers.CacheControl.NoCache = true;
            
            //var content = new StringContent(json, Encoding.UTF8, "application/json");
            //content.Headers.Remove("Content-Type");
            //content.Headers.Add("Content-Type", "application/json");
            ////request.Headers.Remove("Content-Type");
            ////request.Headers.Add("Content-Type", "application/json");
            //request.Content = content;

            //try
            //{
            //    using (HttpResponseMessage httpResponse = _client.SendAsync(request).Result)
            //    {
            //        using (HttpContent content2 = httpResponse.Content)
            //        {
            //            var json2 = content2.ReadAsStringAsync().Result;
            //            var ff = content2.ReadAsStreamAsync().Result;
            //            StreamReader theStreamReader = new StreamReader(ff);
            //            string theLine = null;

            //            while ((theLine = theStreamReader.ReadLine()) != null)
            //            {
            //                Console.WriteLine(theLine);
            //            }
            //            PastOrder quickSearchResult = JsonConvert.DeserializeObject<PastOrder>(json2);
            //        }
            //        //using (System.Net.Http.StreamContent sr = new System.Net.Http.StreamContent(httpResponse.Content.ReadAsStringAsync))
            //        //{
            //        //    string resps = sr.ReadAsStringAsync().Result();
            //        //    //Response.Write(sr.ReadToEnd());

            //        //}
            //    }
            //}
            //catch (WebException e)
            //{
            //    if (e.Status == WebExceptionStatus.ProtocolError)
            //    {
            //        WebResponse resp = e.Response;
            //        using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            //        {
            //            string resps = sr.ReadToEnd();
            //            //Response.Write(sr.ReadToEnd());
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            //}
        }

        private ICollectionView _SkySatSceneListView;
        public ICollectionView SkySatSceneListView
        {
            get { return this._SkySatSceneListView; }
            private set
            {
                if (value == this._SkySatSceneListView)
                {
                    return;
                }

                this._SkySatSceneListView = value;
                OnPropertyChanged("SkySatSceneListView");
            }
        }
        private ICollectionView _PSOrthoTileListView;
        public ICollectionView PSOrthoTileListView
        {
            get { return this._PSOrthoTileListView; }
            private set
            {
                if (value == this._PSOrthoTileListView)
                {
                    return;
                }

                this._PSOrthoTileListView = value;
                OnPropertyChanged("PSOrthoTileListView");
            }
        }

        private ICollectionView _SkySatCollectListView;
        public ICollectionView SkySatCollectListView
        {
            get { return this._SkySatCollectListView; }
            private set
            {
                if (value == this._SkySatCollectListView)
                {
                    return;
                }

                this._SkySatCollectListView = value;
                OnPropertyChanged("SkySatCollectListView");
            }
        }
        private ICollectionView _RESceneListView;
        public ICollectionView RESceneListView
        {
            get { return this._RESceneListView; }
            private set
            {
                if (value == this._RESceneListView)
                {
                    return;
                }

                this._RESceneListView = value;
                OnPropertyChanged("RESceneListView");
            }
        }
        private ICollectionView _PSScene3BandListView;
        public ICollectionView PSScene3BandListView
        {
            get { return this._PSScene3BandListView; }
            private set
            {
                if (value == this._PSScene3BandListView)
                {
                    return;
                }

                this._PSScene3BandListView = value;
                OnPropertyChanged("PSScene3BandListView");
                _PSScene3BandListView.Refresh();
            }
        }
        private ICollectionView _PSScene4BandListView;
        public ICollectionView PSScene4BandListView
        {
            get { return this._PSScene4BandListView; }
            private set
            {
                if (value == this._PSScene4BandListView)
                {
                    return;
                }

                this._PSScene4BandListView = value;
                OnPropertyChanged("PSScene4BandListView");
            }
        }
        
        private ICollectionView _REOrthoTileListView;
        public ICollectionView REOrthoTileListView
        {
            get { return this._REOrthoTileListView; }
            private set
            {
                if (value == this._REOrthoTileListView)
                {
                    return;
                }

                this._REOrthoTileListView = value;
                OnPropertyChanged("REOrthoTileListView");
            }
        }

        private ICollectionView _Landsat8L1GListView;
        public ICollectionView Landsat8L1GListView
        {
            get { return this._Landsat8L1GListView; }
            private set
            {
                if (value == this._Landsat8L1GListView)
                {
                    return;
                }

                this._Landsat8L1GListView = value;
                OnPropertyChanged("Landsat8L1GListView");
            }
        }

        private ICollectionView _Sentinel2L1CListView;
        public ICollectionView Sentinel2L1CListView
        {
            get { return this._Sentinel2L1CListView; }
            private set
            {
                if (value == this._Sentinel2L1CListView)
                {
                    return;
                }

                this._Sentinel2L1CListView = value;
                OnPropertyChanged("Sentinel2L1CListView");
            }
        }

        #region Filters
        public bool FilterMessageList(object item)
        {
            bool ismatch = false;
            Asset asset = (Asset)item;
            if (asset != null )
            {
                if (asset.properties.item_type == "SkySatScene")
                {
                    ismatch = true;
                }

            }
            return ismatch;
        }
        
        public bool FilterPSScene4Band(object item)
        {
            bool ismatch = false;
            Asset asset = (Asset)item;
            if (asset != null)
            {
                if (asset.properties.item_type == "PSScene4Band")
                {
                    ismatch = true;
                }
            }
            return ismatch;
        }
        public bool FilterMessageList2(object item)
        {
            bool ismatch = false;
            Asset asset = (Asset)item;
            if (asset != null)
            {
                if (asset.properties.item_type == "PSOrthoTile")
                {
                    ismatch = true;
                }

            }
            return ismatch;
        }
        public bool FilterREOrthoTile(object item)
        {
            bool ismatch = false;
            Asset asset = (Asset)item;
            if (asset != null)
            {
                if (asset.properties.item_type == "REOrthoTile")
                {
                    ismatch = true;
                }

            }
            return ismatch;
        }
        public bool FilterREScene(object item)
        {
            bool ismatch = false;
            Asset asset = (Asset)item;
            if (asset != null)
            {
                if (asset.properties.item_type == "REScene")
                {
                    ismatch = true;
                }

            }
            return ismatch;
        }
        public bool FilterSkySatCollect(object item)
        {
            bool ismatch = false;
            Asset asset = (Asset)item;
            if (asset != null)
            {
                if (asset.properties.item_type == "SkySatCollect")
                {
                    ismatch = true;
                }

            }
            return ismatch;
        }
        public bool FilterLandsat8L1G(object item)
        {
            bool ismatch = false;
            Asset asset = (Asset)item;
            if (asset != null)
            {
                if (asset.properties.item_type == "Landsat8L1G")
                {
                    ismatch = true;
                }

            }
            return ismatch;
        }
        public bool FilterSentinel2L1C(object item)
        {
            bool ismatch = false;
            Asset asset = (Asset)item;
            if (asset != null)
            {
                if (asset.properties.item_type == "Sentinel2L1C")
                {
                    ismatch = true;
                }

            }
            return ismatch;
        }
        public bool FilterPSScene3Band(object item)
        {
            bool ismatch = false;
            Asset asset = (Asset)item;
            if (asset != null)
            {
                if (asset.properties.item_type == "PSScene3Band")
                {
                    ismatch = true;
                }

            }
            return ismatch;
        }

        #endregion

        #region cboLists
        private List<string> _lstPSScene4Band;
        public List<string> lstPSScene4Band
        {
            get { return _lstPSScene4Band; }
            set
            {
                if (_lstPSScene4Band ==  null)
                {
                    _lstPSScene4Band = new List<string>();
                }
                _lstPSScene4Band = value;
                OnPropertyChanged("lstPSScene4Band");
            }
        }
        public List<string> lstPSScene3Band { get; set; }
        public List<string> lstPSOrthoTile { get; set; }
        public List<string> lstREOrthoTile { get; set; }
        public List<string> lstREScene { get; set; }
        public List<string> lstSkySatScene { get; set; }
        public List<string> lstSkySatCollect { get; set; }
        public List<string> lstLandsat8L1G { get; set; }
        public List<string> lstSentinel2L1C { get; set; }

        #endregion
    }
}