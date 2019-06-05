using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AxonGFEService.Controllers
{
    [RoutePrefix("gfe")]
    public class ValidacionController : ApiController
    {
        [HttpPost]
        [Route("prueba")]
        public JObject PostJSON(JObject json)
        {
            System.Diagnostics.Debug.WriteLine(json);
            return json;
        }

        [HttpGet]
        [Route("pruebaget")]
        public JObject GetJSON()
        {
            var o = new { hola = "hola", id = 123 };
            return JObject.FromObject(o);
        }
    }
}
