using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
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
using System.ComponentModel;
using System.Windows.Input;

namespace Planet
{
    internal class PlanetLogin : Gallery, INotifyPropertyChanged

    {
        public bool _canExecuteMyCommand = true;
        private bool _isInitialized;
        public ICommand LogIn { get; set; }
        protected override void OnDropDownOpened()
        {
            Initialize();
        }

        private void Initialize()
        {
            //PlanetConectionWindow planetConectionWindow = new PlanetConectionWindow();
            //planetConectionWindow.ShowDialog();



            //_canExecuteMyCommand = true;
            if (_isInitialized)
                return;
            //LogIn = new RelayCommand(new Action<object>(getkey), () => _canExecuteMyCommand);
            PlanetConnection planetConnection = new PlanetConnection();
            if (Module1.Current.API_KEY != null)
            {
                planetConnection.API_Key = Module1.Current.API_KEY;
            }
            else
            {

            }
            Add(planetConnection);

            _isInitialized = true;

        }

        private void getkey(object obj)
        {
            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Connecting");
        }

        protected override void OnClick(object item)
        {
            //TODO - insert your code to manipulate the clicked gallery item here
            System.Diagnostics.Debug.WriteLine("Remove this line after adding your custom behavior.");
            this.Clear();
            _isInitialized = false;
            //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Gallery Clicked");
            //base.OnClick(item);
            
        }
        
    }
}
