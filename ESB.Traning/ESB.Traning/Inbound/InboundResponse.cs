using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ESB.Training.Util;
using Microsoft.AspNetCore.Http;

namespace ESB.Training.Inbound
{
    public class InboundResponse : ResponseHandle
    {
        private HttpContext context;
        public InboundResponse(HttpContext context)
        {
            this.context = context;
        }

        public async Task Write(byte[] bytes, int httpCode, Dictionary<string, string> headers = null)
        {
            var Response = context.Response;
			Response.Clear();

            if (headers.TryGetValue("Content-Type", out string content))
            {
                Response.ContentType = content;
                headers.Remove("Content-Type");
            }

            if (headers.ContainsKey("Server"))
                headers.Remove("Server");
            
			if (headers.ContainsKey("Date"))
				headers.Remove("Date");

			if (headers.ContainsKey("Transfer-Encoding"))
				headers.Remove("Transfer-Encoding");
            
            if (headers != null && headers.Count > 0)
            {
				foreach (KeyValuePair<string, string> pair in headers)
				{
                    Response.GetTypedHeaders().Append(pair.Key, pair.Value);
				}
            }

            string body = ESBUtil.ConvertString(bytes);
            Response.StatusCode = httpCode;
            Response.ContentLength = bytes.Length;

            await Response.WriteAsync(body);
        }
    }
}
