using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class FacturaHidrocarburos:Factura
    {
        [JsonProperty("detalle")] new public List<DetalleFacturaHidrocarburos> ListaDetalle { get; set; }

        public FacturaHidrocarburos() {
            Cabecera = new CabeceraFacturaHidrocarburos();
            ListaDetalle = new List<DetalleFacturaHidrocarburos>();
            Tipo = TipoFactura.facturaHidrocarburos;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("Ciudad", "fehfeciud");
            _camposDB.Add("MontoIedh", "fehfemled");
            _camposDB.Add("NombrePropietario", "fehfenpro");
            _camposDB.Add("NombreRepresentanteLegal", "fehfenrle");
            _camposDB.Add("CondicionPago", "fehfecpag");
            _camposDB.Add("PeriodoEntrega", "fehfepent");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
        }
    }

    public class CabeceraFacturaHidrocarburos : Cabecera {
        [JsonProperty("ciudad")] public string Ciudad { get; set; }
        [JsonProperty("montoIedh")] public decimal? MontoIedh { get; set; }
        [JsonProperty("nombrePropietario")] public string NombrePropietario { get; set; }
        [JsonProperty("nombreRepresentanteLegal")] public string NombreRepresentanteLegal { get; set; }
        [JsonProperty("condicionPago")] public short? CondicionPago { get; set; }
        [JsonProperty("periodoEntrega")] public string PeriodoEntrega { get; set; }
    }

    public class DetalleFacturaHidrocarburos : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
    }
}
