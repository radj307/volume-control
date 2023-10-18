using Newtonsoft.Json;
using System.Collections;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Core
{
    /// <summary>
    /// Specifies the target(s) of a hotkey action.
    /// </summary>
    [JsonArray]
    public class ActionTargetSpecifier : ICollection<string>
    {
        #region Properties
        /// <summary>
        /// List of targets.
        /// </summary>
        public ObservableImmutableList<string> Targets { get; } = new();
        #endregion Properties

        #region ICollection Implementation

        #region Properties
        /// <inheritdoc/>
        public int Count => ((ICollection<string>)this.Targets).Count;
        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<string>)this.Targets).IsReadOnly;
        #endregion Properties

        #region Methods
        /// <inheritdoc/>
        public void Add(string item) => ((ICollection<string>)this.Targets).Add(item);
        /// <inheritdoc/>
        public void Clear() => ((ICollection<string>)this.Targets).Clear();
        /// <inheritdoc/>
        public bool Contains(string item) => ((ICollection<string>)this.Targets).Contains(item);
        /// <inheritdoc/>
        public void CopyTo(string[] array, int arrayIndex) => ((ICollection<string>)this.Targets).CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)this.Targets).GetEnumerator();
        /// <inheritdoc/>
        public bool Remove(string item) => ((ICollection<string>)this.Targets).Remove(item);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.Targets).GetEnumerator();
        #endregion Methods

        #endregion ICollection Implementation
    }
}
