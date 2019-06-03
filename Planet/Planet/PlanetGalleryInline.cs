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
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System.Net.Http;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

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
                var lstWebmapItems = await GetMosicsAsync();
                if (lstWebmapItems.Count>0)
                {
                    foreach (var dataItem in lstWebmapItems)
                        Add(dataItem);
                    _isInitialized = true;
                    FrameworkApplication.State.Activate("planet_state_connection");
                }

            }
        }
        //private async Task<bool>ValidKey()
        //{
        //    bool validKey = false;
        //    HttpResponseMessage response = await _client.GetAsync("https://api.planet.com/basemaps/v1/mosaics?api_key=" + Module1.Current.API_KEY.API_KEY_Value);
        //    if (response.IsSuccessStatusCode  )
        //    {
        //        validKey = true;
        //    }
        //    return validKey;
        //}
        private async Task<List<Mosaic>>GetMosicsAsync()
        {
            var lstMosaics = new List<Mosaic>();
            try
            {
                await QueuedTask.Run(async () =>
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            //HttpResponseMessage response = await client.GetAsync("https://api.planet.com/basemaps/v1/mosaics?api_key=1fe575980e78467f9c28b552294ea410");
                            HttpResponseMessage response = await client.GetAsync("https://api.planet.com/basemaps/v1/mosaics?api_key=" + Module1.Current.API_KEY.API_KEY_Value);
                            response.EnsureSuccessStatusCode();
                            string responseBody = await response.Content.ReadAsStringAsync();
                            Mosaics splashInfo = JsonConvert.DeserializeObject<Mosaics>(responseBody);
                            string x = "0";
                            string y = "0";
                            string z = "0";
                            foreach (Mosaic item in splashInfo.mosaics)
                            {
                                //item.Thumbnail = string.Format(item._links.tiles,"0","0","0");
                                item.Thumbnail = item._links.tiles.Replace("{x}", x).Replace("{y}", y).Replace("{z}", z);
                                lstMosaics.Add(item);
                            }
                        }
                        catch  (HttpRequestException hex)
                        {
                            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("There was a problem logging in" + Environment.NewLine + hex.Message + Environment.NewLine + "Please check your key and try again", "Error logging in",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                            FrameworkApplication.State.Deactivate("planet_state_connection");
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                    }

                });
            }
            catch (HttpRequestException hex)
            {
                _isInitialized = false;
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", hex.Message);
            }
            catch (Exception e)
            {
                _isInitialized = false;
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }


            return lstMosaics;
        }

        protected override void OnClick(object item)
        {
            OpenWebMapAsync(item);          
        }

        private async void OpenWebMapAsync(object item)
        {
            if (item is Mosaic mosaic)
            {
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
}
