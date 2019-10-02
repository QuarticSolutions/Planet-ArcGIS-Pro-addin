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
using System.Windows.Threading;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Geoprocessing;
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
    /// <summary>
    /// Tool that downloads the data. 
    /// </summary>
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
        /// The sketch geometry will be turned into an envelope and used to query the Planet api for the relevant quads
        /// The FolderSelector view and various models will then be presented allowing the user to choose a save location
        /// Finally, after the .tif files have been downloaded a raster mosic will be created in the project default FGDB and 
        /// the downloaded .tif files added.
        /// </summary>
        /// <param name="geometry">The geometry created by the sketch.</param>
        /// <returns>A Task returning a Boolean indicating if the sketch complete event was successfully handled.</returns>
        protected  override  Task<bool> OnSketchCompleteAsync(Geometry geometry)
        {
            string rasterseriesname = "";
            string rasterseriesid = "";
            bool _isPlanetRaster = false;
            if (geometry == null)
                return Task.FromResult(false);


            //Make sure a valid Planet layer is selected.
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

            //Create min max points from the envelope and get the coords in Decimal Degrees
            MapPoint point0 = MapPointBuilder.CreateMapPoint(geometry.Extent.XMin, geometry.Extent.YMin, MapView.Active.Extent.SpatialReference);
            MapPoint point1 = MapPointBuilder.CreateMapPoint(geometry.Extent.XMax, geometry.Extent.YMax, MapView.Active.Extent.SpatialReference);
            ToGeoCoordinateParameter ddParam = new ToGeoCoordinateParameter(GeoCoordinateType.DD);

            string geoCoordString = geometry.Extent.Center.ToGeoCoordinateString(ddParam);
            string[] lowP = point0.ToGeoCoordinateString(ddParam).Split(' ');
            string[] highP = point1.ToGeoCoordinateString(ddParam).Split(' ');

            IEnumerable<string> union = lowP.Union(highP);
            string quadparm = "";
            //need to change the formatting of the coords.
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
            var result =  getQuadsAsync(geometry, ff, rasterseriesname, rasterseriesid);
            //result.Wait();
            if (result.IsFaulted)
            {
                MessageBox.Show(result.Exception.Message,"An error Occured",MessageBoxButton.OK,MessageBoxImage.Error);
                return Task.FromResult(false);
            }
            //result.Wait();
            return Task.FromResult(true);
        }

        /// <summary>
        /// Downloads the quads from the Planet servers using a geometry, 
        /// an array of DD coords, raster service name and raster serviceid.
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="ff"></param>
        /// <param name="rasterseriesname"></param>
        /// <param name="rasterseriesid"></param>
        /// <returns></returns>
        private async Task getQuadsAsync(Geometry geometry, string[] ff, string rasterseriesname, string rasterseriesid)
        {
            //Mosicing can only happen with a standard license, warn user
            var ll = ArcGIS.Core.Licensing.LicenseInformation.Level;
            if (ll == ArcGIS.Core.Licensing.LicenseLevels.Basic)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("ArcGIS Pro is currently using a Basic license. The data source tiles can still be downloaded but a Raster Mosaic will not created." + Environment.NewLine+ "Would you like to continue?", "Basic License Detected", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    return;
                }
            }

            //should not use a using statement, need to fix
            //get a list of quads as GeoTiffs2 items by querying the rest endpoint of the selected layer
            //with the geometry and points
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync("https://api.planet.com/basemaps/v1/mosaics/" + rasterseriesid + "/quads?api_key=" + Module1.Current.API_KEY.API_KEY_Value  + "&bbox=" + ff[1] + "," + ff[0] + "," + ff[3] + "," + ff[2]).Result;
                ObservableCollection<Data.GeoTiffs2> geotiffs = new ObservableCollection<Data.GeoTiffs2>();
                if (response.IsSuccessStatusCode)
                {
                    Task<string>  responseContent =  response.Content.ReadAsStringAsync();
                    Quads quads = JsonConvert.DeserializeObject<Quads>(responseContent.Result);
                    if (quads.items.Count() > 50)
                    {
                        MessageBox.Show("More than 50 Quads delected, please download a smaller area.");
                        return;
                    } 
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
                    else if (geometry.SpatialReference.Unit.Name == "Meter")
                    {
                        area = AreaUnit.SquareKilometers.ConvertFromSquareMeters(polygon.Area).ToString();
                    }

                    area = " Approx Sqkm: " + area. Substring(0,area.IndexOf("."));
                    
                }

                //set up the Folderselector view with a list of the quads and area
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

                //If the user clicked OK on the view start the download
                if ((bool)folderSelector.DialogResult)
                {
                    //Do the download
                    string savelocation = folderSelector.SelectedPath;
                    List<string> tiffs = new List<string>() ;
                    int i = 0;
                    int total = geotiffs.Count();
                    
                    foreach (Data.GeoTiffs2 quad in geotiffs)
                    {
                        if (string.IsNullOrEmpty(quad.DownloadURL))
                        {
                            MessageBox.Show(String.Format("No download link provided for quad {0}", quad.Name));
                            continue;
                        }
                        await LoadImage(quad.DownloadURL, savelocation + "\\" + rasterseriesname + quad.Name + ".tif");
                        tiffs.Add(savelocation + "\\" + rasterseriesname + quad.Name + ".tif");
                        if (total%++i == 0)
                        {
                            FrameworkApplication.AddNotification(new Notification()
                            {
                                Title = "Downloading........",
                                Message = String.Format("{0} of {1} files successfully dowloaded",i,total) ,
                                ImageUrl = @"Images/Planet_logo-dark.png"
                                //ImageUrl = @"pack://application:,,,/Planet;component/Images/Planet_logo-dark.png"

                            });
                        }


                    }

                    if (tiffs.Count == 0)
                    {
                        return;
                    }
                    //Download Part finished, tell user
                    FrameworkApplication.AddNotification(new Notification()
                    {
                        Title = "Downloading Finished",
                        Message = String.Format("Successfully dowloaded {0} of {1} files ", i, total) + Environment.NewLine + "Sting the Mosic process",
                        ImageUrl = @"pack://application:,,,/Planet;component/Images/Planet_logo-dark.png"

                    });

                    //Create a raster Mosic in the default FGDB using Geoprocessing tools
                    string inpath = Project.Current.DefaultGeodatabasePath;
                    //inpath = @"C:\Users\Andrew\Documents\ArcGIS\Projects\MyProject22\MyProject22.gdb";
                    string in_mosaicdataset_name = rasterseriesname;
                    var sr = await QueuedTask.Run(() => {
                        return SpatialReferenceBuilder.CreateSpatialReference(3857);
                    });

                    var parameters = Geoprocessing.MakeValueArray(inpath, in_mosaicdataset_name, sr, "3", "8_BIT_UNSIGNED", "NATURAL_COLOR_RGB");
                    string tool_path = "management.CreateMosaicDataset";
                    System.Threading.CancellationTokenSource _cts = new System.Threading.CancellationTokenSource();
                    IGPResult result = await Geoprocessing.ExecuteToolAsync(tool_path, parameters, null, _cts.Token, (event_name, o) =>  // implement delegate and handle events, o is message object.
                    {
                        switch (event_name)
                        {
                            case "OnValidate": // stop execute if any warnings
                                if ((o as IGPMessage[]).Any(it => it.Type == GPMessageType.Warning || it.Type == GPMessageType.Error))
                                {
                                    System.Windows.MessageBox.Show(o.ToString());
                                    _cts.Cancel();
                                }
                                break;
                            //case "OnProgressMessage":
                            //    string msg = string.Format("{0}: {1}", new object[] { event_name, (string)o });
                            //    //System.Windows.MessageBox.Show(msg);
                            //    //_cts.Cancel();
                            //    break;
                            //case "OnProgressPos":
                            //    string msg2 = string.Format("{0}: {1} %", new object[] { event_name, (int)o });
                            //    System.Windows.MessageBox.Show(msg2);
                            //    //_cts.Cancel();
                            //    break;
                        }
                    });
                    result = null;
                    parameters = null;
                    _cts = null;

                    //Load the downloaded .tifs into the Mosic
                    System.Threading.CancellationTokenSource _cts2 = new System.Threading.CancellationTokenSource();
                    tool_path = "management.AddRastersToMosaicDataset";
                    parameters = Geoprocessing.MakeValueArray(inpath + "\\" + in_mosaicdataset_name, "Raster Dataset", String.Join(";",tiffs), "UPDATE_CELL_SIZES", "UPDATE_BOUNDARY","UPDATE_OVERVIEWS","-1");
                    //parameters = Geoprocessing.MakeValueArray(inpath + "\\" + in_mosaicdataset_name, "Raster Dataset", @"D:\Planet\global_monthly_2019_02_mosaic982-1377.tif;D:\Planet\global_monthly_2019_02_mosaic983-1377.tif");
                    try
                    {
                        
                        IGPResult gPResult = await Geoprocessing.ExecuteToolAsync(tool_path, parameters, null, _cts2.Token, (event_name, o) =>  // implement delegate and handle events, o is message object.
                        {
                            switch (event_name)
                            {
                                case "OnValidate": // stop execute if any warnings or errors
                                    if ((o as IGPMessage[]).Any(it => it.Type == GPMessageType.Warning || it.Type == GPMessageType.Error))
                                    {
                                        System.Windows.MessageBox.Show("Failed to add .tif files" + Environment.NewLine + o.ToString() + Environment.NewLine+ "The files have been downloaded but not added to The raster Mosic","Failed to add .tif files");
                                        _cts2.Cancel();
                                    }
                                    break;
                                case "OnProgressMessage":
                                    //string msg = string.Format("{0}: {1}", new object[] { event_name, (string)o });
                                    //System.Windows.MessageBox.Show(msg);
                                    //_cts.Cancel();
                                    break;
                                case "OnProgressPos":
                                    //string msg2 = string.Format("{0}: {1} %", new object[] { event_name, (int)o });
                                    //System.Windows.MessageBox.Show(msg2);
                                    //_cts.Cancel();
                                    break;
                            }
                        });
                        Geoprocessing.ShowMessageBox(gPResult.Messages, "GP Messages", gPResult.IsFailed ? GPMessageBoxStyle.Error : GPMessageBoxStyle.Default);
                        //Geoprocessing.OpenToolDialog(tool_path, parameters);
                    }
                    catch (Exception  ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message.ToString());
                        _cts2.Cancel();
                    }


                    //Everything downloaded and added to the mosic
                    FrameworkApplication.AddNotification(new Notification()
                    {
                        Title = "Download Successful",
                        Message = "Successfully dowloaded the requesed quads",
                        ImageUrl = @"pack://application:,,,/Planet;component/Images/Planet_logo-dark.png"
                        
                    });
                }
            }
        }


        /// <summary>
        /// Does the actual download from the PLanet Servers
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
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
    }
}
