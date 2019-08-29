using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Globalization;

namespace Clean_Tool_and_MV
{
    internal class Data_DocPaneViewModel : DockPane, INotifyPropertyChanged
    {
        private Geometry _geometry = null;
        private const string _dockPaneID = "Clean_Tool_and_MV_Data_DocPane";
        private const string _menuID = "Clean_Tool_and_MV_Data_DocPane_Menu";
        private ObservableCollection<QuickSearchResult> _quickSearchResults = null;
        private int _CloudcoverLow = 0;
        private int _CloudcoverHigh = 100;
        private DateTime _DateFrom = DateTime.Now.AddYears(-1);
        private DateTime _DateTo = DateTime.Now;


        public DateTime DateFrom
        {
            get
            {
                return _DateFrom;
            }
            set
            {
                _DateFrom = value;
                OnPropertyChanged("DateFrom");
            }
        }
        public DateTime DateTo
        {
            get
            {
                return _DateTo;
            }
            set
            {
                _DateTo = value;
                OnPropertyChanged("DateTo");
            }
        }
        public int CloudcoverLow {
            get {
                return _CloudcoverLow;
            }
            set {
                _CloudcoverLow = value;
                OnPropertyChanged("CloudcoverLow");
            }
        }

        public int CloudcoverHigh
        {
            get
            {
                return _CloudcoverHigh;
            }
            set
            {
                _CloudcoverHigh = value;
                OnPropertyChanged("CloudcoverHigh");
            }
        }




        public bool CanExecuteSearch { get; set; } = true;
        private ICommand _searchcommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchcommand == null)
                    _searchcommand = new CommandHandler(() => DoSearch(), CanExecuteSearch);
                return _searchcommand;
            }
        }



        protected Data_DocPaneViewModel()
        {

        }

        /// <summary>
        /// Show the DockPane.
        /// </summary>
        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Activate();
        }

        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "My DockPane";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }
        public Geometry AOIGeometry
        {
            get
            {
                return _geometry;
            }
            set
            {
                _geometry = value;
            }
        }

        public ObservableCollection<QuickSearchResult> QuickSearchResults
        {
            get
            {
                
                if (_quickSearchResults == null)
                {
                    _quickSearchResults = new ObservableCollection<QuickSearchResult>();
                }
                return _quickSearchResults;
            }
            set
            {
                _quickSearchResults = value;
                OnPropertyChanged("QuickSearchResults");
            } 
        }

        #region Burger Button

        /// <summary>
        /// Tooltip shown when hovering over the burger button.
        /// </summary>
        public string BurgerButtonTooltip
        {
            get { return "Options"; }
        }

        /// <summary>
        /// Menu shown when burger button is clicked.
        /// </summary>
        public System.Windows.Controls.ContextMenu BurgerButtonMenu
        {
            get { return FrameworkApplication.CreateContextMenu(_menuID); }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Do a Quick search against the Planet Data api
        /// The results of the search update the QuickSearchResults Collection
        /// </summary>
        private void DoSearch()
        {
            Polygon poly = (Polygon)AOIGeometry;
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

            double[,] sd = new double[AllPts.Count, 2];
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


            //cloudcoverfiler
            RangeFilterConfig cloudconfig = new RangeFilterConfig
            {
                gte = _CloudcoverLow,
                lte = _CloudcoverHigh
            };
            
            Config cloudCoverFilter = new Config
            {
                type = "RangeFilter",
                field_name = "cloud_cover",
                config = cloudconfig

            };




            //DateFilter
            Config dateconfigconfig2 = new Config
            {
                gte = _DateFrom.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo),//"2019-05-19T16:51:19.926Z",
                lte = _DateTo.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo) //"2019-08-19T16:51:19.926Z"
            };

            Config dateconfigconfig = new Config
            {
                type = "DateRangeFilter",
                field_name = "acquired",
                config = dateconfigconfig2
            };
            Config dateconfig = new Config
            {
                type = "OrFilter",
                config = new[] { dateconfigconfig }
            };

            SearchFilter searchFilter = new SearchFilter();
            List<string> typoes = new List<string>();
            typoes.Add("PSScene4Band");
            typoes.Add("SkySatCollect");
            typoes.Add("REOrthoTile");


            List<Config> mainconfigs = new List<Config>
            {
                dateconfig,
                cloudCoverFilter,
                configGeom
            };
            searchFilter.item_types = typoes.ToArray();
            Filter topfilter = new Filter();
            topfilter.type = "AndFilter";
            searchFilter.filter = topfilter;
            Config mainConfig = new Config();
            searchFilter.filter.config = mainconfigs.ToArray();


            //string json = JsonConvert.SerializeObject(searchFilter);
            string json = JsonConvert.SerializeObject(searchFilter, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore

            });
            //string asas = "{\"filter\":{\"type\":\"AndFilter\",\"config\":[{\"type\":\"GeometryFilter\",\"field_name\":\"geometry\",\"config\":{\"type\":\"Polygon\",\"coordinates\":[[[-159.44149017333984,21.877787931279187],[-159.44998741149902,21.87679231243837],[-159.45372104644778,21.872769941600623],[-159.45217609405518,21.866835742000745],[-159.44372177124023,21.864207091531895],[-159.43561077117923,21.86930503623256],[-159.44149017333984,21.877787931279187]]]}},{\"type\":\"OrFilter\",\"config\":[{\"type\":\"DateRangeFilter\",\"field_name\":\"acquired\",\"config\":{\"gte\":\"2019-05-22T16:36:32.254Z\",\"lte\":\"2019-08-22T16:36:32.254Z\"}}]}]},\"item_types\":[\"PSScene4Band\",\"REOrthoTile\",\"SkySatCollect\"]}";
            //var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.somewhere.com/v2/cases");
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            HttpClient client = new HttpClient(handler)
            {

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
            var content = new StringContent(json, Encoding.UTF8, "application/json");
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
                    if (_quickSearchResults is null )
                    {
                        _quickSearchResults = new ObservableCollection<QuickSearchResult>();
                    }
                    _quickSearchResults.Add(quickSearchResult);
                    //Geometry geometry2 = GeometryEngine.Instance.ImportFromJSON(JSONImportFlags.jsonImportDefaults, JsonConvert.SerializeObject( quickSearchResult.features[5].geometry));
                }
            }
        }
    }



    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class Data_DocPane_ShowButton : Button
    {
        protected override void OnClick()
        {
            Data_DocPaneViewModel.Show();
        }
    }

    /// <summary>
    /// Button implementation for the button on the menu of the burger button.
    /// </summary>
    internal class Data_DocPane_MenuButton : Button
    {
        protected override void OnClick()
        {
        }
    }

    /// <summary>
    /// Command Event Handler class
    /// </summary>
    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
