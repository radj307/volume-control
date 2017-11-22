using log4net;
using log4net.Util;
using System;
using System.Globalization;
using System.Threading;

namespace Toastify.Helpers
{
    internal static class Log4NetExtensions
    {
        public static void DebugInvariantCulture(this ILog log, string message, Exception exception)
        {
            CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            log.DebugExt(message, exception);

            Thread.CurrentThread.CurrentUICulture = currentUICulture;
        }

        public static void InfoInvariantCulture(this ILog log, string message, Exception exception)
        {
            CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            log.InfoExt(message, exception);

            Thread.CurrentThread.CurrentUICulture = currentUICulture;
        }

        public static void WarnInvariantCulture(this ILog log, string message, Exception exception)
        {
            CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            log.WarnExt(message, exception);

            Thread.CurrentThread.CurrentUICulture = currentUICulture;
        }

        public static void ErrorInvariantCulture(this ILog log, string message, Exception exception)
        {
            CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            log.ErrorExt(message, exception);

            Thread.CurrentThread.CurrentUICulture = currentUICulture;
        }

        public static void FatalInvariantCulture(this ILog log, string message, Exception exception)
        {
            CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            log.FatalExt(message, exception);

            Thread.CurrentThread.CurrentUICulture = currentUICulture;
        }
    }
}