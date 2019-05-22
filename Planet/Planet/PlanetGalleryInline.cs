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

namespace Planet
{
    internal class PlanetGalleryInline : Gallery 
    {
        private bool _isInitialized;

        public PlanetGalleryInline()
        {
            Initialize();
        }

        private async void Initialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;
            var lstWebmapItems = await GetMosicsAsync();
            foreach (var dataItem in lstWebmapItems)
                Add(dataItem);

            
            //Add 6 items to the gallery
            //for (int i = 0; i < 6; i++)
            //{
            //    string name = string.Format("Item {0}", i);
            //    Add(new GalleryItem(name, this.LargeImage != null ? ((ImageSource)this.LargeImage).Clone() : null, name));
            //}


        }
        private async Task<List<Mosaic>>GetMosicsAsync()
        {
            var lstMosaics = new List<Mosaic>();
            try
            {
                await QueuedTask.Run(async () =>
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync("https://api.planet.com/basemaps/v1/mosaics?api_key=1fe575980e78467f9c28b552294ea410");
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

                });
            }
            catch (HttpRequestException hex)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", hex.Message);
            }
            catch (Exception e)
            {

                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }


            return lstMosaics;
        }

        protected override void OnClick(object item)
        {
            OpenWebMapAsync(item);
            //TODO - insert your code to manipulate the clicked gallery item here
            //Mosaic mosaic = (Mosaic)item;

            //var project = Project.Current;
            //var serverConnection = new CIMProjectServerConnection { URL = "https://api.planet.com/basemaps/v1/mosaics/48fff803-4104-49bc-b913-7467b7a5ffb5/wmts?REQUEST=GetCapabilities&api_key=1fe575980e78467f9c28b552294ea410" };
            //var connection = new CIMWMTSServiceConnection { ServerConnection = serverConnection };
            //BasicRasterLayer layer2 = LayerFactory.Instance.CreateRasterLayer(connection, MapView.Active.Map, 0, connection.LayerName + "_asd");
            //System.Diagnostics.Debug.WriteLine("Remove this line after adding your custom behavior.");
            //base.OnClick(item);
            
        }

        private async void OpenWebMapAsync(object item)
        {
            if (item is Mosaic mosaic)
            {
                Project project = Project.Current;
                var serverConnection = new CIMProjectServerConnection { URL = mosaic._links._self.Substring(0, mosaic._links._self.IndexOf("?")) + "/wmts?REQUEST=GetCapabilities&api_key=1fe575980e78467f9c28b552294ea410" };
                var connection = new CIMWMTSServiceConnection { ServerConnection = serverConnection };
                await QueuedTask.Run(() =>
                {
                    BasicRasterLayer layer2 = LayerFactory.Instance.CreateRasterLayer(connection, MapView.Active.Map, 0, mosaic.name);
                });

            }
        }
    }
}
