using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Planet.View
{
    /// <summary>
    /// Interaction logic for Information_window.xaml
    /// </summary>
    public partial class Information_window : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        public Information_window()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            string tabItem = (sender as Hyperlink).Name as string;
            switch (tabItem)
            {
                case "Documentation":
                    System.Diagnostics.Process.Start("https://developers.planet.com/integrations/");
                    break;
                case "Spec":
                    System.Diagnostics.Process.Start("https://assets.planet.com/docs/Planet_Combined_Imagery_Product_Spec_Oct_2019.pdf");
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

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.planet.com");
        }
    }
}
