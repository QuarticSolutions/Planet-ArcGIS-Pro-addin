using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planet.Model.Item_assets
{

    public class Bundles2
    {
        public List<BundelItem> bundles = new List<BundelItem>()
        {
             new BundelItem()
                { BundleName = "analytic",bundleassets= new List<string>(){
                    "Landsat8L1G",
                    "PSOrthoTile",
                    "PSScene3Band",
                    "PSScene4Band",
                    "REOrthoTile",
                    "REScene",
                    "Sentinel1",
                    "Sentinel2L1C",
                    "SkySatScene"
                }
             },
                new BundelItem()
                { BundleName = "all",bundleassets= new List<string>(){
                    "Landsat8L1G",
                    "MOD09GA",
                    "MOD09GQ",
                    "MYD09GA",
                    "MYD09GQ",
                    "PSOrthoTile",
                    "PSScene3Band",
                    "PSScene4Band",
                    "REOrthoTile",
                    "REScene",
                    "Sentinel1",
                    "Sentinel2L1C",
                    "SkySatCollect",
                    "SkySatScene"
                }
                },
                new BundelItem()
                { BundleName = "all_udm2",bundleassets= new List<string>(){
                    "PSOrthoTile",
                    "PSScene4Band" } },
                new BundelItem()
                { BundleName = "analytic_udm2",bundleassets= new List<string>(){
                    "PSOrthoTile",
                    "PSScene4Band"
                }
                },
                new BundelItem()
                { BundleName = "visual",bundleassets= new List<string>(){
                    "Landsat8L1G",
                    "PSOrthoTile",
                    "PSScene3Band",
                    "REOrthoTile",
                    "Sentinel2L1C",
                    "SkySatCollect",
                    "SkySatScene"
                }
                },
                new BundelItem()
                { BundleName = "uncalibrated_dn",bundleassets= new List<string>(){
                    "PSOrthoTile",
                    "PSScene3Band",
                    "PSScene4Band",
                    "SkySatCollect",
                    "SkySatScene"
                }
                },
                new BundelItem()
                { BundleName = "uncalibrated_dn_udm2",bundleassets= new List<string>(){
                    "PSOrthoTile",
                    "PSScene4Band" } },
                new BundelItem()
                { BundleName = "basic_analytic",bundleassets= new List<string>(){
                    "REScene",
                    "PSScene3Band",
                    "PSScene4Band" } },
                 new BundelItem()
                { BundleName = "basic_analytic_udm2",bundleassets= new List<string>(){
                    "PSScene4Band" } },
                new BundelItem()
                { BundleName = "basic_analytic_dn",bundleassets= new List<string>(){
                    "SkySatScene",
                    "PSScene3Band",
                    "PSScene4Band" } },
                new BundelItem()
                {
                    BundleName = "basic_uncalibrated_dn",bundleassets= new List<string>()
                    {
                        "SkySatScene",
                        "PSScene3Band",
                        "PSScene4Band"
                    }
                },
                new BundelItem()
                {
                    BundleName = "basic_uncalibrated_dn_udm2",bundleassets= new List<string>()
                    {
                        "PSScene4Band"
                    }
                },
                new BundelItem()
                {
                    BundleName = "analytic_sr",bundleassets= new List<string>()
                    {
                        "PSScene4Band"
                    }
                },
                new BundelItem()
                {
                    BundleName = "analytic_sr_udm2",bundleassets= new List<string>()
                    {
                        "PSScene4Band"
                    }
                },
                new BundelItem()
                {
                    BundleName = "basic_uncalibrated_dn_nitf",bundleassets= new List<string>()
                    {
                        "PSScene4Band"
                    }
                },
                new BundelItem()
                {
                    BundleName = "basic_uncalibrated_dn_nitf_udm2",bundleassets= new List<string>()
                    {
                        "PSScene4Band"
                    }
                },
                new BundelItem()
                {
                    BundleName = "basic_analytic_nitf",bundleassets= new List<string>()
                    {
                        "PSScene4Band",
                        "REScene"
                    }
                },
                new BundelItem()
                {
                    BundleName = "basic_analytic_nitf_udm2",bundleassets= new List<string>()
                    {
                        "PSScene4Band"
                    }
                },
                new BundelItem()
                {
                    BundleName = "basic_panchromatic_dn",bundleassets= new List<string>()
                    {
                        "SkySatScene"
                    }
                },
                new BundelItem()
                {
                    BundleName = "ortho_analytic_dn",bundleassets= new List<string>()
                    {
                        "SkySatScene"
                    }
                },
                new BundelItem()
                {
                    BundleName = "ortho_pansharpened",bundleassets= new List<string>()
                    {
                        "SkySatScene"
                    }
                },
                new BundelItem()
                {
                    BundleName = "ortho_visual",bundleassets= new List<string>()
                    {
                        "SkySatScene"
                    }
                },
                new BundelItem()
                {
                    BundleName = "ortho_panchromatic_dn",bundleassets= new List<string>()
                    {
                        "SkySatScene"
                    }
                },
                new BundelItem()
                {
                    BundleName = "panchromatic_dn",bundleassets= new List<string>()
                    {
                        "SkySatCollect",
                        "SkySatScene"
                    }
                },
                new BundelItem()
                {
                    BundleName = "pansharpened",bundleassets= new List<string>()
                    {
                        "SkySatCollect",
                        "SkySatScene"
                    }
                },
                new BundelItem()
                {
                    BundleName = "analytic_without_metadata",bundleassets= new List<string>()
                    {
                        "Landsat8L1G",
                        "PSOrthoTile",
                        "PSScene3Band",
                        "PSScene4Band",
                        "REOrthoTile",
                        "REScene",
                        "Sentinel1",
                        "Sentinel2L1C",
                        "SkySatScene"
                    }
                },
                new BundelItem()
                {
                    BundleName = "basic_analytic_without_metadata",bundleassets= new List<string>()
                    {
                        "REScene",
                        "PSScene3Band",
                        "PSScene4Band"
                    }
                },
                new BundelItem()
                {
                    BundleName = "visual_without_metadata",bundleassets= new List<string>()
                    {
                        "Landsat8L1G",
                        "PSOrthoTile",
                        "PSScene3Band",
                        "REOrthoTile",
                        "Sentinel2L1C",
                        "SkySatCollect",
                        "SkySatScene"
                    }
                }
        };


        public class BundelItem
            {
            public string BundleName { get; set; }
            public List<string> bundleassets { get; set; }
            }

        public class Assets4
        {
            public string[] Landsat8L1G { get; set; }
            public string[] PSOrthoTile { get; set; }
            public string[] PSScene3Band { get; set; }
            public string[] REOrthoTile { get; set; }
            public string[] Sentinel2L1C { get; set; }
            public string[] SkySatCollect { get; set; }
            public string[] SkySatScene { get; set; }
        }
        public static List<string> All_Udm2 = new List<string>()
        {
            "PSOrthoTile",
            "PSScene4Band"
        };

        public List<string> OrthoVisual = new List<string>()
        {
            "SkySatScene"
        };
        public static List<string> all = new List<string>()
        {
            "Landsat8L1G",
            "MOD09GA",
            "MOD09GQ",
            "MYD09GA",
            "MYD09GQ",
            "PSOrthoTile",
            "PSScene3Band",
            "PSScene4Band",
            "REOrthoTile",
            "REScene",
            "Sentinel1",
            "Sentinel2L1C",
            "SkySatCollect",
            "SkySatScene"
        };
        public static List<string> AnalyticList = new List<string>()
        {

            "Landsat8L1G",
            "PSOrthoTile",
            "PSScene3Band",
            "PSScene4Band",
            "REOrthoTile",
            "REScene",
            "Sentinel1",
            "Sentinel2L1C",
            "SkySatScene"
        };

        public static List<string> analytic_udm2 = new List<string>()
        {
            "PSOrthoTile",
            "PSScene4Band"
        };
    }
    public class BundlesClass
    {
        public class Enumerator
        {
            public Bundles Current { get; private set; }
            public bool MoveNext()
            {
                if (this.Current == null)
                {
                    this.Current = new Bundles();
                    return true;
                }
                this.Current = null;
                return false;
            }
            public void Reset() { this.Current = null; }
        }
        public Enumerator GetEnumerator() { return new Enumerator(); }
        public Bundles bundles { get; set; }
        //public Deprecated deprecated { get; set; }
        //public string version { get; set; }
    }

    public class Bundles
    {
        public BAnalytic analytic { get; set; }
        public Analytic_Udm2 analytic_udm2 { get; set; }
        //public Analytic_Dn analytic_dn { get; set; }
        //public Analytic_Nitf analytic_nitf { get; set; }
        public Visual visual { get; set; }
        public Uncalibrated_Dn uncalibrated_dn { get; set; }
        public Uncalibrated_Dn_Udm2 uncalibrated_dn_udm2 { get; set; }
        public BBasic_Analytic basic_analytic { get; set; }
        public Basic_Analytic_Udm2 basic_analytic_udm2 { get; set; }
        //public BBasic_Analytic_Dn basic_analytic_dn { get; set; }
        //public BBasic_Analytic_Dn_Nitf basic_analytic_dn_nitf { get; set; }
        public Basic_Uncalibrated_Dn basic_uncalibrated_dn { get; set; }
        public Basic_Uncalibrated_Dn_Udm2 basic_uncalibrated_dn_udm2 { get; set; }
        public BAnalytic_Sr analytic_sr { get; set; }
        public Analytic_Sr_Udm2 analytic_sr_udm2 { get; set; }
        public Basic_Uncalibrated_Dn_Nitf basic_uncalibrated_dn_nitf { get; set; }
        public Basic_Uncalibrated_Dn_Nitf_Udm2 basic_uncalibrated_dn_nitf_udm2 { get; set; }
        public BBasic_Analytic_Nitf basic_analytic_nitf { get; set; }
        public Basic_Analytic_Nitf_Udm2 basic_analytic_nitf_udm2 { get; set; }
        public Basic_Panchromatic_Dn basic_panchromatic_dn { get; set; }
        //public Ortho_Analytic_Dn ortho_analytic_dn { get; set; }
        //public Ortho_Panchromatic_Dn ortho_panchromatic_dn { get; set; }
        //public Ortho_Pansharpened ortho_pansharpened { get; set; }
        //public Ortho_Visual ortho_visual { get; set; }
        public Panchromatic_Dn panchromatic_dn { get; set; }
        public Pansharpened pansharpened { get; set; }
        public Analytic_Without_Metadata analytic_without_metadata { get; set; }
        public Basic_Analytic_Without_Metadata basic_analytic_without_metadata { get; set; }
        public Visual_Without_Metadata visual_without_metadata { get; set; }
        public All all { get; set; }
        public All_Udm2 all_udm2 { get; set; }
    }

    public class BAnalytic
    {
        public string name { get; set; }
        public string description { get; set; }
        public string[] assets { get; set; }
    }

    //private List<string> _AnalyticList ;
    public class AnalyticList
    {

        public string Landsat8L1G { get { return "Landsat8L1G"; } }
        public string PSOrthoTile { get; set; }
        public string PSScene3Band { get; set; }
        public string PSScene4Band { get; set; }
        public string REOrthoTile { get; set; }
        public string REScene { get; set; }
        public string Sentinel1 { get; set; }
        public string Sentinel2L1C { get; set; }
        public string SkySatScene { get; set; }
    }

    public class Analytic_Udm2
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets1 assets { get; set; }
    }

    public class Assets1
    {
        public string[] PSOrthoTile { get; set; }
        public string[] PSScene4Band { get; set; }
    }

    public class BAnalytic_Dn
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets2 assets { get; set; }
    }

    public class Assets2
    {
        public string[] PSOrthoTile { get; set; }
        public string[] PSScene3Band { get; set; }
        public string[] PSScene4Band { get; set; }
    }

    public class Analytic_Nitf
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets3 assets { get; set; }
    }

    public class Assets3
    {
        public string[] REScene { get; set; }
    }

    public class Visual
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets4 assets { get; set; }
    }

    public class Assets4
    {
        public string[] Landsat8L1G { get; set; }
        public string[] PSOrthoTile { get; set; }
        public string[] PSScene3Band { get; set; }
        public string[] REOrthoTile { get; set; }
        public string[] Sentinel2L1C { get; set; }
        public string[] SkySatCollect { get; set; }
        public string[] SkySatScene { get; set; }
    }

    public class Uncalibrated_Dn
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets5 assets { get; set; }
    }

    public class Assets5
    {
        public string[] PSOrthoTile { get; set; }
        public string[] PSScene3Band { get; set; }
        public string[] PSScene4Band { get; set; }
        public string[] SkySatCollect { get; set; }
        public string[] SkySatScene { get; set; }
    }

    public class Uncalibrated_Dn_Udm2
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets6 assets { get; set; }
    }

    public class Assets6
    {
        public string[] PSOrthoTile { get; set; }
        public string[] PSScene4Band { get; set; }
    }

    public class BBasic_Analytic
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets7 assets { get; set; }
    }

    public class Assets7
    {
        public string[] PSScene3Band { get; set; }
        public string[] PSScene4Band { get; set; }
        public string[] REScene { get; set; }
    }

    public class Basic_Analytic_Udm2
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets8 assets { get; set; }
    }

    public class Assets8
    {
        public string[] PSScene4Band { get; set; }
    }

    public class BBasic_Analytic_Dn
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets9 assets { get; set; }
    }

    public class Assets9
    {
        public string[] PSScene3Band { get; set; }
        public string[] PSScene4Band { get; set; }
        public string[] SkySatScene { get; set; }
    }

    public class BBasic_Analytic_Dn_Nitf
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets10 assets { get; set; }
    }

    public class Assets10
    {
        public string[] PSScene4Band { get; set; }
    }

    public class Basic_Uncalibrated_Dn
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets11 assets { get; set; }
    }

    public class Assets11
    {
        public string[] PSScene3Band { get; set; }
        public string[] PSScene4Band { get; set; }
        public string[] SkySatScene { get; set; }
    }

    public class Basic_Uncalibrated_Dn_Udm2
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets12 assets { get; set; }
    }

    public class Assets12
    {
        public string[] PSScene4Band { get; set; }
    }

    public class BAnalytic_Sr
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets13 assets { get; set; }
    }

    public class Assets13
    {
        public string[] PSScene4Band { get; set; }
        public string[] MOD09GQ { get; set; }
        public string[] MYD09GQ { get; set; }
        public string[] MOD09GA { get; set; }
        public string[] MYD09GA { get; set; }
    }

    public class Analytic_Sr_Udm2
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets14 assets { get; set; }
    }

    public class Assets14
    {
        public string[] PSScene4Band { get; set; }
    }

    public class Basic_Uncalibrated_Dn_Nitf
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets15 assets { get; set; }
    }

    public class Assets15
    {
        public string[] PSScene4Band { get; set; }
    }

    public class Basic_Uncalibrated_Dn_Nitf_Udm2
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets16 assets { get; set; }
    }

    public class Assets16
    {
        public string[] PSScene4Band { get; set; }
    }

    public class BBasic_Analytic_Nitf
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets17 assets { get; set; }
    }

    public class Assets17
    {
        public string[] PSScene4Band { get; set; }
        public string[] REScene { get; set; }
    }

    public class Basic_Analytic_Nitf_Udm2
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets18 assets { get; set; }
    }

    public class Assets18
    {
        public string[] PSScene4Band { get; set; }
    }

    public class Basic_Panchromatic_Dn
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets19 assets { get; set; }
    }

    public class Assets19
    {
        public string[] SkySatScene { get; set; }
    }

    public class Ortho_Analytic_Dn
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets20 assets { get; set; }
    }

    public class Assets20
    {
        public string[] SkySatScene { get; set; }
    }

    public class Ortho_Panchromatic_Dn
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets21 assets { get; set; }
    }

    public class Assets21
    {
        public string[] SkySatScene { get; set; }
    }

    public class Ortho_Pansharpened
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets22 assets { get; set; }
    }

    public class Assets22
    {
        public string[] SkySatScene { get; set; }
    }

    public class Ortho_Visual
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets23 assets { get; set; }
    }

    public class Assets23
    {
        public string[] SkySatScene { get; set; }
    }

    public class Panchromatic_Dn
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets24 assets { get; set; }
    }

    public class Assets24
    {
        public string[] SkySatCollect { get; set; }
        public string[] SkySatScene { get; set; }
    }

    public class Pansharpened
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets25 assets { get; set; }
    }

    public class Assets25
    {
        public string[] SkySatCollect { get; set; }
        public string[] SkySatScene { get; set; }
    }

    public class Analytic_Without_Metadata
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets26 assets { get; set; }
    }

    public class Assets26
    {
        public string[] Landsat8L1G { get; set; }
        public string[] PSOrthoTile { get; set; }
        public string[] PSScene3Band { get; set; }
        public string[] PSScene4Band { get; set; }
        public string[] REOrthoTile { get; set; }
        public string[] REScene { get; set; }
        public string[] Sentinel1 { get; set; }
        public string[] Sentinel2L1C { get; set; }
        public string[] SkySatScene { get; set; }
    }

    public class Basic_Analytic_Without_Metadata
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets27 assets { get; set; }
    }

    public class Assets27
    {
        public string[] PSScene3Band { get; set; }
        public string[] PSScene4Band { get; set; }
        public string[] REScene { get; set; }
    }

    public class Visual_Without_Metadata
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets28 assets { get; set; }
    }

    public class Assets28
    {
        public string[] Landsat8L1G { get; set; }
        public string[] PSOrthoTile { get; set; }
        public string[] PSScene3Band { get; set; }
        public string[] REOrthoTile { get; set; }
        public string[] Sentinel2L1C { get; set; }
        public string[] SkySatCollect { get; set; }
        public string[] SkySatScene { get; set; }
    }

    public class All
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets29 assets { get; set; }
    }

    public class Assets29
    {
        public string[] Landsat8L1G { get; set; }
        public string[] MOD09GA { get; set; }
        public string[] MOD09GQ { get; set; }
        public string[] MYD09GA { get; set; }
        public string[] MYD09GQ { get; set; }
        public string[] PSOrthoTile { get; set; }
        public string[] PSScene3Band { get; set; }
        public string[] PSScene4Band { get; set; }
        public string[] REOrthoTile { get; set; }
        public string[] REScene { get; set; }
        public string[] Sentinel1 { get; set; }
        public string[] Sentinel2L1C { get; set; }
        public string[] SkySatCollect { get; set; }
        public string[] SkySatScene { get; set; }
    }

    public class All_Udm2
    {
        public string name { get; set; }
        public string description { get; set; }
        public Assets30 assets { get; set; }
    }

    public class Assets30
    {
        public string[] PSOrthoTile { get; set; }
        public string[] PSScene4Band { get; set; }
    }

    public class Deprecated
    {
        public Analytic1 analytic { get; set; }
        public Analytic_Dn1 analytic_dn { get; set; }
        public Analytic_Nitf1 analytic_nitf { get; set; }
        public Basic_Analytic_Dn1 basic_analytic_dn { get; set; }
        public Basic_Analytic_Dn_Nitf1 basic_analytic_dn_nitf { get; set; }
        public Ortho_Analytic_Dn1 ortho_analytic_dn { get; set; }
        public Ortho_Panchromatic_Dn1 ortho_panchromatic_dn { get; set; }
        public Ortho_Pansharpened1 ortho_pansharpened { get; set; }
        public Ortho_Visual1 ortho_visual { get; set; }
    }

    public class Analytic1
    {
        public Assets31 assets { get; set; }
    }

    public class Assets31
    {
        public string[] REScene { get; set; }
    }

    public class Analytic_Dn1
    {
        public Assets32 assets { get; set; }
    }

    public class Assets32
    {
        public string[] PSOrthoTile { get; set; }
        public string[] PSScene3Band { get; set; }
        public string[] PSScene4Band { get; set; }
    }

    public class Analytic_Nitf1
    {
        public Assets33 assets { get; set; }
    }

    public class Assets33
    {
        public string[] REScene { get; set; }
    }

    public class Basic_Analytic_Dn1
    {
        public Assets34 assets { get; set; }
    }

    public class Assets34
    {
        public string[] PSScene3Band { get; set; }
        public string[] PSScene4Band { get; set; }
        public string[] SkySatScene { get; set; }
    }

    public class Basic_Analytic_Dn_Nitf1
    {
        public Assets35 assets { get; set; }
    }

    public class Assets35
    {
        public string[] PSScene4Band { get; set; }
    }

    public class Ortho_Analytic_Dn1
    {
        public Assets36 assets { get; set; }
    }

    public class Assets36
    {
        public string[] SkySatScene { get; set; }
    }

    public class Ortho_Panchromatic_Dn1
    {
        public Assets37 assets { get; set; }
    }

    public class Assets37
    {
        public string[] SkySatScene { get; set; }
    }

    public class Ortho_Pansharpened1
    {
        public Assets38 assets { get; set; }
    }

    public class Assets38
    {
        public string[] SkySatScene { get; set; }
    }

    public class Ortho_Visual1
    {
        public Assets39 assets { get; set; }
    }

    public class Assets39
    {
        public string[] SkySatScene { get; set; }
    }

}
