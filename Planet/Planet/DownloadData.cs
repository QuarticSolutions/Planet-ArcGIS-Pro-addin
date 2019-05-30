using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
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
using MessageBox = ArcGIS.Desktop.Framework.Dialogs.MessageBox;

namespace Planet
{
    internal class DownloadData : MapTool
    {
        public DownloadData()
        {
            IsSketchTool = true;
            UseSnapping = true;
            // Select the type of construction tool you wish to implement.  
            // Make sure that the tool is correctly registered with the correct component category type in the daml 
            //SketchType = SketchGeometryType.Point;
            // SketchType = SketchGeometryType.Line;
            SketchType = SketchGeometryType.Polygon;
        }

        /// <summary>
        /// Called when the sketch finishes. This is where we will create the sketch operation and then execute it.
        /// </summary>
        /// <param name="geometry">The geometry created by the sketch.</param>
        /// <returns>A Task returning a Boolean indicating if the sketch complete event was successfully handled.</returns>
        protected  override Task<bool> OnSketchCompleteAsync(Geometry geometry)
        {
            string rasterseriesname = "";
            string rasterseriesid = "";
            bool _isPlanetRaster = false;
            if (geometry == null)
                return Task.FromResult(false);

            IReadOnlyList<Layer> ts = MapView.Active.GetSelectedLayers();
            if(ts.Count > 1)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("More than one layer selected \n Please only choose a single Planet Image layer","More than one layer selected",MessageBoxButton.OK,MessageBoxImage.Warning);
                return Task.FromResult(false);
            }

            foreach (Layer item in ts)
            {
                if (item is TiledServiceLayer tiledService)
                {
                    if (!tiledService.URL.Contains("https://api.planet.com/basemaps/v1/mosaics"))
                    {
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("The selected layer is not a Planet Image layer", "Wrong Layer type", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return Task.FromResult(false);
                    }
                    else
                    {
                        rasterseriesname = tiledService.Name;
                        rasterseriesid = tiledService.URL.Substring(tiledService.URL.IndexOf("mosaics") + 8, 36);
                        _isPlanetRaster = true;
                    }
                }
            }
            if (! _isPlanetRaster)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("The selected layer is not a Planet Image layer", "Wrong Layer type", MessageBoxButton.OK, MessageBoxImage.Warning);
                return Task.FromResult(false);

            }
            //var response = client.GetAsync("https://api.planet.com/basemaps/v1/mosaics/48fff803-4104-49bc-b913-7467b7a5ffb5/quads?api_key=1fe575980e78467f9c28b552294ea410&bbox=" + geometry.Extent.XMin.ToString() + "," + geometry.Extent.YMin.ToString() + "," + geometry.Extent.XMax.ToString() + "," + geometry.Extent.YMax.ToString()).Result;
            //SpatialReference sr = SpatialReferences.WGS84;
            //SpatialReference sr2 = SpatialReferences.WebMercator;

            MapPoint point0 = MapPointBuilder.CreateMapPoint(geometry.Extent.XMin, geometry.Extent.YMin, MapView.Active.Extent.SpatialReference);
            MapPoint point1 = MapPointBuilder.CreateMapPoint(geometry.Extent.XMax, geometry.Extent.YMax, MapView.Active.Extent.SpatialReference);
            ToGeoCoordinateParameter ddParam = new ToGeoCoordinateParameter(GeoCoordinateType.DD);

            string geoCoordString = geometry.Extent.Center.ToGeoCoordinateString(ddParam);
            string[] lowP = point0.ToGeoCoordinateString(ddParam).Split(' ');
            string[] highP = point1.ToGeoCoordinateString(ddParam).Split(' ');

            IEnumerable<string> union = lowP.Union(highP);
            string quadparm = "";

            for (int i = 0; i < union.Count(); i++)
            {
                string crr = union.ElementAt(i);
                if (crr.Contains("W") || crr.Contains("S"))
                {
                    crr = "-" + crr.Substring(0, crr.Length - 1);
                }
                else
                {
                    crr = crr.Substring(0, crr.Length - 1);
                }
                quadparm = quadparm + "," + crr;
            }
            string[] ff = quadparm.Trim(',').Split(',');
            getQuadsAsync(geometry, ff, rasterseriesname, rasterseriesid);

            return Task.FromResult(true);


            //// Create an edit operation
            //var createOperation = new EditOperation();
            //createOperation.Name = string.Format("Create {0}", CurrentTemplate.Layer.Name);
            //createOperation.SelectNewFeatures = true;

            //// Queue feature creation
            //createOperation.Create(CurrentTemplate, geometry);

            //// Execute the operation
            //return createOperation.ExecuteAsync();
        }

        private async Task getQuadsAsync(Geometry geometry, string[] ff, string rasterseriesname, string rasterseriesid)
        {


            using (HttpClient client = new HttpClient())
            {
                //var response = client.GetAsync("https://api.planet.com/basemaps/v1/mosaics/" + rasterseriesid + "/quads?api_key=1fe575980e78467f9c28b552294ea410&bbox=" + ff[1] + "," + ff[0] + "," + ff[3] + "," + ff[2] + ",").Result;
                var response = client.GetAsync("https://api.planet.com/basemaps/v1/mosaics/" + rasterseriesid + "/quads?api_key=" + Module1.Current.API_KEY.API_KEY_Value  + "&bbox=" + ff[1] + "," + ff[0] + "," + ff[3] + "," + ff[2]).Result;
                ObservableCollection<Data.GeoTiffs2> geotiffs = new ObservableCollection<Data.GeoTiffs2>();
                if (response.IsSuccessStatusCode)
                {
                    Task<string>  responseContent =  response.Content.ReadAsStringAsync();
                    Quads quads = JsonConvert.DeserializeObject<Quads>(responseContent.Result);
                    foreach (Item item in quads.items)
                    {
                        geotiffs.Add(new Data.GeoTiffs2() { Name = item.id, DownloadURL=item._links.download });
                    }
                    Console.WriteLine(responseContent.Result);
                }
                else
                {
                    MessageBox.Show("Extent not found");
                    return;
                }
                string area = "";
                if (geometry.GeometryType == GeometryType.Polygon )
                {
                    
                    Polygon polygon = (Polygon)geometry;
                    if (geometry.SpatialReference.Unit.Name == "Foot_US")
                    {
                        double sqMeters = AreaUnit.SquareFeet.ConvertToSquareMeters(polygon.Area);
                        area = AreaUnit.SquareKilometers.ConvertFromSquareMeters(sqMeters).ToString();
                    }
                    else if (geometry.SpatialReference.Unit.Wkt == "meters")
                    {
                        area = AreaUnit.SquareKilometers.ConvertFromSquareMeters(polygon.Area).ToString();
                    }

                    area = "Approx Sqkm: " + area. Substring(0,area.IndexOf("."));
                    
                }
                FolderSelector folderSelector = new FolderSelector();
                folderSelector.lbxGrids.ItemsSource = geotiffs;
                folderSelector.ShowNewFolderButton = false;
                folderSelector.ShowActivated = true;
                folderSelector.SizeToContent = SizeToContent.Width;
                object da = folderSelector.txtGrids.DataContext;
                if(da is Data.BaseItem ba)
                {
                    ba.QuadCount = "Total Quads selected: " + geotiffs.Count.ToString() + area;
                }



                folderSelector.ShowDialog();
                if ((bool)folderSelector.DialogResult)
                {
                    string savelocation = folderSelector.SelectedPath;
                    foreach (Data.GeoTiffs2 quad in geotiffs)
                    {
                        await LoadImage(quad.DownloadURL, savelocation + "\\" + rasterseriesname + quad.Name + ".tif");
                    }
                    FrameworkApplication.AddNotification(new Notification()
                    {
                        Title = "Download Successful",
                        Message = "Successfully dowloaded the requesed quads",
                        ImageUrl = @"pack://application:,,,/Planet;component/Images/Planet_logo-dark.png"
                        
                    });



                }

            }
        }

        public async static Task<bool> LoadImage(string uri, string destination)
        {
            //BitmapImage bitmapImage = new BitmapImage();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (var response = await client.GetAsync(uri,HttpCompletionOption.ResponseHeadersRead))
                    {
                        using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        {
                            string fileToWriteTo = destination; //Path.GetTempFileName();
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
                return true;
            }
            catch (Exception ex)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Error downloading \n" + ex.Message,"Download Problem",MessageBoxButton.OK,MessageBoxImage.Warning);
                Console.WriteLine("Failed to Download the quad: {0}", ex.Message);
            }

            return false;
        }


        //public static IEnumerable<T> Replace<T>(this IEnumerable<T> enumerable, int index, T value)
        //{
        //    return enumerable.Select((x, i) => index == i ? value : x);
        //}

    }
}
