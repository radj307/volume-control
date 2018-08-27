using System;
using JetBrains.Annotations;
using ToastifyAPI.Events;

namespace ToastifyAPI.Model.Interfaces
{
    public interface IAction : IEquatable<IAction>, ICloneable
    {
        #region Public Properties

        [NotNull]
        string Name { get; }

        #endregion

        #region Events

        event EventHandler ActionPerformed;

        event EventHandler<ActionFailedEventArgs> ActionFailed;

        #endregion

        void PerformAction();
    }
}