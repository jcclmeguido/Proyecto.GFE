using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class NotaTurismoReceptivo:Factura
    {
        [JsonProperty("detalle")] new public List<DetalleNotaTurismoReceptivo> ListaDetalle { get; set; }

        public NotaTurismoReceptivo() {
            Cabecera = new CabeceraNotaTurismoReceptivo();
            ListaDetalle = new List<DetalleNotaTurismoReceptivo>();
            Tipo = TipoFactura.notaTurismoReceptivo;
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
            _camposDB.Add("NITOperadorTurismo", "fehfenotu");
            _camposDB.Add("RazonSocialOperadorTurismo", "fehfersot");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
            _camposDB.Add("CodigoTipoHabitacion", "fedfectha");
            _camposDB.Add("PasaporteDocumentoIdentificacion", "fedfepdid");
            _camposDB.Add("Nacionalidad", "fedfenaci");
        }
    }

    public class CabeceraNotaTurismoReceptivo : Cabecera {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }
        [JsonProperty("cantidadHuespedes")] public short? CantidadHuespedes { get; set; }
        [JsonProperty("cantidadHabitaciones")] public short? CantidadHabitaciones { get; set; }
        [JsonProperty("cantidadMayores")] public short? CantidadMayores { get; set; }
        [JsonProperty("cantidadMenores")] public short? CantidadMenores { get; set; }
        [JsonProperty("fechaIngresoHospedaje")] public string FechaIngresoHospedaje { get; set; }
        [JsonProperty("nitOperadorTurismo")] public string NITOperadorTurismo { get; set; }
        [JsonProperty("razonSocialOperadorTurismo")] public string RazonSocialOperadorTurismo { get; set; }
    }

    public class DetalleNotaTurismoReceptivo : Detalle {
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
        [JsonProperty("codigoTipoHabitacion")] public short? CodigoTipoHabitacion { get; set; }
        [JsonProperty("pasaporteDocumentoIdentificacion")] public string PasaporteDocumentoIdentificacion { get; set; }
        [JsonProperty("nacionalidad")] public string Nacionalidad { get; set; }
    }
}
