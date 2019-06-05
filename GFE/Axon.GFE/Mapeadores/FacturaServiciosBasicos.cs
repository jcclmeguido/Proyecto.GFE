using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class FacturaServiciosBasicos: Factura
    {
        [JsonProperty("detalle")] new public List<DetalleFacturaServiciosBasicos> ListaDetalle { get; set; }

        public FacturaServiciosBasicos()
        {
            Cabecera = new CabeceraFacturaServiciosBasicos();
            ListaDetalle = new List<DetalleFacturaServiciosBasicos>();
            Tipo = TipoFactura.facturaServicios;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("Ciudad", "fehfeciud");
            _camposDB.Add("Zona", "fehfezona");
            _camposDB.Add("NumeroMedidor", "fehfenmed");
            _camposDB.Add("Gestion", "fehfegest");
            _camposDB.Add("Mes", "fehfemmes");
            _camposDB.Add("DomicilioComprador", "fehfedoco");
            _camposDB.Add("MontoTotalSujetoIVA", "fehfemtsi");
            _camposDB.Add("ConsumoKwh", "fehfeckwh");
            _camposDB.Add("ConsumoMetrosCubicos", "fehfecmcu");
            _camposDB.Add("MontoDescuentoLey1886", "fehfedley");
            _camposDB.Add("TasaAseo", "fehfetase");
            _camposDB.Add("TasaAlumbrado", "fehfetalu");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
        }
    }

    public class CabeceraFacturaServiciosBasicos : Cabecera
    {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }

        [JsonProperty("ciudad")]
        public string Ciudad { get; set; }

        [JsonProperty("zona")]
        public string Zona { get; set; }

        [JsonProperty("numeroMedidor")]
        public string NumeroMedidor { get; set; }

        [JsonProperty("gestion")]
        public string Gestion { get; set; }

        [JsonProperty("mes")]
        public string Mes { get; set; }

        [JsonProperty("domicilioComprador")]
        public string DomicilioComprador { get; set; }

        [JsonProperty("montoTotalSujetoIva")]
        public decimal MontoTotalSujetoIVA { get; set; }

        [JsonProperty("consumoKwh")]
        public decimal ConsumoKwh { get; set; }

        [JsonProperty("consumoMetrosCubicos")]
        public decimal ConsumoMetrosCubicos { get; set; }

        [JsonProperty("montoDescuentoLey1886")]
        public decimal MontoDescuentoLey1886 { get; set; }

        [JsonProperty("tasaAseo")]
        public decimal TasaAseo { get; set; }

        [JsonProperty("tasaAlumbrado")]
        public decimal TasaAlumbrado { get; set; }
    }

    public class DetalleFacturaServiciosBasicos : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
    }
}
