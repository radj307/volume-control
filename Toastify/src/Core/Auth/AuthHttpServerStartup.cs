using System;
using System.IO.Pipes;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Toastify.Common;

namespace Toastify.Core.Auth
{
    public class AuthHttpServerStartup
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(AuthHttpServerStartup));

        #region Public Properties

        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        #endregion

        public AuthHttpServerStartup(IHostingEnvironment env, IConfiguration config)
        {
            this.HostingEnvironment = env;
            this.Configuration = config;
        }

        // ReSharper disable once UnusedMember.Global
        public void ConfigureServices(IServiceCollection services)
        {
            // No services to configure
        }

        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Uri url = new Uri(this.Configuration["url"]);
            app.Use(async (context, next) =>
            {
                logger.Debug($"[{nameof(AuthHttpServerStartup)}] {context.Request.Path.Value}{context.Request.QueryString.Value}");

                if (context.Request.Scheme == url.Scheme &&
                    context.Request.Host.Equals(context.Request.Host) &&
                    context.Request.Path.Value == url.AbsolutePath &&
                    context.Request.QueryString.HasValue && !string.IsNullOrWhiteSpace(context.Request.QueryString.Value))
                {
                    string code = null;
                    string state = null;
                    string error = null;

                    var query = context.Request.Query;
                    if (query.TryGetValue("code", out StringValues codeValues))
                        code = codeValues[0];
                    if (query.TryGetValue("state", out StringValues stateValues))
                        state = stateValues[0];
                    if (query.TryGetValue("error", out StringValues errorValues))
                        error = errorValues[0];

                    if (!string.IsNullOrWhiteSpace(code) || !string.IsNullOrWhiteSpace(error))
                    {
                        try
                        {
                            using (var pipe = new NamedPipeClientStream(".", this.Configuration["pipeName"], PipeDirection.Out, PipeOptions.None, TokenImpersonationLevel.Anonymous))
                            {
                                await pipe.ConnectAsync();
                                StringStream ss = new StringStream(pipe);

                                var content = HttpUtility.ParseQueryString(string.Empty);
                                if (!string.IsNullOrWhiteSpace(code))
                                    content["code"] = code;
                                if (!string.IsNullOrWhiteSpace(error))
                                    content["state"] = state;
                                if (!string.IsNullOrWhiteSpace(error))
                                    content["error"] = error;

                                ss.WriteString(content.ToString());
                                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Error($"[{nameof(AuthHttpServerStartup)}] ERROR: {e}");
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        }
                    }
                    else
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                    await next();
            });
        }
    }
}