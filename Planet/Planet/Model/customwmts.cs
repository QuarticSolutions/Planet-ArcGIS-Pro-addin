using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planet.Model
{

    public class customwmts
    {
        public float[] bounds { get; set; }
        public string[] items { get; set; }
        public string name { get; set; }
        public string scheme { get; set; }
        public string tilejson { get; set; }
        public string[] tiles { get; set; }
        public string version { get; set; }
        public Uri wmtsURL { get; set; }
    }

}