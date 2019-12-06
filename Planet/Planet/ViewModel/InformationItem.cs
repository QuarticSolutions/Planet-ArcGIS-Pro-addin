using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Planet.ViewModel
{
    public class InformationItem : INotifyPropertyChanged
    {
        private bool CanExecuteAttachmentChecked()
        {
            return true;
        }
        private ICommand _hyperlink_Click;
        public ICommand Hyperlink_Click
        {
            get
            {
                return _hyperlink_Click ?? (_hyperlink_Click = new CommandHandlerParam(param => HyperlinkClick(param), CanExecuteAttachmentChecked()));
            }
        }
        private void HyperlinkClick(object sender)
        {
            string tabItem = (sender as TextBlock).Name as string;
            switch (tabItem)
            {
                case "Documentation":
                    System.Diagnostics.Process.Start("https://developers.planet.com/integrations/");
                    break;
                case "Spec":
                    System.Diagnostics.Process.Start("https://assets.planet.com/docs/Planet_Combined_Imagery_Product_Specs_letter_screen.pdf");
                    break;
                case "Home":
                    System.Diagnostics.Process.Start("https://planet.com");
                    break;
                case "Support":
                    System.Diagnostics.Process.Start("https://support.planet.com/hc/en-us");
                    break;
                case "Disclaimer":
                    System.Diagnostics.Process.Start("https://learn.planet.com/plug-in-tool-supplemental-license-arcgis.html");
                    break;
                default:
                    break;
            }

        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
