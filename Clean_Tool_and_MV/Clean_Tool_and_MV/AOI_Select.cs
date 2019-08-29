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
        private IDisposable _graphic = null;
        private CIMPolygonSymbol _polygonSymbol = SymbolFactory.Instance.ConstructPolygonSymbol(ColorFactory.Instance.BlackRGB, SimpleFillStyle.Null, SymbolFactory.Instance.ConstructStroke(ColorFactory.Instance.BlackRGB, 1.0, SimpleLineStyle.Solid));
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
            if (_graphic != null)
            {
                _graphic.Dispose();
                _graphic = null;
            }
            
            addgraphic(geometry);
            DockPane pane = FrameworkApplication.DockPaneManager.Find("Clean_Tool_and_MV_Data_DocPane");
            Data_DocPaneViewModel data_DocPaneViewModel = (Data_DocPaneViewModel)pane;
            data_DocPaneViewModel.AOIGeometry = geometry;
            data_DocPaneViewModel.Activate(true);
            //addwmts("asdasd");

            return base.OnSketchCompleteAsync(geometry);
        }

        private async void addwmts(string url)
        {
            if (MapView.Active == null)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("A map must be added the the project and be active");
                //FrameworkApplication.State.Deactivate("planet_state_connection");
                
            }
            Project project = Project.Current;
            var serverConnection = new CIMProjectServerConnection { URL = "https://tiles.planet.com/data/v1/layers/wmts/-7zefdf-UmCPwvBPaBJ0EmLRwx2M33-YL0Jdww?api_key=1fe575980e78467f9c28b552294ea410" };// "1fe575980e78467f9c28b552294ea410"
            var connection = new CIMWMTSServiceConnection { ServerConnection = serverConnection };
            await QueuedTask.Run(() =>
            {
                BasicRasterLayer layer2 = LayerFactory.Instance.CreateRasterLayer(connection, MapView.Active.Map, 0, "test");
            });
        }
        private async void addgraphic(Geometry geometry)
        {
            //_graphic =  this.AddOverlayAsync(geometry, _polygonSymbol.MakeSymbolReference());
            await QueuedTask.Run(() =>
            {
                _graphic = MapView.Active.AddOverlay(geometry, _polygonSymbol.MakeSymbolReference());
            });
        }
    }
}
