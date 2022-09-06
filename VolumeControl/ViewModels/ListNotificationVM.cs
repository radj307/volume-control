using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using VolumeControl.Core;
using VolumeControl.Core.Interfaces;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

namespace VolumeControl.ViewModels
{
    /// <summary>
    /// View model for the <see cref="ListNotification"/> window.
    /// </summary>
    public class ListNotificationVM : INotifyPropertyChanged
    {
        #region Statics
        private static LogWriter Log => FLog.Log;
        private static Config Settings => (Config.Default as Config)!;
        #endregion Statics

        #region Properties
        public List<ListDisplayTarget> DisplayTargets { get; } = new();
        private ListDisplayTarget? _currentDisplayTarget;
        /// <summary>
        /// The currently-selected display target.
        /// </summary>
        /// <remarks>
        /// This property's setter method calls <see cref="SetDisplayTarget(ListDisplayTarget)"/> internally, so <b>it is safe to use</b>.
        /// </remarks>
        public ListDisplayTarget? CurrentDisplayTarget
        {
            get => _currentDisplayTarget;
            set
            {
                if (value is null) return;
                this.SetDisplayTarget(value);
            }
        }
        /// <summary>
        /// The <see cref="CurrentDisplayTarget"/>'s <see cref="ListDisplayTarget.ItemsSource"/> property.
        /// </summary>
        public IEnumerable<IListDisplayable>? ItemsSource => this.CurrentDisplayTarget?.ItemsSource;
        /// <summary>
        /// The <see cref="CurrentDisplayTarget"/>'s <see cref="ListDisplayTarget.SelectedItem"/> property.
        /// </summary>
        public IListDisplayable? SelectedItem
        {
            get => this.CurrentDisplayTarget?.SelectedItem;
            set
            {
                if (this.CurrentDisplayTarget is null) return;
                this.CurrentDisplayTarget.SelectedItem = value;
            }
        }
        /// <summary>
        /// The <see cref="CurrentDisplayTarget"/>'s <see cref="ListDisplayTarget.LockSelection"/> property.
        /// </summary>
        public bool LockSelection
        {
            get => this.CurrentDisplayTarget?.LockSelection ?? false;
            set
            {
                if (this.CurrentDisplayTarget is null) return;
                this.CurrentDisplayTarget.LockSelection = value;
            }
        }
        /// <summary>
        /// The <see cref="CurrentDisplayTarget"/>'s <see cref="ListDisplayTarget.Background"/> property.
        /// </summary>
        public Brush Background
        {
            get => this.CurrentDisplayTarget?.Background ?? Config.NotificationDefaultBrush;
            set
            {
                if (this.CurrentDisplayTarget is null) return;
                this.CurrentDisplayTarget.Background = value;
            }
        }
        #endregion Properties

        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        /// <summary>
        /// Notifies the <see cref="ListNotification"/> window that it should become visible.
        /// </summary>
        internal event EventHandler<object>? Show;
        #endregion Events

        #region Methods
        #region ...DisplayTargets
        /// <summary>
        /// Adds the given <paramref name="displayable"/> to the <see cref="DisplayTargets"/> list if it isn't already present.
        /// </summary>
        /// <param name="displayable">An enumerable list of objects that implement <see cref="IListDisplayable"/>.</param>
        public bool AddDisplayTarget(ListDisplayTarget displayable)
        {
            bool result = this.DisplayTargets.AddIfUnique(displayable);
            this.NotifyPropertyChanged(nameof(this.DisplayTargets));
            return result;
        }
        /// <summary>
        /// Creates a new <see cref="ListDisplayTarget"/> instance and adds it to the <see cref="DisplayTargets"/> list.
        /// </summary>
        /// <param name="bindings">Any number of property bindings to set on the new instance.</param>
        /// <returns>The new <see cref="ListDisplayTarget"/> instance, which has already been added to the list. You can use this reference to add <see cref="ListDisplayTarget.ConditionalShowTriggers"/> to the new instance.</returns>
        public ListDisplayTarget AddDisplayTarget(string name, IEnumerable<(DependencyProperty, BindingBase)> bindings)
        {
            ListDisplayTarget displayable = new(name);

            // Apply property bindings:
            bindings.ForEach((dp, binding) => displayable.SetBinding(dp, binding));

            // add the new display target
            _ = this.AddDisplayTarget(displayable);

            // return the new display target
            return displayable;
        }
        /// <summary>
        /// Creates a new <see cref="ListDisplayTarget"/> instance and adds it to the <see cref="DisplayTargets"/> list.
        /// </summary>
        /// <param name="bindings">Any number of property bindings to set on the new instance.</param>
        /// <returns>The new <see cref="ListDisplayTarget"/> instance, which has already been added to the list. You can use this reference to add <see cref="ListDisplayTarget.ConditionalShowTriggers"/> to the new instance.</returns>
        public ListDisplayTarget AddDisplayTarget(string name, params (DependencyProperty, BindingBase)[] bindings) => this.AddDisplayTarget(name, bindings.AsEnumerable());
        /// <summary>
        /// Sets the active display target to the specified <paramref name="displayable"/>.
        /// </summary>
        /// <remarks>
        /// This method triggers the necessary <see cref="PropertyChanged"/> events to prevent the view from using stale data.
        /// </remarks>
        /// <param name="displayable">An enumerable list of objects that implement <see cref="IListDisplayable"/>. If this isn't present in <see cref="DisplayTargets"/>, it is added.</param>
        public void SetDisplayTarget(ListDisplayTarget displayable)
        {
            if (!this.DisplayTargets.Contains(displayable)) _ = this.AddDisplayTarget(displayable);

            if (_currentDisplayTarget is not null)
            {
                // unhook event triggers from outgoing display target
                _currentDisplayTarget.ShowEvent -= this.ListDisplayTarget_ShowEvent;

                // set the display target to null
                _currentDisplayTarget = null;
                this.NotifyPropertyChanged(nameof(this.CurrentDisplayTarget));
                this.NotifyPropertyChanged(nameof(this.ItemsSource));
                this.NotifyPropertyChanged(nameof(this.SelectedItem));
                this.NotifyPropertyChanged(nameof(this.LockSelection));
                this.NotifyPropertyChanged(nameof(this.Background));
            }

            // set the new display target
            _currentDisplayTarget = displayable;
            this.NotifyPropertyChanged(nameof(this.CurrentDisplayTarget));
            this.NotifyPropertyChanged(nameof(this.ItemsSource));
            this.NotifyPropertyChanged(nameof(this.SelectedItem));
            this.NotifyPropertyChanged(nameof(this.LockSelection));
            this.NotifyPropertyChanged(nameof(this.Background));

            // hook up event triggers for incoming display target
            _currentDisplayTarget.ShowEvent += this.ListDisplayTarget_ShowEvent;
        }
        /// <summary>
        /// Removes the specified <paramref name="displayable"/> from <see cref="DisplayTargets"/>.
        /// </summary>
        /// <param name="displayable">An enumerable list of objects that implement <see cref="IListDisplayable"/>.</param>
        public bool RemoveDisplayTarget(ListDisplayTarget displayable)
        {
            bool result = this.DisplayTargets.Remove(displayable);
            this.NotifyPropertyChanged(nameof(this.DisplayTargets));
            return result;
        }
        /// <summary>
        /// Checks if the specified <paramref name="displayable"/> instance is the <see cref="CurrentDisplayTarget"/>, using the default <see cref="ListDisplayTarget.Equals"/>
        /// </summary>
        /// <param name="displayable"></param>
        /// <returns></returns>
        public bool IsDisplayTarget(ListDisplayTarget displayable) => displayable.Equals(this.CurrentDisplayTarget);
        #endregion ...DisplayTargets
        #endregion Methods

        #region EventHandlers
        private void ListDisplayTarget_ShowEvent(object? sender, object e) => Show?.Invoke(sender, e);
        #endregion EventHandlers
    }
}
