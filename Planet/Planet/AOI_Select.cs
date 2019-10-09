using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using MessageBox = ArcGIS.Desktop.Framework.Dialogs.MessageBox;


namespace Planet
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

            //APIKeyChangedEvent.Subscribe((args) =>
            //{
            //    if (Module1.Current.API_KEY == null || Module1.Current.API_KEY.API_KEY_Value == null || Module1.Current.API_KEY.API_KEY_Value == "")
            //    {
            //        if (_graphic != null)
            //        {
            //            _graphic.Dispose();
            //            FrameworkApplication.SetCurrentToolAsync(null);
            //        }
            //    }
            //});
        }

        /// <summary>
        /// Called when the sketch finishes. This is where we will create the sketch operation and then execute it.
        /// </summary>
        /// <param name="geometry">The geometry created by the sketch.</param>
        /// <returns>A Task returning a Boolean indicating if the sketch complete event was successfully handled.</returns>
        protected override Task<bool> OnSketchCompleteAsync(Geometry geometry)
        {
            if (geometry.GeometryType == GeometryType.Polygon)
            {
                Polygon polygon = (Polygon)geometry;
                if (polygon.PointCount > 500)
                {
                    MessageBox.Show("Too Many Vertices. Please simplify your AOI");
                    return base.OnSketchCompleteAsync(geometry);
                }
            }
            if (_graphic != null)
            {
                _graphic.Dispose();
                _graphic = null;
            }

            addgraphic(geometry);
            DockPane pane = FrameworkApplication.DockPaneManager.Find("Planet_Data_DocPane");
            Data_DocPaneViewModel data_DocPaneViewModel = (Data_DocPaneViewModel)pane;
            data_DocPaneViewModel.AOIGeometry = geometry;
            data_DocPaneViewModel.Activate(true);
            //addwmts("asdasd");
            FrameworkApplication.SetCurrentToolAsync(null);
            return base.OnSketchCompleteAsync(geometry);
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
