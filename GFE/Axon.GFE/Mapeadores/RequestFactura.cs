using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class RequestFactura
    {
        #region PROPIEDADES
        //[JsonProperty("facturaEstandar")]
        public Factura Factura { get; set; }

        [JsonProperty("idDocFiscalERP")]
        public string IdDocFiscalERP { get; set; } //fehfeiddf

        [JsonProperty("cufd")]
        public string Cufd { get; set; }        //fehfecufd

        [JsonProperty("contingencia")]
        public bool Contingencia { get; set; } //fehfecont

        [JsonProperty("esLote")]
        public bool EsLote { get; set; } //fehfelote

        [JsonProperty("idLoteERP")]
        public string IdLoteERP { get; set; } //fehfeidlo

        [JsonProperty("ultFacturaLote")]
        public bool UltFacturaLote { get; set; } //fehfeufac

        [JsonProperty("codigoTipoFactura")]
        public int CodigoTipoFactura { get; set; } //fehfectip

        #endregion

        public override string ToString()
        { 
            if (Factura != null) {
                if (Factura.Tipo == TipoFactura.notaMonedaExtranjera) {
                    var jsonResolver = new IgnorarRenombrarPropiedadJson();
                    jsonResolver.RenombrarPropiedad(typeof(RequestFactura), "Factura", "notaMonedaExtranjera");
                    jsonResolver.RenombrarPropiedad(typeof(Cabecera), "CodigoMoneda", "codigoTipoMoneda");
                    var jsonSerializer = new JsonSerializerSettings();
                    jsonSerializer.ContractResolver = jsonResolver;
                    return JsonConvert.SerializeObject(this, jsonSerializer);
                }
                return HelperJson.RenombrarPropiedad(typeof(RequestFactura), this, "Factura", Factura.Tipo.ToString());
            }
            return JObject.FromObject(this).ToString();
        }

    }
}
