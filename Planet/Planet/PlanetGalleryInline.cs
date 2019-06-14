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
        //private HttpClient _client;

        
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


            }
        }

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
                                    //double centerlong = (item.bbox[0] + item.bbox[2]) / 2;
                                    //double centerlat = (item.bbox[1] + item.bbox[3]) / 2;
                                    //PointF point = tilePointConvert.WorldToTilePos(centerlong, centerlat, z);
                                    item.Thumbnail = item._links.tiles.Replace("{x}", x).Replace("{y}", y).Replace("{z}", "0");
                                    //item.Thumbnail = item._links.tiles.Replace("{x}", Math.Floor(point.X).ToString()).Replace("{y}", Math.Floor(point.Y).ToString()).Replace("{z}", z.ToString());
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
        protected override void OnClick(object item)
        {
            OpenWebMapAsync(item);          
        }

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
    }
}
