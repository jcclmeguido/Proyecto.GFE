using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public enum EstadoFacturaFEEL
    {
        FacturaRecibidaDeOrigen = 1,    //D1
        FacturaEnviadaAlSIN,            //D2
        FacturarValidadaPorElSIN,       //D3
        FacturaNotificadaAOrigen,       //D4
        AnulacionRecibidaDeOrigen,      //DA1
        AnulacionEnviadaAlSIN,          //DA2
        AnulacionValidadaPorSIN,        //DA3
        AnulacionNotificadaAOrigen      //DA4
    }

    public class ResponseConsultaDocumentoFiscal
    {
        [JsonProperty("respuesta")] public Respuesta Respuesta { get; set; }
        [JsonProperty("estado")] public EstadoFacturaFEEL Estado { get; set; }
        [JsonProperty("factura")] public Factura Factura { get; set; }

        public ResponseConsultaDocumentoFiscal(string json)
        {
            JObject jo = JObject.Parse(json);

            if (!jo.ContainsKey("respuesta")) throw new Exception(Error.E300);
            if (jo["respuesta"]["codRespuesta"].ToString() != "0")
            {
                Respuesta = new Respuesta()
                {
                    CodRespuesta = jo["respuesta"]["codRespuesta"].ToString(),
                    TxtRespuesta = jo["respuesta"]["txtRespuesta"].ToString()
                };
            }
            else
            {
                string tf = string.Empty;
                foreach (string tipoDocFiscal in Enum.GetNames(typeof(TipoFactura)))
                {
                    if (jo.ContainsKey(tipoDocFiscal)) { tf = tipoDocFiscal; break; }
                }
                if (string.IsNullOrEmpty(tf) || !jo.ContainsKey("estado")) throw new Exception(Error.E300);

                Enum.TryParse(tf, out TipoFactura tipoFactura);

                Factura = FabricaFactura.Fabricar(tipoFactura);
                Respuesta = new Respuesta();

                switch (jo["estado"].ToString())
                {
                    case "D1": Estado = EstadoFacturaFEEL.FacturaRecibidaDeOrigen; break;
                    case "D2": Estado = EstadoFacturaFEEL.FacturaEnviadaAlSIN; break;
                    case "D3": Estado = EstadoFacturaFEEL.FacturarValidadaPorElSIN; break;
                    case "D4": Estado = EstadoFacturaFEEL.FacturaNotificadaAOrigen; break;
                    case "DA1": Estado = EstadoFacturaFEEL.AnulacionRecibidaDeOrigen; break;
                    case "DA2": Estado = EstadoFacturaFEEL.AnulacionEnviadaAlSIN; break;
                    case "DA3": Estado = EstadoFacturaFEEL.AnulacionValidadaPorSIN; break;
                    case "DA4": Estado = EstadoFacturaFEEL.AnulacionNotificadaAOrigen; break;
                    default: throw new Exception(Error.E301);
                }

                JsonConvert.PopulateObject(jo["respuesta"].ToString(), Respuesta);
                JsonConvert.PopulateObject(jo[tipoFactura.ToString()].ToString(), Factura);
            }
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
    }
}
