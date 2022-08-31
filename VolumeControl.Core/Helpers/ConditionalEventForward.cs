using VolumeControl.TypeExtensions;

namespace VolumeControl.Core.Helpers
{
    /// <summary>
    /// Forwards event triggers when the specified <see cref="Condition"/> is <see langword="true"/>.
    /// </summary>
    public class ConditionalEventForward
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="ConditionalEventForward"/> instance with the given <paramref name="condition"/> &amp; any number of <paramref name="customEventHandlers"/>..
        /// </summary>
        /// <param name="condition">The condition <see langword="delegate"/> to use when evaluating whether or not to forward an event.</param>
        /// <param name="customEventHandlers">Any number of custom event handlers to bind from within the constructor. <i>(See <see cref="EventHandler"/>)</i></param>
        public ConditionalEventForward(ConditionEvaluator condition, params EventHandler<object>[] customEventHandlers)
        {
            Condition = condition;
            customEventHandlers.ForEach(handler => Event += handler);
        }
        #endregion Constructor

        #region Delegates
        /// <summary>
        /// A <see langword="delegate"/> method or lambda that determines whether or not events received through the <see cref="Handler(object?, object)"/> are forwarded to the <see cref="Event"/>.
        /// </summary>
        /// <returns><see langword="true"/> causes the event to be forwarded via the <see cref="Event"/> event; <see langword="false"/> causes the event to be ignored, and <see cref="Event"/> is not triggered.</returns>
        public delegate bool ConditionEvaluator();
        #endregion Delegates

        #region Properties
        /// <summary>
        /// A <see langword="delegate"/> of type <see cref="ConditionEvaluator"/> that, when it returns <see langword="true"/>, forwards the event sender &amp; arguments to the <see cref="Event"/> event trigger.
        /// </summary>
        public ConditionEvaluator Condition { get; set; }
        #endregion Properties

        #region Events
        /// <summary>
        /// Triggered whenever the <see cref="Handler(object?, object)"/> is called and <see cref="Condition"/> returns <see langword="true"/>.
        /// </summary>
        public EventHandler<object>? Event;
        #endregion Events

        #region EventHandlers
        /// <summary>
        /// An event handler that can be attached to any <see cref="EventHandler"/> or <see cref="EventHandler{TEventArgs}"/> compatible event trigger.<br/>
        /// Note that if the generic TEventArgs type of the event handler is <b>not an object type</b>, such as a tuple, you will have to 'wrap' the handler call with a lambda:
        /// <code>
        /// ConditionalEventForward cef = new(() => allowEventCondition);
        /// MyEvent += (s, e) => cef.Handler(s, e);
        /// </code>
        /// </summary>
        /// <param name="sender">This is forwarded to the first parameter in any handlers attached to <see cref="Event"/>.</param>
        /// <param name="e">This is forwarded to the second parameter in any handlers attached to <see cref="Event"/>.</param>
        public void Handler(object? sender, object e)
        {
            if (!Condition()) return;
            Event?.Invoke(sender, e);
        }
        #endregion EventHandlers
    };
}
