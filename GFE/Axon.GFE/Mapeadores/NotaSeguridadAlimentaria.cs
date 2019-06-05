using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class NotaSeguridadAlimentaria: Factura
    {
        [JsonProperty("detalle")] new public List<DetalleNotaSeguridadAlimentaria> ListaDetalle { get; set; }

        public NotaSeguridadAlimentaria() {
            Cabecera = new CabeceraNotaSeguridadAlimentaria();
            ListaDetalle = new List<DetalleNotaSeguridadAlimentaria>();
            Tipo = TipoFactura.notaSeguridadAlimentaria;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("CantidadDias", "fedfecdia");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
        }
    }

    public class CabeceraNotaSeguridadAlimentaria : Cabecera {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }
        [JsonProperty("cantidadDias")] public short? CantidadDias { get; set; }
    }

    public class DetalleNotaSeguridadAlimentaria : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
    }
}
