using JetBrains.Annotations;
using System;
using ToastifyAPI.Events;

namespace ToastifyAPI.Model.Interfaces
{
    public interface IAction : IEquatable<IAction>
    {
        [NotNull]
        string Name { get; }

        void PerformAction();

        event EventHandler ActionPerformed;

        event EventHandler<ActionFailedEventArgs> ActionFailed;
    }
}