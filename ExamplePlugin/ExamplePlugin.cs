using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExamplePlugin
{
    //Sample Plugin. Simply logs the songs being played to a text file.
    //A reference is added to the ToastifyApi.dll(PluginBase)
    //The plugin class below implements the PluginBase interface

    
    //Toastify.xml
    //<Plugins>
    //  <PluginDetails>
    //    <FileName>ExamplePlugin.dll</FileName>                            Plugin filename
    //    <TypeName>ExamplePlugin.ExamplePlugin/TypeName>                   Plugin type name (Namespace + "." + ClassName)
    //    <SettingsC:\ToastifyLog.txt</Settings>                            Plugin settings data
    //  </PluginDetails>
    //</Plugins>

    public class ExamplePlugin : Toastify.Plugin.PluginBase
    {
        public ExamplePlugin()
        {
        }

        string logFilename;
        public void Init(string settings)
        {
            //Init is called direcly after the constructor.
            //Any data from the <Settings> element in Toastify.xml is passed on.
            //For multiple values, use a seperated list of some sort. Eg. "value1|value2|value3"... settings.Split('|')...

            this.logFilename = settings;
        }

        public void Started()
        {
            System.IO.File.AppendAllText(logFilename, "ExamplePlugin Started - " + DateTime.Now.ToLongTimeString() + Environment.NewLine);
        }

        public void Closing()
        {
            System.IO.File.AppendAllText(logFilename, "ExamplePlugin Closing - " + DateTime.Now.ToLongTimeString() + Environment.NewLine);
        }

        public void TrackChanged(string artist, string title)
        {
            System.IO.File.AppendAllText(logFilename, string.Format("{0} - {1}{2}", artist, title, Environment.NewLine));
        }

        public void Dispose()
        {
            //Dispose of any resources here.
            //This is called direcly after Closing()
        }
    }
}
