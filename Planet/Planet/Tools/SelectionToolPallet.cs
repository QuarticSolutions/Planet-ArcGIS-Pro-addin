using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Events;
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
using ArcGIS.Desktop.Mapping.Events;

namespace Planet.Tools
{

    internal class SelectionToolPallet_button1 : Tool
    {
        protected override void OnClick()
        {
            // ArcGIS Pro Command's DAML ID. 
            var commandId = "esri_mapping_selectByRectangleTool";
            // get the ICommand interface from the ArcGIS Pro Button
            // using command's plug-in wrapper
            var iCommand = FrameworkApplication.GetPlugInWrapper(commandId) as ICommand;
            if (iCommand != null)
            {
                // Let ArcGIS Pro do the work for us
                if (iCommand.CanExecute(null))
                {
                    iCommand.Execute(null);
                    MessageBox.Show("Here is my own optional add-on functionality");
                }
            }
        }
    }

    internal class SelectionToolPallet_button2 : Tool
    {
        protected override void OnClick()
        {
            // ArcGIS Pro Command's DAML ID. 
            var commandId = "esri_mapping_selectByPolygonTool";
            // get the ICommand interface from the ArcGIS Pro Button
            // using command's plug-in wrapper
            var iCommand = FrameworkApplication.GetPlugInWrapper(commandId) as ICommand;
            if (iCommand != null)
            {
                // Let ArcGIS Pro do the work for us
                if (iCommand.CanExecute(null))
                {
                    iCommand.Execute(null);
                    MessageBox.Show("Here is my own optional add-on functionality");
                }
            }
        }
    }

    internal class SelectionToolPallet_button3 : Tool
    {
        private SubscriptionToken _eventToken = null;
        public SelectionToolPallet_button3()
        {
            if (_eventToken == null) //Subscribe to the selection change event which will fire then the tool has finished executing
            {
                _eventToken = MapSelectionChangedEvent.Subscribe(handleSelectionListeners.MapSelectionChangedEventfromclass);
            }
        }



        protected override void OnClick()
        {
            // ArcGIS Pro Command's DAML ID. 
            var commandId = "esri_mapping_selectByLassoTool";
            // get the ICommand interface from the ArcGIS Pro Button
            // using command's plug-in wrapper
            ICommand iCommand = FrameworkApplication.GetPlugInWrapper(commandId) as ICommand;
            if (iCommand != null)
            {
                // Let ArcGIS Pro do the work for us
                if (iCommand.CanExecute(null))
                {
                    iCommand.Execute(null);

                    //MessageBox.Show("Here is my own optional add-on functionality");
                }
            }
        }
    }

    internal class handleSelectionListeners
    {
        public static async  void MapSelectionChangedEventfromclass( MapSelectionChangedEventArgs obj)
        {
            try
            {
                var selection = obj.Selection;
                var keyValuePairs = selection.Where(kvp => (kvp.Key is BasicFeatureLayer));
                Geometry geometry = null;
                geometry = await QueuedTask.Run<Geometry>(() =>
                {
                    try
                    {
                        //Get the active map view.
                        Geometry geometry2 = null;
                        //var mapView = MapView.Active;
                        //if (mapView == null)
                        //    return geometry2;

                        ////Get the selected features from the map 
                        //var selectedFeatures = mapView.Map.GetSelection()
                        //   .Where(kvp => kvp.Key is BasicFeatureLayer)
                        //   .ToDictionary(kvp => (BasicFeatureLayer)kvp.Key, kvp => kvp.Value);
                        //foreach (var item in selectedFeatures)
                        //{
                        //    Console.WriteLine(item.Key);
                        //}
                        //var rowCursor = mapView.Map.GetSelection();
                        using (PolygonBuilder pb = new PolygonBuilder())
                        {
                            foreach (var kvp in keyValuePairs)
                            {
                                var layer = kvp.Key as BasicFeatureLayer;
                                if (layer.ShapeType == esriGeometryType.esriGeometryPolygon)
                                {
                                    foreach (long item in kvp.Value)
                                    {
                                        Polygon polygone = null;
                                        var oid = item;
                                        var oidField = layer.GetTable().GetDefinition().GetObjectIDField();
                                        var qf = new ArcGIS.Core.Data.QueryFilter() { WhereClause = string.Format("{0} = {1}", oidField, oid) };
                                        var cursor = layer.Search(qf);
                                        Feature row = null;
                                        if (cursor.MoveNext())
                                            row = cursor.Current as Feature;

                                        if (row == null)
                                            continue;
                                        polygone = (Polygon)row.GetShape();
                                        pb.AddParts(polygone.Parts);
                                        pb.SpatialReference = polygone.SpatialReference;
                                    }
                                }
                                else if (layer.ShapeType == esriGeometryType.esriGeometryPoint)
                                {

                                }

                            }
                            if ((pb.ToGeometry()).PointCount > 500)
                            {
                                MessageBox.Show("Too many vertices. Please simplify the selected Polygon or choose another one. Max vertices: 500");
                                return geometry2;
                            }

                            geometry2 = pb.ToGeometry();
                            return geometry2;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("There was an error getting the geometry of the selected shape. Please try your selection again." + Environment.NewLine + "Error: " + ex.Message);
                        Console.WriteLine("Error getting select shape geom");
                        return null;
                    }
                });

                DockPane pane = FrameworkApplication.DockPaneManager.Find("Planet_Data_DocPane");
                Data_DocPaneViewModel data_DocPaneViewModel = (Data_DocPaneViewModel)pane;
                data_DocPaneViewModel.AOIGeometry = geometry;
                pane.IsVisible = true;
                data_DocPaneViewModel.Activate(true);

            }
            catch (Exception exc)
            {
                MessageBox.Show("There was an error getting the geometry of the selected shape. Please try your selection again." + Environment.NewLine + "Error: " + exc.Message);
            }
            
        }
    }


}
