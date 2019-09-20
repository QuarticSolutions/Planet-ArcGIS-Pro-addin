using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Core.Events;
using System.ComponentModel;
using System.Net.Http;
using ArcGIS.Core.Events;

namespace Planet
{
    /// <summary>
    /// Addin control module. Connects addin to Pro framework
    /// </summary>
    internal class Module1 : Module, INotifyPropertyChanged
    {
        private static Module1 _this = null;
        public API_KEY API_KEY = new API_KEY();
        public bool IsTrial = false;
        private Module1()
        {
            FrameworkApplication.State.Deactivate("planet_state_connection"); 
            ProjectOpenedEvent.Subscribe(OnProjectOpen);
            ProjectClosedEvent.Subscribe(OnProjectClose);
        }

        private void OnProjectClose(ProjectEventArgs obj)
        {
            hasSettings = false;
        }

        private void OnProjectOpen(ProjectEventArgs obj)
        {
            
        }

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static Module1 Current
        {
            get
            {
                return _this ?? (_this = (Module1)FrameworkApplication.FindModule("Planet_Module"));
            }
        }
        private Dictionary<string, string> _moduleSettings = new Dictionary<string, string>();

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        private bool hasSettings = false;
        /// <summary>
        /// When the project forst loads the setting are read. If a planet_api_key key value pair
        /// exists then make a call to the Planet api to test key va;idity. If good set the project state
        /// planet_state_connection so Planet tab controls will be enabled.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected override Task OnReadSettingsAsync(ModuleSettingsReader settings)
        {

            // set the flag
            hasSettings = true;
            // clear existing setting values
            _moduleSettings.Clear();

            if (settings == null) return Task.FromResult(0);

            // Settings defined in the Property sheet’s viewmodel.	
            string[] keys = new string[] { "planet_api_key" };
            
            foreach (string key in keys)
            {

                object value = settings.Get(key);
                if (value != null)
                {
                    API_KEY.API_KEY_Value = value.ToString();
                    if (_moduleSettings.ContainsKey(key))
                    {
                        _moduleSettings[key] = value.ToString();

                    }
                    else
                        _moduleSettings.Add(key, value.ToString());
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = client.GetAsync("https://api.planet.com/basemaps/v1/mosaics?api_key=" + Module1.Current.API_KEY.API_KEY_Value).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            FrameworkApplication.State.Activate("planet_state_connection");
                            //IPlugInWrapper wrapper = FrameworkApplication.GetPlugInWrapper("Planet_PlanetGalleryInline");
                            //FrameworkApplication.SetCurrentToolAsync("Planet_PlanetGalleryInline");
                        }
                    }

                }
            }

            object trial = settings.Get("IsTrial");
            if (trial != null )
            {
                if (trial.ToString() == "true")
                {
                    IsTrial = true;
                }
                
            }
            return Task.FromResult(0);
        }
        /// <summary>
        /// If a api key value has been set write  it to the project settings.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected override Task OnWriteSettingsAsync(ModuleSettingsWriter settings)
        {
            if (API_KEY.API_KEY_Value != "")
            {
                settings.Add("planet_api_key", API_KEY.API_KEY_Value);
            }
            if (IsTrial==true)
            {
                settings.Add("IsTrial", "true");
            }
            //foreach (string key in _moduleSettings.Keys)
            //{
            //    settings.Add(key, _moduleSettings[key]);
            //}
            return Task.FromResult(0);

        }
        #endregion Overrides

    }
    /// <summary>
    /// Class to store the api key setting. tools will use this value to query the server.
    /// Planet connection MVVM will use it as the inital value to bind to.
    /// </summary>
    public class API_KEY : INotifyPropertyChanged
    {

        private string _apikey;
        public string API_KEY_Value
        {
            get
            {
                return _apikey;
            }
            set
            {
                _apikey = value;
                OnPropertyChanged("API_KEY_Value");
            }
        }
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
    }

    /// <summary>
    /// ESRI Framwaork custom event args that we use to track and react to api key value changes
    /// </summary>
    public class APIKeyChangedEventArgs : EventBase
    {

        /// <summary>
        /// Gets the old name
        /// </summary>
        public string OldName { get; private set; }

        /// <summary>
        /// Gets the new name
        /// </summary>
        public string NewName { get; private set; }

        /// <summary>
        /// Constructor. Create a name changed event passing in the new and old names
        /// </summary>
        /// <param name="oldName">The old name</param>
        /// <param name="newName">The new name</param>
        public APIKeyChangedEventArgs(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }
    }

    public class PlanetGalleryChangedEventArgs : EventBase
    {

        /// <summary>
        /// Gets the old name
        /// </summary>
        public string OldPage { get; private set; }

        /// <summary>
        /// Gets the new name
        /// </summary>
        public string NewPage { get; private set; }

        /// <summary>
        /// Constructor. Create a name changed event passing in the new and old names
        /// </summary>
        /// <param name="oldName">The old name</param>
        /// <param name="newName">The new name</param>
        public PlanetGalleryChangedEventArgs(string oldPage, string newPage)
        {
            OldPage = oldPage;
            NewPage = newPage;
        }
    }

    /// <summary>
    /// A custom CompositePresentationEvent to be published when we change the api key
    /// </summary>
    public class APIKeyChangedEvent : CompositePresentationEvent<APIKeyChangedEventArgs>
    {

        /// <summary>
        /// Allow subscribers to register for our custom event
        /// </summary>
        /// <param name="action">The callback which will be used to notify the subscriber</param>
        /// <param name="keepSubscriberReferenceAlive">Set to true to retain a strong reference</param>
        /// <returns><see cref="ArcGIS.Core.Events.SubscriptionToken"/></returns>
        public static SubscriptionToken Subscribe(Action<APIKeyChangedEventArgs> action, bool keepSubscriberReferenceAlive = false)
        {
            return FrameworkApplication.EventAggregator.GetEvent<APIKeyChangedEvent>()
                .Register(action, keepSubscriberReferenceAlive);
        }

        /// <summary>
        /// Allow subscribers to unregister from our custom event
        /// </summary>
        /// <param name="subscriber">The action that will be unsubscribed</param>
        public static void Unsubscribe(Action<APIKeyChangedEventArgs> subscriber)
        {
            FrameworkApplication.EventAggregator.GetEvent<APIKeyChangedEvent>().Unregister(subscriber);
        }
        /// <summary>
        /// Allow subscribers to unregister from our custom event
        /// </summary>
        /// <param name="token">The token that will be used to find the subscriber to unsubscribe</param>
        public static void Unsubscribe(SubscriptionToken token)
        {
            FrameworkApplication.EventAggregator.GetEvent<APIKeyChangedEvent>().Unregister(token);
        }

        /// <summary>
        /// Event owner calls publish to raise the event and notify subscribers
        /// </summary>
        /// <param name="payload">The associated event information</param>
        internal static void Publish(APIKeyChangedEventArgs payload)
        {
            FrameworkApplication.EventAggregator.GetEvent<APIKeyChangedEvent>().Broadcast(payload);
        }
    }

    public class PlanetGalleryChangedEvent : CompositePresentationEvent<PlanetGalleryChangedEventArgs>
    {

        /// <summary>
        /// Allow subscribers to register for our custom event
        /// </summary>
        /// <param name="action">The callback which will be used to notify the subscriber</param>
        /// <param name="keepSubscriberReferenceAlive">Set to true to retain a strong reference</param>
        /// <returns><see cref="ArcGIS.Core.Events.SubscriptionToken"/></returns>
        public static SubscriptionToken Subscribe(Action<PlanetGalleryChangedEventArgs> action, bool keepSubscriberReferenceAlive = false)
        {
            return FrameworkApplication.EventAggregator.GetEvent<PlanetGalleryChangedEvent>()
                .Register(action, keepSubscriberReferenceAlive);
        }

        /// <summary>
        /// Allow subscribers to unregister from our custom event
        /// </summary>
        /// <param name="subscriber">The action that will be unsubscribed</param>
        public static void Unsubscribe(Action<PlanetGalleryChangedEventArgs> subscriber)
        {
            FrameworkApplication.EventAggregator.GetEvent<PlanetGalleryChangedEvent>().Unregister(subscriber);
        }
        /// <summary>
        /// Allow subscribers to unregister from our custom event
        /// </summary>
        /// <param name="token">The token that will be used to find the subscriber to unsubscribe</param>
        public static void Unsubscribe(SubscriptionToken token)
        {
            FrameworkApplication.EventAggregator.GetEvent<PlanetGalleryChangedEvent>().Unregister(token);
        }

        /// <summary>
        /// Event owner calls publish to raise the event and notify subscribers
        /// </summary>
        /// <param name="payload">The associated event information</param>
        internal static void Publish(PlanetGalleryChangedEventArgs payload)
        {
            FrameworkApplication.EventAggregator.GetEvent<PlanetGalleryChangedEvent>().Broadcast(payload);
        }
    }

}
