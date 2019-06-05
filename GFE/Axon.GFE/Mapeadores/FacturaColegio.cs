using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class FacturaColegio : Factura
    {
        [JsonProperty("detalle")] new public List<DetalleFacturaColegio> ListaDetalle { get; set; }

        public FacturaColegio()
        {
            Cabecera = new CabeceraFacturaColegio();
            ListaDetalle = new List<DetalleFacturaColegio>();
            Tipo = TipoFactura.facturaColegio;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("NombreEstudiante", "fehfenest");
            _camposDB.Add("PeriodoFacturado", "fehfepfac");
            _camposDB.Add("Cantidad", "fedfecant");
        }
    }

    public class CabeceraFacturaColegio : Cabecera
    {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }

        [JsonProperty("nombreEstudiante")]
        public string NombreEstudiante { get; set; }

        [JsonProperty("periodoFacturado")]
        public string PeriodoFacturado { get; set; }
    }

    public class DetalleFacturaColegio : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
    }

}
