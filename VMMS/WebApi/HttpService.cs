using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace VMMS
{
        public class HttpService : IDisposable
        {
            /// <summary>
            /// 端口号
            /// </summary>
            public int Port { get; set; }

            /// <summary>
            /// Http self hosting
            /// </summary>
            private readonly HttpSelfHostServer _server;

            public HttpService(int port)
            {
                this.Port = port;
                var config = new HttpSelfHostConfiguration($"http://localHost:{this.Port}");

                config.MapHttpAttributeRoutes();
                config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}");
                _server = new HttpSelfHostServer(config);
            }

            #region HTTP Service
            /// <summary>
            /// start HTTP server
            /// </summary>
            /// <returns></returns>
            public Task StartHttpServer()
            {
                return _server.OpenAsync();
            }
            /// <summary>
            /// Close HTTP service
            /// </summary>
            /// <returns></returns>
            public Task CloseHttpServer()
            {
                return _server.CloseAsync();
            }
            #endregion


            public void Dispose()
            {
                _server.Dispose();
            }
        }
}
