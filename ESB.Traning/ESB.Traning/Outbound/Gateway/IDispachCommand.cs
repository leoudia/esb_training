using System;
using System.Threading.Tasks;
using ESB.Training.Model;

namespace ESB.Training.Outbound.Gateway
{
    public interface IDispachCommand
    {
        Task Dispatch(ESBContext context, Inbound.ResponseHandle responseHandle);
    }
}
