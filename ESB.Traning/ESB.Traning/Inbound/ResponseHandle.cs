using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ESB.Training.Inbound
{
    public interface ResponseHandle
    {
        Task Write(byte[] bytes, int httpCode, 
                   Dictionary<string, string> headers = null);
    }
}
