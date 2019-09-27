using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planet.Model.Item_assets
{
    public class tempBundles
    {
        public Dictionary<string, string> PlanetScope4 { get; } = new Dictionary<string, string>()
        {
            { "analytic_sr_udm2,analytic_sr_udm" ,"Analytic Surface Reflectance with UDM2" },
            { "basic_analytic_udm2,basic_analytic","Non-Orthorectified Analytic Radiance with UDM2" }
        };
        public Dictionary<string, string> PlanetScope3 { get; } = new Dictionary<string, string>()
        {
            { "analytic_sr_udm2" ,"Analytic Surface Reflectance with UDM2" },
            { "basic_analytic_udm2","Non-Orthorectified Analytic Radiance with UDM2" }
        };
        public Dictionary<string, string> SkySat { get; } = new Dictionary<string, string>()
        {
            { "pansharpened_udm2,pansharpened" ,"Pansharpened with UDM2" },
            { "analytic_udm2,analytic","Analytic Radiance with UDM2" },
            { "basic_analytic_udm2,basic_analytic" ,"Non-Orthorectified Analytic Radiance with UDM2" },
            { "panchromatic_dn_udm2,panchromatic_dn","Panchromatic DN with UDM2" }
        };
    }

}
