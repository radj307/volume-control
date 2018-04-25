using Newtonsoft.Json;
using System;
using Toastify.Core;
using ToastifyAPI.Events;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Model
{
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class ToastifyAction : IAction
    {
        /// <inheritdoc />
        public virtual string Name { get; }

        public virtual ToastifyActionEnum ToastifyActionEnum { get; }

        /// <inheritdoc />
        public event EventHandler ActionPerformed;

        /// <inheritdoc />
        public event EventHandler<ActionFailedEventArgs> ActionFailed;

        protected ToastifyAction() : this(string.Empty)
        {
        }

        protected ToastifyAction(string name) : this(name, ToastifyActionEnum.None)
        {
        }

        [JsonConstructor]
        protected ToastifyAction(string name, ToastifyActionEnum actionEnum)
        {
            this.Name = name;
            this.ToastifyActionEnum = actionEnum;
        }

        /// <inheritdoc />
        public abstract void PerformAction();

        #region Equals / GetHashCode

        protected bool Equals(ToastifyAction other)
        {
            return this.Equals((IAction)other);
        }

        /// <inheritdoc />
        public bool Equals(IAction other)
        {
            if (other is null)
                return false;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(this.Name, other.Name);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == this.GetType() && this.Equals((ToastifyAction)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        #endregion Equals / GetHashCode

        protected virtual void RaiseActionPerformed(object sender)
        {
            this.ActionPerformed?.Invoke(sender, EventArgs.Empty);
        }

        protected virtual void RaiseActionFailed(object sender, ActionFailedEventArgs e)
        {
            this.ActionFailed?.Invoke(sender, e);
        }
    }
}