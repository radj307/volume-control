using System;
using System.Net;

namespace ToastifyAPI.Core
{
    public interface IProxyConfig : ICloneable
    {
        #region Public Properties

        string Host { get; }

        int Port { get; }

        bool UseDefaultCredentials { get; }

        /// <summary>
        ///     Whether to bypass the proxy server for local addresses.
        /// </summary>
        bool BypassProxyOnLocal { get; }

        #endregion

        bool IsValid();

        IWebProxy CreateWebProxy();
    }
}