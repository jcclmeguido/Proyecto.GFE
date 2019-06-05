using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class NotaLibreConsignacion : Factura
    {
        [JsonProperty("detalle")] new public List<DetalleNotaLibreConsignacion> ListaDetalle { get; set; }

        public NotaLibreConsignacion()
        {
            Cabecera = new CabeceraNotaLibreConsignacion();
            ListaDetalle = new List<DetalleNotaLibreConsignacion>();
            Tipo = TipoFactura.notaLibreConsignacion;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("CodigoPais", "fehfecpai");
            _camposDB.Add("MontoTotalPuerto", "fehfemtpu");
            _camposDB.Add("PuertoDestino", "fehfepdes");
            _camposDB.Add("LugarDestino", "fehfeldes");
            _camposDB.Add("Remitente", "fehferemi");
            _camposDB.Add("Consignatario", "fehfecons");
            _camposDB.Add("LugarAcopioPuerto", "fehfelapu");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
        }

        
      
    }
    public class CabeceraNotaLibreConsignacion : Cabecera
    {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }

        [JsonProperty("codigoPais")]
        public int CodigoPais { get; set; }

        [JsonProperty("montoTotalPuerto")]
        public decimal MontoTotalPuerto { get; set; }

        [JsonProperty("puertoDestino")]
        public string PuertoDestino { get; set; }

        [JsonProperty("lugarDestino")]
        public string LugarDestino { get; set; }

        [JsonProperty("remitente")]
        public string Remitente { get; set; }

        [JsonProperty("consignatario")]
        public string Consignatario { get; set; }

        [JsonProperty("lugarAcopioPuerto")]
        public string LugarAcopioPuerto { get; set; }
    }

    public class DetalleNotaLibreConsignacion : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
    }
}
