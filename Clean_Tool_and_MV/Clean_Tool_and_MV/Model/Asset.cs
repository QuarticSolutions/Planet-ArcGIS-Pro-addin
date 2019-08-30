using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using test_docing_Panel.Models;

namespace Clean_Tool_and_MV.Model
{
    class Asset : Feature
    {
        public Strip parent { get; set; }
        public string image
        {
            get
            {
                string url = _links.thumbnail + "?api_key=" + "1fe575980e78467f9c28b552294ea410";
                return url;
            }
        }
        public string title
        {
            get
            {
                return properties.acquired.ToLongTimeString();
            }
        }
    }
}
