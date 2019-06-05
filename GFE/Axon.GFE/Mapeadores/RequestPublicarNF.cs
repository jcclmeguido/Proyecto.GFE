using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Axon.GFE.Mapeadores
{
   public class RequestPublicarNF
    {
        //Obligatorio:Si
        [JsonProperty("nitEmisor")]
        public Int64 NitEmisor { get; set; }

        //Obligatorio:Si
        [JsonProperty("cuf")]
        public string Cuf { get; set; }

        //Obligatorio:Si
        [JsonProperty("archivo")]
        public string Archivo { get; set; }


        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
    }
}
