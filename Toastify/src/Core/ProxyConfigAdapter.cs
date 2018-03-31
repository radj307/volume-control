using Newtonsoft.Json;
using SpotifyAPI;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using ToastifyAPI.Core;

namespace Toastify.Core
{
    /// <summary>
    /// An adapter for <see cref="SpotifyAPI.ProxyConfig"/>
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class ProxyConfigAdapter : IProxyConfig
    {
        internal ProxyConfig ProxyConfig { get; }

        /// <inheritdoc />
        public string Host
        {
            get { return this.ProxyConfig.Host; }
            set { this.ProxyConfig.Host = value; }
        }

        /// <inheritdoc />
        public int Port
        {
            get { return this.ProxyConfig.Port; }
            set { this.ProxyConfig.Port = value; }
        }

        public string Username
        {
            get { return this.ProxyConfig.Username; }
            set { this.ProxyConfig.Username = value; }
        }

        [JsonIgnore]
        public string Password
        {
            get { return this.ProxyConfig.Password; }
            set { this.ProxyConfig.Password = value; }
        }

        /// <inheritdoc />
        public bool UseDefaultCredentials
        {
            get { return !string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Password); }
        }

        /// <inheritdoc />
        public bool BypassProxyOnLocal
        {
            get { return this.ProxyConfig.BypassProxyOnLocal; }
            set { this.ProxyConfig.BypassProxyOnLocal = value; }
        }

        public ProxyConfigAdapter() : this(new ProxyConfig())
        {
        }

        internal ProxyConfigAdapter(ProxyConfig proxyConfig)
        {
            this.ProxyConfig = new ProxyConfig();
            this.ProxyConfig.Set(proxyConfig);
        }

        internal void Set(ProxyConfig proxyConfig)
        {
            this.ProxyConfig.Set(proxyConfig);
        }

        /// <inheritdoc />
        public bool IsValid()
        {
            return this.ProxyConfig.IsValid();
        }

        /// <inheritdoc />
        public IWebProxy CreateWebProxy()
        {
            return this.ProxyConfig.CreateWebProxy();
        }

        /// <inheritdoc />
        public object Clone()
        {
            return new ProxyConfigAdapter(this.ProxyConfig);
        }

        public override string ToString()
        {
            return this.IsValid() ? $"{(!string.IsNullOrEmpty(this.Username) ? $"{this.Username}@" : "")}{this.Host}:{this.Port}" : "";
        }

        public string ToString(bool objectHash)
        {
            string @string = this.ToString();
            if (objectHash)
            {
                if (this.ProxyConfig != null)
                    @string += $" @ {RuntimeHelpers.GetHashCode(this.ProxyConfig)}";
                @string = @string.Trim();
            }

            return @string;
        }
    }
}