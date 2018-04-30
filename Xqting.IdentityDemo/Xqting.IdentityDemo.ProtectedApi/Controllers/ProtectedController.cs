using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Xqting.IdentityDemo.ProtectedApi.Controllers
{
    /// <summary>
    /// The protected controller can be called by opening the browser or postman to http://localhost:12345/api/protected and will return the 
    /// same header values as teh public controller. However, now it is protected and will require authentication from our identity service
    /// with a token. if the token is not present, the user will be redirected
    /// </summary>
    [Route("api/[controller]")]
    public class ProtectedController : Controller
    {
        // GET api/values
        [HttpGet]
        [Authorize] //--> Will require authentication for this call !! (can also be put on the class level to protect all the functions in the controller)
        public IEnumerable<string> Get()
        {
            return this.Request.Headers.Select(h => h.Key + " -> " + string.Join(";", h.Value.ToList())).ToList();
        }
    }
}
