using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Planet.Model.Item_assets
{
    class PSScene4Band : Asset , INotifyPropertyChanged
    {
        public Analytic analytic { get; set; }
        public Analytic_Dn analytic_dn { get; set; }
        public Analytic_Dn_Xml analytic_dn_xml { get; set; }
        public Analytic_Sr analytic_sr { get; set; }
        public Analytic_Xml analytic_xml { get; set; }
        public Basic_Analytic basic_analytic { get; set; }
        public Basic_Analytic_Dn basic_analytic_dn { get; set; }
        public Basic_Analytic_Dn_Nitf basic_analytic_dn_nitf { get; set; }
        public Basic_Analytic_Dn_Rpc basic_analytic_dn_rpc { get; set; }
        public Basic_Analytic_Dn_Rpc_Nitf basic_analytic_dn_rpc_nitf { get; set; }
        public Basic_Analytic_Dn_Xml basic_analytic_dn_xml { get; set; }
        public Basic_Analytic_Dn_Xml_Nitf basic_analytic_dn_xml_nitf { get; set; }
        public Basic_Analytic_Nitf basic_analytic_nitf { get; set; }
        public Basic_Analytic_Rpc basic_analytic_rpc { get; set; }
        public Basic_Analytic_Rpc_Nitf basic_analytic_rpc_nitf { get; set; }
        public Basic_Analytic_Xml basic_analytic_xml { get; set; }
        public Basic_Analytic_Xml_Nitf basic_analytic_xml_nitf { get; set; }
        public Basic_Udm basic_udm { get; set; }
        public Basic_Udm2 basic_udm2 { get; set; }
        public Udm udm { get; set; }
        public Udm2 udm2 { get; set; }
        private string _selectedBundle;
        public string selectedBundle
        {
            get { return _selectedBundle; }

            set
            {
                _selectedBundle = value;
                OnPropertyChanged("selectedBundle");

            }
        }
    
        private bool _basic = false;
        public bool oBasic {
            get {return _basic; }

            set { _basic = value;
                OnPropertyChanged("oBasic");

            } }
        private bool _Visual = false;
        public bool oVisual
        {
            get { return _Visual; }

            set
            {
                _Visual = value;
                OnPropertyChanged("oVisual");

            }
        }
        private bool _analytic = false;
        public bool oAnalytic
        {
            get { return _analytic; }

            set
            {
                _analytic = value;
                OnPropertyChanged("oAnalytic");

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Analytic
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public DateTime expires_at { get; set; }
        public string location { get; set; }
        public string md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }

    public class _Links
    {
        public string _self { get; set; }
        public string activate { get; set; }
        public string type { get; set; }
    }

    public class Analytic_Dn
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }

    public class Analytic_Dn_Xml
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }



    public class Analytic_Sr
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public DateTime expires_at { get; set; }
        public string location { get; set; }
        public string md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }



    public class Analytic_Xml
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public DateTime expires_at { get; set; }
        public string location { get; set; }
        public string md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }



    public class Basic_Analytic
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }



    public class Basic_Analytic_Dn
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }

    public class Basic_Analytic_Dn_Nitf
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }


    public class Basic_Analytic_Dn_Rpc
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }


    public class Basic_Analytic_Dn_Rpc_Nitf
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }

    public class Basic_Analytic_Dn_Xml
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }



    public class Basic_Analytic_Dn_Xml_Nitf
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }



    public class Basic_Analytic_Nitf
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }


    public class Basic_Analytic_Rpc
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }

    public class Basic_Analytic_Rpc_Nitf
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }


    public class Basic_Analytic_Xml
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }

    public class Basic_Analytic_Xml_Nitf
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }

    public class Basic_Udm
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }


    public class Basic_Udm2
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public object md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }


    public class Udm
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public DateTime expires_at { get; set; }
        public string location { get; set; }
        public string md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }


    public class Udm2
    {
        public _Links _links { get; set; }
        public string[] _permissions { get; set; }
        public DateTime expires_at { get; set; }
        public string location { get; set; }
        public string md5_digest { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }


}
