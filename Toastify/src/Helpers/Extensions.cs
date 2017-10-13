using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Toastify.Helpers
{
    public static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static string ToStringInvariantCulture(this Exception exception)
        {
            CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            string message = exception.ToString();

            Thread.CurrentThread.CurrentUICulture = currentUICulture;
            return message;
        }

        public static bool CheckCancellation(this BackgroundWorker backgroundWorker, DoWorkEventArgs doWorkEventArgs)
        {
            if (backgroundWorker.CancellationPending)
                doWorkEventArgs.Cancel = true;
            return doWorkEventArgs.Cancel;
        }
    }
}