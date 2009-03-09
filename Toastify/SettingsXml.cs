using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Toastify
{
    [Serializable]
    public class SettingsXml
    {
        public bool GlobalHotKeys { get; set; }
        public bool DisableToast { get; set; }
        public int FadeOutTime { get; set; }
        public string ToastColorTop { get; set; }
        public string ToastColorBottom { get; set; }
        public string ToastBorderColor { get; set; }
        public double ToastBorderThickness { get; set; }
        public string ToastBorderCornerRadious { get; set; }
        public double ToastWidth { get; set; }
        public double ToastHeight { get; set; }
        public List<Hotkey> HotKeys { get; set; }

        internal static SettingsXml Defaul
        {
            get
            {
                return new SettingsXml
                {
                    FadeOutTime = 2000,
                    GlobalHotKeys = true,
                    DisableToast = false,
                    ToastColorTop = "#FFCFCFCF",
                    ToastColorBottom = "#FFA4A4A4",
                    ToastBorderColor = "#FF292929",
                    ToastBorderThickness = 1.0,
                    ToastWidth = 300,
                    ToastHeight = 75,
                    ToastBorderCornerRadious = "4.0,4.0,4.0,4.0",
                    HotKeys = new List<Hotkey> 
                    {
                        new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.Up , Action= SpotifyAction.PlayPause },
                        new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.Down , Action= SpotifyAction.Stop },
                        new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.Left , Action= SpotifyAction.PreviousTrack },
                        new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.Right , Action= SpotifyAction.NextTrack },
                        new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.M , Action= SpotifyAction.Mute },
                        new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.PageDown , Action= SpotifyAction.VolumeDown },
                        new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.PageUp , Action= SpotifyAction.VolumeUp },
                        new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.Space , Action= SpotifyAction.ShowToast },
                        new Hotkey { Ctrl=true, Alt=true, Key= System.Windows.Input.Key.S , Action= SpotifyAction.ShowSpotify }
                    }
                };
            }
        }

        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsXml));
                xmlSerializer.Serialize(sw, this);
            }
        }

        public static SettingsXml Open(string filename)
        {
            if (!System.IO.File.Exists(filename))
                throw new FileNotFoundException();

            using (StreamReader sr = new StreamReader(filename))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsXml));
                SettingsXml xml = xmlSerializer.Deserialize(sr) as SettingsXml;
                return xml;
            }
        }
    }
}
