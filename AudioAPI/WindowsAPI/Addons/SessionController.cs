namespace AudioAPI.WindowsAPI.Addons
{
    public class SessionController
    {
        public SessionController(ISessionController sessionController)
        {
            Controller = sessionController;
            IsVirtual = false;
        }
        public SessionController()
        {
            Controller = null;
            IsVirtual = true;
        }

        public ISessionController? Controller;
        public bool IsVirtual { get; }
    }
}