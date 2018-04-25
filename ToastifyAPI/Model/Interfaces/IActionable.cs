namespace ToastifyAPI.Model.Interfaces
{
    public interface IActionable
    {
        IAction Action { get; set; }

        void PerformAction();
    }
}