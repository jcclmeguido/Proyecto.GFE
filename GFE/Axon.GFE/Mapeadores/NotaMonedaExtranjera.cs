using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class NotaMonedaExtranjera : Factura
    {
        [JsonProperty("detalle")] new public List<DetalleNotaMonedaExtranjera> ListaDetalle { get; set; }

        public NotaMonedaExtranjera()
        {
            Cabecera = new CabeceraNotaMonedaExtranjera();
            ListaDetalle = new List<DetalleNotaMonedaExtranjera>();
            Tipo = TipoFactura.notaMonedaExtranjera;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("IngresoDiferenciaCambio", "fehfeidca");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
        }
    }

    public class CabeceraNotaMonedaExtranjera : Cabecera
    {
        [JsonProperty("numeroTarjeta")]
        public long NumeroTarjeta { get; set; }

        [JsonProperty("ingresoDiferenciaCambio")]
        public decimal IngresoDiferenciaCambio { get; set; }
    }

    public class DetalleNotaMonedaExtranjera : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }
    }
}
