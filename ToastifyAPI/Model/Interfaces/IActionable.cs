namespace ToastifyAPI.Model.Interfaces
{
    public interface IActionable
    {
        #region Public Properties

        IAction Action { get; set; }

        #endregion

        void PerformAction();
    }
}