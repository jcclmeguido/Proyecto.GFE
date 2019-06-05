using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class FacturaEmbotelladora : Factura
    {
        [JsonProperty("detalle")] new public List<DetalleFacturaEmbotelladora> ListaDetalle { get; set; }

        public FacturaEmbotelladora() {
            Cabecera = new CabeceraFacturaEmbotelladora();
            ListaDetalle = new List<DetalleFacturaEmbotelladora>();
            Tipo = TipoFactura.facturaEmbotelladoras;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("MontoICE", "fehfemice");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
            _camposDB.Add("MarcaICE", "fedfemice");
        }
    }

    public class CabeceraFacturaEmbotelladora : Cabecera {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }
        [JsonProperty("montoIce")] public decimal? MontoICE { get; set; }
    }

    public class DetalleFacturaEmbotelladora : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
        [JsonProperty("marcaICE")] public int? MarcaICE { get; set; }
    }

}
