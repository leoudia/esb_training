using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESB.Training.Inbound.Processor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ESB.Training.Inbound
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class HttpESBEndpoint
    {
        private readonly RequestDelegate _next;
        private HttpProcess process;

        public HttpESBEndpoint(RequestDelegate next)
        {
            _next = next;

            process = new HttpProcess();
        }

        public static ILogger Logger { get; set; }

        public async Task Invoke(HttpContext httpContext)
        {
            ResponseHandle resp = new InboundResponse(httpContext);

            try{
                if(!await process.Process(httpContext, resp))
                {
                    Logger.LogInformation("error");
                }

                await _next(httpContext);
            }
            catch(ESBException ex)
            {
                resp = null;
                Logger.LogError("HttpESBEndpoint:invoke", ex);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class HttpESBEndpointExtensions
    {
        public static IApplicationBuilder UseMiddlewareClassTemplate(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpESBEndpoint>();
        }
    }
}
