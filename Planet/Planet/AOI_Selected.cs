using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Planet
{

    internal class AOI_Selected : Button
    {
        private SubscriptionToken _eventToken = null;


        protected override async void OnClick()
        {
            Polygon polygone = null;
            await QueuedTask.Run(() =>
           {
               try
               {
                   //Get the active map view.
                   var mapView = MapView.Active;
                   if (mapView == null)
                       return;

                   //Get the selected features from the map and filter out the standalone table selection.
                   var selectedFeatures = mapView.Map.GetSelection()
                      .Where(kvp => kvp.Key is BasicFeatureLayer)
                      .ToDictionary(kvp => (BasicFeatureLayer)kvp.Key, kvp => kvp.Value);
                   foreach (var item in selectedFeatures)
                   {
                       Console.WriteLine(item.Key);
                   }
                   var rowCursor = mapView.Map.GetSelection();

                   var selection = mapView.Map.GetSelection();
                   var keyValuePairs = selection.Where(kvp => (kvp.Key is BasicFeatureLayer)
                     && (kvp.Key as BasicFeatureLayer).ShapeType == esriGeometryType.esriGeometryPolygon);
                   foreach (var kvp in keyValuePairs)
                   {
                       var layer = kvp.Key as BasicFeatureLayer;
                       if (kvp.Value.Count > 1)
                       {
                           var oid = kvp.Value.First();
                           var oidField = layer.GetTable().GetDefinition().GetObjectIDField();
                           var qf = new ArcGIS.Core.Data.QueryFilter() { WhereClause = string.Format("{0} = {1}", oidField, oid) };
                           var cursor = layer.Search(qf);
                           Feature row = null;

                           if (cursor.MoveNext())
                               row = cursor.Current as Feature;

                           if (row == null)
                               continue;
                           polygone = (Polygon)row.GetShape();
                       }
                       else
                       {
                           var oid = kvp.Value.First();
                           var oidField = layer.GetTable().GetDefinition().GetObjectIDField();
                           var qf = new ArcGIS.Core.Data.QueryFilter() { WhereClause = string.Format("{0} = {1}", oidField, oid) };
                           var cursor = layer.Search(qf);
                           Feature row = null;

                           if (cursor.MoveNext())
                               row = cursor.Current as Feature;

                           if (row == null)
                               continue;
                           polygone = (Polygon)row.GetShape();
                       }
                   }
               }
               catch (Exception ex)
               {
                   MessageBox.Show("There was an error getting the geometry of the selected shape. Please try your selection again." + Environment.NewLine + "Error: " + ex.Message);
                   Console.WriteLine("Error getting select shape geom");
                   return;
               }
           });


            if (polygone.PointCount > 500)
            {
                MessageBox.Show("Too many vertices. Please simplify the selected Polygon or choose another one. Max vertices: 500");
                return;
            }

            DockPane pane = FrameworkApplication.DockPaneManager.Find("Planet_Data_DocPane");
            Data_DocPaneViewModel data_DocPaneViewModel = (Data_DocPaneViewModel)pane;
            data_DocPaneViewModel.AOIGeometry = (Geometry)polygone;
            pane.IsVisible = true;
            data_DocPaneViewModel.Activate(true);
        }

        public AOI_Selected()
        {
            if ( _eventToken == null) //Subscribe to event when dockpane is visible
            {
                _eventToken = MapSelectionChangedEvent.Subscribe(OnMapSelectionChangedEvent);
            }
        }

        private void OnMapSelectionChangedEvent(MapSelectionChangedEventArgs obj)
        {
            var selection = obj.Selection;
            var keyValuePairs = selection.Where(kvp => (kvp.Key is BasicFeatureLayer)
              && (kvp.Key as BasicFeatureLayer).ShapeType == esriGeometryType.esriGeometryPolygon);
            if (keyValuePairs.Count() > 0)
            {
                FrameworkApplication.State.Activate("planet_state_ispolyselected");
            }
            else
            {
                FrameworkApplication.State.Deactivate("planet_state_ispolyselected");
            }
        }


    }
}
