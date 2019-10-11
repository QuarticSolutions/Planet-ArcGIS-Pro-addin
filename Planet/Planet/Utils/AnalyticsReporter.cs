using Segment;
using Segment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planet.Utils
{
    public class AnalyticsReporter
    {
        public void MakeReport(string operation, Traits keyValuePairs)
        {
            if (!string.IsNullOrEmpty(Module1.Current.API_KEY.EMAIL_Value) && !string.IsNullOrEmpty(Module1.Current.API_KEY.API_KEY_Value))
            {
                if (Analytics.Client == null)
                {
                    Analytics.Initialize("at3uKKI8tvtIzsvXU4MpmxKBWSfnUPwR");
                }
                Analytics.Client.Track(Module1.Current.API_KEY.EMAIL_Value, operation, keyValuePairs);
            }
        }
    
    }
}
