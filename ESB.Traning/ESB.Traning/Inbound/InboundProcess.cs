using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESB.Training.Factory;
using ESB.Training.Inbound.Middle.Contract;
using ESB.Training.Model;
using ESB.Training.Outbound;
using ESB.Training.Outbound.Gateway;
using ESB.Training.Util;
using ESB.Traning.Resource;

namespace ESB.Training.Inbound
{
    public class InboundProcess
    {
        private GatewayDispatch gateway;

        public InboundProcess()
        {
            gateway = new GatewayDispatch();
        }

        public async Task<bool> Process(ESBContext context, ResponseHandle responseHandle)
		{
            if(!APIAccepted(context)){
                return false;
            }

            if(!MiddlesAccepted(context, responseHandle))
            {
                throw new ESBException("Request not valid", 501);
            }

            InvokeInterceptor(context);

            await gateway.Dispatch(context, responseHandle, DispatchType.REST);

            return true;
		}

        private void InvokeInterceptor(ESBContext context)
        {
            IMiddleHandle[] middles = ESBFactory.Instance.Middles;

            foreach(IMiddleHandle handle in middles)
            {
                handle.RequestInterceptor(context);
            }
        }

        private bool MiddlesAccepted(ESBContext context, ResponseHandle responseHandle)
        {

            IMiddleHandle[] middles = ESBFactory.Instance.Middles;

            string msg = null;
            int code = 400;
            foreach(IMiddleHandle handle in middles)
            {
                if(!handle.CanHandle(context, out msg, out code))
                {
                    string json = ESBUtil.ParseJson(new
                    {
                        MiddleName = handle.GetType().FullName,
                        Message = "O middle rejeitou a solicitação",
                        Url = context.Url,
                        Detail = msg
                    });

                    responseHandle.Write(ESBUtil.CreateBytes(json), code);
                    return false;
                }
            }

            return true;
        }

        protected bool APIAccepted(ESBContext context)
        {
            if (context.Path.StartsWith("/", StringComparison.CurrentCulture))
                context.Path = context.Path.Remove(0, 1);

            if(context.Path.EndsWith("/", StringComparison.CurrentCulture))
                context.Path = context.Path.Remove((context.Path.Length - 1), 1);

            var paths = context.Path.Split(new char[]{'/'});

            if (paths.Length <= 2)
                return false;

            string apiName = paths[0];
            string version = paths[1];

            var apiInfo = ESBFactory.Instance.APIStore.Get(apiName, version);
            if (apiInfo == null || apiInfo.Count == 0)
                return false;

            return FindAPI(context, paths, apiInfo);
        }

        private bool FindAPI(ESBContext context, string[] paths, 
                             List<APIInfo> apiInfo)
        {
            paths = RemoveAppInfo(paths);

            foreach(APIInfo api in apiInfo)
            {
                if (api.Validate(context, paths))
                {
                    context.ApiInfo = api;
                    return true;
                }   
            }

            return false;
        }

        private string[] RemoveAppInfo(string[] paths)
        {
            string[] itens = new string[paths.Length - 2];

            for (int i = 2; i < paths.Length; i++)
            {
                int itensIndex = i - 2;
                itens[itensIndex] = paths[i];
            }

            return itens;
        }
    }
}
