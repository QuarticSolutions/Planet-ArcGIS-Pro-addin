using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using test_docing_Panel.Models;

namespace Planet.Model
{
    class Page
    {
        private QuickSearchResult _QuickSearchResult = null;
        public QuickSearchResult QuickSearchResult
        {
            get
            {
                return _QuickSearchResult;
            }
            set
            {
                _QuickSearchResult = value;
            }
        }
        private ObservableCollection<Model.AcquiredDateGroup> _items = null;
        public ObservableCollection<Model.AcquiredDateGroup> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ObservableCollection<Model.AcquiredDateGroup>();
                }
                return _items;
            }
            set
            {
                _items = value;
                //OnPropertyChanged("Items");
            }
        }

        /// <summary>
        /// Sort through quick search results and create list of items
        /// Items are grouped by acquired date and item type
        /// Each item contains a list of strips
        /// Strips are grouped by strip id
        /// Each strip contains a list of assets
        /// Assets inherit from test_docing_Panel.Models.Feature 
        /// </summary>
        public static List<Model.AcquiredDateGroup> ProcessQuickSearchResults(QuickSearchResult result)
        {
            //group results
            List<Model.AcquiredDateGroup> groupedResults = new List<Model.AcquiredDateGroup>();
            Feature[] features = result.features;
            foreach (Feature feature in features)
            {
                Model.AcquiredDateGroup acquiredDateGroup = null;
                DateTime acquired = feature.properties.acquired;
                DateTime acquired_day = acquired.Date;
                int acquiredDateIndex = groupedResults.FindIndex(i => i.acquired == acquired_day);
                if (acquiredDateIndex < 0)
                {
                    acquiredDateGroup = new Model.AcquiredDateGroup
                    {
                        acquired = acquired_day,
                        items = new List<Model.Item>()
                    };
                    groupedResults.Add(acquiredDateGroup);
                }
                else
                {
                    acquiredDateGroup = groupedResults[acquiredDateIndex];
                }
                string itemType = feature.properties.item_type;
                Model.Item item = null;
                List<Model.Item> items = acquiredDateGroup.items;
                int index = items.FindIndex(i => i.itemType == itemType);
                if (index < 0)
                {
                    item = new Model.Item
                    {
                        itemType = itemType,
                        acquired = acquired,
                        strips = new List<Model.Strip>(),
                        parent = acquiredDateGroup
                    };
                    items.Add(item);
                }
                else
                {
                    item = items[index];
                }
                Model.Strip strip = null;
                List<Model.Strip> strips = item.strips;
                string stripId = feature.properties.strip_id;
                int stripIndex = strips.FindIndex(s => s.stripId == stripId);
                if (stripIndex < 0)
                {
                    strip = new Model.Strip
                    {
                        stripId = stripId,
                        acquired = acquired,
                        parent = item,
                        assets = new List<Model.Asset>()
                    };
                    strips.Add(strip);
                }
                else
                {
                    strip = strips[stripIndex];
                }
                List<Model.Asset> assets = strip.assets;
                Model.Asset asset = new Model.Asset
                {
                    parent = strip,
                    properties = feature.properties,
                    id = feature.id,
                    type = feature.type,
                    _links = feature._links,
                    _permissions = feature._permissions,
                    geometry = feature.geometry
                };
                asset.setFootprintVertices();
                asset.setFootprintSymbol();
                asset.setPolygon();
                assets.Add(asset);
            }

            //sort the collections
            if (groupedResults.Count > 0)
            {
                foreach (Model.AcquiredDateGroup group in groupedResults)
                {
                    group.items = group.items.OrderBy(itemGroup => itemGroup.itemType).ToList();
                    foreach (Model.Item item in group.items)
                    {
                        item.strips = item.strips.OrderByDescending(strip => strip.acquired).ToList();
                        foreach (Model.Strip strip in item.strips)
                        {
                            strip.assets = strip.assets.OrderByDescending(asset => asset.properties.acquired).ToList();
                        }
                    }
                }
                groupedResults = groupedResults.OrderByDescending(group => group.acquired).ToList();
            }

            return groupedResults;
        }

        public static async Task<Page> GetNextPage(Page page)
        {
            QuickSearchResult qSResult = page.QuickSearchResult;
            string nextUrl = qSResult._links._next;

            HttpClient client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes(Module1.Current.API_KEY.API_KEY_Value + ":hgvhgv");
            client.DefaultRequestHeaders.Host = "api.planet.com";
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("User-Agent", "ArcGISProC#");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            try
            {
                var response = await client.GetAsync(nextUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<QuickSearchResult>(responseBody);
                    Page newPage = new Page
                    {
                        QuickSearchResult = result
                    };
                    List<AcquiredDateGroup> items = ProcessQuickSearchResults(result);
                    newPage.Items = new ObservableCollection<Model.AcquiredDateGroup>(items);
                    return newPage;
                }
            }
            catch (Exception)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Error Getting Basemaps. Please check api key and try again");
            }
            return null;
        }

        public static async Task<List<Page>> GetAllPages(QuickSearchResult QuickSearchResult)
        {
            List<Page> pages = new List<Page>();
            QuickSearchResult qSResult = QuickSearchResult;
            string nextUrl = qSResult._links._next;

            HttpClient client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes(Module1.Current.API_KEY.API_KEY_Value + ":hgvhgv");
            client.DefaultRequestHeaders.Host = "api.planet.com";
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("User-Agent", "ArcGISProC#");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            try
            {
                do
                {
                    var response = await client.GetAsync(nextUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<QuickSearchResult>(responseBody);
                        Page page = new Page
                        {
                            QuickSearchResult = result
                        };
                        pages.Add(page);
                        // Get the URL for the next page
                        if (result._links._next != null)
                        {
                            nextUrl = result._links._next;
                        }
                        else
                        {
                            nextUrl = string.Empty;
                        }
                    }
                    else
                    {
                        // End loop if we get an error response.
                        nextUrl = string.Empty;
                    }
                }
                while (!string.IsNullOrEmpty(nextUrl));
            }
            catch (Exception)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Error retrieving page");
            }
            return pages;
        }
    }
}
