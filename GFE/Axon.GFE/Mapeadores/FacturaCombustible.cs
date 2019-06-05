using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class FacturaCombustible:Factura
    {
        [JsonProperty("detalle")] new public List<DetalleFacturaCombustible> ListaDetalle { get; set; }

        public FacturaCombustible()
        {
            Cabecera = new CabeceraFacturaCombustible();
            ListaDetalle = new List<DetalleFacturaCombustible>();
            Tipo = TipoFactura.facturaCombustible;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("MontoLey317", "fehfel317");
            _camposDB.Add("MontoTotalSujetoIva", "fehfemtsi");
            _camposDB.Add("CodigoPais", "fehfecpai");
            _camposDB.Add("PlacaVehiculo", "fehfepveh");
            _camposDB.Add("TipoEnvase", "fehfetenv");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
        }
    }

    public class CabeceraFacturaCombustible : Cabecera {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }
        [JsonProperty("montoLey317")] public decimal? MontoLey317 { get; set; }
        [JsonProperty("montoTotalSujetoIva")] public decimal? MontoTotalSujetoIva { get; set; }
        [JsonProperty("codigoPais")] public short? CodigoPais { get; set; }
        [JsonProperty("placaVehiculo")] public string PlacaVehiculo { get; set; }
        [JsonProperty("tipoEnvase")] public string TipoEnvase { get; set; }
    }

    public class DetalleFacturaCombustible : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
    }
}
