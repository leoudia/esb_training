using System;
namespace ESB.Training.Model
{
    public class EndpointModel
    {
        public EndpointModel()
        {
        }

        public string ApiName { get; set; }
        public string Version { get; set; }
        public string Endpoint { get; set; }
        public string Method { get; set; }
    }
}
