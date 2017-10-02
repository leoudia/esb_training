using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ESB.Training.Test.URI
{
    [TestClass]
    public class URiTestBasic
    {
        
        public URiTestBasic()
        {
            
        }
        [TestMethod]
        public void CreateURITest()
        {
            Uri uri = new Uri("http://localhost:8080/api/load?p=1");

            Assert.AreEqual(uri.Scheme, "http");
            Assert.AreEqual(uri.Host, "localhost");
            Assert.AreEqual(uri.Port, 8080);
            Assert.AreEqual(uri.LocalPath, "/api/load");
            Assert.AreEqual(uri.Query, "?p=1");

            UriBuilder builder = new UriBuilder(uri);
            builder.Path = builder.Path + "/test";

            uri = builder.Uri;

            Assert.AreEqual(uri.LocalPath, "/api/load/test");
        }
    }
}
