using System;
using System.Windows;

namespace VMMS
{

    public class WebApi
    {
        /// <summary>
        /// 启动和停止WebApi
        /// </summary>
        private HttpService _http;
        public async void StartWebApi()
        {
            try
            {
                var port = 8080;

                _http = new HttpService(port);

                await _http.StartHttpServer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //LogHelper.WriteLogByType(LogEnum.Error, ex.Message);
            }
        }
        public async void StopWebApi()
        {
            try
            {
                await _http.CloseHttpServer();
                _http.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //LogHelper.WriteLogByType(LogEnum.Error, ex.Message);
            }
        }
    }
}
