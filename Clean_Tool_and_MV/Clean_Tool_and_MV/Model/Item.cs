using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test_docing_Panel.Models;

namespace Clean_Tool_and_MV.Model
{
    class Item
    {
       public List<Strip> strips { get; set; }
       public string itemType { get; set; }
       public searchGeometry geometry { get; set; }
       public string thumbnail { get; set; }
       public DateTime acquired { get; set; }
    }
}
