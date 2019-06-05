using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
   public class FacturaHotel:Factura
    {
        [JsonProperty("detalle")] new public List<DetalleFacturaHotel> ListaDetalle { get; set; }

        public FacturaHotel()
        {
            Cabecera = new CabeceraFacturaHotel();
            ListaDetalle = new List<DetalleFacturaHotel>();
            Tipo = TipoFactura.facturaHoteles;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("CantidadHuespedes", "fehfechue");
            _camposDB.Add("CantidadHabitaciones", "fehfechab");
            _camposDB.Add("CantidadMayores", "fehfecmay");
            _camposDB.Add("CantidadMenores", "fehfecmen");
            _camposDB.Add("FechaIngresoHospedaje", "fehfefiho");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
            _camposDB.Add("CodigoTipoHabitacion", "fedfectha");
            _camposDB.Add("CantidadDias", "fedfecdia");
            _camposDB.Add("PasaporteDocumentoIdentificacion", "fedfepdid");
            _camposDB.Add("Nacionalidad", "fedfenaci");
        }
    }

    public class CabeceraFacturaHotel : Cabecera
    {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }
        [JsonProperty("cantidadHuespedes")] public int CantidadHuespedes { get; set; }
        [JsonProperty("cantidadHabitaciones")] public int CantidadHabitaciones { get; set; }
        [JsonProperty("cantidadMayores")] public int CantidadMayores { get; set; }
        [JsonProperty("cantidadMenores")] public int CantidadMenores { get; set; }
        [JsonProperty("fechaIngresoHospedaje")] public string FechaIngresoHospedaje { get; set; }
    }

    public class DetalleFacturaHotel : Detalle
    {
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
        [JsonProperty("codigoTipoHabitacion")] public short CodigoTipoHabitacion { get; set; }
        [JsonProperty("cantidadDias")] public short CantidadDias { get; set; }
        [JsonProperty("pasaporteDocumentoIdentificacion")] public short PasaporteDocumentoIdentificacion { get; set; }
        [JsonProperty("nacionalidad")] public short Nacionalidad { get; set; }

    }
}
