using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using test_docing_Panel.Models;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework;

namespace test_docing_Panel.ViewModel
{
    internal class PlanetResultViewModel : DockPane
    {
        private IList<PlanetResultModel> _UsersList;
        private const string _dockPaneID = "test_docing_Panel_testDocPanel_burger";
        private const string _menuID = "test_docing_Panel_testDocPanel_burger_Menu";
        public PlanetResultViewModel()
        {
            _UsersList = new List<PlanetResultModel>
            {
                new PlanetResultModel{UserId = 1,FirstName="Raj",LastName="Beniwal",City="Delhi",State="DEL",Country="INDIA"},
                new PlanetResultModel{UserId=2,FirstName="Mark",LastName="henry",City="New York", State="NY", Country="USA"},
                new PlanetResultModel{UserId=3,FirstName="Mahesh",LastName="Chand",City="Philadelphia", State="PHL", Country="USA"},
                new PlanetResultModel{UserId=4,FirstName="Vikash",LastName="Nanda",City="Noida", State="UP", Country="INDIA"},
                new PlanetResultModel{UserId=5,FirstName="Harsh",LastName="Kumar",City="Ghaziabad", State="UP", Country="INDIA"},
                new PlanetResultModel{UserId=6,FirstName="Reetesh",LastName="Tomar",City="Mumbai", State="MP", Country="INDIA"},
                new PlanetResultModel{UserId=7,FirstName="Deven",LastName="Verma",City="Palwal", State="HP", Country="INDIA"},
                new PlanetResultModel{UserId=8,FirstName="Ravi",LastName="Taneja",City="Delhi", State="DEL", Country="INDIA"}
            };
        }
        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Activate();
        }
        public IList<PlanetResultModel> Users
        {
            get { return _UsersList; }
            set { _UsersList = value; }
        }

        private ICommand mUpdater;
        public ICommand UpdateCommand
        {
            get
            {
                if (mUpdater == null)
                    mUpdater = new Updater();
                return mUpdater;
            }
            set
            {
                mUpdater = value;
            }
        }

        private class Updater : ICommand
        {
            #region ICommand Members  

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {

            }

            #endregion
        }
        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "My DockPane";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }

        #region Burger Button

        /// <summary>
        /// Tooltip shown when hovering over the burger button.
        /// </summary>
        public string BurgerButtonTooltip
        {
            get { return "Options"; }
        }

        /// <summary>
        /// Menu shown when burger button is clicked.
        /// </summary>
        public System.Windows.Controls.ContextMenu BurgerButtonMenu
        {
            get { return FrameworkApplication.CreateContextMenu(_menuID); }
        }
        #endregion
    }
    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class testDocPanel_burger_ShowButton : Button
    {
        protected override void OnClick()
        {

            testDocPanel_burgerViewModel.Show();
        }
    }

    /// <summary>
    /// Button implementation for the button on the menu of the burger button.
    /// </summary>
    internal class testDocPanel_burger_MenuButton : Button
    {
        protected override void OnClick()
        {
        }
    }
}
