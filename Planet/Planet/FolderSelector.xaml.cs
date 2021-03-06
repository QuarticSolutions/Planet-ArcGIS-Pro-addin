﻿using System;
using System.Collections.Generic;
using System.Linq;
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

using Planet.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ArcGIS.Desktop.Framework;

namespace Planet
{
	/// <summary>
	/// Interaction logic for FolderSelector.xaml
	/// </summary>
	public partial class FolderSelector : Window 

	{
		#region Properties
		public string SelectedPath
		{
			get;
			private set;
		}

		public string InitialPath
		{
			set
			{
				string initialPath = value;
				BaseItem foundItem = (DataContext as BaseItem).FindFullPath(initialPath);
			}
		}

		public bool ShowNewFolderButton
		{
			get
			{
				return btnNewFolder.IsVisible;
			}
			set
			{
				if (value == false)
					btnNewFolder.Visibility = Visibility.Hidden;
				else
					btnNewFolder.Visibility = Visibility.Visible;
			}
		}
        //ObservableCollection<Data.GeoTiffs2> geotiffs;

		#endregion

		#region CTOR

		public FolderSelector()
		{

            InitializeComponent();

			DataContext = new ItemsManager().Root;
            var ff = this.tvFolders;

        }


        #endregion

        #region UI events
        private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			SelectedPath = tbSelectedFolder.Text;

			DialogResult = true;
			Close();
		}

		private void tvFolders_Selected(object sender, RoutedEventArgs e)
		{
			TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            

            tvi.BringIntoView();
		}
        #endregion

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (FrameworkApplication.ApplicationTheme == ApplicationTheme.Dark)
            {
                //Dark theme use white txt
                StackPanel tvi = e.OriginalSource as StackPanel;
                foreach (var item in tvi.Children)
                {
                    if (item is TextBlock)
                    {
                        TextBlock textBlock = (TextBlock)item;

                        textBlock.Foreground = Brushes.White;
                    }
                }
            }


        }
    }
}
