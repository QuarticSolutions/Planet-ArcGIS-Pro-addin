using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Dialogs;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Segment;
using Segment.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Planet
{
    public class PlanetConnection : INotifyPropertyChanged
    {
        private API_KEY _api_key;
        public API_KEY API_Key
        {
            get
            {
                if (_api_key == null)
                {
                    _api_key = new API_KEY();
                }
                return _api_key;
            }
            set
            {
                _api_key = value;
                Module1.Current.API_KEY = _api_key;
                OnPropertyChanged("API_Key");
                
                //Module1.Current.API_KEY.API_KEY_Value = _api_key.API_KEY_Value;
                //Module1.Current.API_KEY.EMAIL_Value = _api_key.EMAIL_Value;
            }
        }

        private bool _LoginVisible = true;
        public bool LoginVisible
        {
            get
            {
                return _LoginVisible;
            }
            set
            {
                _LoginVisible = value;
                OnPropertyChanged("LoginVisible");
            }
        }

        private string _UserName = "";
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
                OnPropertyChanged("UserName");
            }
        }
        //private string _password;
        //public string Password
        //{
        //    get
        //    {
        //        return _password;
        //    }
        //    set
        //    {
        //        _password = value;
        //        OnPropertyChanged("Password");
        //    }
        //}
        //private ICommand _clicklogin;
        //public ICommand ClickLogin
        //{
        //    get
        //    {
        //        return _clicklogin ?? (_clicklogin = new CommandHandler(() => Login(), CanExecute));
        //    }
        //}

        private ICommand _LogOut;
        public ICommand LogOut
        {
            get
            {
                if (_LogOut == null)
                    _LogOut = new CommandHandler(() => DoLogOut(), CanExecute);
                return _LogOut;
            }
        }

        private void DoLogOut()
        {
            //API_Key.API_KEY_Value = null;
            Module1.Current.API_KEY = null;
            FrameworkApplication.State.Deactivate("planet_state_connection");
            getkey();
            LoginVisible = true;
        }

        private ICommand _clicklogin2;
        public ICommand ClickLogin2
        {
            get
            {
                return _clicklogin2 ?? (_clicklogin2 = new CommandHandler2(param => ExecuteAttachmentChecked(param),CanExecuteAttachmentChecked()));
            }
        }
        private void ExecuteAttachmentChecked(object param)
        {
            //Console.WriteLine(param.ToString());
            if (param is PasswordBox passwordBox)
            {
                //string pass = passwordBox.Password;
                //Console.WriteLine(pass);
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        //HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("https://api.planet.com/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("User-Agent", "C# App");
                        client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                        //client.DefaultRequestHeaders.Add("content-length", "application/json");
                        Data.User user = new Data.User
                        {
                            email = _UserName,
                            password = passwordBox.Password
                        };
                        var requestMessage = JsonConvert.SerializeObject(user);
                        var content = new StringContent(requestMessage, Encoding.UTF8, "application/json");
                        content.Headers.Remove("Content-Type");
                        content.Headers.Add("Content-Type", "application/json");
                        var postResp = client.PostAsync("auth/v1/experimental/public/users/authenticate", content);
                        postResp.Wait();
                        var response = postResp.Result;

                        var me = response.Content.ReadAsStringAsync();
                        me.Wait();
                        var rr = JsonConvert.DeserializeObject<dynamic>(me.Result);
                        IDictionary<string, JToken> Jsondata = JObject.Parse(me.Result);
                        if (Jsondata.ContainsKey("success"))
                        {
                            if (Jsondata["success"].ToString() == "False")
                            {
                                MessageBox.Show("There was a problem with the user name or password that you entered." + Environment.NewLine + Jsondata["errors"].ToString(), "Problem logging in");
                                return;
                            }

                        }
                        Data.Token result = JsonConvert.DeserializeObject<Data.Token>(me.Result);
                        var jwtHandler = new JwtSecurityTokenHandler();
                        var readableToken = jwtHandler.CanReadToken(result.token);
                        if (readableToken != true)
                        {
                            Console.WriteLine("The token doesn't seem to be in a proper JWT format.");
                            MessageBox.Show("The token doesn't seem to be in a proper JWT format.", "Problem logging in");
                            return;
                        }
                        if (readableToken == true)
                        {
                            IdentityModelEventSource.ShowPII = true;
                            var token = jwtHandler.ReadJwtToken(result.token);

                            ////Extract the headers of the JWT
                            //var headers = token.Header;
                            //var jwtHeader = "{";
                            //foreach (var h in headers)
                            //{
                            //    jwtHeader += '"' + h.Key + "\":\"" + h.Value + "\",";
                            //}
                            //jwtHeader += "}";
                            //Console.WriteLine("Header:\r\n" + JToken.Parse(jwtHeader).ToString(Formatting.Indented));

                            //Extract the payload of the JWT
                            var claims = token.Claims;
                            var jwtPayload = "{";
                            foreach (Claim c in claims)
                            {
                                jwtPayload += '"' + c.Type + "\":\"" + c.Value + "\",";
                            }
                            jwtPayload += "}";
                            //string txtJwtOut = "\r\nPayload:\r\n" + JToken.Parse(jwtPayload).ToString(Formatting.Indented);
                            Data.Payload payload = JsonConvert.DeserializeObject<Data.Payload>(jwtPayload);
                            API_Key.API_KEY_Value = payload.api_key;
                            API_Key.EMAIL_Value = payload.email;
                            API_Key.programId_Value = payload.program_id;
                            API_Key.organizationId_Value = payload.organization_id;
                            if (payload.program_id == "29")
                            {
                                Module1.Current.IsTrial = true;
                            }
                            else
                            {
                                Module1.Current.IsTrial = false;
                            }
                            if (Analytics.Client == null)
                            {
                                Analytics.Initialize("at3uKKI8tvtIzsvXU4MpmxKBWSfnUPwR");
                            }
                            Analytics.Client.Identify(payload.email, new Traits() {
                            { "apiKey", payload.api_key },
                            { "email", payload.email },
                            { "organizationId", payload.organization_id },
                            { "programId", payload.program_id }
                        });
                        }

                        getkey();
                        //var response = await postResp.Content.ReadAsStringAsync();
                        Console.WriteLine(result.token);
                    }
                   

                    LoginVisible = false;
                }
                catch (WebException wex)
                {
                    MessageBox.Show(wex.Message);
                }

            }
        }

        private bool CanExecuteAttachmentChecked()
        {
            return true;
        }
        //private RelayCommand _loginCommand = new RelayCommand(param => Login(UserName, (PasswordBox) param), param => CanLogIn);

        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() => getkey(), CanExecute));
            }
        }
        private ICommand _openHyperlinkCommand;
        public ICommand OpenHyperlinkCommand
        {
            get
            {
                if (_openHyperlinkCommand == null)
                    _openHyperlinkCommand = new CommandHandler(() => ExecuteHyperlink(),CanExecute);
                return _openHyperlinkCommand;
            }
        }

        private void ExecuteHyperlink()
        {
            System.Diagnostics.Process.Start("https://go.planet.com/basemaps-trial-esri");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void getkey()
        {
            APIKeyChangedEvent.Publish(new APIKeyChangedEventArgs(_api_key.API_KEY_Value, _api_key.API_KEY_Value));
        }
        //private  void  Login()
        //{
        //    try
        //    {
        //        HttpClient client = new HttpClient();
        //        client.BaseAddress = new Uri("https://api.planet.com/");
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        client.DefaultRequestHeaders.Add("User-Agent", "C# App");
        //        client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        //        //client.DefaultRequestHeaders.Add("content-length", "application/json");
        //        Data.User user = new Data.User
        //        {
        //            email = "drew@quarticsolutions.com",
        //            password = "dawHmsPAtnlDeofD5yHe"
        //        };
        //        var requestMessage = JsonConvert.SerializeObject(user);
        //        var content = new StringContent(requestMessage,Encoding.UTF8, "application/json");
        //        content.Headers.Remove("Content-Type");
        //        content.Headers.Add("Content-Type", "application/json");
        //        var postResp =  client.PostAsync("auth/v1/experimental/public/users/authenticate", content);
        //        postResp.Wait();
        //        var response = postResp.Result;
                
        //        var me = response.Content.ReadAsStringAsync();
        //        me.Wait();
        //        Data.Token result = JsonConvert.DeserializeObject<Data.Token>(me.Result);
        //        var jwtHandler = new JwtSecurityTokenHandler();
        //        var readableToken = jwtHandler.CanReadToken(result.token);
        //        if (readableToken != true)
        //        {
        //            Console.WriteLine("The token doesn't seem to be in a proper JWT format.");
        //        }
        //        if (readableToken == true)
        //        {
        //            var token = jwtHandler.ReadJwtToken(result.token);

        //            //Extract the headers of the JWT
        //            var headers = token.Header;
        //            var jwtHeader = "{";
        //            foreach (var h in headers)
        //            {
        //                jwtHeader += '"' + h.Key + "\":\"" + h.Value + "\",";
        //            }
        //            jwtHeader += "}";
        //            Console.WriteLine("Header:\r\n" + JToken.Parse(jwtHeader).ToString(Formatting.Indented));

        //            //Extract the payload of the JWT
        //            var claims = token.Claims;
        //            var jwtPayload = "{";
        //            foreach (Claim c in claims)
        //            {
        //                jwtPayload += '"' + c.Type + "\":\"" + c.Value + "\",";
        //            }
        //            jwtPayload += "}";
        //            //string txtJwtOut = "\r\nPayload:\r\n" + JToken.Parse(jwtPayload).ToString(Formatting.Indented);
        //            Data.Payload payload = JsonConvert.DeserializeObject<Data.Payload>( jwtPayload );
        //            _api_key.API_KEY_Value = payload.api_key;
        //            if (payload.program_id=="29")
        //            {
        //                Module1.Current.IsTrial = true;
        //            }
        //        }
        //        getkey();
        //        //var response = await postResp.Content.ReadAsStringAsync();
        //        Console.WriteLine(result.token);
        //    }
        //    catch (WebException wex )
        //    {
        //        MessageBox.Show(wex.Message) ;
        //    }
        //}
        public bool CanExecute { get; set; } = true;
    }
    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
    public class CommandHandler2 : ICommand
    {
        private Action<PasswordBox> _action;
        private bool _canExecute;
        public CommandHandler2(Action<PasswordBox> action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action((PasswordBox)parameter);
        }
    }
}
