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
using System.Runtime.CompilerServices;
using Planet.Model;
using System.Windows;
using ArcGIS.Desktop.Framework.DragDrop;
using Planet.ViewModel;
using Planet.Model.Item_assets;
using System.IO;
using System.Windows.Documents;
using System.IO.Compression;
using Planet.Utils;
using Segment.Model;

namespace Planet
{
    internal class Data_DocPaneViewModel : DockPane, INotifyPropertyChanged
    {
        private Geometry _geometry = null;
        private ObservableCollection<Asset> _selectedAssets = new ObservableCollection<Asset>();
        //private ObservableCollection<PastOrder> _pastOrders = new ObservableCollection<PastOrder>();
        
        private const string _dockPaneID = "Planet_Data_DocPane";
        private const string _menuID = "Planet_Data_DocPane_Menu";
        private ObservableCollection<QuickSearchResult> _quickSearchResults = null;
        private int _CloudcoverLow = 0;
        private int _CloudcoverHigh = 100;
        private int _AreaCoverLow = 0;
        private int _AreaCoverHigh = 100;
        private DateTime _DateFrom = DateTime.Now.AddMonths(-1);
        private DateTime _DateTo = DateTime.Now;
        public Data_DocPaneView View { get; set; }
        private bool _hasGeom = false;

        public  Data_DocPaneViewModel()
        {
            _selectedAssets.CollectionChanged += _selectedAssets_CollectionChanged;
            //GetPastOrders();
            
        }


        #region OrderLoadLogic
        private ObservableCollection<Order2> _pastOrders;
        public ObservableCollection<Order2> PastOrders
        {
            get {
                if (_pastOrders == null)
                {
                    _pastOrders = new ObservableCollection<Order2>();
                }
                return _pastOrders;
            }
            set
            {
                _pastOrders = value;
                OnPropertyChanged("PastOrders");
            }
        }
        private Task GetPastOrders()
        {
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
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "compute/ops/orders/v2");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "compute/ops/orders/v2");
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
            string json = "{\"name\":\"Pro1\",\"products\":[{\"item_ids\":[\"20190910_205244_101b\",\"20190908_195741_1048\"],\"item_type\":\"PSScene4Band\",\"product_bundle\":\"analytic\"}],\"include_metadata_assets\":true,\"order_type\":\"partial\",\"delivery\":{\"single_archive\":true,\"archive_type\":\"zip\"}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            //request.Content = content;
            try
            {
                using (HttpResponseMessage httpResponse = _client.SendAsync(request).Result)
                {
                    using (HttpContent content2 = httpResponse.Content)
                    {
                        var json2 = content2.ReadAsStringAsync().Result;
                        AllOrders2 allOrders2 = JsonConvert.DeserializeObject<AllOrders2>(json2);
                        List<Order2> quickSearchResult =  allOrders2.orders.ToList();
                        PastOrders = new ObservableCollection<Order2>(quickSearchResult);
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
                //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return Task.FromResult<bool>(true);

        }
        #endregion


        public bool HasGeom
        {
            get { return _hasGeom; }
            set
            {
                _hasGeom = value;
                OnPropertyChanged("HasGeom");
            }
        }
        private Visibility _paginatorVisibility = Visibility.Collapsed;
        public Visibility PaginatorVisibility
        {
            get { return _paginatorVisibility; }
            set
            {
                _paginatorVisibility = value;
                OnPropertyChanged("PaginatorVisibility");
            }
        }



        private Model.Page _currentPage;
        public Model.Page CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        private bool _hasPages = false;
        public bool HasPages
        {
            get { return _hasPages; }
            set
            {
                _hasPages = value;
                OnPropertyChanged("HasPages");
                
            }
        }



        private bool _treeEnabled = true;
        public bool TreeEnabled
        {
            get { return _treeEnabled; }
            set
            {
                _treeEnabled = value;
                OnPropertyChanged("TreeEnabled");
            }
        }

        private bool _hasNextPage = false;
        public bool HasNextPage
        {
            get { return _hasNextPage; }
            set
            {
                _hasNextPage = value;
                OnPropertyChanged("HasNextPage");
            }
        }
        private bool _isNotFirstPage = false;
        public bool IsNotFirstPage
        {
            get { return _isNotFirstPage; }
            set
            {
                _isNotFirstPage = value;
                OnPropertyChanged("IsNotFirstPage");
            }
        }

        private int _pageNumber = 0;
        public int PageNumber
        {
            get { return _pageNumber; }
            set
            {
                _pageNumber = value;
                OnPropertyChanged("PageNumber");
            }
        }

        private string _pageTotal = "many";
        public string PageTotal
        {
            get { return _pageTotal; }
            set
            {
                _pageTotal = value;
                OnPropertyChanged("PageTotal");
            }
        }

        private Visibility _showCircularAnimation = Visibility.Collapsed;
        public Visibility ShowCircularAnimation
        {
            get { return _showCircularAnimation; }
            set
            {
                _showCircularAnimation = value;
                OnPropertyChanged("ShowCircularAnimation");
            }
        }
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
        public int CloudcoverLow
        {
            get
            {
                return _CloudcoverLow;
            }
            set
            {
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
        
        public int AreaCoverLow
        {
            get
            {
                return _AreaCoverLow;
            }
            set
            {
                _AreaCoverLow = value;
                OnPropertyChanged("AreaCoverLow");
            }
        }
        public int AreaCoverHigh
        {
            get
            {
                return _AreaCoverHigh;
            }
            set
            {
                _AreaCoverHigh = value;
                OnPropertyChanged("AreaCoverHigh");
            }
        }

        public ObservableCollection<Asset> SelectAssets
        {
            get { return _selectedAssets; }
            set
            {
                _selectedAssets = value;
                //OnPropertyChanged("SelectAssets");
                //OnPropertyChanged("HasSelectedAssets");
            }
        }


        private void _selectedAssets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //_selectedAssets = value;
            OnPropertyChanged("SelectAssets");
            OnPropertyChanged("HasSelectedAssets");
        }
        private bool _hasSelectedAssets = false;
        public bool HasSelectedAssets
        {
            get
            {
                if (_selectedAssets.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            

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

        public bool CanViewOrders { get; set; } = true;
        private ICommand _viewordercommand;
        public ICommand ViewOrderCommand
        {
            get
            {
                if (_viewordercommand == null)
                    _viewordercommand = new CommandHandler3(() => GetPastOrders(), CanExecuteOrder);
                return _viewordercommand;
            }
        }
        private void DoOrder()
        {

            OrderWindow orderWindow = new OrderWindow();
            OrderWindowViewModel orderWindowViewModel = new OrderWindowViewModel();
            ObservableCollection<Asset> permittedassets = new ObservableCollection<Asset>();
            foreach (Asset ass in SelectAssets)
            {
                if (ass._permissions.Length > 0)
                {
                    permittedassets.Add(ass);
                }
            }
            orderWindowViewModel.SelectAssets = permittedassets;
            orderWindow.DataContext = orderWindowViewModel;
            orderWindow.Show();
        }

        public bool CanExecuteSearch { get; set; } = true;
        private ICommand _searchcommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchcommand == null)
                    _searchcommand = new CommandHandler3(() => DoSearch(), CanExecuteSearch);
                return _searchcommand;
            }
        }

        public bool CanExecuteGoToNext { get; set; } = true;
        private ICommand _goToPrevPage;
        public ICommand GoToPrevPage
        {
            get
            {
                if (_goToPrevPage == null)
                    _goToPrevPage = new ArcGIS.Desktop.Framework.RelayCommand(() => doGoToPrevPage());
                return _goToPrevPage;
            }
        }

        public bool CanExecuteGoToPrev { get; set; } = true;
        private void doGoToPrevPage()
        {
            ShowCircularAnimation = Visibility.Visible;
            TreeEnabled = false;
            HasNextPage = false;
            IsNotFirstPage = false;
            PageNumber -= 1;
            Items = Pages[PageNumber - 1].Items;
            CurrentPage = Pages[PageNumber - 1];
            IsNotFirstPage = PageNumber == 1 ? false : true;
            HasNextPage = true;
            ShowCircularAnimation = Visibility.Hidden;
            TreeEnabled = true;
        }

        private ICommand _goToNextPage;
        public ICommand GoToNextPage
        {
            get
            {
                if (_goToNextPage == null)
                    _goToNextPage = new ArcGIS.Desktop.Framework.RelayCommand(() => doGoToNextPage());
                return _goToNextPage;
            }
        }

        private async void doGoToNextPage()
        {
            ShowCircularAnimation = Visibility.Visible;
            TreeEnabled = false;
            HasNextPage = false;
            IsNotFirstPage = false;
            Model.Page nextPage = null;
            if (Pages.Count >= PageNumber + 1)
            {
                nextPage = Pages[PageNumber];
            }
            else
            {
                nextPage = await Model.Page.GetNextPage(CurrentPage);
                Pages.Add(nextPage);
            }
            CurrentPage = nextPage;
            Items = nextPage.Items;
            PageNumber += 1;
            IsNotFirstPage = true;
            HasNextPage = nextPage.QuickSearchResult._links._next != null;
            PageTotal = HasNextPage ? "many" : Pages.Count.ToString();
            ShowCircularAnimation = Visibility.Hidden;
            TreeEnabled = true;
        }

        private List<Model.Page> _Pages;
        public List<Model.Page> Pages
        {
            get
            {
                if (_Pages == null)
                {
                    _Pages = new List<Model.Page>();
                }
                return _Pages;
            }
            set
            {
                _Pages = value;
            }
        }

        private ObservableCollection<Model.AcquiredDateGroup> _items;
        public ObservableCollection<Model.AcquiredDateGroup> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ObservableCollection<Model.AcquiredDateGroup>();
                }
                return _items;
            }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        public void AddFootprint(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.StackPanel stackPanel = sender as System.Windows.Controls.StackPanel;
                string id = stackPanel.Tag.ToString();
                Model.Asset asset = Model.Asset.FindAsset(Items, id);
                asset.drawFootprint();
            }
            catch (Exception ex)
            {
                //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public void RemoveFootprint(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.StackPanel stackPanel = sender as System.Windows.Controls.StackPanel;
                string id = stackPanel.Tag.ToString();
                Model.Asset asset = Model.Asset.FindAsset(Items, id);
                asset.disposeFootprint();
            }
            catch (Exception ex)
            {
                //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        private ObservableCollection<OrderStatusItem> _CurrentDownloads;
        public ObservableCollection<OrderStatusItem> CurrentDownloads
        {
            get
            {
                if (_CurrentDownloads == null)
                {
                    _CurrentDownloads = new ObservableCollection<OrderStatusItem>();
                }
                return _CurrentDownloads;
            }
            set
            {
                _CurrentDownloads = value;
                OnPropertyChanged("CurrentDownloads");
            }
        }
        private Visibility _DownloadGridVisibility = Visibility.Collapsed;
        public Visibility DownloadGridVisibility
        {
            get { return _DownloadGridVisibility; }
            set
            {
                _DownloadGridVisibility = value;
                OnPropertyChanged("DownloadGridVisibility");
            }
        }
        public bool CanExecuteDownload { get; set; } = true;
        private ICommand _OpenDownloadHyperlink;
        public ICommand OpenDownloadHyperlink
        {
            get
            {
                if (_OpenDownloadHyperlink == null)
                    _OpenDownloadHyperlink = new DownloadCommandHandler(param => DoDownloadHyperlink(param), CanExecuteDownload);
                return _OpenDownloadHyperlink;
            }
        }

        private async void DoDownloadHyperlink(object order)
        {
            Order2 order2 = order as Order2;

            FolderSelector folderSelector = new FolderSelector();
            folderSelector.lbxGrids.ItemsSource = null;
            folderSelector.ShowNewFolderButton = false;
            folderSelector.ShowActivated = true;
            folderSelector.SizeToContent = SizeToContent.Width;
            object da = folderSelector.txtGrids.DataContext;

            foreach (System.Windows.Controls.TreeViewItem item in folderSelector.tvFolders.Items)
            {
                item.Foreground = System.Windows.Media.Brushes.White;
            }

            if (da is Data.BaseItem ba)
            {
                ba.QuadCount = "";
            }

            OpenItemDialog downloadDialog = new OpenItemDialog();
            downloadDialog.Title = "Download Location";
            downloadDialog.MultiSelect = false;
            downloadDialog.Filter = ItemFilters.folders;
            downloadDialog.InitialLocation = Project.Current.URI;

            bool? ok = downloadDialog.ShowDialog();
            if (ok == true)
            {
                string savelocation = downloadDialog.Items.First().Path;

                OrderStatusItem download = new OrderStatusItem();
                download.name = order2.name;
                download.status = "Downloading archive";
                download.path = savelocation;
                download.id = order2.id;
                CurrentDownloads.Add(download);
                DownloadGridVisibility = Visibility.Visible;

                bool complete = await LoadImage(order2._links._self, savelocation);
                if (complete)
                {
                    
                    var notification = new NotificationItem("Planet_Download_Complete_Notification_" + order2.id, false,
                        "The download of Order: " + order2.name + " is complete" + Environment.NewLine + "The file is saved to: " + savelocation, NotificationType.Information);
                    NotificationManager.AddNotification(notification);
                    FrameworkApplication.AddNotification(new Notification()
                    {
                        Title = "Downloading Finished",
                        Message = String.Format("The download of Order: {0} is complete", order2.name),
                        ImageUrl = @"pack://application:,,,/Planet;component/Images/Planet_logo-dark.png"

                    });

                    string fileName = String.Format("{0}.{1}", order2.name, order2.delivery.archive_type);
                    string zipPath = Path.Combine(download.path, fileName);
                    string extractPath = Path.Combine(download.path, order2.name);
                    download.status = "Extracting archive";
                    bool extracted = await ExtractDownload(zipPath, extractPath);
                    if (extracted)
                    {
                        download.status = download.status = "Complete";
                    } else
                    {
                        download.status = "Error extracting";
                    }

                }
                else
                {
                    download.status = "Error downloading";
                }
            }
        }

        public async Task<bool> ExtractDownload(string archivePath, string extractPath)
        {
            try
            {
                
                System.IO.Compression.ZipFile.ExtractToDirectory(archivePath, extractPath);
                return true;
            } catch (Exception e)
            {
                return false;
            }
        }

        public async static Task<bool> LoadImage(string uri, string destination)
        {
            OrderDownload orderDownload = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var byteArray = Encoding.ASCII.GetBytes(Module1.Current.API_KEY.API_KEY_Value + ":");// "1fe575980e78467f9c28b552294ea410:");
                    client.DefaultRequestHeaders.Host = "api.planet.com";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.16.3");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    
                    try
                    {
                        using (HttpResponseMessage httpResponse = client.GetAsync(uri).Result)
                        {
                            using (HttpContent content2 = httpResponse.Content)
                            {
                                var json2 = content2.ReadAsStringAsync().Result;
                                orderDownload = JsonConvert.DeserializeObject<OrderDownload>(json2);

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
                        //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
                    }

                    if (orderDownload != null)
                    {
                        if (orderDownload.state == "success")
                        {
                            using (var response = await client.GetAsync(orderDownload._links.results[1].location, HttpCompletionOption.ResponseHeadersRead))
                            {
                                if (!response.IsSuccessStatusCode)
                                {
                                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("There was a problem downloading your data" + Environment.NewLine + response.ReasonPhrase, "Download Error", MessageBoxButton.OK, MessageBoxImage.Information);
                                    return false;
                                }
                                using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                                {
                                    string fileToWriteTo = destination + "\\" + orderDownload.name + "." + orderDownload.delivery.archive_type; //Path.GetTempFileName();
                                    if (File.Exists(fileToWriteTo))
                                    {
                                        File.Delete(fileToWriteTo);
                                    }

                                    using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                                    {
                                        await streamToReadFrom.CopyToAsync(streamToWriteTo);
                                        //MessageBox.Show("Download Completed Succesfully","Dowload Complete",MessageBoxButton.OK,MessageBoxImage.Information);
                                    }

                                }
                            }
                        }
                        else
                        {
                            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Order is not ready");
                        }
                    }


                }
                return true;
            }
            catch (Exception ex)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Error downloading \n" + ex.Message, "Download Problem", MessageBoxButton.OK, MessageBoxImage.Warning);
                Console.WriteLine("Failed to Download the quad: {0}", ex.Message);

            }

            return false;
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
                if (value != null)
                {
                    _geometry = value;
                    if (_geometry.IsEmpty)
                    {
                        HasGeom = false;
                    }
                    else
                    {
                        HasGeom = true;
                    }
                }
                else
                {
                    HasGeom = false;
                }

                OnPropertyChanged("AOIGeometry");
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
        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Do a Quick search against the Planet Data api
        /// The results of the search update the QuickSearchResults Collection
        /// </summary>
        private async void DoSearch()
        {
            try
            {
                ShowCircularAnimation = Visibility.Visible;
                SelectAssets.Clear();
                TreeEnabled = false;
                Polygon poly = (Polygon)AOIGeometry;
                Polygon polyReporject = (Polygon)GeometryEngine.Instance.Project(poly, SpatialReferences.WGS84);
                string geojson = GeometryEngine.Instance.ExportToJSON(JSONExportFlags.jsonExportSkipCRS, polyReporject);
                IReadOnlyList<Coordinate2D> coordinates = poly.Copy2DCoordinatesToList();
                IReadOnlyList<Coordinate2D> coordinates2 = polyReporject.Copy2DCoordinatesToList();
                string ejson = poly.ToJson(true);
                ToGeoCoordinateParameter ddParam = new ToGeoCoordinateParameter(GeoCoordinateType.DD);
                List<string> geocoords = new List<string>();
                List<Tuple<double, double>> AllPts = new List<Tuple<double, double>>();
                await QueuedTask.Run(() =>
                {
                    Polygon wgsPoly = PolygonBuilder.CreatePolygon(poly, SpatialReferences.WGS84);

                    ejson = wgsPoly.ToJson();
                });


                double x;
                double y;
                foreach (Coordinate2D item in coordinates2)
                {
                    //MapPoint mapPoint = MapPointBuilder.CreateMapPoint(item, MapView.Active.Extent.SpatialReference);
                    //List<Tuple<string, string>> pts = new List<Tuple<string, string>>();
                    //string dd1 = mapPoint.ToGeoCoordinateString(ddParam).Split(' ')[0];
                    //pts.Add(new Tuple<string, string>(mapPoint.ToGeoCoordinateString(ddParam).Split(' ')[1], mapPoint.ToGeoCoordinateString(ddParam).Split(' ')[0]));
                    //if (pts[0].Item1.Contains("W"))
                    //{
                    //    x = double.Parse("-" + pts[0].Item1.Substring(0, pts[0].Item1.Length - 1));
                    //    y = double.Parse(pts[0].Item2.Substring(0, pts[0].Item2.Length - 1));
                    //    //AllPts.Add(new Tuple<int, int>(int.Parse("-" + pts[0].Item1.Substring(0, pts[0].Item1.Length - 1)), int.Parse(pts[0].Item2.Substring(0, pts[0].Item2.Length -1))));
                    //}
                    //else if (pts[1].Item2.Contains("S"))
                    //{
                    //    x = double.Parse(pts[0].Item1.Substring(0, pts[0].Item1.Length - 1));
                    //    y = double.Parse("-" + pts[0].Item2.Substring(0, pts[1].Item2.Length - 1));
                    //    //AllPts.Add(new Tuple<int, int>(int.Parse(pts[0].Item1.Substring(0, pts[0].Item1.Length - 1)), int.Parse("-" + pts[0].Item2.Substring(0, pts[1].Item2.Length - 1))));
                    //}
                    //else
                    //{
                    //    x = double.Parse(pts[0].Item1.Substring(0, pts[0].Item1.Length - 1));
                    //    y = double.Parse(pts[0].Item2.Substring(0, pts[0].Item2.Length - 1));
                    //    //AllPts.Add(new Tuple<int, int>(int.Parse(pts[0].Item1.Substring(0, pts[0].Item1.Length - 1)), int.Parse(pts[0].Item2.Substring(0, pts[1].Item2.Length - 1))));
                    //}
                    //AllPts.Add(new Tuple<double, double>(x, y));
                    AllPts.Add(new Tuple<double, double>(item.X, item.Y));
                    //geocoords.Add(mapPoint.ToGeoCoordinateString(ddParam));
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

                //areacovered filter
                RangeFilterConfig areaconfig = new RangeFilterConfig()
                {
                    gte = _AreaCoverLow,
                    lte = _AreaCoverHigh
                };
                Config areacoverfilter = new Config()
                {
                    type = "RangeFilter",
                    field_name = "visible_percent"
                };

                //cloudcoverfiler
                RangeFilterConfig cloudconfig = new RangeFilterConfig
                {
                    gte = Convert.ToDouble(_CloudcoverLow) / 100,
                    lte = Convert.ToDouble(_CloudcoverHigh) / 100
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
                List<string> types = new List<string>();
                //typoes.Add("PSScene4Band");
                //typoes.Add("SkySatCollect");
                //typoes.Add("REOrthoTile");
                foreach (var prop in this.GetType().GetProperties())
                {
                    if (prop.PropertyType.Name == "Boolean")
                    {
                        if (((bool)prop.GetValue(this, null)) && (prop.Name.StartsWith("Product")))
                        {
                            types.Add(prop.Name.Substring(7));
                        }
                        //Console.WriteLine(prop.MemberType.ToString());
                    }
                }

                List<Config> mainconfigs = new List<Config>
            {
                dateconfig,
                cloudCoverFilter,
                configGeom
            };
                searchFilter.item_types = types.ToArray();
                Filter topfilter = new Filter();
                topfilter.type = "AndFilter";
                searchFilter.filter = topfilter;
                //Config mainConfig = new Config();
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
                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://api.planet.com");
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "data/v1/quick-search?_sort=acquired desc&_page_size=250");
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.CacheControl = new CacheControlHeaderValue();

                    request.Headers.CacheControl.NoCache = true;
                    request.Headers.Host = "api.planet.com";
                    request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    //request.Headers.Remove("Content-Type");
                    //request.Headers.Add("Content-Type", "application/json");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    request.Content = content;
                    var byteArray = Encoding.ASCII.GetBytes(Module1.Current.API_KEY.API_KEY_Value + ":hgvhgv");
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
                    try
                    {
                        using (HttpResponseMessage httpResponse = client.SendAsync(request).Result)
                        {
                            if (httpResponse.IsSuccessStatusCode)
                            {
                                using (HttpContent content2 = httpResponse.Content)
                                {
                                    var json2 = content2.ReadAsStringAsync().Result;
                                    QuickSearchResult quickSearchResult = JsonConvert.DeserializeObject<QuickSearchResult>(json2);
                                    //if (_quickSearchResults is null )
                                    //{
                                    _quickSearchResults = new ObservableCollection<QuickSearchResult>();
                                    Items = new ObservableCollection<AcquiredDateGroup>();
                                    Pages = new List<Model.Page>();
                                    //}
                                    _quickSearchResults.Add(quickSearchResult);
                                    Model.Page page = new Model.Page
                                    {
                                        QuickSearchResult = quickSearchResult
                                    };
                                    List<AcquiredDateGroup> groupedItems = Model.Page.ProcessQuickSearchResults(quickSearchResult);
                                    page.Items = new ObservableCollection<Model.AcquiredDateGroup>(groupedItems);
                                    Pages.Add(page);
                                    CurrentPage = page;
                                    Items = new ObservableCollection<Model.AcquiredDateGroup>(groupedItems);
                                    //ProcessQuickSearchResults(quickSearchResult, page);
                                    HasPages = true;
                                    HasNextPage = quickSearchResult._links._next != null;
                                    IsNotFirstPage = false;
                                    PageNumber = 1;
                                    PageTotal = HasNextPage ? "many" : "1";
                                    PaginatorVisibility = Visibility.Visible;
                                    AnalyticsReporter analyticsReporter = new AnalyticsReporter();
                                    analyticsReporter.MakeReport("Search executed", new Traits() { {"query", json } });
                                    //Pages.AddRange(await Model.Page.GetAllPages(quickSearchResult));
                                }
                            }
                            else
                            {
                                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("There was a problem with the Search. PLease try again." + Environment.NewLine + httpResponse.StatusCode + Environment.NewLine + httpResponse.ReasonPhrase);
                            }

                        }


                    }
                    catch (Exception e)
                    {
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);

                    }
                }
                //HttpClient client = new HttpClient(handler)
                //{

                //    BaseAddress = new Uri("https://api.planet.com")
                //};
               

                ShowCircularAnimation = Visibility.Hidden;
                TreeEnabled = true;
            }
            catch (Exception e)
            {

                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace,"Error with the search query");
            }
            
        }


        #region ProductBooleans set get
        private bool _PSScene3Band = false;
        public bool ProductPSScene3Band
        {
            get { return _PSScene3Band; }
            set
            {
                if (_PSScene3Band == value) return;
                _PSScene3Band = value;
                NotifyPropertyChanged(() => ProductPSScene3Band);
            }
        }
        private bool _PSScene4Band = true;
        public bool ProductPSScene4Band
        {
            get { return _PSScene4Band; }
            set
            {
                if (_PSScene4Band == value) return;
                _PSScene4Band = value;
                NotifyPropertyChanged(() => ProductPSScene4Band);
            }
        }

        private bool _PSOrthoTile = false;
        public bool ProductPSOrthoTile
        {
            get { return _PSOrthoTile; }
            set
            {
                if (_PSOrthoTile == value) return;
                _PSOrthoTile = value;
                NotifyPropertyChanged(() => ProductPSOrthoTile);
            }
        }

        private bool _REOrthoTile = true;
        public bool ProductREOrthoTile
        {
            get { return _REOrthoTile; }
            set
            {
                if (_REOrthoTile == value) return;
                _REOrthoTile = value;
                NotifyPropertyChanged(() => ProductREOrthoTile);
            }
        }

        private bool _REScene = false;
        public bool ProductREScene
        {
            get { return _REScene; }
            set
            {
                if (_REScene == value) return;
                _REScene = value;
                NotifyPropertyChanged(() => ProductREScene);
            }
        }
        private bool _SkySatScene = false;
        public bool ProductSkySatScene
        {
            get { return _SkySatScene; }
            set
            {
                if (_SkySatScene == value) return;
                _SkySatScene = value;
                NotifyPropertyChanged(() => ProductSkySatScene);
            }
        }
        private bool _SkySatCollect = true;
        public bool ProductSkySatCollect
        {
            get { return _SkySatCollect; }
            set
            {
                if (_SkySatCollect == value) return;
                _SkySatCollect = value;
                NotifyPropertyChanged(() => ProductSkySatCollect);
            }
        }
        private bool _Landsat8L1G = false;
        public bool ProductLandsat8L1G
        {
            get { return _Landsat8L1G; }
            set
            {
                if (_Landsat8L1G == value) return;
                _Landsat8L1G = value;
                NotifyPropertyChanged(() => ProductLandsat8L1G);
            }
        }
        private bool _Sentinel2L1C = false;
        public bool ProductSentinel2L1C
        {
            get { return _Sentinel2L1C; }
            set
            {
                if (_Sentinel2L1C == value) return;
                _Sentinel2L1C = value;
                NotifyPropertyChanged(() => ProductSentinel2L1C);
            }
        }
        #endregion

        #region treeviewselectionchanged

        private static object _selectedItem = null;
        // This is public get-only here but you could implement a public setter which
        // also selects the item.
        // Also this should be moved to an instance property on a VM for the whole tree, 
        // otherwise there will be conflicts for more than one tree.
        public static object SelectedItem
        {
            get { return _selectedItem; }
            private set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnSelectedItemChanged();
                }
            }
        }

        private static void OnSelectedItemChanged()
        {
            Console.WriteLine("ItemChanged");
            // Raise event / do other things
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                    if (_isSelected)
                    {
                        SelectedItem = this;
                    }
                }
            }
        }
        #endregion
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
    /// Search Command Event Handler class
    /// </summary>
    public class CommandHandler3 : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler3(Action action, bool canExecute)
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

    public class DownloadCommandHandler : ICommand
    {
        private Action<object> _action;
        private bool _canExecute;
        public DownloadCommandHandler(Action<object> action, bool canExecute)
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
            _action(parameter);
        }
    }



    //public class TreeViewHelper
    //{
    //    private static Dictionary<System.Windows.DependencyObject, TreeViewSelectedItemBehavior> behaviors = new Dictionary<System.Windows.DependencyObject, TreeViewSelectedItemBehavior>();

    //    public static object GetSelectedItem(System.Windows.DependencyObject obj)
    //    {
    //        return (object)obj.GetValue(SelectedItemProperty);
    //    }

    //    public static void SetSelectedItem(System.Windows.DependencyObject obj, object value)
    //    {
    //        obj.SetValue(SelectedItemProperty, value);
    //    }

    //    // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
    //    public static readonly System.Windows.DependencyProperty SelectedItemProperty =
    //        System.Windows.DependencyProperty.RegisterAttached("SelectedItem", typeof(object), typeof(TreeViewHelper), new System.Windows.UIPropertyMetadata(null, SelectedItemChanged));

    //    private static void SelectedItemChanged(System.Windows.DependencyObject obj, System.Windows.DependencyPropertyChangedEventArgs e)
    //    {
    //        if (!(obj is System.Windows.Controls.TreeView))
    //            return;

    //        if (!behaviors.ContainsKey(obj))
    //            behaviors.Add(obj, new TreeViewSelectedItemBehavior(obj as System.Windows.Controls.TreeView));

    //        TreeViewSelectedItemBehavior view = behaviors[obj];
    //        view.ChangeSelectedItem(e.NewValue);
    //    }

    //    private class TreeViewSelectedItemBehavior
    //    {
    //        System.Windows.Controls.TreeView view;
    //        public TreeViewSelectedItemBehavior(System.Windows.Controls.TreeView view)
    //        {
    //            this.view = view;
    //            view.SelectedItemChanged += (sender, e) => SetSelectedItem(view, e.NewValue);
    //        }

    //        internal void ChangeSelectedItem(object p)
    //        {
    //            System.Windows.Controls.TreeViewItem item = (System.Windows.Controls.TreeViewItem)view.ItemContainerGenerator.ContainerFromItem(p);
    //            item.IsSelected = true;
    //        }
    //    }
    //}

}
