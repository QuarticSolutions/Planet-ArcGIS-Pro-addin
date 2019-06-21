using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;

namespace Planet
{
    internal class ShowTrialWarning : Button
    {

        private TrialWarning _trialwarning = null;

        protected override void OnClick()
        {
            //already open?
            if (_trialwarning != null)
                return;
            _trialwarning = new TrialWarning();
            _trialwarning.Owner = FrameworkApplication.Current.MainWindow;
            _trialwarning.Closed += (o, e) => { _trialwarning = null; };
            _trialwarning.Show();
            //uncomment for modal
            //_trialwarning.ShowDialog();
        }

    }
}
