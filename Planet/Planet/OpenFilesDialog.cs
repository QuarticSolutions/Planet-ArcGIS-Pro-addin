using Planet.Interfaces;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Planet
{
    class OpenFilesDialog : IDialogWindow
    {
        public List<string> ExecuteFileDialog(object owner, string extFilter)
        {
            var fd = new OpenFileDialog();
            fd.Multiselect = true;
            if (!string.IsNullOrWhiteSpace(extFilter))
            {
                fd.Filter = extFilter;
            }
            fd.ShowDialog(owner as Window);

            return fd.FileNames.ToList();
        }
    }
}
