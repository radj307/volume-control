using System.Windows;

namespace VolumeControl.Core
{
    /// <summary>
    /// Extends the <see cref="DataTemplate"/> class with metadata specific to action setting data template providers.
    /// </summary>
    public class ActionSettingDataTemplate : DataTemplate
    {
        #region Properties
        /// <summary>
        /// Gets or sets the type of value supported by this action setting data template.
        /// </summary>
        public Type ValueType { get; set; } = null!;
        /// <summary>
        /// Gets or sets whether this <see cref="ActionSettingDataTemplate"/> instance can be used for action settings
        ///  that don't specify a key as long as it has a matching ValueType.
        /// </summary>
        public bool IsExplicit { get; set; } = false;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Checks if this <see cref="ActionSettingDataTemplate"/> instance supports the specified <paramref name="valueType"/>.
        /// </summary>
        /// <param name="valueType">The value type of an action setting to check.</param>
        /// <returns><see langword="true"/> when the <paramref name="valueType"/> is supported and this instance is not explicit; otherwise <see langword="false"/>.</returns>
        public bool SupportsValueType(Type valueType)
        {
            if (IsExplicit || ValueType == null) return false;

            return ValueType.IsAssignableFrom(valueType);
        }
        #endregion Methods
    }
}
