using Newtonsoft.Json;

namespace AppConfig
{
    /// <summary>
    /// Extends the <see cref="Configuration"/> class with a built-in file <see cref="Location"/>, and adds methods that use this filepath implicitly.
    /// </summary>
    [JsonObject]
    [Serializable]
    public abstract class ConfigurationFile : Configuration
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="ConfigurationFile"/> instance using the given <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The location of the JSON file in the filesystem.</param>
        public ConfigurationFile(string location) : base() => this.Location = location;
        /// <summary>
        /// Creates a new <see cref="ConfigurationFile"/> instance without a filepath.<br/>
        /// Note that the filepath must be provided before calling methods that use it implicitly.
        /// </summary>
        [JsonConstructor]
        public ConfigurationFile() : this(string.Empty) { }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// The location of the JSON configuration file in the local filesystem.
        /// </summary>
        [JsonIgnore] public string Location { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Loads config values from the JSON file specified by <paramref name="Location"/>
        /// </summary>
        /// <remarks>This method may be overloaded in derived classes.</remarks>
        /// <returns><see langword="true"/> when the file specified by <paramref name="Location"/> exists and was successfully loaded; otherwise <see langword="false"/>.</returns>
        public virtual bool Load() => this.Load(this.Location);
        /// <summary>
        /// Saves config values to the JSON file specified by <paramref name="Location"/>
        /// </summary>
        /// <remarks>This method may be overloaded in derived classes.</remarks>
        /// <param name="formatting">Formatting type to use when serializing this class instance.</param>
        public virtual void Save(Formatting formatting = Formatting.Indented) => this.Save(this.Location, formatting);
        #endregion Methods
    }
}
