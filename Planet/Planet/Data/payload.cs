using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planet.Data
{
    public class Payload
    {
            public string program_id { get; set; }
            public string token_type { get; set; }
            public string role_level { get; set; }
            public string organization_id { get; set; }
            public string user_id { get; set; }
            public string plan_template_id { get; set; }
            public string membership_id { get; set; }
            public string organization_name { get; set; }
            public string _2fa { get; set; }
            public string exp { get; set; }
            public string api_key { get; set; }
            public string user_name { get; set; }
            public string email { get; set; }
    }
}
