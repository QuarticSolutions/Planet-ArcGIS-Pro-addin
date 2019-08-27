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

namespace Clean_Tool_and_MV
{
    internal class AOI_Select : MapTool
    {
        public AOI_Select()
        {
            IsSketchTool = true;
            SketchType = SketchGeometryType.Polygon;
            SketchOutputMode = SketchOutputMode.Map;
        }

        protected override Task OnToolActivateAsync(bool active)
        {
            return base.OnToolActivateAsync(active);
        }

        protected override Task<bool> OnSketchCompleteAsync(Geometry geometry)
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find("Clean_Tool_and_MV_Data_DocPane");
            Data_DocPaneViewModel data_DocPaneViewModel = (Data_DocPaneViewModel)pane;
            data_DocPaneViewModel.AOIGeometry = geometry;
            data_DocPaneViewModel.Activate(true);
            return base.OnSketchCompleteAsync(geometry);
        }
    }
}
