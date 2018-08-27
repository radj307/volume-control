using System;
using Newtonsoft.Json;
using Toastify.Core;
using ToastifyAPI.Events;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Model
{
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class ToastifyAction : IAction
    {
        #region Public Properties

        /// <inheritdoc />
        public virtual string Name { get; }

        public virtual ToastifyActionEnum ToastifyActionEnum { get; }

        #endregion

        #region Events

        /// <inheritdoc />
        public event EventHandler ActionPerformed;

        /// <inheritdoc />
        public event EventHandler<ActionFailedEventArgs> ActionFailed;

        #endregion

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

        protected virtual void RaiseActionPerformed(object sender)
        {
            this.ActionPerformed?.Invoke(sender, EventArgs.Empty);
        }

        protected virtual void RaiseActionFailed(object sender, ActionFailedEventArgs e)
        {
            this.ActionFailed?.Invoke(sender, e);
        }

        public sealed override string ToString()
        {
            return this.Name;
        }

        public virtual object Clone()
        {
            return (ToastifyAction)this.MemberwiseClone();
        }

        #region Equals / GethashCode

        protected bool Equals(ToastifyAction other)
        {
            return string.Equals(this.Name, other.Name) &&
                   this.ToastifyActionEnum == other.ToastifyActionEnum;
        }

        public bool Equals(IAction other)
        {
            if (other == null)
                return false;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(this.Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == this.GetType() && this.Equals((ToastifyAction)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Name.GetHashCode() * 397) ^ this.ToastifyActionEnum.GetHashCode();
            }
        }

        #endregion
    }
}