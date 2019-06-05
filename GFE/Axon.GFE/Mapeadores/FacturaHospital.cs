using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class FacturaHospital:Factura
    {
        [JsonProperty("detalle")] new public List<DetalleFacturaHospital> ListaDetalle { get; set; }

        public FacturaHospital()
        {
            Cabecera = new CabeceraFacturaHospital();
            ListaDetalle = new List<DetalleFacturaHospital>();
            Tipo = TipoFactura.facturaHospitales;
            CargarCamposDB();
        }

        protected override void CargarCamposDB()
        {
            base.CargarCamposDB();
            _camposDB.Add("NumeroTarjeta", "fehfentar");
            _camposDB.Add("ModalidadServicio", "fehfemser");
            _camposDB.Add("Cantidad", "fedfecant");
            _camposDB.Add("PrecioUnitario", "fedfepuni");
            _camposDB.Add("Especialidad", "fedfeespe");
            _camposDB.Add("EspecialidadDetalle", "fedfeedet");
            _camposDB.Add("NroQuirofanoSalaOperaciones", "fedfenqso");
            _camposDB.Add("EspecialidadMedico", "fedfeemed");
            _camposDB.Add("NombreApellidoMedico", "fedfename");
            _camposDB.Add("NitDocumentoMedico", "fedfenifm");
            _camposDB.Add("NroMatriculaMedico", "fedfenofm");
            _camposDB.Add("NroFacturaMedico", "fedfefmed");
        }
    }


    public class CabeceraFacturaHospital : Cabecera
    {
        [JsonProperty("numeroTarjeta")]
        public long? NumeroTarjeta { get; set; }

        [JsonProperty("modalidadServicio")]
        public string ModalidadServicio { get; set; }
    }

    public class DetalleFacturaHospital : Detalle
    {
        [JsonProperty("cantidad")] public long Cantidad { get; set; }
        [JsonProperty("precioUnitario")] public decimal PrecioUnitario { get; set; }

        [JsonProperty("especialidad")]
        public string Especialidad { get; set; }

        [JsonProperty("especialidadDetalle")]
        public string EspecialidadDetalle { get; set; }

        [JsonProperty("nroQuirofanoSalaOperaciones")]
        public int NroQuirofanoSalaOperaciones { get; set; }

        [JsonProperty("especialidadMedico")]
        public string EspecialidadMedico { get; set; }

        [JsonProperty("nombreApellidoMedico")]
        public string NombreApellidoMedico { get; set; }

        [JsonProperty("nitDocumentoMedico")]
        public string NitDocumentoMedico { get; set; }

        [JsonProperty("nroMatriculaMedico")]
        public string NroMatriculaMedico { get; set; }

        [JsonProperty("nroFacturaMedico")]
        public int NroFacturaMedico { get; set; }
    }


}
