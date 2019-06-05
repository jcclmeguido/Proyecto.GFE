using Axon.DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;

/// <summary>
/// autor:      wterrazas
/// fecha:      ene-2019
/// </summary>
namespace Axon.GFE.Mapeadores
{
    public enum TipoValidacion
    {
        FacturaElectronica = 1,                         //ValidacionFactura
        AnulacionFacturaElectronica,
        NotaCredDebElectronica,
        AnulacionNotaCredDebElectronica,
        NotaFiscalElectronica,
        AnulacionNotaFiscalElectronica,
        AnulacionBoletaContingenciaElectronica,
        PaqueteFacturaElectronica,                      //ValidacionPaquete
        PaqueteNotaFiscalElectronica,                   //ValidacionPaquete
        PaqueteNotaCredDebElectronica,                  //ValidacionPaquete
        PaqueteFacturaContingenciaElectronica,          //ValidacionPaquete
        FacturaComputarizada,                           //ValidacionFactura
        AnulacionFacturaComputarizada,
        NotasCredDebComputarizada,
        AnulacionNotasCredDebComputarizada,
        NotaFiscalComputarizada,
        AnulacionNotaFiscalComputarizada,
        AnulacionFacturaContingenciaComputarizada,
        PaqueteFacturaComputarizada,                    //ValidacionPaquete
        PaqueteNotaFiscalComputarizada,                 //ValidacionPaquete
        PaqueteNotasCredDebComputarizada,               //ValidacionPaquete
        PaqueteFacturaContingenciaComputarizada,        //ValidacionPaquete
        RecepcionFacturaCompraManual,
        RecepcionPaqueteFacturaCompraManual,
        PaqueteFacturaPrevaloradaElectronica,           //ValidacionPaquete
        FacturaPrevaloradaElectronica,                  //ValidacionPaquete
        PaqueteFacturaPrevalordaComputarizada,          //ValidacionPaquete
        FacturaPrevaloradaComputarizada                 //ValidacionPaquete
    }

    public enum EstadoValidacion {
        Procesada = 903,
        Observada = 904
    }

    [Serializable]
    public class Validacion
    {
        [JsonProperty("tipoValidacion")] public TipoValidacion Tipo { get; set; }
        [JsonProperty("idDocFiscalFEEL")] public string IdDocFiscalFEEL { get; set; }
        [JsonProperty("respuestaSIN")] public bool RespuestaSIN { get; set; }
        [JsonProperty("codigoRecepcion")] public long CodigoRecepcion { get; set; }
        [JsonProperty("estado")] public EstadoValidacion Estado { get; set; }
        [JsonProperty("listaMensajes")] public List<short> ListaMensajes { get; set; }

        protected string update;

        public Validacion() {
            update = "UPDATE fehfe SET fehfestat = ?, fehfesest = ?, fehfescre = ?, fehfeslme = ?, fehfesval = ?, fehfestva = ?, fehfevfva = ?, fehfevhva = ? WHERE fehfeifee = ";
        }

        virtual protected List<DBAxon.Parameters> ParametrosSQL() {
            List<DBAxon.Parameters> paramsUPD = new List<DBAxon.Parameters>();
            string listamensajes = string.Empty;
            if (ListaMensajes != null) listamensajes = string.Join("|", ListaMensajes.ToArray());
            paramsUPD.Add(new DBAxon.Parameters("stat", (Estado == EstadoValidacion.Procesada) ? EstadoDocumentoFiscal.E400_ValidadoOK : EstadoDocumentoFiscal.E401_FacturaObservada, ParameterDirection.Input, DbType.Int32));
            paramsUPD.Add(new DBAxon.Parameters("sest", RespuestaSIN ? 1 : 2, ParameterDirection.Input, DbType.Int16));
            paramsUPD.Add(new DBAxon.Parameters("scre", CodigoRecepcion, ParameterDirection.Input, DbType.Int64));
            paramsUPD.Add(new DBAxon.Parameters("slme", listamensajes, ParameterDirection.Input, DbType.String, 200));
            paramsUPD.Add(new DBAxon.Parameters("sval", Estado, ParameterDirection.Input, DbType.Int16));
            paramsUPD.Add(new DBAxon.Parameters("stva", Tipo, ParameterDirection.Input, DbType.Int16));
            paramsUPD.Add(new DBAxon.Parameters("vfva", DateTime.Now, ParameterDirection.Input, DbType.Date));
            paramsUPD.Add(new DBAxon.Parameters("vhva", DateTime.Now.ToString("HH:mm:ss"), ParameterDirection.Input, DbType.String, 8));
            return paramsUPD;
        }

        virtual public int GuardarRespuestaValidacion()
        {
            DBAxon db = new DBAxon();
            int resp = 0;
            try
            {
                Conexion cnx = new Conexion();
                cnx.CargarDatosConfiguracion((TipoConexion)Convert.ToInt32(ConfigurationManager.AppSettings["TipoConn"].ToString()),
                    ConfigurationManager.AppSettings["BaseDatos"].ToString(),
                    ConfigurationManager.AppSettings["DataSource"].ToString(),
                    ConfigurationManager.AppSettings["UserId"].ToString(),
                    ConfigurationManager.AppSettings["Password"].ToString(),
                    ConfigurationManager.AppSettings["dbLocale"].ToString(),
                    ConfigurationManager.AppSettings["clientLocale"].ToString());
                db.OpenFactoryConnection();

                db.SetLockModeToWait();
                db.SetDateFormat();
                DBAxon.Parameters[] prs = ParametrosSQL().ToArray();
                resp = db.ExecuteNonQuery(CommandType.Text, update + IdDocFiscalFEEL, prs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.CloseFactoryConnection();
                db = null;
            }
            return resp;
        }

        public override string ToString()
        {
            return JObject.FromObject(this).ToString();
        }
    }

    [Serializable]
    public class ValidacionFactura : Validacion {

        [JsonProperty("fechaRecepcion")] public string FechaRecepcion { get; set; }

        public ValidacionFactura() {
            Update();
        }

        private void Update()
        {
            update = "UPDATE fehfe SET fehfestat = ?, fehfesest = ?, fehfescre = ?, fehfeslme = ?, fehfesval = ?, fehfestva = ?, fehfevfva = ?, fehfevhva = ?";
            if (!string.IsNullOrEmpty(FechaRecepcion))
                update += ", fehfesfre = ?, fehfeshre = ?";
            update += " WHERE fehfeifee = ";
        }

        protected override List<DBAxon.Parameters> ParametrosSQL()
        {
            List<DBAxon.Parameters> paramsUPD = base.ParametrosSQL();
            if (!string.IsNullOrEmpty(FechaRecepcion)){
                DateTime fechahora = DateTime.ParseExact(FechaRecepcion, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                paramsUPD.Add(new DBAxon.Parameters("sfre", fechahora, ParameterDirection.Input, DbType.Date));
                paramsUPD.Add(new DBAxon.Parameters("shre", fechahora.ToString("HH:mm:ss"), ParameterDirection.Input, DbType.String, 8));
            }
            Update();
            return paramsUPD;
        }

    }

    [Serializable]
    public class ValidacionPaquete : Validacion {

        [JsonProperty("listaErrores")] public List<short> ListaErrores { get; set; }

        public ValidacionPaquete() {
            update = "UPDATE fehfe SET fehfestat = ?, fehfesest = ?, fehfescre = ?, fehfeslme = ?, fehfesval = ?, fehfestva = ?, fehfevfva = ?, fehfevhva = ?, fehfesler = ? WHERE fehfeifee = ";
        }

        protected override List<DBAxon.Parameters> ParametrosSQL() {
            List<DBAxon.Parameters> paramsUPD = base.ParametrosSQL();
            string listaerrores = string.Empty;
            if (ListaErrores != null) listaerrores = string.Join("|", ListaErrores.ToArray());
            paramsUPD.Add(new DBAxon.Parameters("sler", listaerrores, ParameterDirection.Input, DbType.String, 200));
            return paramsUPD;
        }
    }
}
