using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Core.Events;
using System.ComponentModel;
using ArcGIS.Core.Events;
using Segment;
using Segment.Model;

using ArcGIS.Desktop.Framework.Utilities;
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
        protected override bool Initialize()
        {
            return true;
        }

        private Module1()
        {
            FrameworkApplication.State.Deactivate("planet_state_connection");
            ProjectOpenedEvent.Subscribe(OnProjectOpen);
            ProjectClosedEvent.Subscribe(OnProjectClose);
            ProjectSavingEvent.Subscribe(OnProjectSaving);
        }

        private Task OnProjectSaving(ProjectEventArgs arg)
        {
            return QueuedTask.Run(() =>
            {
                bool didwarn = false;
                if (Module1.Current.API_KEY != null && Module1.Current.API_KEY.API_KEY_Value != null)
                {
                    MessageBox.Show("Please be aware that sharing ArcGIS Pro projects may expose your Planet api key if the projects contains Planet map layers.");
                    didwarn = true;
                }
                if (!didwarn)
                {
                    var mapProjectItems = Project.Current.GetItems<MapProjectItem>();
                    foreach (var mapProjectItem in mapProjectItems)
                    {
                        Map map = mapProjectItem.GetMap();
                        //check for any planet layers
                        IEnumerable<TiledServiceLayer> tiledServiceLayers = map.GetLayersAsFlattenedList().OfType<TiledServiceLayer>().Where(layer =>
                            layer.URL.Contains("planet.com") &&
                            layer.URL.Contains("api_key"));
                        if (tiledServiceLayers.Count() > 0)
                        {
                            MessageBox.Show("Planet layers contain your API key which will be saved within the project. To protect your API key, remove all Planet layers before saving.", "Saving Planet layers");
                        }
                    }
                }
            });
        }

        private void OnProjectClose(ProjectEventArgs obj)
        {
            hasSettings = false;
            if (API_KEY != null)
            {
                if (!String.IsNullOrEmpty(API_KEY.EMAIL_Value))
                {
                    Analytics.Client.Identify(API_KEY.EMAIL_Value, new Traits() { });
                    MessageBox.Show("Please be aware that sharing ArcGIS Pro projects may expose your Planet api key if the projects contains Planet map layers.");
                }
            }
           

        }

        private void OnProjectOpen(ProjectEventArgs obj)
        {
            
        }

        //private async Task OnProjectSaving(ProjectEventArgs obj)
        //{
        //    await QueuedTask.Run(() =>
        //    {
        //        var mapProjectItems = Project.Current.GetItems<MapProjectItem>();
        //        foreach (var mapProjectItem in mapProjectItems)
        //        {
        //            Map map = mapProjectItem.GetMap();
        //            //check for any planet layers
        //            IEnumerable<TiledServiceLayer> tiledServiceLayers = map.GetLayersAsFlattenedList().OfType<TiledServiceLayer>().Where(layer =>
        //                layer.URL.Contains("planet.com") &&
        //                layer.URL.Contains("api_key"));
        //            if (tiledServiceLayers.Count() > 0)
        //            {
        //                MessageBox.Show("Planet layers contain your API key which will be saved within the project. To protect your API key, remove all Planet layers before saving.", "Saving Planet layers");
        //            }
        //        }
        //    });
        //}

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
        //private Dictionary<string, string> _moduleSettings = new Dictionary<string, string>();

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
            //_moduleSettings.Clear();

            if (settings == null) return Task.FromResult(0);

            //// Settings defined in the Property sheet’s viewmodel.	
            //string[] keys = new string[] { "planet_api_key", "planet_email", "planet_organizationId", "planet_programId" };


            //foreach (string key in keys)
            //{
            //    object value = settings.Get(key);
            //    if (value != null)
            //    {
            //        if (key=="planet_email")
            //        {
            //            API_KEY.EMAIL_Value = value.ToString();
                       
            //        }
            //        else if (key == "planet_organizationId")
            //        {
            //            API_KEY.organizationId_Value = value.ToString();
            //        }
            //        else if (key == "planet_programId")
            //        {
            //            API_KEY.programId_Value = value.ToString();
            //        }
            //        else if (key == "planet_api_key")
            //        {
            //            API_KEY.API_KEY_Value = value.ToString();
            //            //if (_moduleSettings.ContainsKey(key))
            //            //{
            //            //    _moduleSettings[key] = value.ToString();
            //            //}
            //            //else
            //            //    _moduleSettings.Add(key, value.ToString());
            //            using (HttpClient client = new HttpClient())
            //            {
            //                HttpResponseMessage response = client.GetAsync("https://api.planet.com/basemaps/v1/mosaics?api_key=" + Module1.Current.API_KEY.API_KEY_Value).Result;
            //                if (response.IsSuccessStatusCode)
            //                {

            //                    FrameworkApplication.State.Activate("planet_state_connection");
            //                    //IPlugInWrapper wrapper = FrameworkApplication.GetPlugInWrapper("Planet_PlanetGalleryInline");
            //                    //FrameworkApplication.SetCurrentToolAsync("Planet_PlanetGalleryInline");
            //                }
            //            }
            //        }
            //    }
            //}
            
            //if (!string.IsNullOrEmpty(API_KEY.EMAIL_Value) && !string.IsNullOrEmpty(API_KEY.API_KEY_Value))
            //{
            //    if (Analytics.Client == null)
            //    {
            //        Analytics.Initialize("at3uKKI8tvtIzsvXU4MpmxKBWSfnUPwR");
            //    }
            //    Analytics.Client.Identify(API_KEY.EMAIL_Value, new Traits() {

            //            { "apiKey", API_KEY.API_KEY_Value },
            //            { "email", API_KEY.EMAIL_Value },
            //            { "organizationId", API_KEY.organizationId_Value },
            //            { "programId", API_KEY.programId_Value }
            //        });
            //    Analytics.Client.Track(API_KEY.EMAIL_Value,"Login from ArcGIS Pro", new Traits()
            //    {

            //        { "apiKey", API_KEY.API_KEY_Value },
            //        { "email", API_KEY.EMAIL_Value },
            //        { "organizationId", API_KEY.organizationId_Value },
            //        { "programId", API_KEY.programId_Value }
            //    });
            //}

            //object trial = settings.Get("IsTrial");
            //if (trial != null )
            //{
            //    if (trial.ToString() == "true")
            //    {
            //        IsTrial = true;
            //    }
                
            //}
            return Task.FromResult(0);
        }
        /// <summary>
        /// If a api key value has been set write  it to the project settings.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected override Task OnWriteSettingsAsync(ModuleSettingsWriter settings)
        {
            //if (API_KEY != null)
            //{
            //    if (API_KEY.API_KEY_Value != "")
            //    {
            //        settings.Add("planet_api_key", API_KEY.API_KEY_Value);
            //    }
            //    if (IsTrial == true)
            //    {
            //        settings.Add("IsTrial", "true");
            //    }
            //    if (API_KEY.EMAIL_Value != "")
            //    {
            //        settings.Add("planet_email", API_KEY.EMAIL_Value);
            //    }
            //    if (API_KEY.organizationId_Value != "")
            //    {
            //        settings.Add("planet_organizationId", API_KEY.organizationId_Value);
            //    }
            //    if (API_KEY.programId_Value != "")
            //    {
            //        settings.Add("planet_programId", API_KEY.programId_Value);
            //    }
            //    //foreach (string key in _moduleSettings.Keys)
            //    //{
            //    //    settings.Add(key, _moduleSettings[key]);
            //    //}
               
            //}
            //else
            //{
            //    settings.Add("planet_api_key", "");
            //    settings.Add("IsTrial", "");
            //    settings.Add("planet_email", "");
            //    settings.Add("planet_organizationId", "");
            //    settings.Add("planet_programId", "");
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
        private string _email;
        public string EMAIL_Value
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged("EMAIL_Value");
            }
        }
        private string _organizationId;
        public string organizationId_Value
        {
            get
            {
                return _organizationId;
            }
            set
            {
                _organizationId = value;
                OnPropertyChanged("organizationId_Value");
            }
        }
        private string _programId;
        public string programId_Value
        {
            get
            {
                return _programId;
            }
            set
            {
                _programId = value;
                OnPropertyChanged("programId_Value");
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

    public class PlanetGalleryFilterEventArgs : EventBase
    {
        /// <summary>
        /// Gets the old name
        /// </summary>
        public string FilterText { get; private set; }

        /// <summary>
        /// Constructor. Create a name changed event passing in the new and old names
        /// </summary>
        /// <param name="oldName">The old name</param>
        /// <param name="newName">The new name</param>
        public PlanetGalleryFilterEventArgs(string filterText)
        {
            FilterText = filterText;
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

    public class PlanetGalleryFilterEvent : CompositePresentationEvent<PlanetGalleryFilterEventArgs>
    {

        /// <summary>
        /// Allow subscribers to register for our custom event
        /// </summary>
        /// <param name="action">The callback which will be used to notify the subscriber</param>
        /// <param name="keepSubscriberReferenceAlive">Set to true to retain a strong reference</param>
        /// <returns><see cref="ArcGIS.Core.Events.SubscriptionToken"/></returns>
        public static SubscriptionToken Subscribe(Action<PlanetGalleryFilterEventArgs> action, bool keepSubscriberReferenceAlive = false)
        {
            return FrameworkApplication.EventAggregator.GetEvent<PlanetGalleryFilterEvent>()
                .Register(action, keepSubscriberReferenceAlive);
        }

        /// <summary>
        /// Allow subscribers to unregister from our custom event
        /// </summary>
        /// <param name="subscriber">The action that will be unsubscribed</param>
        public static void Unsubscribe(Action<PlanetGalleryFilterEventArgs> subscriber)
        {
            FrameworkApplication.EventAggregator.GetEvent<PlanetGalleryFilterEvent>().Unregister(subscriber);
        }
        /// <summary>
        /// Allow subscribers to unregister from our custom event
        /// </summary>
        /// <param name="token">The token that will be used to find the subscriber to unsubscribe</param>
        public static void Unsubscribe(SubscriptionToken token)
        {
            FrameworkApplication.EventAggregator.GetEvent<PlanetGalleryFilterEvent>().Unregister(token);
        }

        /// <summary>
        /// Event owner calls publish to raise the event and notify subscribers
        /// </summary>
        /// <param name="payload">The associated event information</param>
        internal static void Publish(PlanetGalleryFilterEventArgs payload)
        {
            FrameworkApplication.EventAggregator.GetEvent<PlanetGalleryFilterEvent>().Broadcast(payload);
        }
    }

}
