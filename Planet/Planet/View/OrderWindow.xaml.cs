using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        public OrderWindow()
        {
            InitializeComponent();
        }

        private void TxtOrderName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool result = true;
            Regex regex = new Regex(@"[^a-zA-Z0-9_-]+");
            if (!regex.IsMatch(e.Text))
            {
                result = false;
                
            }
            e.Handled = result;
        }
    }
}
