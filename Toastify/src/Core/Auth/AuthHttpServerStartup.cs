using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebUtilities;
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
            logger.Debug($"Configuring {nameof(AuthHttpServerStartup)} at {url}");

            app.UseStaticFiles();
            //app.UseCors(builder => builder.AllowAnyOrigin());
            app.Use(async (context, next) =>
            {
                // CORS
                context.Response.Headers.Add("Access-Control-Allow-Origin", "https://aleab.github.io");
                context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

                // Read body
                string requestBody;
                using (StreamReader sr = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    requestBody = await sr.ReadToEndAsync();
                }

                logger.Debug($"[{nameof(AuthHttpServerStartup)}] {context.Request.Method}  {context.Request.Path.Value}{context.Request.QueryString.Value}  |  BODY: {requestBody}");

                if (context.Request.Scheme == url.Scheme &&
                    context.Request.Host.Equals(context.Request.Host) &&
                    context.Request.Path.Value == url.AbsolutePath)
                {
                    string code = null;
                    string state = null;
                    string error = null;

                    var body = QueryHelpers.ParseQuery(requestBody);
                    if (body.TryGetValue("code", out StringValues codeValues))
                        code = codeValues[0];
                    if (body.TryGetValue("state", out StringValues stateValues))
                        state = stateValues[0];
                    if (body.TryGetValue("error", out StringValues errorValues))
                        error = errorValues[0];

                    if (!string.IsNullOrWhiteSpace(code) || !string.IsNullOrWhiteSpace(error))
                    {
                        try
                        {
                            using (var pipe = new NamedPipeClientStream(".", this.Configuration["pipeName"], PipeDirection.Out, PipeOptions.None, TokenImpersonationLevel.Anonymous))
                            {
                                await pipe.ConnectAsync().ConfigureAwait(false);
                                StringStream ss = new StringStream(pipe);

                                var content = HttpUtility.ParseQueryString(string.Empty);
                                if (!string.IsNullOrWhiteSpace(code))
                                    content["code"] = code;
                                if (!string.IsNullOrWhiteSpace(error))
                                    content["state"] = state;
                                if (!string.IsNullOrWhiteSpace(error))
                                    content["error"] = error;

                                context.Response.StatusCode = (int)HttpStatusCode.OK;
                                ss.WriteString(content.ToString());
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
                    await next().ConfigureAwait(false);
            });
        }
    }
}