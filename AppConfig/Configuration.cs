using Newtonsoft.Json;
using PropertyChanged;
using System.ComponentModel;
using System.Reflection;

namespace AppConfig
{
    /// <summary>
    /// The most basic <see langword="abstract"/> class in the AppConfig project.
    /// </summary>
    /// <remarks>
    /// Note that this class implements the <see cref="INotifyPropertyChanged"/> interface using Fody, as a result, all derived classes will automatically have event triggers injected into their property setters.
    /// </remarks>
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
        [SuppressPropertyChangedWarnings]
        public object? this[string name]
        {
            get => this.GetType().GetProperty(name)?.GetValue(this);
            set => this.GetType().GetProperty(name)?.SetValue(this, value);
        }
        #endregion Properties

        #region Events
#       pragma warning disable CS0067
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
#       pragma warning restore CS0067
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
            Type myType = this.GetType();
            Type otherType = other.GetType();
            foreach (MemberInfo? member in myType.GetMembers())
            {
                if (member.Name.Equals("Item"))
                    continue;
                if (member is FieldInfo fInfo && !fInfo.IsStatic && fInfo.IsPublic && otherType.GetField(fInfo.Name) is FieldInfo otherFInfo && fInfo.Equals(otherFInfo))
                { // member:
                    fInfo.SetValue(this, otherFInfo.GetValue(other));
                }
                else if (member is PropertyInfo pInfo
                    && !pInfo.SetMethod!.IsStatic
                    && (pInfo.SetMethod?.IsPublic ?? false)
                    && !pInfo.GetMethod!.IsStatic
                    && (pInfo.GetMethod?.IsPublic ?? false))
                { // property:
                    if (otherType.GetProperty(pInfo.Name) is PropertyInfo otherPInfo)
                    {
                        pInfo.SetValue(this, otherPInfo.GetValue(other));
                    }
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
            if (JsonFile.Load(path, this.GetType()) is Configuration cfg)
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