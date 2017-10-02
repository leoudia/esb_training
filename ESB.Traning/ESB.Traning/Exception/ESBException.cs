using System;
namespace ESB.Training
{
    public class ESBException : Exception
    {
        public ESBException(string msg, int code) : base(msg)
        {
            base.HResult = code;
        }
    }
}
