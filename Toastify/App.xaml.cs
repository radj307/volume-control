using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;

namespace Toastify
{
    //Special entry point to allow for single instance check
    public class EntryPoint
    {
        [STAThread]
        public static void Main(string[] args)
        {
            string appSpecificGuid = "{B8F3CA50-CE27-4ffa-A812-BBE1435C9485}";    
            bool exclusive;
            Mutex m = new Mutex(true, appSpecificGuid, out exclusive);
            if (exclusive) 
            {
                App app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }
}
