using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class NotaZonaFranca : Factura
    {
        [JsonProperty("detalle")] new public List<DetalleNotaZonaFranca> ListaDetalle { get; set; }

        public NotaZonaFranca()
        {
            Cabecera = new CabeceraNotaZonaFranca();
            ListaDetalle = new List<DetalleNotaZonaFranca>();
            Tipo = TipoFactura.notaZonaFranca;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("NumeroParteRecepcion", "fehfenpre");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
        }
    }

    public class CabeceraNotaZonaFranca : Cabecera
    {
        [JsonProperty("numeroTarjeta")] public long? NumeroTarjeta { get; set; }
        [JsonProperty("numeroParteRecepcion")] public long NumeroParteRecepcion { get; set; }
    }

    public class DetalleNotaZonaFranca : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
    }
}
