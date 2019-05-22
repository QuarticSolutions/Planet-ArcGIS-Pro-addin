using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Planet.Interfaces
{
    using System.Collections.Generic;
    public interface IDialogWindow
    {
        List<string> ExecuteFileDialog(object owner, string extFilter);
    }
}
namespace Planet
{
    using Planet.Interfaces;
    using System.Windows.Input;

    public class MyViewModel
    {
        public ICommand DoSomeThingCmd { get; } = new CommandImpl((dialogType) =>
        {
            var dlgObj = Activator.CreateInstance(dialogType) as IDialogWindow;
            var fileNames = dlgObj?.ExecuteFileDialog(null, "*.txt");
            object sdd = new object();
            return sdd;
            //Do something with fileNames..
        });
    }

    internal class CommandImpl : ICommand
    {
        private Func<System.Type, object> p;

        public CommandImpl(Func<System.Type, object> p)
        {
            this.p = p;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
