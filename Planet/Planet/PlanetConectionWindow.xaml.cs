using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//using Windows.Web.Http;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Planet
{
    /// <summary>
    /// Interaction logic for PlanetConectionWindow.xaml
    /// </summary>
    public partial class PlanetConectionWindow : Window
    {
        public PlanetConectionWindow()
        {
            InitializeComponent();
            DataContext = new PlanetConnection();

        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                //var url = "https://api.planet.com/auth/v1/experimental/public/users/authenticate";
                //var paras = new Dictionary<string, string> { { "email", txtUserName.Text }, { "password", PassBox.Password } };
                //var user = (dynamic)new JObject();
                //user.email = txtUserName.Text;
                //user.password = PassBox.Password;
                //string user2 = @"{email:'drew@quarticsolutions.com',password:'dawHmsPAtnlDeofD5yHe'}";
                //JObject o = JObject.Parse(user2);
                //var stringPayload =  JsonConvert.SerializeObject(user);
                //var httpContent = new StringContent(o.ToString(), Encoding.UTF8, "application/json");

                ////var paras = new {  email= txtUserName.Text ,  password= PassBox.Password  };
                ////var encodedContent = new FormUrlEncodedContent(paras);
                ////client.DefaultRequestHeaders.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
                //var response = client.PostAsync(url, httpContent).Result;
                
                //if (response.StatusCode == HttpStatusCode.OK)
                //{

                //}
                    
            }
                
        }
    }

}
