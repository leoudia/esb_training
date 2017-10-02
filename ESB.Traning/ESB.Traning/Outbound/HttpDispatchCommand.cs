using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ESB.Training.Factory;
using ESB.Training.Model;
using ESB.Training.Outbound.Gateway;
using ESB.Training.Util;

namespace ESB.Training.Outbound
{
    public class HttpDispatchCommand : IDispachCommand
    {
        public HttpDispatchCommand()
        {
        }

        public async Task Dispatch(ESBContext context, 
                             Inbound.ResponseHandle responseHandle)
        {
            try
            {
				//using (HttpClientHandler handler = new HttpClientHandler())
				//{
					//handler.ServerCertificateCustomValidationCallback = delegate {

					//	return true;
					//};

					//using (HttpClient client = new HttpClient(handler))
                using (HttpClient client = new HttpClient())
				{

					HttpContent content = null;
					if (context.Body != null && context.Body.Length > 0)
					{
						content =
							new ByteArrayContent(context.Body);

                        content.Headers.ContentType =
                            new MediaTypeHeaderValue(GetConteType(context));
					}

                    HttpRequestMessage msg = new HttpRequestMessage()
                    {
                        Content = content
                    };

                    msg.Headers.Add("User-Agent", ESBContants.HEADER_USER_AGENTE);
					msg.Headers.Add("X-Forwarded-For", context.IP);
					msg.Headers.Add("Accept", GetAccept(context));
					msg.Method = new HttpMethod(context.Method);
                    msg.RequestUri = CreateUri(context);
					var resp = await client.SendAsync(msg);
					
                    Dictionary<string, string> headersMap = new Dictionary<string, string>();

                    var headers = resp.Headers.GetEnumerator();
                    while(headers.MoveNext())
                    {
                        headersMap.Add(headers.Current.Key, 
                                       string.Join(",", headers.Current.Value));
                    }

                    string contentType = resp.Content != null 
                                             && resp.Content.Headers != null
                                             ? resp.Content.Headers.ContentType.ToString() : null;

                    byte[] bytes;

                    using(var strem = await resp.Content.ReadAsStreamAsync())
                    {
                        bytes = new byte[strem.Length];

						await strem.ReadAsync(bytes, 0, bytes.Length);
                    }

                    if (!string.IsNullOrEmpty(contentType))
                        headersMap.Add("Content-Type", contentType);

                    context.ResponseBody = bytes;
                    context.StatusCode = (int)resp.StatusCode;
                    context.ResponseHeaders = headersMap;

                    await ESBFactory
                        .Instance
                        .OutboundProcess
                        .Process(context, responseHandle);
				}
				//}    
            }
            catch(Exception e)
            {
                var json = ESBUtil.ParseJson(new
                {
                    Message = "Não foi possível enviar a solicitação para a origem",
                    Url = context.Url,
                    Detail = e.Message,
                    Trace = e.StackTrace
                });

                await responseHandle.Write(ESBUtil.CreateBytes(json), 501);
            }
        }

        private Uri CreateUri(ESBContext context)
        {
            return context.ApiInfo.CreateUri(context);
        }

        private string GetAccept(ESBContext context)
        {
            string header = null;
            context.Headers.TryGetValue("Accept", out header);

            if (string.IsNullOrEmpty(header))
            {   
                header = GetConteType(context, "*/*"); ;
            }

            return header;
        }

        private string GetConteType(ESBContext context, string other = null)
        {
            string contentType = null;
            context.Headers.TryGetValue("Content-Type", out contentType);
            if (string.IsNullOrEmpty(contentType))
            {
                if (string.IsNullOrEmpty(other))
                    contentType = "application/json";
                else
                    contentType = other;
            }
                

            return contentType;
        }
    }
}
