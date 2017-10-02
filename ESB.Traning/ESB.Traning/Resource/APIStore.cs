using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using ESB.Training.Model;

namespace ESB.Traning.Resource
{
    public class APIStore
    {
        private ConcurrentDictionary<APIKey, List<APIInfo>> apiStore =
            new ConcurrentDictionary<APIKey, List<APIInfo>>();
        
        public APIStore()
        {
        }

        public bool Save(string apiName, string version, string url, string method)
        {
            ValidateApiKey(apiName, version);

            if (string.IsNullOrEmpty(url))
                throw new Exception("A url não foi informada");

            if (string.IsNullOrEmpty(method))
                throw new Exception("Metodo http não foi informado");
            
            Uri uri = new Uri(url);

            string path = uri.LocalPath;

            if (path.IndexOf('/') == 0)
                path = path.Remove(0, 1);

            if(path.EndsWith("/", StringComparison.Ordinal))
            {
                path = path.Remove(path.Length - 1, 1);
            }

            string[] pathList = path.Split(new char[] { '/' });

            APIKey key = new APIKey(apiName, version);
            APIInfo info = new APIInfo(url, uri, pathList, method);

            List<APIInfo> apiInfo;

            if(!apiStore.TryGetValue(key, out apiInfo))
            {
                apiInfo = new List<APIInfo>();

                while (apiStore.TryAdd(key, apiInfo)){}
            }

            apiInfo.Add(info);

            return true;
        }

        private static void ValidateApiKey(string apiName, string version)
        {
            if (string.IsNullOrEmpty(apiName))
                throw new Exception("Nome da API está inválido");

            if (string.IsNullOrEmpty(version))
                throw new Exception("A versão da API está inválida");
        }

        public IEnumerable<List<APIInfo>> Get()
        {
            return apiStore.Values;
        }

        public List<APIInfo> Get(string apiName, string version)
        {
            ValidateApiKey(apiName, version);

            APIKey key = new APIKey(apiName, version);

            apiStore.TryGetValue(key, out List<APIInfo> resp);

            return resp;
        }
    }

    public class APIInfo
    {
        public APIInfo(string url, Uri uri, string[] pathList, string method)
        {
            this.Url = url;
            this.Uri = uri;
            this.PathList = pathList;
            this.Method = method;
        }

        public string Url { get; }
        public Uri Uri { get; }
        public string[] PathList { get; }
        public string Method { get; }

        public bool Validate(ESBContext context, string[] paths)
        {
            if (PathList.Length != paths.Length)
                return false;
            
            StringBuilder sb = new StringBuilder();

            if (!context.Method.Equals(Method))
                return false;

            for (int i = 0; i < PathList.Length; i++)
            {
                string pReq = paths[i];
                string pSource = PathList[i];

                if (pSource.StartsWith("{", StringComparison.CurrentCulture) &&
                                    pSource.EndsWith("}", StringComparison.CurrentCulture))
                {
                    sb.Append("/").Append(pReq);

                    continue;
                }

                if (!pReq.Equals(pSource))
                    return false;

                sb.Append("/").Append(pReq);
            }

            context.Attributes(ESBContants.URL_PATH_OUT_KEY, sb.ToString());


            return true;
        }

        public Uri CreateUri(ESBContext context)
        {
            UriBuilder builder = new UriBuilder(Uri)
            {
                Path = context.Attributes(ESBContants.URL_PATH_OUT_KEY),
                Query = context.QueryString
            };

            return builder.Uri;
        }
    }

    public class APIKey : IEqualityComparer<APIKey>
    {

        private string name;
        private string version;

        public APIKey(string name, string version)
        {
            if (string.IsNullOrEmpty((name)) 
                || string.IsNullOrEmpty((version)))
                throw new Exception("invalid key");
            
            this.name = name;
            this.version = version;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Version
        {
            get
            {
                return version;
            }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(this, (APIKey)obj);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public bool Equals(APIKey x, APIKey y)
        {
            return x.name.Equals(y.name) && x.version.Equals((y.version));
        }

        public int GetHashCode(APIKey obj)
        {
            return obj.name.GetHashCode() + obj.version.GetHashCode();
        }
    }
}
