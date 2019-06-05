using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class FacturaJuego : Factura
    {
        [JsonProperty("detalle")] new public List<DetalleFacturaJuego> ListaDetalle { get; set; }

        public FacturaJuego() {
            Cabecera = new CabeceraFacturaJuego();
            ListaDetalle = new List<DetalleFacturaJuego>();
            Tipo = TipoFactura.facturaJuegos;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("MontoTotalIj", "fehfemtoj");
            _camposDB.Add("MontoTotalSujetoIpj", "fehfemtsl");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
        }
    }

    public class CabeceraFacturaJuego : Cabecera {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }
        [JsonProperty("montoTotalIj")] public decimal? MontoTotalIj { get; set; }
        [JsonProperty("montoTotalSujetoIpj")] public decimal? MontoTotalSujetoIpj { get; set; }
    }

    public class DetalleFacturaJuego : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
    }
}
