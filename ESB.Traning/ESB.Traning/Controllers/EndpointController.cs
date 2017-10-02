using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESB.Training.Factory;
using ESB.Training.Model;
using ESB.Traning.Resource;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ESB.Training.Controllers
{
    [Route("api/[controller]")]
    public class EndpointController : Controller
    {
        [HttpGet]
        public IEnumerable<List<APIInfo>> Get()
        {
            return ESBFactory.Instance.APIStore.Get();
        }

        [HttpGet("{apiName}/{version}")]
        public List<APIInfo> Get(string apiName, string version)
        {
            return ESBFactory.Instance.APIStore.Get(apiName,
                                                    version);
        }

        [HttpPost]
        public bool Post([FromBody]EndpointModel endpoint)
        {
            return ESBFactory.Instance.APIStore.Save(endpoint.ApiName, 
                                              endpoint.Version, 
                                              endpoint.Endpoint,
                                                     endpoint.Method);
        }
    }
}
