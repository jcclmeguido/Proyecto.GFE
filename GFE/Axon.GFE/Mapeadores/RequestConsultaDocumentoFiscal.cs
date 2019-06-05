using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    [Serializable]
    public class RequestConsultaDocumentoFiscal
    {
        //Obligatorios
        [JsonProperty("nitEmisor")] public long NITEmisor { get; set; }
        [JsonProperty("cuf")] public string CUF { get; set; }

        //Opcionales
        [JsonProperty("fechaDesde")] public long FechaDesde { get; set; }
        [JsonProperty("fechaHasta")] public long FechaHasta { get; set; }
        [JsonProperty("nitCliente")] public long NITCliente { get; set; }
        [JsonProperty("cufd")] public string CUFD { get; set; }
        [JsonProperty("sucursal")] public int Sucursal { get; set; }
        [JsonProperty("idDocFiscalERP")] public string IdDocFiscalERP { get; set; }
        [JsonProperty("estado")] public string Estado { get; set; }

        private const string IGNORAR = "<<IGNORAR>>";
        private const int IGNORARNUM = -1000;

        public RequestConsultaDocumentoFiscal() {
            FechaDesde = FechaHasta = NITCliente = Sucursal = IGNORARNUM;
            CUFD = IdDocFiscalERP = Estado = IGNORAR;
        }

        private string[] ObtenerIgnorados() {
            List<string> ignorados = new List<string>();
            if (FechaDesde == IGNORARNUM) ignorados.Add("fechaDesde");
            if (FechaHasta == IGNORARNUM) ignorados.Add("fechaHasta");
            if (NITCliente == IGNORARNUM) ignorados.Add("nitCliente");
            if (Sucursal == IGNORARNUM) ignorados.Add("sucursal");
            if (CUFD == IGNORAR) ignorados.Add("cufd");
            if (IdDocFiscalERP == IGNORAR) ignorados.Add("idDocFiscalERP");
            if (Estado == IGNORAR) ignorados.Add("estado");
            return (ignorados.Count > 0) ? ignorados.ToArray() : null;
        }

        public override string ToString()
        {
            string[] ignorados = ObtenerIgnorados();
            return (ignorados == null) ? JObject.FromObject(this).ToString() : HelperJson.IgnorarPropiedades(typeof(RequestConsultaDocumentoFiscal), this, ignorados);
        }
    }
}
