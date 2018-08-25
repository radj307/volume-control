using System;
using log4net;
using Newtonsoft.Json;
using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Model
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Actionable : IActionable
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Actionable));

        protected DateTime lastTimePerformed = DateTime.MinValue;

        #region Public Properties

        /// <inheritdoc />
        [JsonProperty]
        public IAction Action { get; set; }

        /// <summary>
        ///     Maximum amount of times the action can be performed in 1 second.
        /// </summary>
        public float MaxFrequency { get; set; } = 6.0f;

        /// <summary>
        ///     Whether the action can be performed or not.
        /// </summary>
        public virtual bool CanPerformAction
        {
            get
            {
                DateTime now = DateTime.Now;
                TimeSpan sinceLast = now.Subtract(this.lastTimePerformed);
                return sinceLast.TotalMilliseconds >= 1000.0 / this.MaxFrequency;
            }
        }

        #endregion

        public Actionable()
        {
        }

        public Actionable(IAction action)
        {
            this.Action = action;
        }

        /// <inheritdoc />
        public virtual void PerformAction()
        {
            try
            {
                if (this.Action != null && this.CanPerformAction)
                {
                    DateTime now = DateTime.Now;
                    this.Action.PerformAction();
                    this.lastTimePerformed = now;
                }
            }
            catch (Exception e)
            {
                logger.Error($"Error while performing action \"{this.Action?.Name}\".", e);
            }
        }

        #region Equals / GethashCode

        protected bool Equals(Actionable other)
        {
            return Equals(this.Action, other.Action);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == this.GetType() && this.Equals((Actionable)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Action != null ? this.Action.GetHashCode() : 0;
        }

        #endregion Equals / GethashCode
    }
}