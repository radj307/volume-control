using VolumeControl.TypeExtensions;

namespace VolumeControl.SDK
{
    /// <summary>
    /// Forwards event invocations only when the specified <see cref="Condition"/> returns <see langword="true"/>.<br/>
    /// This essentially acts like a switch that can prevent events from firing under certain circumstances.
    /// </summary>
    /// <remarks>
    /// To use, first identify an <b>input</b> <i>(an <see langword="event"/> definition)</i>, an <b>output</b> <i>(a compatible event handler method)</i>, and a <b>condition</b> <i>(A callback that fulfills <see cref="ConditionEvaluator"/>)</i>.<br/>
    /// <list type="number">
    /// <item><description>Connect your <b>input</b> <see langword="event"/> to this instance's <see cref="Handler(object?, object)"/> method.</description></item>
    /// <item><description>Connect this instance's <see cref="Event"/> trigger to your <b>output</b> handler.</description></item>
    /// <item><description>Set this instance's <see cref="Condition"/> property to your <b>condition</b> callback.</description></item>
    /// </list>
    /// When your <b>input</b> <see langword="event"/> is triggered, the <b>condition</b> is evaluated and if it returned <see langword="true"/> this instance's <see cref="Event"/> will fire with the same sender/args.
    /// </remarks>
    public class ConditionalEventForward
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="ConditionalEventForward"/> instance with the given <paramref name="condition"/> &amp; any number of <paramref name="customEventHandlers"/>..
        /// </summary>
        /// <param name="condition">The condition <see langword="delegate"/> to use when evaluating whether or not to forward an event.</param>
        /// <param name="customEventHandlers">Any number of <b>output</b> event handlers to connect.</param>
        public ConditionalEventForward(ConditionEvaluator condition, params EventHandler<object>[] customEventHandlers)
        {
            this.Condition = condition;
            _ = customEventHandlers.ForEach(handler => Event += handler);
        }
        #endregion Constructor

        #region Delegates
        /// <summary>
        /// A <see langword="delegate"/> that returns a <see cref="bool"/> and accepts sender &amp; event arguments parameters.
        /// </summary>
        /// <returns><see langword="true"/> causes the event to be forwarded via the <see cref="Event"/> event; <see langword="false"/> causes the event to be ignored, and <see cref="Event"/> is not triggered.</returns>
        public delegate bool ConditionEvaluator(object? sender, object e);
        #endregion Delegates

        #region Properties
        /// <summary>
        /// Gets or sets this instance's <b>condition</b>.<br/>
        /// If this is force-set to <see langword="null"/>, event triggers are always forwarded and the conditional evaluation step is skipped entirely.
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
            if (this.Condition != null && !this.Condition(sender, e)) return;
            Event?.Invoke(sender, e);
        }
        /// <inheritdoc cref="Handler(object?, object)"/>
        public void Handler<T>(object? sender, T e) where T : struct
        {
            if (this.Condition != null && !this.Condition(sender, e)) return;
            Event?.Invoke(sender, e);
        }
        #endregion EventHandlers
    };
}
