using ArcGIS.Desktop.Core.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planet
{
    public class WebMapItem
    {
        public WebMapItem(PortalItem portalItem)
        {
            _id = portalItem.ID;
            _title = portalItem.Title;
            _name = portalItem.Name;
            _thumbnail = portalItem.ThumbnailPath;
            _snippet = string.IsNullOrEmpty(portalItem.Summary) ? _title : portalItem.Summary;
            _group = portalItem.Owner;
        }
        private string _id;
        public string ID => _id;

        private string _title;
        public string Title => _title;

        private string _name;
        public string Name => _name;

        private string _thumbnail;
        public string Thumbnail => _thumbnail;

        private string _snippet;
        public string Snippet => _snippet;
        public string Text => _name;

        private string _group;
        public string Group
        {
            get { return _group; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
