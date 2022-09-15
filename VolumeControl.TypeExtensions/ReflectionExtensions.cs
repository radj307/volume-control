using System.ComponentModel;
using System.Reflection;

namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extension methods that use C#'s reflection capabilities.
    /// </summary>
    public static class ReflectionExtensions
    {
        #region RaisePropertyChanged
        /// <summary>
        /// Raises the PropertyChanged event on the given object that implements <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <param name="bindableObject">An <see cref="object"/> instance that implements <see cref="INotifyPropertyChanged"/>.</param>
        /// <param name="propertyName">The name of the property on <paramref name="bindableObject"/> to raise an event for.</param>
        /// <param name="sender">The value to pass as the sender parameter to all attached <see cref="PropertyChangedEventHandler"/> instances.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance to pass as the event args parameter to all attached <see cref="PropertyChangedEventHandler"/> instances.</param>
        public static void RaisePropertyChanged(this INotifyPropertyChanged bindableObject, string propertyName, object? sender, PropertyChangedEventArgs e)
        {
            if (bindableObject.GetType().GetProperty(propertyName) is null) return;

            // get the internal eventDelegate
            var bindableObjectType = bindableObject.GetType();

            // search the base type, which contains the PropertyChanged event field.
            FieldInfo? propChangedFieldInfo = null;
            while (bindableObjectType != null)
            {
                propChangedFieldInfo = bindableObjectType.GetField(nameof(INotifyPropertyChanged.PropertyChanged), BindingFlags.Instance | BindingFlags.NonPublic);
                if (propChangedFieldInfo != null)
                    break;

                bindableObjectType = bindableObjectType.BaseType;
            }
            if (propChangedFieldInfo == null)
                return;

            // get prop changed event field value
            if (propChangedFieldInfo.GetValue(bindableObject) is not object fieldValue)
                return;

            if (fieldValue is not MulticastDelegate eventDelegate)
                return;

            // get invocation list
            Delegate[] delegates = eventDelegate.GetInvocationList();

            // invoke each delegate
            foreach (Delegate propertyChangedDelegate in delegates)
            {
                propertyChangedDelegate.Method.Invoke(propertyChangedDelegate.Target, new object?[] { sender, e });
            }
        }
        /// <summary>
        /// Raises the PropertyChanged event on the given object that implements <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <param name="bindableObject">An <see cref="object"/> instance that implements <see cref="INotifyPropertyChanged"/>.</param>
        /// <param name="propertyName">The name of the property on <paramref name="bindableObject"/> to raise an event for.</param>
        /// <param name="sender">The value to pass as the sender parameter to all attached <see cref="PropertyChangedEventHandler"/> instances.</param>
        public static void RaisePropertyChanged(this INotifyPropertyChanged bindableObject, string propertyName, object? sender) => RaisePropertyChanged(bindableObject, propertyName, sender, new PropertyChangedEventArgs(propertyName));
        /// <summary>
        /// Raises the PropertyChanged event on the given object that implements <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <param name="bindableObject">An <see cref="object"/> instance that implements <see cref="INotifyPropertyChanged"/>.</param>
        /// <param name="propertyName">The name of the property on <paramref name="bindableObject"/> to raise an event for.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance to pass as the event args parameter to all attached <see cref="PropertyChangedEventHandler"/> instances.</param>
        public static void RaisePropertyChanged(this INotifyPropertyChanged bindableObject, string propertyName, PropertyChangedEventArgs e) => RaisePropertyChanged(bindableObject, propertyName, bindableObject, e);
        /// <summary>
        /// Raises the PropertyChanged event on the given object that implements <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <param name="bindableObject">An <see cref="object"/> instance that implements <see cref="INotifyPropertyChanged"/>.</param>
        /// <param name="propertyName">The name of the property on <paramref name="bindableObject"/> to raise an event for.</param>
        public static void RaisePropertyChanged(this INotifyPropertyChanged bindableObject, string propertyName) => RaisePropertyChanged(bindableObject, propertyName, bindableObject, new PropertyChangedEventArgs(propertyName));
        #endregion RaisePropertyChanged

        #region RaiseEvent
        /// <summary>
        /// Raises the event specified by <paramref name="eventName"/> on the given <paramref name="source"/> <see cref="object"/> by invoking all of its bound handler delegates.
        /// </summary>
        /// <param name="source">The object instance <i>(or <see cref="Type"/> for <see langword="static"/> events/objects)</i> on which to raise an event.</param>
        /// <param name="eventName">The name of the <see langword="event"/> property in the <paramref name="source"/> type.</param>
        /// <param name="args">The arguments to invoke the event handlers with.</param>
        public static void RaiseEvent(this object source, string eventName, object[] args)
        {
            if (source.GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic) is FieldInfo fInfo)
            {
                if (fInfo?.GetValue(source) is MulticastDelegate multicastDelegate)
                {
                    foreach (Delegate handler in multicastDelegate.GetInvocationList())
                    {
                        handler.Method.Invoke(handler.Target, args);
                    }
                }
            }            
        }

        /// <summary>
        /// Raises the event named <paramref name="eventName"/> on the object <paramref name="source"/> with the given <paramref name="sender"/> &amp; <paramref name="eventArgs"/> parameters.
        /// <i>Src: <see href="https://stackoverflow.com/a/586156/8705305"/></i>
        /// </summary>
        /// <typeparam name="TEventArgs">Any type derived from <see cref="EventArgs"/></typeparam>
        /// <param name="source">The object instance <i>(or <see cref="Type"/> for <see langword="static"/> events/objects)</i> on which to raise an event.</param>
        /// <param name="eventName">The name of the <see langword="event"/> property in the <paramref name="source"/> type.</param>
        /// <param name="sender">The sender parameter to use when invoking the event handlers.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/>-derived instance to use for the event args parameter when invoking the event handlers.</param>
        public static void RaiseEvent<TEventArgs>(this object source, string eventName, object? sender, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            if (source.GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(source) is MulticastDelegate eventDelegate)
            {
                foreach (var handler in eventDelegate.GetInvocationList())
                {
                    handler.Method.Invoke(handler.Target, new object?[] { sender, eventArgs });
                }
            }
        }
        /// <summary>
        /// Raises the event named <paramref name="eventName"/> on the object <paramref name="source"/> with the given <paramref name="eventArgs"/> parameters.
        /// <i>Src: <see href="https://stackoverflow.com/a/586156/8705305"/></i>
        /// </summary>
        /// <typeparam name="TEventArgs">Any type derived from <see cref="EventArgs"/></typeparam>
        /// <param name="source">The object instance <i>(or <see cref="Type"/> for <see langword="static"/> events/objects)</i> on which to raise an event.</param>
        /// <param name="eventName">The name of the <see langword="event"/> property in the <paramref name="source"/> type.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/>-derived instance to use for the event args parameter when invoking the event handlers.</param>
        public static void RaiseEvent<TEventArgs>(this object source, string eventName, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            if (source.GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(source) is MulticastDelegate eventDelegate)
            {
                foreach (var handler in eventDelegate.GetInvocationList())
                {
                    handler.Method.Invoke(handler.Target, new object?[] { source, eventArgs });
                }
            }
        }
        /// <summary>
        /// Raises the event named <paramref name="eventName"/> on the object <paramref name="source"/> with the given <paramref name="sender"/> &amp; <paramref name="eventArgs"/> parameters.
        /// <i>Src: <see href="https://stackoverflow.com/a/586156/8705305"/></i>
        /// </summary>
        /// <param name="source">The object instance <i>(or <see cref="Type"/> for <see langword="static"/> events/objects)</i> on which to raise an event.</param>
        /// <param name="eventName">The name of the <see langword="event"/> property in the <paramref name="source"/> type.</param>
        /// <param name="sender">The sender parameter to use when invoking the event handlers.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/>-derived instance to use for the event args parameter when invoking the event handlers.</param>
        public static void RaiseEvent(this object source, string eventName, object? sender, object eventArgs)
        {
            if (source.GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(source) is MulticastDelegate eventDelegate)
            {
                foreach (var handler in eventDelegate.GetInvocationList())
                {
                    handler.Method.Invoke(handler.Target, new object?[] { sender, eventArgs });
                }
            }
        }
        /// <summary>
        /// Raises the event named <paramref name="eventName"/> on the object <paramref name="source"/> with the given <paramref name="eventArgs"/> parameters.
        /// <i>Src: <see href="https://stackoverflow.com/a/586156/8705305"/></i>
        /// </summary>
        /// <param name="source">The object instance <i>(or <see cref="Type"/> for <see langword="static"/> events/objects)</i> on which to raise an event.</param>
        /// <param name="eventName">The name of the <see langword="event"/> property in the <paramref name="source"/> type.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/>-derived instance to use for the event args parameter when invoking the event handlers.</param>
        public static void RaiseEvent(this object source, string eventName, object eventArgs)
        {
            if (source.GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(source) is MulticastDelegate eventDelegate)
            {
                foreach (var handler in eventDelegate.GetInvocationList())
                {
                    handler.Method.Invoke(handler.Target, new object?[] { source, eventArgs });
                }
            }
        }
        #endregion RaiseEvent
    }
}
