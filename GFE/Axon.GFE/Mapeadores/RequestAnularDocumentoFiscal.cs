using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    /// <summary>
    /// ::C1 -> ResponseAnular_DocumentoFiscal
    /// </summary>
    public class RequestAnularDocumentoFiscal
    {
        //Obligatorio:Si
        [JsonProperty("nitEmisor")]
        public Int64 NitEmisor { get; set; }

        //Obligatorio:No
        [JsonProperty("cuf")]
        public string Cuf { get; set; }

        //Obligatorio:No
        [JsonProperty("numeroFactura")]
        public string NumeroFactura { get; set; }

        //Obligatorio:No
        [JsonProperty("idDocFiscalERP")]
        public string IdDocFiscalERP { get; set; }

        //Obligatorio:No
        //[JsonProperty("tokenCliente")]
        //public string TokenCliente { get; set; }


        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }

    }
}
