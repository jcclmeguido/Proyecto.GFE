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
    /// ::Request Factura Estandar
    /// </summary>
    public class ResponseFactura
    {
        [JsonProperty("respuesta")]
        public Respuesta Respuesta { get; set; }

        [JsonProperty("proceso")]
        public Proceso Proceso { get; set; }

        [JsonProperty("factura")]
        public Factura Factura { get; set; }

        public ResponseFactura(){}

        public ResponseFactura(string json) {
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
            else {
                string tf = string.Empty;
                foreach (string tipoDocFiscal in Enum.GetNames(typeof(TipoFactura)))
                {
                    if (jo.ContainsKey(tipoDocFiscal)) { tf = tipoDocFiscal; break; }
                }
                if (string.IsNullOrEmpty(tf) || !jo.ContainsKey("proceso")) throw new Exception(Error.E300);

                Enum.TryParse(tf, out TipoFactura tipoFactura);

                Factura = FabricaFactura.Fabricar(tipoFactura);
                Respuesta = new Respuesta();
                Proceso = new Proceso();
                JsonConvert.PopulateObject(jo["respuesta"].ToString(), Respuesta);
                JsonConvert.PopulateObject(jo["proceso"].ToString(), Proceso);
                JsonConvert.PopulateObject(jo[tipoFactura.ToString()].ToString(), Convert.ChangeType(Factura, Factura.GetType()));
            }
        }

        public override string ToString()
        {
            if (Factura != null)
            {
                return HelperJson.RenombrarPropiedad(typeof(RequestFactura), this, "Factura", Factura.Tipo.ToString());
            }
            return JObject.FromObject(this).ToString();
        }
    }


    public class Proceso
    {
        [JsonProperty("idDocFiscalERP")] public string IdDocFiscalERP { get; set; } //fehfecorr
        [JsonProperty("idDocFiscalFEEL")]  public string IdDocFiscalFEEL { get; set; } //fehfeifee
        [JsonProperty("cufd")] public string CUFD { get; set; } //fehfecufd
        [JsonProperty("codEstado")] public string CodEstado { get; set; }   //fehfecsta
        [JsonProperty("codigoTipoFactura")]public int CodigoTipoFactura { get; set; } //fehfectip

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
    }

}
