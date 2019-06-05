using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class FacturaBanco:Factura
    {
        [JsonProperty("detalle")] new public List<DetalleFacturaBanco> ListaDetalle { get; set; }

        public FacturaBanco() {
            Cabecera = new CabeceraFacturaBanco();
            ListaDetalle = new List<DetalleFacturaBanco>();
            Tipo = TipoFactura.facturaBancos;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("MontoTotalArrendamientoFinanciero", "fehfemtaf");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
        }
    }

    public class CabeceraFacturaBanco : Cabecera {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }
        [JsonProperty("montoTotalArrendamientoFinanciero")] public decimal? MontoTotalArrendamientoFinanciero { get; set; }
    }

    public class DetalleFacturaBanco : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
    }
}
