using System;
using ESB.Training.Model;

namespace ESB.Training.Inbound.Middle.Contract
{
    public interface IMiddleHandle
    {
        bool CanHandle(ESBContext context, out string msg, out int code);

        void RequestInterceptor(ESBContext context);

        void ResponseInterceptor(ESBContext context);
    }
}
