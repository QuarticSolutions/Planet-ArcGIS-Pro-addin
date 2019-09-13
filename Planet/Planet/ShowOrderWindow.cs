using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;

namespace Planet
{
    internal class ShowOrderWindow : Button
    {

        private OrderWindow _orderwindow = null;

        protected override void OnClick()
        {
            //already open?
            if (_orderwindow != null)
                return;
            _orderwindow = new OrderWindow();
            _orderwindow.Owner = FrameworkApplication.Current.MainWindow;
            _orderwindow.Closed += (o, e) => { _orderwindow = null; };
            _orderwindow.Show();
            //uncomment for modal
            //_orderwindow.ShowDialog();
        }

    }
}
