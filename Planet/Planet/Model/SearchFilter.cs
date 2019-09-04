using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_docing_Panel.Models
{
    public class SearchFilter
    {
        public Filter filter { get; set; }
        public string[] item_types { get; set; }
    }

    public class Filter
    {
        public string type { get; set; }
        public Config[] config { get; set; }
    }

    public class Config
    {
        public string type { get; set; }
        public string field_name { get; set; }
        public object config { get; set; }
        public object[] coordinates { get; set; }
        public string gte { get; set; }
        public string lte { get; set; }
    }
    public class DateRangeFilterConfig
    {
        public string gte { get; set; }
        public string lte { get; set; }
    }
    public class RangeFilterConfig
    {
        public int gte { get; set; }
        public int lte { get; set; }
    }

    public class RangeFilter
    {
        public string type { get; set; }
        public string field_name { get; set; }
        public RangeFilterConfig config { get; set; }

    }

}
