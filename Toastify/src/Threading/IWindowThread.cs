using System;
using System.Windows.Threading;

namespace Toastify.Threading
{
    public interface IWindowThread : IDisposable
    {
        #region Public Properties

        bool IsBackground { get; }
        string ThreadName { get; }
        Dispatcher Dispatcher { get; }

        #endregion

        void Start();
        void Abort();
        void Join();
        void Join(TimeSpan timeout);
        void Join(int millisecondsTimeout);

        void CloseWindow();
    }
}