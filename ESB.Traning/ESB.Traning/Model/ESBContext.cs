using System;
using System.Collections.Generic;
using System.IO;
using ESB.Traning.Resource;

namespace ESB.Training.Model
{
    public class ESBContext
    {
        private Dictionary<string, string> attributes;

        public ESBContext()
        {
            attributes = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Headers { get; set; }

        public string CharSet { get; set; }
        public byte[] Body { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string IP { get; set; }
        public string QueryString { get; set; }

        public byte[] ResponseBody { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, string> ResponseHeaders { get; set; }

        public APIInfo ApiInfo { get; set; }

        public void Attributes(string key, string val)
        {
            attributes.Add(key, val);
        }

		public string Attributes(string key)
		{
            string val = null;
            attributes.TryGetValue(key, out val);
            return val;
		}
    }
}
