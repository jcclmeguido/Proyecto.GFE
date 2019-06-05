using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class NotaExportacion: Factura
    {
        [JsonProperty("detalle")] new public List<DetalleNotaExportacion> ListaDetalle { get; set; }

        public NotaExportacion()
        {
            Cabecera = new CabeceraNotaExportacion();
            ListaDetalle = new List<DetalleNotaExportacion>();
            Tipo = TipoFactura.notaExportacion;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("CodigoPais", "fehfecpai");
            _camposDB.Add("MontoTotalPuerto", "fehfemtpu");
            _camposDB.Add("OtrosMontos", "fehfeomon");
            _camposDB.Add("IncoTerm", "fehfeinco");
            _camposDB.Add("PuertoDestino", "fehfepdes");
            _camposDB.Add("LugarDestino", "fehfeldes");
            _camposDB.Add("PrecioValorBruto", "fehfepvbr");
            _camposDB.Add("GastosTransporteFrontera", "fehfegtfr");
            _camposDB.Add("GastosSeguroFrontera", "fehfesfro");
            _camposDB.Add("TotalFobFrontera", "fehfetffr");
            _camposDB.Add("MontoTransporteFrontera", "fehfemtfr");
            _camposDB.Add("MontoSeguroInternacional", "fehfemsin");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
            _camposDB.Add("CodigoNandina", "fedfecnan");
        }
    }

    public class CabeceraNotaExportacion : Cabecera
    {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }

        [JsonProperty("codigoPais")]
        public int CodigoPais { get; set; }

        [JsonProperty("montoTotalPuerto")]
        public decimal MontoTotalPuerto { get; set; }

        [JsonProperty("otrosMontos")]
        public decimal OtrosMontos { get; set; }

        [JsonProperty("incoterm")]
        public string IncoTerm { get; set; }

        [JsonProperty("puertoDestino")]
        public string PuertoDestino { get; set; }

        [JsonProperty("lugarDestino")]
        public string LugarDestino { get; set; }

        [JsonProperty("precioValorBruto")]
        public decimal PrecioValorBruto { get; set; }

        [JsonProperty("gastosTransporteFrontera")]
        public decimal GastosTransporteFrontera { get; set; }

        [JsonProperty("gastosSeguroFrontera")]
        public decimal GastosSeguroFrontera { get; set; }

        [JsonProperty("totalFobFrontera")]
        public decimal TotalFobFrontera { get; set; }

        [JsonProperty("montoTransporteFrontera")]
        public decimal MontoTransporteFrontera { get; set; }

        [JsonProperty("montoSeguroInternacional")]
        public decimal MontoSeguroInternacional { get; set; }

    }

    public class DetalleNotaExportacion : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
        [JsonProperty("codigoNandina")] public int CodigoNandina { get; set; }
    }

}
