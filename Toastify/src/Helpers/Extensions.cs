using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Toastify.Helpers
{
    public static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static bool CheckCancellation(this BackgroundWorker backgroundWorker, DoWorkEventArgs doWorkEventArgs)
        {
            if (backgroundWorker.CancellationPending)
                doWorkEventArgs.Cancel = true;
            return doWorkEventArgs.Cancel;
        }
    }
}