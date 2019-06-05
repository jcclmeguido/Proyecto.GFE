using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class FacturaEspectaculoPublicoInternacional : Factura
    {
        [JsonProperty("detalle")] new public List<DetalleFacturaEspectaculoPublicoInternacional> ListaDetalle { get; set; }

        public FacturaEspectaculoPublicoInternacional() {
            Cabecera = new CabeceraFacturaEspectaculoPublicoInternacional();
            ListaDetalle = new List<DetalleFacturaEspectaculoPublicoInternacional>();
            Tipo = TipoFactura.facturaEspectaculoPublicoInternacional;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("TipoEvento", "fehfeteve");
            _camposDB.Add("LugarEvento", "fehfeleve");
            _camposDB.Add("FechaEvento", "fehfefeve");
            _camposDB.Add("ArtistaEvento", "fehfeaeve");
            _camposDB.Add("NITSalon", "fehfensal");
            _camposDB.Add("DireccionSalon", "fehfedsal");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
        }
    }

    public class CabeceraFacturaEspectaculoPublicoInternacional : Cabecera {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }
        [JsonProperty("tipoEvento")] public string TipoEvento { get; set; }
        [JsonProperty("lugarEvento")] public string LugarEvento { get; set; }
        [JsonProperty("fechaEvento")] public DateTime? FechaEvento { get; set; }
        [JsonProperty("artistaEvento")] public string ArtistaEvento { get; set; }
        [JsonProperty("nitSalon")] public string NITSalon { get; set; }
        [JsonProperty("DireccionSalon")] public string DireccionSalon { get; set; }
    }

    public class DetalleFacturaEspectaculoPublicoInternacional : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
    }
}
