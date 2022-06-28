using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;

namespace AppConfig
{
    /// <summary>
    /// The most basic <see langword="abstract"/> class in the AppConfig project.
    /// </summary>
    /// <remarks>This implements <see cref="INotifyPropertyChanged"/> using Fody.</remarks>
    [JsonObject]
    [Serializable]
    public abstract class Configuration : INotifyPropertyChanged
    {
        #region Constructors
        /// <summary>
        /// Default Constructor.<br/>
        /// When <see cref="Default"/> is <see langword="null"/>, it is set to the newly-created instance.
        /// </summary>
        public Configuration()
        {
            if (Default == null)
                Default = this;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// The default <see cref="Configuration"/>-derived instance.
        /// </summary>
        [JsonIgnore]
        public static Configuration Default { get; set; } = null!;
        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion Events

        #region Methods
        /// <summary>
        /// Sets the values of all properties and fields in this instance to the values from another instance specified by <paramref name="other"/>.<br/>
        /// This method uses reflection.
        /// </summary>
        /// <remarks>This method may be overloaded in derived classes.</remarks>
        /// <param name="other">Another <see cref="Configuration"/>-derived instance.</param>
        public virtual void SetTo(Configuration other)
        {
            Type myType = GetType();
            Type otherType = other.GetType();
            foreach (var member in myType.GetMembers())
            {
                if (member is FieldInfo fInfo && otherType.GetField(fInfo.Name) is FieldInfo otherFInfo)
                {
                    fInfo.SetValue(this, otherFInfo.GetValue(other));
                }
                else if (member is PropertyInfo pInfo && pInfo.CanWrite && otherType.GetProperty(pInfo.Name) is PropertyInfo otherPInfo)
                {
                    pInfo.SetValue(this, otherPInfo.GetValue(other));
                }
            }
        }
        /// <summary>
        /// Loads config values from the JSON file specified by <paramref name="path"/>
        /// </summary>
        /// <remarks>This method may be overloaded in derived classes.</remarks>
        /// <param name="path">The location of the JSON file to read.<br/><b>This cannot be empty.</b></param>
        /// <returns><see langword="true"/> when the file specified by <paramref name="path"/> exists and was successfully loaded; otherwise <see langword="false"/>.</returns>
        public virtual bool Load(string path)
        {
            if (path.Length == 0)
                return false;
            if (JsonFile.Load(path, GetType()) is Configuration cfg)
            {
                this.SetTo(cfg);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Saves config values to the JSON file specified by <paramref name="path"/>
        /// </summary>
        /// <remarks>This method may be overloaded in derived classes.</remarks>
        /// <param name="path">The location of the JSON file to write.<br/><b>This cannot be empty.</b></param>
        /// <param name="formatting">Formatting type to use when serializing this class instance.</param>
        public virtual void Save(string path, Formatting formatting = Formatting.Indented)
        {
            if (path.Length == 0)
                return;
            JsonFile.Save(path, this, formatting);
        }
        #endregion Methods
    }
}