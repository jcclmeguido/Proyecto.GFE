using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class Respuesta
    {
        [JsonProperty("codRespuesta")] public string CodRespuesta { get; set; }         //fehfecres
        [JsonProperty("txtRespuesta")] public string TxtRespuesta { get; set; }        //fehfetres

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }

    }

    public class ResponseFEEL
    {
        public Respuesta respuesta { get; set; }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
    }

}
