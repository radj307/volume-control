using PropertyChanged;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VolumeControl.ViewModels
{
    public class DebugWindowVM : INotifyPropertyChanged
    {
        #region Constructor
        public DebugWindowVM()
        {

        }
        #endregion Constructor

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Fields
        private Mixer Mixer => (App.Current.MainWindow as Mixer)!;
        private ListNotification Notification => (App.Current.FindResource("Notification") as ListNotification)!;
        #endregion Fields

        #region Properties
        public ListNotification ListNotification => (App.Current.FindResource("Notification") as ListNotification)!;
        [SuppressPropertyChangedWarnings]
        public double NotifX
        {
            set => Notification.SetPosAtCorner(Notification.GetCurrentScreenCorner(), new(value, Notification.GetPosAtCurrentCorner().Y));
        }
        [SuppressPropertyChangedWarnings]
        public double NotifY
        {
            set => Notification.SetPosAtCorner(Notification.GetCurrentScreenCorner(), new(Notification.GetPosAtCurrentCorner().X, value));
        }
        #endregion Properties

        #region Methods
        #endregion Methods
    }
}
