using System;

namespace VolumeControl.Audio.Interfaces
{
    /// <summary>Base interface type for <see cref="IDevice"/> and <see cref="IProcess"/>/<see cref="ISession"/>.</summary>
    /// <remarks>This is an empty interface.</remarks>
    public interface ITargetable
    {
        /// <summary>
        /// A constant hashcode used when indexing targets.
        /// </summary>
        //int HashCode { get; }
    }
}
