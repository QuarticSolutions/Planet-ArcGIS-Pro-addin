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

namespace test_docing_Panel
{
    internal class embedgallery : Gallery
    {
        private bool _isInitialized;

        protected override void OnDropDownOpened()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_isInitialized)
                return;

            //Add 6 items to the gallery
            for (int i = 0; i < 6; i++)
            {
                string name = string.Format("Item {0}", i);
                Add(new GalleryItem(name, this.LargeImage != null ? ((ImageSource)this.LargeImage).Clone() : null, name));
            }
            _isInitialized = true;

        }

        protected override void OnClick(GalleryItem item)
        {
            //TODO - insert your code to manipulate the clicked gallery item here
            System.Diagnostics.Debug.WriteLine("Remove this line after adding your custom behavior.");
            base.OnClick(item);
        }
    }
}
