
using ArcGIS.Desktop.Framework.Contracts;
using Planet.View;

namespace Planet.Tools
{
    internal class btnInfo : Button
    {
        protected override void OnClick()
        {
            Information_window information_Window = new Information_window();
            information_Window.ShowDialog();
        }
    }
}
