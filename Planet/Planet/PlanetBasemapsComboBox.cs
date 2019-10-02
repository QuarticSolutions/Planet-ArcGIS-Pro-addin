using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
using Newtonsoft.Json;

namespace Planet
{
    /// <summary>
    /// Represents the ComboBox
    /// </summary>
    internal class PlanetBasemapsComboBox : ComboBox 
    {

        private bool _isInitialized;
        private int PageSize = 10;
        private int PageNumber = 0;
        private int TotalPages = 0;
        private List<Mosaic> _Items;
        public List<Mosaic> Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new List<Mosaic>();
                }
                return _Items;
            }
            set
            {
                _Items = value;
            }
        }

        private List<Mosaic> _ItemsClean;
        public List<Mosaic> ItemsClean
        {
            get
            {
                if (_ItemsClean == null)
                {
                    _ItemsClean = new List<Mosaic>();
                }
                return _ItemsClean;
            }
            set
            {
                _ItemsClean = value;
            }
        }

        /// <summary>
        /// Combo Box constructor
        /// </summary>
        public PlanetBasemapsComboBox() 
        {
            APIKeyChangedEvent.Subscribe((args) =>
            {
                Clear();
                _isInitialized = false;
                UpdateCombo();
            });

            PlanetGalleryFilterEvent.Subscribe((args) =>
            {
                FilterItems(args.FilterText);
            });

            PlanetGalleryChangedEvent.Subscribe((args) =>
            {
                AppendItems();
            });

            UpdateCombo();
        }

        /// <summary>
        /// Updates the combo box with all the items.
        /// </summary>
 
        private async void UpdateCombo()
        {
            // TODO – customize this method to populate the combobox with your desired items  
            if (_isInitialized)
            {
                //SelectedItem = ItemCollection.FirstOrDefault(); //set the default item in the comboBox
            }


            if (!_isInitialized)
            {
                //Clear();

                ////Add 6 items to the combobox
                //for (int i = 0; i < 6; i++)
                //{
                //    string name = string.Format("Item {0}", i);
                //    Add(new ComboBoxItem(name));
                //}
                //_isInitialized = true;
                if (Module1.Current.API_KEY == null)
                {
                    FrameworkApplication.State.Deactivate("planet_state_connection");
                    return;
                }
                if (Module1.Current.API_KEY.API_KEY_Value == null || Module1.Current.API_KEY.API_KEY_Value == "")
                {
                    FrameworkApplication.State.Deactivate("planet_state_connection");
                    return;
                }
                else
                {
                    try
                    {
                        Items = await QueuedTask.Run(() => GetMosicsAsync2(ResultCallBack));
                        ItemsClean = Items;
                        PageNumber = 0;
                        TotalPages = 0;
                        if (Items.Count > 0)
                        {
                            Clear();
                            TotalPages = (Items.Count + PageSize - 1) / PageSize;
                            var currentItems = Items.Take(PageSize);
                            foreach (var dataItem in currentItems)
                            {
                                Add(new ComboBoxItem(dataItem.name));
                            }
                            PageNumber = 1;
                            _isInitialized = true;
                            FrameworkApplication.State.Activate("planet_state_connection");
                        }
                    }
                    catch (AggregateException ae)
                    {
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ae.Message);
                        foreach (var e in ae.Flatten().InnerExceptions)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message);

                    }
                }
            }
            Enabled = true; //enables the ComboBox
            //SelectedItem = ItemCollection.FirstOrDefault(); //set the default item in the comboBox

          }
       
        /// <summary>
        /// The on comboBox selection change event. 
        /// </summary>
        /// <param name="item">The newly selected combo box item</param>
        protected override void OnSelectionChange(ComboBoxItem item) 
        {

            if (item == null)
                return;

            if (string.IsNullOrEmpty(item.Text))
                return;

            string name = item.Text;
            List<Mosaic> filteredItems = ItemsClean.Where(i => i.name == name).ToList();
            Mosaic mosaic = filteredItems.First();
            OpenWebMapAsync(mosaic);

            // TODO  Code behavior when selection changes.    
        }

        /// <summary>
        /// Create a map raster layer using the URL of the gallery item
        /// </summary>
        /// <param name="item"></param>
        private async void OpenWebMapAsync(object item)
        {
            if (item is Mosaic mosaic)
            {
                if (MapView.Active == null)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("A map must be added the the project and be active");
                    //FrameworkApplication.State.Deactivate("planet_state_connection");
                    return;
                }
                Project project = Project.Current;
                var serverConnection = new CIMProjectServerConnection { URL = mosaic._links._self.Substring(0, mosaic._links._self.IndexOf("?")) + "/wmts?REQUEST=GetCapabilities&api_key=" + Module1.Current.API_KEY.API_KEY_Value };
                var connection = new CIMWMTSServiceConnection { ServerConnection = serverConnection };
                await QueuedTask.Run(() =>
                {
                    var extent = MapView.Active.Extent;
                    string layerName = "Planet Basemaps";
                    GroupLayer groupLayer = MapView.Active.Map.FindLayers(layerName).FirstOrDefault() as GroupLayer;
                    if (groupLayer == null)
                    {
                        int index = MapView.Active.Map.Layers.Count;
                        groupLayer = LayerFactory.Instance.CreateGroupLayer(MapView.Active.Map, index, layerName);
                    }
                    BasicRasterLayer layer2 = LayerFactory.Instance.CreateRasterLayer(connection, groupLayer, 0, mosaic.name);
                    MapView.Active.ZoomTo(extent, TimeSpan.Zero);
                });

            }

            //Hardcoding trial to false so warning is never shown per Annies request 20190703
            Module1.Current.IsTrial = false;
            if (Module1.Current.IsTrial)
            {
                TrialWarning _trialwarning = new TrialWarning();
                _trialwarning.Owner = FrameworkApplication.Current.MainWindow;
                _trialwarning.Closed += (o, e) => { _trialwarning = null; };
                _trialwarning.ShowDialog();
            }
        }

        /// <summary>
        /// query the Planet api to get a list of mosics that the user is allowed to see.
        /// Use pagination as the server returns 50 by default and there my be more.
        /// Has a call back funtion to access the results between pages but not currently used
        /// Might use in the future to asyn load the thumbnails
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        private static async Task<List<Mosaic>> GetMosicsAsync2(Action<Mosaics> callBack = null)
        {
            var lstMosaics = new List<Mosaic>();

            HttpClient httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri("https://api.planet.com/basemaps/v1/mosaics?api_key=" + Module1.Current.API_KEY.API_KEY_Value);
            var nextUrl = "https://api.planet.com/basemaps/v1/mosaics?api_key=" + Module1.Current.API_KEY.API_KEY_Value;
            try
            {
                do
                {
                    await httpClient.GetAsync(nextUrl).ContinueWith(async (getmaoicstask2) =>
                    {
                        var response = await getmaoicstask2;
                        //response.EnsureSuccessStatusCode();
                        if (response.IsSuccessStatusCode)
                        {
                            try
                            {
                                string responseBody = await response.Content.ReadAsStringAsync();
                                var result = JsonConvert.DeserializeObject<Mosaics>(responseBody);
                                if (result != null)
                                {
                                    foreach (Mosaic item in result.mosaics)
                                    {
                                        lstMosaics.Add(item);
                                    }

                                    // Run the callback method, passing the current page of data from the API.
                                    //callBack?.Invoke(result);

                                    // Get the URL for the next page
                                    nextUrl = result._links._next ?? string.Empty;
                                }
                            }
                            catch (Exception sd)
                            {
                                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Error getting the basemaps" + Environment.NewLine + sd.Message, "Basemap error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                        }
                        else
                        {
                            // End loop if we get an error response.
                            nextUrl = string.Empty;
                        }
                    });
                }
                while (!string.IsNullOrEmpty(nextUrl));
                return lstMosaics;
            }
            catch (Exception)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Error Getting Basemaps. Please check api key and try again");
                return lstMosaics;
            }

        }

        private void FilterItems(string text)
        {
            List<Mosaic> filteredItems = new List<Mosaic>();
            if (text.Trim().Length > 0)
            {
                filteredItems = ItemsClean.Where(i => i.name.Contains(text)).ToList();
            }
            else
            {
                filteredItems = ItemsClean;
            }
            PageNumber = 0;
            TotalPages = 0;
            if (filteredItems.Count > 0)
            {
                Items = filteredItems;
                Clear();
                TotalPages = (Items.Count + PageSize - 1) / PageSize;
                var currentItems = Items.Take(PageSize);
                foreach (var dataItem in currentItems)
                {
                    Add(new ComboBoxItem(dataItem.name));
                }
                PageNumber = 1;
                //SelectedItem = ItemCollection.FirstOrDefault();
            }

        }

        private void AppendItems()
        {
            if (PageNumber != TotalPages)
            {
                PageNumber = PageNumber + 1;
                var currentItems = Items.Skip((PageNumber - 1) * PageSize).Take(PageSize);
                //Clear();
                foreach (var dataItem in currentItems)
                {
                    Add(new ComboBoxItem(dataItem.name));
                }
            }
        }

        /// <summary>
        /// call back to access results of a page while inside the pagination loop
        /// not currently used
        /// </summary>
        /// <param name="getmaoicstask2"></param>
        private static void ResultCallBack(Mosaics getmaoicstask2)
        {
            if (getmaoicstask2 != null)
            {
                foreach (Mosaic item in getmaoicstask2.mosaics)
                {
                    string x = "0";
                    string y = "0";
                    string z = "0";
                    //PointF point = tilePointConvert.WorldToTilePos(item.bbox[0], item.bbox[1], 15);
                    //PointF point2 = tilePointConvert.TileToWorldPos(item.bbox[0], item.bbox[1], 15);
                    //item.Thumbnail = string.Format(item._links.tiles,"0","0","0");
                    //item.Thumbnail = item._links.tiles.Replace("{x}", x).Replace("{y}", y).Replace("{z}", z);
                    //lstMosaics.Add(item);
                }
            }
        }
    }
}
