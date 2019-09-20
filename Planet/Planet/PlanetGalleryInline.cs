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

        private int PageSize = 10;
        private int PageNumber = 0;
        private int TotalPages = 0;

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

            PlanetGalleryChangedEvent.Subscribe((args) =>
            {
                if (args.NewPage == "next")
                {
                    SetNextPage();
                }
                if (args.NewPage == "prev")
                {
                    SetPrevPage();
                }
            });

            PlanetGalleryFilterEvent.Subscribe((args) =>
            {
                FilterItems(args.FilterText);
            });

            //MapViewCameraChangedEvent.Subscribe(OnCameraChanged);
            Initialize();
        }

        private void OnCameraChanged(MapViewCameraChangedEventArgs obj)
        {
            if (obj.MapView == MapView.Active)
            {
                var camera = obj.CurrentCamera;
                if (ItemCollection.Count > 0)
                {
                    //foreach(Mosaic item in ItemCollection)
                    //{
                    //    SetThumbnail(item);
                    //}
                }
            }
        }

        private void SetNextPage()
        {
            if (PageNumber != TotalPages)
            {
                PageNumber = PageNumber + 1;
                var currentItems = Items.Skip((PageNumber - 1) * PageSize).Take(PageSize);
                Clear();
                foreach (var dataItem in currentItems)
                {
                    Add(dataItem);
                }
            }
        }

        private void SetPrevPage()
        {
            if (PageNumber != 1)
            {
                PageNumber = PageNumber - 1;
                var currentItems = Items.Skip((PageNumber - 1) * PageSize).Take(PageSize);
                Clear();
                foreach (var dataItem in currentItems)
                {
                    Add(dataItem);
                }
            } else
            {
                
            }
        }

        private void FilterItems(string text)
        {
            List<Mosaic> filteredItems = new List<Mosaic>();
            if (text.Trim().Length > 0)
            {
                filteredItems = ItemsClean.Where(i => i.name.Contains(text)).ToList();
            } else
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
                    Add(dataItem);
                }
                PageNumber = 1;
            }

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
                    Items = await GetMosicsAsync2(ResultCallBack);
                    ItemsClean = Items;
                    PageNumber = 0;
                    TotalPages = 0;
                    if (Items.Count > 0)
                    {
                        this.Clear();
                        TotalPages = (Items.Count + PageSize - 1) / PageSize;
                        var currentItems = Items.Take(PageSize);
                        foreach (var dataItem in currentItems)
                        {
                            Add(dataItem);
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

                                var extent = MapView.Active.Extent;
                                TilePointConvert tilePointConvert = new TilePointConvert();
                                foreach (Mosaic item in result.mosaics)
                                {
                                    MapPoint sw = MapPointBuilder.CreateMapPoint(item.bbox[0], item.bbox[1], MapView.Active.Extent.SpatialReference);
                                    MapPoint ne = MapPointBuilder.CreateMapPoint(item.bbox[2], item.bbox[3], MapView.Active.Extent.SpatialReference);
                                    //MapPoint sw = MapPointBuilder.CreateMapPoint(extent.XMin, extent.YMin, extent.SpatialReference);
                                    //sw = GeometryEngine.Instance.Project(sw, SpatialReferences.WGS84) as MapPoint;
                                    //MapPoint ne = MapPointBuilder.CreateMapPoint(extent.XMax, extent.YMax, extent.SpatialReference);
                                    //ne = GeometryEngine.Instance.Project(ne, SpatialReferences.WGS84) as MapPoint;

                                    IList<MapPoint> mapPoints = new List<MapPoint>();
                                    mapPoints.Add(sw);
                                    mapPoints.Add(ne);
                                    double zz = TilePointConvert.BestMapView(mapPoints, 100, 56, 2);
                                    double[] bounds = { item.bbox[0], item.bbox[1], item.bbox[2], item.bbox[3] };

                                    double centerlat2;
                                    double centerlong2;
                                    double z2;
                                    TilePointConvert.BestMapView2(bounds, 90, 46, 2, 256, out centerlat2, out centerlong2, out z2);
                                    z = (int)Math.Floor(zz);
                                    if (z < 0)
                                    {
                                        z = z * -1;
                                    }
                                    double centerlong = (item.bbox[0] + item.bbox[2]) / 2;
                                    double centerlat = (item.bbox[1] + item.bbox[3]) / 2;
                                    //var centerProjected = GeometryEngine.Instance.Project(extent.Center, SpatialReferences.WGS84) as MapPoint;
                                    //double centerlong = centerProjected.X;
                                    //double centerlat = centerProjected.Y;
                                    //z=4;
                                    PointF point = tilePointConvert.WorldToTilePos(centerlong, centerlat, z);
                                    double[] pos = { centerlong2, centerlat2 };
                                    z2 = Convert.ToInt16(z2);
                                    if (z2 < 0)
                                    {
                                        z2 = 0;
                                    }
                                    else if (z2 > 0)
                                    {
                                        z2 = z2 + 2;
                                    }
                                    tilePointConvert.WorldToTilePos2(pos, Convert.ToInt32(z2), 256, out int tileX, out int tileY);

                                    //item.Thumbnail = item._links.tiles.Replace("{x}", x).Replace("{y}", y).Replace("{z}", "0");
                                    //item.Thumbnail = item._links.tiles.Replace("{x}", Math.Floor(point.X).ToString()).Replace("{y}", Math.Floor(point.Y).ToString()).Replace("{z}", z.ToString());
                                    item.Thumbnail = item._links.tiles.Replace("{x}", tileX.ToString()).Replace("{y}", tileY.ToString()).Replace("{z}", z2.ToString());
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

        private void SetThumbnail(Mosaic item)
        {
            TilePointConvert tilePointConvert = new TilePointConvert();
            var extent = MapView.Active.Extent;
            int z = 7;
            MapPoint sw = MapPointBuilder.CreateMapPoint(extent.XMin, extent.YMin, extent.SpatialReference);
            sw = GeometryEngine.Instance.Project(sw, SpatialReferences.WGS84) as MapPoint;
            MapPoint ne = MapPointBuilder.CreateMapPoint(extent.XMax, extent.YMax, SpatialReferences.WGS84);
            ne = GeometryEngine.Instance.Project(ne, SpatialReferences.WGS84) as MapPoint;
            IList<MapPoint> mapPoints = new List<MapPoint>();
            mapPoints.Add(sw);
            mapPoints.Add(ne);
            double zz = TilePointConvert.BestMapView(mapPoints, 100, 56, 2);
            z = (int)Math.Floor(zz);
            if (z < 0)
            {
                z = z * -1;
            }
            var centerProjected = GeometryEngine.Instance.Project(extent.Center, SpatialReferences.WGS84) as MapPoint;
            double centerlong = centerProjected.X;
            double centerlat = centerProjected.Y;
            PointF point = tilePointConvert.WorldToTilePos(centerlong, centerlat, z);
            item.Thumbnail = item._links.tiles.Replace("{x}", Math.Floor(point.X).ToString()).Replace("{y}", Math.Floor(point.Y).ToString()).Replace("{z}", z.ToString());
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

    }
    public class TilePointConvert
    {
        private const double MinLatitude = -85.05112878;
        private const double MaxLatitude = 85.05112878;
        private const double MinLongitude = -180;
        private const double MaxLongitude = 180;
        public PointF WorldToTilePos(double lon, double lat, int zoom)
        {
            PointF p = new PointF();

            p.X = (float)((lon + 180.0) / 360.0 * (1 << zoom));
            p.Y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

            return p;
        }
        /// <summary>
        /// Calculates the XY tile coordinates that a coordinate falls into for a specific zoom level.
        /// </summary>
        /// <param name="position">Position coordinate in the format [longitude, latitude]</param>
        /// <param name="zoom">Zoom level</param>
        /// <param name="tileSize">The size of the tiles in the tile pyramid.</param>
        /// <param name="tileX">Output parameter receiving the tile X position.</param>
        /// <param name="tileY">Output parameter receiving the tile Y position.</param>
        public  void WorldToTilePos2(double[] position, int zoom, int tileSize, out int tileX, out int tileY)
        {
            var latitude = Clip(position[1], MinLatitude, MaxLatitude);
            var longitude = Clip(position[0], MinLongitude, MaxLongitude);

            var x = (longitude + 180) / 360;
            var sinLatitude = Math.Sin(latitude * Math.PI / 180);
            var y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

            var mapSize = MapSize(zoom, tileSize);
            tileX = (int)Math.Floor(Clip(x * mapSize + 0.5, 0, mapSize - 1) / tileSize);
            tileY = (int)Math.Floor(Clip(y * mapSize + 0.5, 0, mapSize - 1) / tileSize);
        }
        /// <summary>
        /// Clips a number to the specified minimum and maximum values.
        /// </summary>
        /// <param name="n">The number to clip.</param>
        /// <param name="minValue">Minimum allowable value.</param>
        /// <param name="maxValue">Maximum allowable value.</param>
        /// <returns>The clipped value.</returns>
        private static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }
        /// <summary>
        /// Calculates width and height of the map in pixels at a specific zoom level from -180 degrees to 180 degrees.
        /// </summary>
        /// <param name="zoom">Zoom Level to calculate width at</param>
        /// <param name="tileSize">The size of the tiles in the tile pyramid.</param>
        /// <returns>Width and height of the map in pixels</returns>
        public static double MapSize(double zoom, int tileSize)
        {
            return Math.Ceiling(tileSize * Math.Pow(2, zoom));
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
            //zoom = zoom1;
            //mapView = new MapViewSpecification(center, zoomLevel);

            return zoom;
        }

        /// <summary>
        /// Calculates the best map view (center, zoom) for a bounding box on a map.
        /// </summary>
        /// <param name="bounds">A bounding box defined as an array of numbers in the format of [west, south, east, north].</param>
        /// <param name="mapWidth">Map width in pixels.</param>
        /// <param name="mapHeight">Map height in pixels.</param>
        /// <param name="padding">Width in pixels to use to create a buffer around the map. This is to keep markers from being cut off on the edge</param>
        /// <param name="tileSize">The size of the tiles in the tile pyramid.</param>
        /// <param name="latitude">Output parameter receiving the center latitude coordinate.</param>
        /// <param name="longitude">Output parameter receiving the center longitude coordinate.</param>
        /// <param name="zoom">Output parameter receiving the zoom level</param>
        public static void BestMapView2(double[] bounds, double mapWidth, double mapHeight, int padding, int tileSize, out double centerLat, out double centerLon, out double zoom)
        {
            if (bounds == null || bounds.Length < 4)
            {
                centerLat = 0;
                centerLon = 0;
                zoom = 1;
                return;
            }

            double boundsDeltaX;

            //Check if east value is greater than west value which would indicate that bounding box crosses the antimeridian.
            if (bounds[2] > bounds[0])
            {
                boundsDeltaX = bounds[2] - bounds[0];
                centerLon = (bounds[2] + bounds[0]) / 2;
            }
            else
            {
                boundsDeltaX = 360 - (bounds[0] - bounds[2]);
                centerLon = ((bounds[2] + bounds[0]) / 2 + 360) % 360 - 180;
            }

            var ry1 = Math.Log((Math.Sin(bounds[1] * Math.PI / 180) + 1) / Math.Cos(bounds[1] * Math.PI / 180));
            var ry2 = Math.Log((Math.Sin(bounds[3] * Math.PI / 180) + 1) / Math.Cos(bounds[3] * Math.PI / 180));
            var ryc = (ry1 + ry2) / 2;

            centerLat = Math.Atan(Math.Sinh(ryc)) * 180 / Math.PI;

            var resolutionHorizontal = boundsDeltaX / (mapWidth - padding * 2);

            var vy0 = Math.Log(Math.Tan(Math.PI * (0.25 + centerLat / 360)));
            var vy1 = Math.Log(Math.Tan(Math.PI * (0.25 + bounds[3] / 360)));
            var zoomFactorPowered = (mapHeight * 0.5 - padding) / (40.7436654315252 * (vy1 - vy0));
            var resolutionVertical = 360.0 / (zoomFactorPowered * tileSize);

            var resolution = Math.Max(resolutionHorizontal, resolutionVertical);

            zoom = Math.Log(360 / (resolution * tileSize), 2);
        }

    }
}
