using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class FacturaEstandar:Factura
    {
        [JsonProperty("detalle")] new public List<DetalleFacturaEstandar> ListaDetalle { get; set; }

        public FacturaEstandar()
        {
            Cabecera = new CabeceraFacturaEstandar();
            ListaDetalle = new List<DetalleFacturaEstandar>();
            Tipo = TipoFactura.facturaEstandar;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("NumeroSerie", "fedfenser");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
        }
    }

    public class CabeceraFacturaEstandar : Cabecera
    {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }
    } 

    public class DetalleFacturaEstandar : Detalle
    {
        [JsonProperty("numeroSerie")] public string NumeroSerie { get; set; }
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
    }
}
