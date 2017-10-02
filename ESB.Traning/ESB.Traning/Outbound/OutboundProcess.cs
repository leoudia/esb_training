using System;
using System.Threading.Tasks;
using ESB.Training.Factory;
using ESB.Training.Inbound.Middle.Contract;
using ESB.Training.Model;

namespace ESB.Training.Outbound
{
    public class OutboundProcess
    {
        public async Task Process(ESBContext context,
                            Inbound.ResponseHandle responseHandle)
        {
            InvokeInterceptor(context);

            await responseHandle.Write(context.ResponseBody, 
                                 context.StatusCode, context.ResponseHeaders);
        }

		private void InvokeInterceptor(ESBContext context)
		{
			IMiddleHandle[] middles = ESBFactory.Instance.Middles;

			foreach (IMiddleHandle handle in middles)
			{
				handle.ResponseInterceptor(context);
			}
		}
    }
}
