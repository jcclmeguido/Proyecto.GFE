using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{

#warning Actualizar enumeración cuando Tesabiz termine su trabajo
    public enum TipoFactura
    {                                                   //AUN NO TIENEN MAPEADORES
        facturaEstandar = 1,
        facturaColegio = 2,
        facturaAlquiler = 3,
        facturaCombustible = 4,                         
        facturaServicios = 5,
        facturaEmbotelladoras = 6,                      
        facturaBancos = 7,                              
        facturaHoteles = 8,         
        facturaHospitales = 9,
        facturaJuegos = 10,                             
        facturaEspectaculoPublicoInternacional = 11,    
        notaExportacion = 12,  
        notaLibreConsignacion = 13,
        notaZonaFranca = 14,
        notaEspectaculoPublicoNacional = 15,            
        notaSeguridadAlimentaria = 16,                  
        notaMonedaExtranjera = 17,
        NOTA_DE_CREDITO_DEBITO = 18,                    //X
        NOTA_DE_CONCILIACION = 19,                      //X
        BOLETO_AEREO = 20,                              //X
        notaTurismoReceptivo = 21,                      
        notaTasaCero = 22,                              //Factura generica
        facturaHidrocarburos = 23                       
    };

    public class Factura
    {
        protected Dictionary<string, string> _camposDB;

        [JsonIgnore] public TipoFactura Tipo { get; set; }
        [JsonIgnore] public Dictionary<string,string> CamposDB { get { return _camposDB; } }
        [JsonProperty("cabecera")] public Cabecera Cabecera { get; set; }
        [JsonProperty("detalle")] virtual public List<Detalle> ListaDetalle { get; set; }

        public Factura() {
            ListaDetalle = new List<Detalle>();
            CargarCamposDB();
        }

        virtual protected void CargarCamposDB() {
            _camposDB = new Dictionary<string, string>();
            _camposDB.Add("NumeroFactura", "fehfenfac");
            _camposDB.Add("Direccion", "fehfedire");
            _camposDB.Add("FechaEmision", "fehfefemi");
            _camposDB.Add("CodigoTipoDocumentoIdentidad", "fehfectdi");
            _camposDB.Add("CUF", "fehfeccuf");
            _camposDB.Add("NumeroDocumento", "fehfendoc");
            _camposDB.Add("Complemento", "fehfecomp");
            _camposDB.Add("CodigoSucursal", "fehfecsuc");
            _camposDB.Add("CodigoPuntoVenta", "fehfecpve");
            _camposDB.Add("NombreRazonSocial", "fehfersoc");
            _camposDB.Add("CodigoMoneda", "fehfecmon");
            _camposDB.Add("MontoTotal", "fehfemtot");
            _camposDB.Add("CodigoCliente", "fehfeccli");
            _camposDB.Add("MontoTotalMoneda", "fehfemtmo");
            _camposDB.Add("TipoCambio", "fehfetcam");
            _camposDB.Add("CodigoDocumentoSector", "fehfecdse");
            _camposDB.Add("NITEmisor", "fehfenemi");
            _camposDB.Add("CodigoMetodoPago", "fehfecmpa");
            _camposDB.Add("MontoDescuento", "fehfemdes");
            _camposDB.Add("Leyenda", "fehfeleye");
            _camposDB.Add("Usuario", "fehfeusua");
            _camposDB.Add("ActividadEconomica", "fedfeaeco");
            _camposDB.Add("CodigoProductoSIN", "fedfecpsi");
            _camposDB.Add("CodigoProducto", "fedfecpro");
            _camposDB.Add("Descripcion", "fedfedesc");
            _camposDB.Add("SubTotal", "fedfestot");
            _camposDB.Add("MontoDescuentoDetalle", "fedfemdes");    
            _camposDB.Add("UnidadMedida", "fedfeumed");
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
    }
    public class Cabecera
    {
        [JsonProperty("numeroFactura")] public long NumeroFactura { get; set; }
        [JsonProperty("direccion")] public string Direccion { get; set; }
        [JsonProperty("fechaEmision")] public string FechaEmision { get; set; }
        [JsonProperty("codigoTipoDocumentoIdentidad")] public string CodigoTipoDocumentoIdentidad { get; set; }
        [JsonProperty("cuf")] public string CUF { get; set; }
        [JsonProperty("numeroDocumento")] public string NumeroDocumento { get; set; }
        [JsonProperty("complemento")] public string Complemento { get; set; }
        [JsonProperty("codigoSucursal")] public int CodigoSucursal { get; set; }
        [JsonProperty("codigoPuntoVenta")] public int CodigoPuntoVenta { get; set; }
        [JsonProperty("nombreRazonSocial")] public string NombreRazonSocial { get; set; }
        [JsonProperty("codigoMoneda")] public int CodigoMoneda { get; set; }
        [JsonProperty("montoTotal")] public decimal MontoTotal { get; set; }
        [JsonProperty("codigoCliente")] public string CodigoCliente { get; set; }
        [JsonProperty("montoTotalMoneda")] public decimal MontoTotalMoneda { get; set; }
        [JsonProperty("tipoCambio")] public decimal TipoCambio { get; set; }
        [JsonProperty("codigoDocumentoSector")] public int CodigoDocumentoSector { get; set; }
        [JsonProperty("nitEmisor")] public long NITEmisor { get; set; }
        [JsonProperty("codigoMetodoPago")] public int CodigoMetodoPago { get; set; }
        [JsonProperty("montoDescuento")] public decimal? MontoDescuento { get; set; }
        [JsonProperty("leyenda")] public string Leyenda { get; set; }
        [JsonProperty("usuario")] public string Usuario { get; set; }
    }

    public class Detalle
    {
        [JsonProperty("actividadEconomica")] public int? ActividadEconomica { get; set; }
        [JsonProperty("codigoProductoSin")] public int? CodigoProductoSIN { get; set; }
        [JsonProperty("codigoProducto")] public int CodigoProducto { get; set; }
        [JsonProperty("descripcion")] public string Descripcion { get; set; }
        [JsonProperty("subTotal")] public decimal SubTotal { get; set; }
        [JsonProperty("montoDescuento")] public decimal? MontoDescuentoDetalle { get; set; }
        [JsonProperty("unidadMedida")] public string UnidadMedida { get; set; }
    }



}
