using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System.Net.Http;
using Newtonsoft.Json;

using System.Drawing;
using ArcGIS.Desktop.Mapping.Events;

namespace Planet
{
    internal class PlanetGalleryInline : Gallery 
    {
        private bool _isInitialized;
        

        /// <summary>
        /// Initial load but also
        /// listen for changes to the api key value and reload the gallery if detected
        /// </summary>
        public PlanetGalleryInline()
        {
            APIKeyChangedEvent.Subscribe((args) =>
            {
                this.Clear();
                //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show($"Name has changed:\r\nOld: {args.OldName}\r\nNew: {args.NewName}", "NameChangedEvent");
                _isInitialized = false;
                Initialize();
            });
            Initialize();
        }

        /// <summary>
        /// Load the gallery items into the Gallery listbox
        /// Items are based on the PlanetGalleryInlineTemplate.xaml and Mosics model
        /// </summary>
        private async void Initialize()
        {
            
            if (_isInitialized)
                return;
            
            
            if (Module1.Current.API_KEY.API_KEY_Value == null || Module1.Current.API_KEY.API_KEY_Value == "" )
            {
                FrameworkApplication.State.Deactivate("planet_state_connection");
                return;
            }
            else
            {
                try
                {
                    var lstWebmapItems = await GetMosicsAsync2(ResultCallBack);
                    if (lstWebmapItems.Count > 0)
                    {
                        this.Clear();
                        foreach (var dataItem in lstWebmapItems)
                            Add(dataItem);
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
                catch  (Exception ex)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message);

                }


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
        private static async Task<List<Mosaic>>GetMosicsAsync2(Action<Mosaics> callBack = null)
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
                            string responseBody = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<Mosaics>(responseBody);
                            if (result != null)
                            {
                                string x = "0";
                                string y = "0";
                                int z = 7;
                                
                                 
                                TilePointConvert tilePointConvert = new TilePointConvert();
                                foreach (Mosaic item in result.mosaics)
                                
                                {
                                    MapPoint sw = MapPointBuilder.CreateMapPoint(item.bbox[0], item.bbox[1], MapView.Active.Extent.SpatialReference);
                                    MapPoint ne = MapPointBuilder.CreateMapPoint(item.bbox[2], item.bbox[3], MapView.Active.Extent.SpatialReference);
                                    IList<MapPoint> mapPoints = new List<MapPoint>();
                                    mapPoints.Add(sw);
                                    mapPoints.Add(ne);
                                    double zz = TilePointConvert.BestMapView(mapPoints, 100, 56, 2);
                                    z = (int)Math.Floor(zz);
                                    if (z < 0)
                                    {
                                        z = z * -1;
                                    }
                                    double centerlong = (item.bbox[0] + item.bbox[2]) / 2;
                                    double centerlat = (item.bbox[1] + item.bbox[3]) / 2;
                                    //z=4;
                                    PointF point = tilePointConvert.WorldToTilePos(centerlong, centerlat, z);
                                    //item.Thumbnail = item._links.tiles.Replace("{x}", x).Replace("{y}", y).Replace("{z}", "0");
                                    item.Thumbnail = item._links.tiles.Replace("{x}", Math.Floor(point.X).ToString()).Replace("{y}", Math.Floor(point.Y).ToString()).Replace("{z}", z.ToString());
                                    lstMosaics.Add(item);
                                }

                                // Run the callback method, passing the current page of data from the API.
                                //callBack?.Invoke(result);

                                // Get the URL for the next page
                                nextUrl = result._links._next ?? string.Empty;
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
        /// <summary>
        /// call back to access results of a page while inside the pagination loop
        /// not currently used
        /// </summary>
        /// <param name="getmaoicstask2"></param>
        private static void ResultCallBack(Mosaics getmaoicstask2)
        {
            if (getmaoicstask2 != null )
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
        /// <summary>
        /// Click event for gallery items
        /// </summary>
        /// <param name="item"></param>
        protected override void OnClick(object item)
        {
            OpenWebMapAsync(item);          
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
                var serverConnection = new CIMProjectServerConnection { URL = mosaic._links._self.Substring(0, mosaic._links._self.IndexOf("?")) + "/wmts?REQUEST=GetCapabilities&api_key=" + Module1.Current.API_KEY.API_KEY_Value };// "1fe575980e78467f9c28b552294ea410"
                var connection = new CIMWMTSServiceConnection { ServerConnection = serverConnection };
                await QueuedTask.Run(() =>
                {
                    BasicRasterLayer layer2 = LayerFactory.Instance.CreateRasterLayer(connection, MapView.Active.Map, 0, mosaic.name);
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

    }
    public class TilePointConvert
    {
        public PointF WorldToTilePos(double lon, double lat, int zoom)
        {
            PointF p = new PointF();

            p.X = (float)((lon + 180.0) / 360.0 * (1 << zoom));
            p.Y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

            return p;
        }

        public  PointF TileToWorldPos(double tile_x, double tile_y, int zoom)
        {
            PointF p = new PointF();
            double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

            p.X = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
            p.Y = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

            return p;
        }

        /// <summary>
        /// Calculates the best map view for a list of locations for a map
        /// </summary>
        /// <param name="locations">List of location objects</param>
        /// <param name="mapWidth">Map width in pixels</param>
        /// <param name="mapHeight">Map height in pixels</param>
        /// <param name="buffer">Width in pixels to use to create a buffer around the map. This is to keep pushpins from being cut off on the edge</param>
        /// <returns>Returns a MapViewSpecification with the best map center point and zoom level for the given set of locations</returns>
        public static double BestMapView(IList<MapPoint> locations, double mapWidth, double mapHeight, int buffer)
        {
            double zoom = 0;
            double zoomLevel = 0;

            //double maxLat = -85;
            //double minLat = 85;
            //double maxLon = -180;
            //double minLon = 180;
            double maxLat = -180;
            double minLat = 180;
            double maxLon = -85;
            double minLon = 85;
            //calculate bounding rectangle
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i].X > maxLat)
                {
                    maxLat = locations[i].X;
                }

                if (locations[i].X < minLat)
                {
                    minLat = locations[i].X;
                }

                if (locations[i].Y > maxLon)
                {
                    maxLon = locations[i].Y;
                }

                if (locations[i].Y < minLon)
                {
                    minLon = locations[i].Y;
                }
            }
            MapPoint center = MapPointBuilder.CreateMapPoint((maxLat + minLat) / 2, (maxLon + minLon) / 2, MapView.Active.Extent.SpatialReference);

            //center.X = (maxLat + minLat) / 2;
            //center.Y = (maxLon + minLon) / 2;

            double zoom1 = 0, zoom2 = 0;

            //Determine the best zoom level based on the map scale and bounding coordinate information
            if (maxLon != minLon && maxLat != minLat)
            {
                //best zoom level based on map width

                zoom1 = Math.Log(360.0 / 256.0 * (mapWidth - 2 * buffer) / (maxLon - minLon)) / Math.Log(2);
                //best zoom level based on map height
                zoom2 = Math.Log(180.0 / 256.0 * (mapHeight - 2 * buffer) / (maxLat - minLat)) / Math.Log(2);
            }

            if (zoom1 < 0)
            {
                zoom1 = zoom1 * -1;
            }
            if (zoom2 < 0)
            {
                zoom2 = zoom2 * -1;
            }
            //use the most zoomed out of the two zoom levels
            zoom = (zoom1 < zoom2) ? zoom1 : zoom2;
            zoom = zoom1;
            //mapView = new MapViewSpecification(center, zoomLevel);

            return zoom;
        }

    }
}
