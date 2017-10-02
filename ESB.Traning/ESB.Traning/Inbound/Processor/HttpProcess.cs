using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESB.Training.Model;
using Microsoft.AspNetCore.Http;

namespace ESB.Training.Inbound.Processor
{
    public class HttpProcess
    {
        private InboundProcess process;

        public HttpProcess()
        {
            process = new InboundProcess();
        }

        public async Task<bool> Process(HttpContext context, ResponseHandle resp)
        {
            ESBContext esbContext = new ESBContext();

            var headers = context.Request.Headers.GetEnumerator();
            esbContext.Headers = new Dictionary<string, string>();

            if (context.Request.Headers != null)
            {
                while (headers.MoveNext())
                {
                    esbContext.Headers.Add(headers.Current.Key, headers.Current.Value);
                }
            }

            await ReadBody(context, esbContext);

            esbContext.Host = context.Request.Host.Value;

            ExtractIP(context, esbContext);

            esbContext.Method = context.Request.Method;
            esbContext.Path = context.Request.Path;
            esbContext.QueryString = context.Request.QueryString.Value;
            esbContext.Url = string.Format("{0}://{1}{2}{3}", context.Request.Scheme,
                                            context.Request.Host, context.Request.Path,
                                            context.Request.QueryString);

            return await process.Process(esbContext, resp);
        }

        private static void ExtractIP(HttpContext context, ESBContext esbContext)
        {
            string ip;

            if(!esbContext.Headers.TryGetValue("HTTP_X_FORWARDED_FOR", out ip))
            {
				if (!esbContext.Headers.TryGetValue("REMOTE_ADDR", out ip))
				{
					if (!esbContext.Headers.TryGetValue("HTTP_X_FORWARDED_FOR", out ip))
					{
						if (!esbContext.Headers.TryGetValue("X_FORWARDED_FOR", out ip))
						{
							ip = context.Connection.RemoteIpAddress.ToString();
						}
					}
				}
            }


            esbContext.IP = ip;
        }

        private static async Task ReadBody(HttpContext context, ESBContext esbContext)
        {
            if (context.Request.ContentLength.HasValue &&
                           context.Request.Body.CanRead)
            {
                var bufferSize = context.Request.ContentLength.HasValue ?
                                        context.Request.ContentLength.Value :
                                        1024 * 4;
                
                byte[] bytes = new byte[bufferSize];

                var count = await context.Request.Body.ReadAsync(bytes, 0, bytes.Length);
                if (count != bytes.Length)
                {
                    var arrayCopy = new byte[count];
                    Array.Copy(bytes, arrayCopy, count);
                }

                esbContext.Body = bytes;
            }
        }
    }
}
