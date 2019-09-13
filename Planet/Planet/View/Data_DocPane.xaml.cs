﻿using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Planet
{
    /// <summary>
    /// Interaction logic for Data_DocPaneView.xaml
    /// </summary>
    public partial class Data_DocPaneView : UserControl
    {

        public Data_DocPaneView()
        {
            InitializeComponent();
        }
        private async void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            //Process.Start(link.NavigateUri.AbsoluteUri);
            FolderSelector folderSelector = new FolderSelector();
            folderSelector.lbxGrids.ItemsSource = null;
            folderSelector.ShowNewFolderButton = false;
            folderSelector.ShowActivated = true;
            folderSelector.SizeToContent = SizeToContent.Width;
            object da = folderSelector.txtGrids.DataContext;

            if (da is Data.BaseItem ba)
            {
                ba.QuadCount = "";
            }
            folderSelector.ShowDialog();
            if ((bool)folderSelector.DialogResult)
            {
                string savelocation = folderSelector.SelectedPath;
                await LoadImage(link.NavigateUri.AbsoluteUri, savelocation + "\\" + link.NavigateUri.AbsoluteUri.Substring(33, link.NavigateUri.AbsoluteUri.LastIndexOf("/") - 33) + ".zip");
            }
                
        }
        public async static Task<bool> LoadImage(string uri, string destination)
        {
            //BitmapImage bitmapImage = new BitmapImage();
            //try
            //{

            //    //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "compute/ops/orders/v2");
            //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "v0/orders/");
            //    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            //    request.Headers.CacheControl = new CacheControlHeaderValue
            //    {
            //        NoCache = true
            //    };
            //    request.Headers.Host = "api.planet.com";
            //    request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            //    request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            //    request.Headers.CacheControl = new CacheControlHeaderValue();
            //    request.Headers.CacheControl.NoCache = true;
            //    string json = "{\"name\":\"Pro1\",\"products\":[{\"item_ids\":[\"20190910_205244_101b\",\"20190908_195741_1048\"],\"item_type\":\"PSScene4Band\",\"product_bundle\":\"analytic\"}],\"include_metadata_assets\":true,\"order_type\":\"partial\",\"delivery\":{\"single_archive\":true,\"archive_type\":\"zip\"}}";
            //    var content = new StringContent(json, Encoding.UTF8, "application/json");
            //    content.Headers.Remove("Content-Type");
            //    content.Headers.Add("Content-Type", "application/json");

            //    //request.Content = content;
            //    try
            //    {
            //        using (HttpResponseMessage httpResponse = _client.SendAsync(request).Result)
            //        {
            //            using (HttpContent content2 = httpResponse.Content)
            //            {
            //                var json2 = content2.ReadAsStringAsync().Result;


            //            }
            //            //using (System.Net.Http.StreamContent sr = new System.Net.Http.StreamContent(httpResponse.Content.ReadAsStringAsync))
            //            //{
            //            //    string resps = sr.ReadAsStringAsync().Result();
            //            //    //Response.Write(sr.ReadToEnd());

            //            //}
            //        }

            //    }
            //    catch (WebException e)
            //    {
            //        if (e.Status == WebExceptionStatus.ProtocolError)
            //        {
            //            WebResponse resp = e.Response;
            //            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            //            {
            //                string resps = sr.ReadToEnd();
            //                //Response.Write(sr.ReadToEnd());
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //        //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            //    }
            try
            { 
                using (HttpClient client = new HttpClient())
                {
                    var byteArray = Encoding.ASCII.GetBytes(Module1.Current.API_KEY.API_KEY_Value + ":");// "1fe575980e78467f9c28b552294ea410:");
                    client.DefaultRequestHeaders.Host = "api.planet.com";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.16.3");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    using (var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                    {
                        using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        {
                            string fileToWriteTo = destination; //Path.GetTempFileName();
                            if (File.Exists(fileToWriteTo))
                            {
                                File.Delete(fileToWriteTo);
                            }
                            using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                            {
                                await streamToReadFrom.CopyToAsync(streamToWriteTo);

                                //MessageBox.Show("Download Completed Succesfully","Dowload Complete",MessageBoxButton.OK,MessageBoxImage.Information);
                            }

                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Error downloading \n" + ex.Message, "Download Problem", MessageBoxButton.OK, MessageBoxImage.Warning);
                Console.WriteLine("Failed to Download the quad: {0}", ex.Message);
            }

            return false;
        }

    }
}