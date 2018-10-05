using System;
using System.Windows;

namespace Toastify.Threading
{
    public class WindowThreadOptions<T> where T : Window, new()
    {
        #region Public Properties

        public Action<T> WindowInitialization { get; set; }
        public Action<T> BeforeWindowShownAction { get; set; }
        public Action<T> AfterWindowShownAction { get; set; }
        public Action<WindowThread<T>> OnWindowClosingAction { get; set; }

        #endregion
    }
}