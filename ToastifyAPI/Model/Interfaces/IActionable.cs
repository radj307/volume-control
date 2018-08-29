namespace ToastifyAPI.Model.Interfaces
{
    /// <summary>
    ///     Defines an object that can perform an action.
    /// </summary>
    public interface IActionable
    {
        #region Public Properties

        IAction Action { get; set; }

        float MaxFrequency { get; set; }

        #endregion

        void PerformAction();
    }
}