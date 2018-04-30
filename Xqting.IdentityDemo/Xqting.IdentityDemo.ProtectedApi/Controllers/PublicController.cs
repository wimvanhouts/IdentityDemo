using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Xqting.IdentityDemo.ProtectedApi.Controllers
{
    /// <summary>
    /// The public controller can be called by opening the browser or postman to http://localhost:12345/api/public and will return the 
    /// header values from the request as an example of an API that is publicly available
    /// </summary>
    [Route("api/[controller]")]
    public class PublicController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return this.Request.Headers.Select(h => h.Key + " -> " + string.Join(";", h.Value.ToList())).ToList();
        }
    }
}
