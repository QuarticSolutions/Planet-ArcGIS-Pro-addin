using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Planet
{
    /// <summary>
    /// Interaction logic for TrialWarning.xaml
    /// </summary>
    public partial class TrialWarning : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        public TrialWarning()
        {
            InitializeComponent();
        }

        private void Purchase_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://go.planet.com/basemaps-stripe-esri");
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
