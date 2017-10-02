using System;
using System.Collections.Generic;
using ESB.Training.Inbound.Middle.Contract;
using ESB.Training.Outbound;
using ESB.Traning.Resource;

namespace ESB.Training.Factory
{
    public class ESBFactory
    {
        private APIStore store;
        private List<IMiddleHandle> middles = new List<IMiddleHandle>();
        private static ESBFactory instance = new ESBFactory();
        private OutboundProcess outboundProcess = new OutboundProcess();

        private ESBFactory()
        {
            store = new APIStore();
        }

        public APIStore APIStore{ get { return store; }}

        public static ESBFactory Instance{ get { return instance; }}

        public void RegisterMiddle(IMiddleHandle handle)
        {
            lock(middles)
            {
                middles.Add(handle);   
            }
        }

        public IMiddleHandle[] Middles
        {
            get
            {
                lock(middles)
                {
                    return middles.ToArray();
                }
            }
        }

        public OutboundProcess OutboundProcess
        {
            get
            {
                return outboundProcess;
            }
        }
    }
}
