using System.Net;
using System.Net.Http.Headers;

namespace ToastifyAPI.GitHub.Model
{
    public abstract class BaseModel
    {
        public HttpResponseHeaders HttpResponseHeaders { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }
    }
}