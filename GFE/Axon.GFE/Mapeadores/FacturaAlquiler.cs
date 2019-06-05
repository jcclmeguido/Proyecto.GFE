using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class FacturaAlquiler : Factura
    {
        [JsonProperty("detalle")] new public List<DetalleFacturaAlquiler> ListaDetalle { get; set; }

        public FacturaAlquiler()
        {
            Cabecera = new CabeceraFacturaAlquiler();
            ListaDetalle = new List<DetalleFacturaAlquiler>();
            Tipo = TipoFactura.facturaAlquiler;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("PeriodoFacturado", "fehfepfac");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
        }
    }

    public class CabeceraFacturaAlquiler : Cabecera
    {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }
        [JsonProperty("periodoFacturado")] public string PeriodoFacturado { get; set; }
    }

    public class DetalleFacturaAlquiler : Detalle {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
    }

}
