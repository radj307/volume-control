using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace VolumeControl.WPF
{
    /// <summary>
    /// ViewModel object for creating a dictionary-style list of <see cref="System.Type"/> &lt;=&gt; <see cref="System.Windows.DataTemplate"/> bindings in XAML.
    /// </summary>
    public sealed class DataTemplateTypeBinding : INotifyPropertyChanged
    {
        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="System.Type"/> that the <see cref="DataTemplate"/> is for.
        /// </summary>
        public Type Type
        {
            get => _type;
            set
            {
                _type = value;
                NotifyPropertyChanged();
            }
        }
        private Type _type = typeof(object);
        /// <summary>
        /// Gets or sets the <see cref="System.Windows.DataTemplate"/> to use for the <see cref="Type"/> of the data.
        /// </summary>
        public DataTemplate? DataTemplate
        {
            get => _dataTemplate;
            set
            {
                _dataTemplate = value;
                NotifyPropertyChanged();
            }
        }
        private DataTemplate? _dataTemplate = null;
        #endregion Properties
    }
}
