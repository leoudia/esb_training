using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ESB.Training.Util
{
    public static class ESBUtil
    {

        public static string ParseJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static Stream CreateStream(string val)
        {
            var bytes = Encoding.UTF8.GetBytes(val);
            return new MemoryStream(bytes);
        }

        public static byte[] CreateBytes(string val)
        {
            return Encoding.UTF8.GetBytes(val);
        }

        internal static string ConvertString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
