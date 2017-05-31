using System;

namespace Toastify.Events
{
    internal class CheckVersionCompleteEventArgs : EventArgs
    {
        public string Version { get; set; }
        public bool New { get; set; }
    }
}