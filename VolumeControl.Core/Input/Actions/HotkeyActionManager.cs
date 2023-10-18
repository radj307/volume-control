namespace VolumeControl.Core.Input.Actions
{
    /// <summary>
    /// Manages a list of <see cref="HotkeyActionDefinition"/>s.
    /// </summary>
    public class HotkeyActionManager
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="HotkeyActionManager"/> instance.
        /// </summary>
        public HotkeyActionManager() { }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the list of currently-loaded <see cref="HotkeyActionDefinition"/> objects.
        /// </summary>
        public IReadOnlyList<HotkeyActionDefinition> ActionDefinitions => _actionDefinitions;
        private readonly List<HotkeyActionDefinition> _actionDefinitions = new();
        #endregion Properties

        #region Events
        /// <summary>
        /// Occurs when an action definition is added to the list for any reason.
        /// </summary>
        public event EventHandler<HotkeyActionDefinition>? AddedActionDefinition;
        private void NotifyAddedActionDefinition(HotkeyActionDefinition actionDefinition) => AddedActionDefinition?.Invoke(this, actionDefinition);
        /// <summary>
        /// Occurs when an action definition is removed from the list for any reason.
        /// </summary>
        public event EventHandler<HotkeyActionDefinition>? RemovedActionDefinition;
        private void NotifyRemovedActionDefinition(HotkeyActionDefinition actionDefinition) => RemovedActionDefinition?.Invoke(this, actionDefinition);
        #endregion Events

        #region Methods

        #region FindActionDefinition
        /// <summary>
        /// Gets the hotkey action definition with the specified <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">The identifier string associated with the action definition to get.</param>
        /// <param name="stringComparison">The comparison type to use for string comparisons.</param>
        /// <returns>The <see cref="HotkeyActionDefinition"/> instance with the specified <paramref name="identifier"/> if found; otherwise <see langword="null"/>.</returns>
        public HotkeyActionDefinition? FindActionDefinition(string identifier, StringComparison stringComparison = StringComparison.Ordinal)
            => ActionDefinitions.FirstOrDefault(actionDef => actionDef.Identifier.Equals(identifier, stringComparison));
        #endregion FindActionDefinition

        #region Add/Remove ActionDefinition
        /// <summary>
        /// Adds the specified <paramref name="actionDefinition"/> to the manager.
        /// </summary>
        /// <param name="actionDefinition">A <see cref="HotkeyActionDefinition"/> instance to add to the list.</param>
        /// <exception cref="InvalidOperationException">The specified <paramref name="actionDefinition"/> is already in the list.</exception>
        public void AddActionDefinition(HotkeyActionDefinition actionDefinition)
        {
            if (ActionDefinitions.Contains(actionDefinition))
                throw new InvalidOperationException($"Action instance '{actionDefinition.Identifier}' is already being managed by this {nameof(HotkeyActionManager)} instance!");

            _actionDefinitions.Add(actionDefinition);
            NotifyAddedActionDefinition(actionDefinition);
        }
        /// <summary>
        /// Removes the specified <paramref name="actionDefinition"/> from the manager.
        /// </summary>
        /// <param name="actionDefinition">A <see cref="HotkeyActionDefinition"/> instance to remove from the list.</param>
        /// <exception cref="InvalidOperationException">The specified <paramref name="actionDefinition"/> is not in the list.</exception>
        public void RemoveActionDefinition(HotkeyActionDefinition actionDefinition)
        {
            if (!ActionDefinitions.Contains(actionDefinition))
                throw new InvalidOperationException($"Action instance '{actionDefinition.Identifier}' is not being managed by this {nameof(HotkeyActionManager)} instance!");

            _actionDefinitions.Remove(actionDefinition);
            NotifyRemovedActionDefinition(actionDefinition);
        }
        /// <summary>
        /// Adds the specified <paramref name="actionDefinitions"/> to the manager.
        /// </summary>
        /// <param name="actionDefinitions">Any number of <see cref="HotkeyActionDefinition"/> instances to add to the list.</param>
        public void AddActionDefinitions(params HotkeyActionDefinition[] actionDefinitions)
        {
            for (int i = 0, max = actionDefinitions.Length; i < max; ++i)
            {
                AddActionDefinition(actionDefinitions[i]);
            }
        }
        #endregion Add/Remove ActionDefinition

        #endregion Methods
    }
}
