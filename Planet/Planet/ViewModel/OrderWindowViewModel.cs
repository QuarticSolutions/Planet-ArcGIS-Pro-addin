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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using ArcGIS.Desktop.Framework;
using System.Text.RegularExpressions;

namespace Planet.ViewModel
{
    class OrderWindowViewModel : INotifyPropertyChanged
    {
        private string _btnOrdertext = "Place Order";
        private bool _hasOrdered = false;
        public string btnOrdertext
        {
            get
            {
                return _btnOrdertext;
            }
            set
            {
                _btnOrdertext = value;
                OnPropertyChanged(btnOrdertext);
            }
        }
        private string _orderName;
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
                //if (!string.IsNullOrWhiteSpace(_orderName))
                //{
                //    CanExecuteOrder = true;
                //}
            }
        }
        private static HttpClientHandler _handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        private HttpClient _client = new HttpClient(_handler)
        {
            BaseAddress = new Uri("https://api.planet.com")
        };
        private ObservableCollection<Asset> _selectedAssets = new ObservableCollection<Asset>();
        //private string ResultsColl;


        public OrderWindowViewModel()
        {
            var byteArray = Encoding.ASCII.GetBytes(Module1.Current.API_KEY.API_KEY_Value + ":hgvhgv");
            _client.DefaultRequestHeaders.Host = "api.planet.com";
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _client.DefaultRequestHeaders.Add("User-Agent", "ArcGISProC#");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            _client.DefaultRequestHeaders.ExpectContinue = false;
            //TempBundles = new tempBundles();
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
                                lstPSScene3Band.Add(" ");
                            }
                            lstPSScene3Band.Add(item.BundleName);
                            break;
                        case "PSOrthoTile":
                            if (lstPSOrthoTile == null)
                            {
                                lstPSOrthoTile = new List<string>();
                                lstPSOrthoTile.Add(" ");
                            }
                            lstPSOrthoTile.Add(item.BundleName);
                            break;
                        case "REOrthoTile":
                            if (lstREOrthoTile == null)
                            {
                                lstREOrthoTile = new List<string>();
                                lstREOrthoTile.Add(" ");
                            }
                            lstREOrthoTile.Add(item.BundleName);
                            break;
                        case "REScene":
                            if (lstREScene == null)
                            {
                                lstREScene = new List<string>();
                                lstREScene.Add(" ");
                            }
                            lstREScene.Add(item.BundleName);
                            break;
                        case "SkySatScene":
                            if (lstSkySatScene == null)
                            {
                                lstSkySatScene = new List<string>();
                                lstSkySatScene.Add(" ");
                            }
                            lstSkySatScene.Add(item.BundleName);
                            break;
                        case "SkySatCollect":
                            if (lstSkySatCollect == null)
                            {
                                lstSkySatCollect = new List<string>();
                                lstSkySatCollect.Add(" ");
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
            }
        }

        private ObservableCollection<string> _resultscoll = new ObservableCollection<string>();
        public ObservableCollection<string> Resultscoll
        {
            get
            {
                return _resultscoll;
            }
            set
            {
                if (_resultscoll is null)
                {
                    _resultscoll = new ObservableCollection<string>();
                }
                _resultscoll = value;
                OnPropertyChanged("Resultscoll");
            }

        }

        #region listboxvisibility


        private string _psscene4Bandvis = "Collapsed";
        public string PSScene4BandVis { get { return _psscene4Bandvis; } set { _psscene4Bandvis = value; OnPropertyChanged("PSScene4BandVis"); } }

        private string _psscene3Bandvis = "Collapsed";
        public string PSScene3BandVis { get { return _psscene3Bandvis; } set { _psscene3Bandvis = value; OnPropertyChanged("PSScene3BandVis"); } }

        private string _PSOrthoTilevis = "Collapsed";
        public string PSOrthoTileVis { get { return _PSOrthoTilevis; } set { _PSOrthoTilevis = value; OnPropertyChanged("PSOrthoTileVis"); } }

        private string _REOrthoTilevis = "Collapsed";
        public string REOrthoTilevis { get { return _REOrthoTilevis; } set { _REOrthoTilevis = value; OnPropertyChanged("REOrthoTilevis"); } }

        private string _REScenevis = "Collapsed";
        public string REScenevis { get { return _REScenevis; } set { _REScenevis = value; OnPropertyChanged("REScenevis"); } }

        private string _SkySatScenevis = "Collapsed";
        public string SkySatScenevis { get { return _SkySatScenevis; } set { _SkySatScenevis = value; OnPropertyChanged("SkySatScenevis"); } }

        private string _SkySatCollectvis = "Collapsed";
        public string SkySatCollectvis { get { return _SkySatCollectvis; } set { _SkySatCollectvis = value; OnPropertyChanged("SkySatCollectvis"); } }

        private string _Landsat8L1Gvis = "Collapsed";
        public string Landsat8L1Gvis { get { return _Landsat8L1Gvis; } set { _Landsat8L1Gvis = value; OnPropertyChanged("Landsat8L1Gvis"); } }

        private string _Sentinel2L1Cvis = "Collapsed";
        public string Sentinel2L1Cvis { get { return _Sentinel2L1Cvis; } set { _Sentinel2L1Cvis = value; OnPropertyChanged("Sentinel2L1Cvis"); } }
        #endregion


        #region imagery product group all selected bools

        private bool _psscene4Bandselall = false;
        public bool psscene4Bandselall
        {
            get { return _psscene4Bandselall; }
            set
            {
                _psscene4Bandselall = value;
                OnPropertyChanged("psscene4Bandselall");
                foreach (PSScene4Band pSScene4 in PSScene4Band)
                {
                    pSScene4.oAnalytic = _psscene4Bandselall;
                }
            }
        }

        private bool _psscene3Bandselall = false;
        public bool psscene3Bandselall
        {
            get { return _psscene3Bandselall; }
            set
            {
                _psscene3Bandselall = value;
                OnPropertyChanged("psscene3Bandselall");
                foreach (PSScene4Band pSScene4 in PSScene3Band)
                {
                    pSScene4.oAnalytic = _psscene3Bandselall;
                }
            }
        }

        private bool _PSOrthoTileselall = false;
        public bool PSOrthoTileselall
        {
            get { return _PSOrthoTileselall; }
            set
            {
                _PSOrthoTileselall = value;
                OnPropertyChanged("PSOrthoTileselall");
                foreach (PSScene4Band pSScene4 in PSOrthoTile)
                {
                    pSScene4.oAnalytic = _PSOrthoTileselall;
                }
            }
        }
        private bool _REOrthoTileselall = false;
        public bool REOrthoTileselall
        {
            get { return _REOrthoTileselall; }
            set
            {
                _REOrthoTileselall = value;
                OnPropertyChanged("REOrthoTileselall");
                foreach (PSScene4Band pSScene4 in REOrthoTile)
                {
                    pSScene4.oAnalytic = _REOrthoTileselall;
                }
            }
        }
        private bool _RESceneselall = false;
        public bool RESceneselall
        {
            get { return _RESceneselall; }
            set
            {
                _RESceneselall = value;
                OnPropertyChanged("RESceneselall");
                foreach (PSScene4Band pSScene4 in REScene)
                {
                    pSScene4.oAnalytic = _RESceneselall;
                }
            }
        }
        private bool _SkySatSceneselall = false;
        public bool SkySatSceneselall
        {
            get { return _SkySatSceneselall; }
            set
            {
                _SkySatSceneselall = value;
                OnPropertyChanged("SkySatSceneselall");
                foreach (PSScene4Band pSScene4 in SkySatScene)
                {
                    pSScene4.oAnalytic = _SkySatSceneselall;
                }
            }
        }
        private bool _SkySatCollectselall = false;
        public bool SkySatCollectselall
        {
            get { return _SkySatCollectselall; }
            set
            {
                _SkySatCollectselall = value;
                OnPropertyChanged("SkySatCollectselall");
                foreach (PSScene4Band pSScene4 in SkySatCollect)
                {
                    pSScene4.oAnalytic = _SkySatCollectselall;
                }
            }
        }

        private bool _Landsat8L1Gselall = false;
        public bool Landsat8L1Gselall
        {
            get { return _Landsat8L1Gselall; }
            set
            {
                _Landsat8L1Gselall = value;
                OnPropertyChanged("Landsat8L1Gselall");
                foreach (PSScene4Band pSScene4 in Landsat8L1G)
                {
                    pSScene4.oAnalytic = _Landsat8L1Gselall;
                }
            }
        }
        private bool _Sentinel2L1Cselall = false;
        public bool Sentinel2L1Cselall
        {
            get { return _Sentinel2L1Cselall; }
            set
            {
                _Sentinel2L1Cselall = value;
                OnPropertyChanged("Sentinel2L1Cselall");
                foreach (PSScene4Band pSScene4 in Sentinel2L1C)
                {
                    pSScene4.oAnalytic = _Sentinel2L1Cselall;
                }
            }
        }

        #endregion


        #region coboBox currently selected values
        private string _PSScene4Bandcurrselect;
        public string PSScene4Bandcurrselect
        {
            get { return _PSScene4Bandcurrselect; }
            set {
                _PSScene4Bandcurrselect = value;
                foreach (PSScene4Band item in PSScene4Band)
                {
                    item.selectedBundle = _PSScene4Bandcurrselect;
                }
                OnPropertyChanged("PSScene4Bandcurrselect");
            }
        }
        private string _PSScene3Bandcurrselect;
        public string PSScene3Bandcurrselect
        {
            get { return _PSScene3Bandcurrselect; }
            set
            {
                _PSScene3Bandcurrselect = value;
                foreach (PSScene4Band item in PSScene3Band)
                {
                    item.selectedBundle = _PSScene3Bandcurrselect;
                }
                OnPropertyChanged("PSScene3Bandcurrselect");
            }
        }
        private string _PSOrthoTilecurrselect;
        public string PSOrthoTilecurrselect
        {
            get { return _PSOrthoTilecurrselect; }
            set
            {
                _PSOrthoTilecurrselect = value;
                foreach (PSScene4Band item in PSOrthoTile)
                {
                    item.selectedBundle = _PSOrthoTilecurrselect;
                }
                OnPropertyChanged("PSOrthoTilecurrselect");
            }
        }

        private string _REOrthoTilecurrselect;
        public string REOrthoTilecurrselect
        {
            get { return _PSOrthoTilecurrselect; }
            set
            {
                _REOrthoTilecurrselect = value;
                foreach (PSScene4Band item in REOrthoTile)
                {
                    item.selectedBundle = _REOrthoTilecurrselect;
                }
                OnPropertyChanged("REOrthoTilecurrselect");
            }
        }

        private string _REScenecurrselect;
        public string REScenecurrselect
        {
            get { return _PSOrthoTilecurrselect; }
            set
            {
                _REScenecurrselect = value;
                foreach (PSScene4Band item in REScene)
                {
                    item.selectedBundle = _REScenecurrselect;
                }
                OnPropertyChanged("REScenecurrselect");
            }
        }

        private string _SkySatScenecurrselect;
        public string SkySatScenecurrselect
        {
            get { return _SkySatScenecurrselect; }
            set
            {
                _SkySatScenecurrselect = value;
                foreach (PSScene4Band item in SkySatScene)
                {
                    item.selectedBundle = _SkySatScenecurrselect;
                }
                OnPropertyChanged("SkySatScenecurrselect");
            }
        }

        private string _SkySatCollectcurrselect;
        public string SkySatCollectcurrselect
        {
            get { return _SkySatCollectcurrselect; }
            set
            {
                _SkySatCollectcurrselect = value;
                foreach (PSScene4Band item in SkySatCollect)
                {
                    item.selectedBundle = _SkySatCollectcurrselect;
                }
                OnPropertyChanged("SkySatCollectcurrselect");
            }
        }

        private string _Landsat8L1Gcurrselect;
        public string Landsat8L1Gcurrselect
        {
            get { return _Landsat8L1Gcurrselect; }
            set
            {
                _Landsat8L1Gcurrselect = value;
                foreach (PSScene4Band item in Landsat8L1G)
                {
                    item.selectedBundle = _Landsat8L1Gcurrselect;
                }
                OnPropertyChanged("Landsat8L1Gcurrselect");
            }
        }

        private string _Sentinel2L1Ccurrselect;
        public string Sentinel2L1Ccurrselect
        {
            get { return _Sentinel2L1Ccurrselect; }
            set
            {
                _Sentinel2L1Ccurrselect = value;
                foreach (PSScene4Band item in Sentinel2L1C)
                {
                    item.selectedBundle = _Sentinel2L1Ccurrselect;
                }
                OnPropertyChanged("Sentinel2L1Ccurrselect");
            }
        }
        #endregion

        #region imagery product collections

        private List<ObservableCollection<PSScene4Band>> _products = new List<ObservableCollection<PSScene4Band>>();


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
        #endregion

        private void doUpdateItems()
        {
            try
            {
                foreach (Asset asset in _selectedAssets)
                {
                    PSScene4Band psscene4Band = new PSScene4Band();
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
                                _products.Add(PSScene4Band);
                            }
                            break;
                        case "PSScene3Band":
                            PSScene3Band.Add(psscene4Band);
                            if (PSScene3BandVis != "Visible")
                            {
                                PSScene3BandVis = "Visible";
                                _products.Add(PSScene3Band);
                            }
                            break;
                        case "PSOrthoTile":
                            PSOrthoTile.Add(psscene4Band);
                            if (PSOrthoTileVis != "Visible")
                            {
                                PSOrthoTileVis = "Visible";
                                _products.Add(PSOrthoTile);
                            }
                            break;
                        case "REOrthoTile":
                            REOrthoTile.Add(psscene4Band);
                            if (REOrthoTilevis != "Visible")
                            {
                                REOrthoTilevis = "Visible";
                                _products.Add(REOrthoTile);
                            }
                            break;
                        case "REScene":
                            REScene.Add(psscene4Band);
                            if (REScenevis != "Visible")
                            {
                                REScenevis = "Visible";
                                _products.Add(REScene);
                            }
                            break;
                        case "SkySatScene":
                            SkySatScene.Add(psscene4Band);
                            if (SkySatScenevis != "Visible")
                            {
                                SkySatScenevis = "Visible";
                                _products.Add(SkySatScene);
                            }
                            break;
                        case "SkySatCollect":
                            SkySatCollect.Add(psscene4Band);
                            if (SkySatCollectvis != "Visible")
                            {
                                SkySatCollectvis = "Visible";
                                _products.Add(SkySatCollect);
                            }
                            break;
                        case "Landsat8L1G":
                            Landsat8L1G.Add(psscene4Band);
                            if (Landsat8L1Gvis != "Visible")
                            {
                                Landsat8L1Gvis = "Visible";
                                _products.Add(Landsat8L1G);
                            }
                            break;
                        case "Sentinel2L1C":
                            Sentinel2L1C.Add(psscene4Band);
                            if (Sentinel2L1Cvis != "Visible")
                            {
                                Sentinel2L1Cvis = "Visible";
                                _products.Add(Sentinel2L1C);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {

                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("There was an error processing the Itemes for ordering" + Environment.NewLine + "Please try again" + Environment.NewLine + ex.Message, "Processing error");
            }




            //HttpClientHandler handler = new HttpClientHandler()
            //{
            //    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            //};
            //int counter = 251;
            //using (HttpClient client = new HttpClient(handler))
            //{
            //    client.BaseAddress = new Uri("https://api.planet.com");
            //    var byteArray = Encoding.ASCII.GetBytes(Module1.Current.API_KEY.API_KEY_Value + ":hgvhgv");
            //    client.DefaultRequestHeaders.Host = "api.planet.com";
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            //    client.DefaultRequestHeaders.Add("User-Agent", "ArcGISProC#");
            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            //    foreach (Asset asset in _selectedAssets)
            //    {
            //        HttpRequestMessage request = new HttpRequestMessage();
            //        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //        request.Headers.CacheControl = new CacheControlHeaderValue
            //        {
            //            NoCache = true
            //        };
            //        request.Headers.Host = "api.planet.com";
            //        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            //        request.Method = HttpMethod.Get;
            //        request.RequestUri = new Uri("https://api.planet.com/data/v1/item-types/" + asset.properties.item_type + "/items/" + asset.id + "/assets");
            //        try
            //        {
            //            using (HttpResponseMessage httpResponse = _client.SendAsync(request).Result)
            //            {
            //                if (httpResponse.IsSuccessStatusCode)
            //                {
            //                    using (HttpContent content2 = httpResponse.Content)
            //                    {
            //                        var json2 = content2.ReadAsStringAsync().Result;
            //                        PSScene4Band psscene4Band = JsonConvert.DeserializeObject<PSScene4Band>(json2);
            //                        psscene4Band.properties = asset.properties;
            //                        psscene4Band.id = asset.id;
            //                        psscene4Band._links = asset._links;
            //                        psscene4Band._permissions = asset._permissions;
            //                        switch (psscene4Band.properties.item_type)
            //                        {
            //                            case "PSScene4Band":
            //                                PSScene4Band.Add(psscene4Band);
            //                                if (PSScene4BandVis != "Visible")
            //                                {
            //                                    PSScene4BandVis = "Visible";
            //                                    _products.Add(PSScene4Band);
            //                                }
            //                                break;
            //                            case "PSScene3Band":
            //                                PSScene3Band.Add(psscene4Band);
            //                                if (PSScene3BandVis != "Visible")
            //                                {
            //                                    PSScene3BandVis = "Visible";
            //                                    _products.Add(PSScene3Band);
            //                                }
            //                                break;
            //                            case "PSOrthoTile":
            //                                PSOrthoTile.Add(psscene4Band);
            //                                if (PSOrthoTileVis != "Visible")
            //                                {
            //                                    PSOrthoTileVis = "Visible";
            //                                    _products.Add(PSOrthoTile);
            //                                }
            //                                break;
            //                            case "REOrthoTile":
            //                                REOrthoTile.Add(psscene4Band);
            //                                if (REOrthoTilevis != "Visible")
            //                                {
            //                                    REOrthoTilevis = "Visible";
            //                                    _products.Add(REOrthoTile);
            //                                }
            //                                break;
            //                            case "REScene":
            //                                REScene.Add(psscene4Band);
            //                                if (REScenevis != "Visible")
            //                                {
            //                                    REScenevis = "Visible";
            //                                    _products.Add(REScene);
            //                                }
            //                                break;
            //                            case "SkySatScene":
            //                                SkySatScene.Add(psscene4Band);
            //                                if (SkySatScenevis != "Visible")
            //                                {
            //                                    SkySatScenevis = "Visible";
            //                                    _products.Add(SkySatScene);
            //                                }
            //                                break;
            //                            case "SkySatCollect":
            //                                SkySatCollect.Add(psscene4Band);
            //                                if (SkySatCollectvis != "Visible")
            //                                {
            //                                    SkySatCollectvis = "Visible";
            //                                    _products.Add(SkySatCollect);
            //                                }
            //                                break;
            //                            case "Landsat8L1G":
            //                                Landsat8L1G.Add(psscene4Band);
            //                                if (Landsat8L1Gvis != "Visible")
            //                                {
            //                                    Landsat8L1Gvis = "Visible";
            //                                    _products.Add(Landsat8L1G);
            //                                }
            //                                break;
            //                            case "Sentinel2L1C":
            //                                Sentinel2L1C.Add(psscene4Band);
            //                                if (Sentinel2L1Cvis != "Visible")
            //                                {
            //                                    Sentinel2L1Cvis = "Visible";
            //                                    _products.Add(Sentinel2L1C);
            //                                }
            //                                break;
            //                            default:
            //                                break;
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    if (httpResponse.ReasonPhrase == "Too Many Requests")
            //                    {
            //                        Thread.Sleep(counter);
            //                        counter = counter * 2;
            //                    }
            //                    else
            //                    {
            //                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Error getting past orders, the server returned an error: " + httpResponse.ReasonPhrase);
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            //        }
            //    }
            //}


        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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

            var combined = PSScene4Band.Concat(PSScene3Band).Concat(PSOrthoTile).Concat(REOrthoTile).Concat(REScene).Concat(SkySatScene).Concat(SkySatCollect).Concat(Landsat8L1G).Concat(Sentinel2L1C);
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
                    if (pSScene4Band.properties.item_type == item.Item2 && pSScene4Band.selectedBundle == item.Item1 && pSScene4Band.oAnalytic)
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

            foreach (Product product in productlist)
            {
                Order order2 = new Order();
                if (product.product_bundle == null)
                {
                    MessageBox.Show("No Product selected for " + product.item_type + Environment.NewLine + " Please select a Product type  or deselect the images and try again");
                    return;
                }
                if (product.product_bundle.ToString().LastIndexOf(",") > -1)
                {
                    order2.name = _orderName + "_" + product.item_type + "_" + product.product_bundle.ToString().Substring(0, product.product_bundle.ToString().LastIndexOf(","));
                }
                else
                {
                    order2.name = _orderName + "_" + product.item_type + "_" + product.product_bundle.ToString();
                }

                Delivery delivery2 = new Delivery();
                order2.delivery = delivery2;
                order2.delivery.single_archive = true;
                List<Product> listproduct = new List<Product>();
                listproduct.Add(product);
                order2.products = listproduct.ToArray();
                order2.delivery.archive_filename = _orderName + ".zip";
                order2.delivery.archive_type = "zip";
                if (order2.products.Length == 0)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("No bundles selected. Please choose at least one bundle for one Item to download");
                    return;
                }
                string json = JsonConvert.SerializeObject(order2, new JsonSerializerSettings()
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
                var byteArray = Encoding.ASCII.GetBytes(Module1.Current.API_KEY.API_KEY_Value + ":");
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
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                content.Headers.Remove("Content-Type");
                content.Headers.Add("Content-Type", "application/json");

                request.Content = content;
                try
                {
                    using (HttpResponseMessage httpResponse = _client.SendAsync(request).Result)
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            using (HttpContent content2 = httpResponse.Content)
                            {
                                var json2 = content2.ReadAsStringAsync().Result;
                                OrderResponse2 orderResponse2 = JsonConvert.DeserializeObject<OrderResponse2>(json2);
                                if (Resultscoll == null)
                                {
                                    Resultscoll = new ObservableCollection<string>();
                                }
                                if (orderResponse2.state == "failed")
                                {
                                    //_resultscoll
                                    Resultscoll.Add(order2.name + " failed. There was an error placing the order. Possible problems are: " + orderResponse2.error_hints.ToString());
                                    //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("There was an error placing the order. Possible problems are:" + Environment.NewLine + orderResponse2.error_hints.ToString(), "Order Failed", MessageBoxButton.OK);
                                }
                                else if (orderResponse2.state == "initializing" || orderResponse2.state == "queued")
                                {
                                    Resultscoll.Add(order2.name + " " + orderResponse2.state + ". The order has been placed and is being processed. Please see the Orders tab for details");
                                    //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("The order has been placed and is being processed." + Environment.NewLine + "Please see the Orders tab for details." + Environment.NewLine + "Order Name:" + orderResponse2.name.ToString(), "Order Success", System.Windows.MessageBoxButton.OK);
                                }
                                else if (orderResponse2.state == "partial")
                                {
                                    Resultscoll.Add(order2.name + " " + orderResponse2.state + ". Your order was only partially successful. Possible problems are: " + orderResponse2.error_hints.ToString());
                                    //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Your order was only partially successful. Possible problems are:" + Environment.NewLine + orderResponse2.error_hints.ToString(), "Partial Success", System.Windows.MessageBoxButton.OK);
                                }
                                Utils.AnalyticsReporter analyticsReporter = new Utils.AnalyticsReporter();
                                analyticsReporter.MakeReport("Order", new Segment.Model.Traits() { { "name", order2.name }, { "numItems", order2.products.Length } });
                            }
                            btnOrdertext = "Close";
                            _hasOrdered = true;
                        }
                        else
                        {
                            Console.WriteLine(httpResponse.StatusCode);
                            //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("There was an error placing the order. Possible problem was:" + Environment.NewLine + httpResponse.StatusCode + ": " + httpResponse.ReasonPhrase);
                            using (HttpContent content2 = httpResponse.Content)
                            {
                                var json2 = content2.ReadAsStringAsync().Result;
                                Resultscoll.Add("There was an error placing the order for: " + order2.name + ". Problem was: " + json2.ToString());
                                //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("There was an error placing the order for: " + order2.name + ". Problem was:" + Environment.NewLine + json2.ToString());
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
            }
        }


        #region Commands
        private RelayCommand _executeOrder;
        public RelayCommand ExecuteOrder
        {
            get
            {
                if (_executeOrder == null)
                {
                    _executeOrder = new RelayCommand(
                        (parameter) => ExecuteOrder1(parameter),
                        (parameter) => CanExecuteOrder1(parameter)
                        );
                }
                return _executeOrder;
            }
        }

        private bool CanExecuteOrder1(object parameter)
        {
            if (!_hasOrdered && !string.IsNullOrWhiteSpace(OrderName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ExecuteOrder1(object parameter)
        {
            if (btnOrdertext == "Close")
            {
                foreach (Window item in Application.Current.Windows)
                {
                    if (item.DataContext == this) item.Close();
                }
            }
            else
            {
                DoOrder();
            }
            
        }

        private RelayCommand _txtOrderNameChangedCommand;
        public RelayCommand txtOrderNameChangedCommand
        {
            get
            {
                if (_txtOrderNameChangedCommand == null)
                {
                    _txtOrderNameChangedCommand = new RelayCommand(
                        (parameter) => textChanged(parameter),
                        (parameter) => IsValidtext(parameter)
                    );
                }
                return _txtOrderNameChangedCommand;
            }
        }

        public void textChanged(object parameter)
        {
            KeyEventArgs e = (KeyEventArgs)parameter;
            if (e.Source is TextBox tb)
            {
                if (!string.IsNullOrWhiteSpace(tb.Text))
                {
                    OrderName = tb.Text;
                }
                   
            }
        }

        public bool IsValidtext(object parameter)
        {
            return true;
        }
    #endregion



    #region Filters - not used
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
        public tempBundles TempBundles { get; set; } = new tempBundles();

        public List<string> lstPSScene3Band { get; set; }
        public List<string> lstPSOrthoTile { get; set; }
        public List<string> lstREOrthoTile { get; set; }
        public List<string> lstREScene { get; set; }
        public List<string> lstSkySatScene { get; set; }
        public List<string> lstSkySatCollect { get; set; }
        public List<string> lstLandsat8L1G { get; set; }
        public List<string> lstSentinel2L1C { get; set; }

        #endregion

        #region expanderlogic
        string _CurrentExpanded;
        public string CurrentExpanded
        {
            get
            {
                return _CurrentExpanded;
            }
            set
            {
                if (_CurrentExpanded != value)
                {
                    _CurrentExpanded = value;
                    OnPropertyChanged("CurrentExpanded");
                }
            }
        }

        string _CurrentExpanded2;
        public string CurrentExpanded2
        {
            get
            {
                return _CurrentExpanded2;
            }
            set
            {
                if (_CurrentExpanded2 != value)
                {
                    _CurrentExpanded2 = value;
                    OnPropertyChanged("CurrentExpanded2");
                }
            }
        }

        string _CurrentExpanded3 = "1";
        public string CurrentExpanded3
        {
            get
            {
                return _CurrentExpanded3;
            }
            set
            {
                if (_CurrentExpanded3 != value)
                {
                    _CurrentExpanded3 = value;
                    OnPropertyChanged("CurrentExpanded3");
                }
            }
        }


        public ObservableCollection<ExpanderItem> Expanders { get; set; }

        public class ExpanderItem
        {
            public string Header { get; set; }
            public string ItemId { get; set; }
            public FrameworkElement Content { get; set; }
        }
        #endregion
    }
}