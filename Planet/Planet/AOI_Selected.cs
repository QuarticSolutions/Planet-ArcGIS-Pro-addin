using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Planet
{
    internal class AOI_Selected : Button
    {
        protected override void OnClick()
        {
            Polygon polygone = null;
             QueuedTask.Run(() =>
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
                DockPane pane = FrameworkApplication.DockPaneManager.Find("Planet_Data_DocPane");
                Data_DocPaneViewModel data_DocPaneViewModel = (Data_DocPaneViewModel)pane;
                data_DocPaneViewModel.AOIGeometry = (Geometry)polygone;
                data_DocPaneViewModel.Activate(true);
                //Flash the collection of features.
                mapView.FlashFeature(selectedFeatures);
            });
        }
    }
}
