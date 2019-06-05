using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using RestSharp;
using Axon.GFE.Mapeadores;
using Axon.GFE;
using System.IO;
using Axon.GFE.Servicios;

namespace Axon.WinSerAppCensador
{
    public partial class ServiceCensador : ServiceBase
    {
        #region VARIABLES GLOBALES
        Axon.DAL.Conexion oConexion = new Axon.DAL.Conexion();
        string baseDatos = ConfigurationManager.AppSettings["BaseDatos"].ToString();
        string dataSource = ConfigurationManager.AppSettings["DataSource"].ToString();
        string userId = ConfigurationManager.AppSettings["UserId"].ToString();
        string password = ConfigurationManager.AppSettings["Password"].ToString();
        string dbLocale = ConfigurationManager.AppSettings["dbLocale"].ToString();
        string clientLocale = ConfigurationManager.AppSettings["clientLocale"].ToString();
        DAL.TipoConexion tipoConexion = (DAL.TipoConexion)Convert.ToInt32(ConfigurationManager.AppSettings["TipoConn"].ToString());

        #endregion

        static string sSource = "WinSerAppGFE";
        static string sLog = "Application";
        static int idEvento;


        public ServiceCensador()
        {
            InitializeComponent();
            oConexion.CargarDatosConfiguracion(tipoConexion, baseDatos, dataSource, userId, password, clientLocale, dbLocale);

            if (!EventLog.SourceExists(sSource))
                    EventLog.CreateEventSource(sSource, sLog);

                eventLog1.Source = sSource;
                eventLog1.Log = sLog;
        }

        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }

        protected override void OnStart(string[] args)
        {
            string file = string.Empty;
            EventLog eventLog = new EventLog();

            try
            {
                if (ValidarDatosConexion())
                {
                    //while (true)
                    //{
                    //    EjecutarProcesoEnviarFacturasPendiente();
                    //}

                    //Task.Run(() =>
                    //{
                    //    Parallel.Invoke(() =>
                    //    {
                    bool bandera = true;
                    
#warning Quitar para producción
                    //#if !DEBUG
                    //                            while (bandera)
                    //                            {
                    //#else

                    DateTime dtInicio = DateTime.Now;
                    Debug.WriteLine("Iniciando..." + dtInicio.ToString("hh:mm:ss"));
                    Stopwatch cronometro = new Stopwatch();
                    RegistroTiempos.Instancia.IniciarRegistros();
                    cronometro.Start();
                    Cronometro.Instancia.Iniciar();
//#endif
                    EjecutarProcesoEnviarFacturasPendiente();
//#if !DEBUG
//                                EjecutarProcesoConsultaDocumentoFiscal();
//                                EjecutarProcesoPublicarNotaFiscal();
//#endif
//#if DEBUG
                    cronometro.Stop();
                    Debug.WriteLine(cronometro.ElapsedMilliseconds);
                    DateTime dtFin = DateTime.Now;
                    Debug.WriteLine("FIN ...:" + dtFin.ToString("hh:mm:ss"));
                    Debug.WriteLine("TOTAL: " + (dtFin - dtInicio).ToString("HH:mm:ss"));
                    RegistroTiempos.Instancia.GuardarRegistros();
                    bandera = false;
//#else
//                        }
//#endif
                    //},
                    //() =>  
                    //{
                    //while (true)
                    //{
                    //    EjecutarProcesoFacturasXAnular();
                    //}
                    //    });
                    //});
                }
                else
                {
                    file = "OnStart -> ValidarDatosConexion()";
                    Log.Instancia.LogWS_Mensaje_FSX(file, "Fallo la validacion. \r\n");
                    eventLog.WriteEntry("No paso la validacion de datos!!!", EventLogEntryType.Error, idEvento, 999);
                }
            }
            catch (Exception ex)
            {
                //Log.Instancia.LogWS_Mensaje_FSX(file, "Saltó una excepción. \r\n" + ex.Message + "\r\n");
                eventLog.WriteEntry("Excepción en: OnStart!!!", EventLogEntryType.Error, idEvento, 997);
            }
        }

        private bool ValidarDatosConexion()
        {
            idEvento = 1;
            EventLog eventLog = new EventLog();

            //string nombre = eventLog.LogDisplayName;
            //eventLog.WriteEntry("Validando datos archivo de configuracion.", EventLogEntryType.Information, idEvento, 999);

            //Nombre de la base de datos
            ParametrosConexion.DATABASE = ConfigurationManager.AppSettings["BaseDatos"].ToString();

            if (ParametrosConexion.DATABASE.Trim() == string.Empty)
            {
                idEvento = 4;
                eventLog.WriteEntry("Nombre de la base de datos no puede ser vacío/nulo.", EventLogEntryType.Error, idEvento, 999);
                return false;
            }

            ParametrosConexion.SERVERNAME = ConfigurationManager.AppSettings["DataSource"];     //Nombre del servidor
            if (ParametrosConexion.SERVERNAME.Trim() == string.Empty)
            {
                idEvento = 8;
                eventLog.WriteEntry("Nombre del servidor no puede ser vacío/nulo.", EventLogEntryType.Error, idEvento, 999);
                return false;
            }

            ParametrosConexion.User = ConfigurationManager.AppSettings["UserId"];     //Usuario
            if (ParametrosConexion.User.Trim() == string.Empty)
            {
                idEvento = 16;
                eventLog.WriteEntry("Usuario no puede ser vacío/nulo.", EventLogEntryType.Error, idEvento, 999);
                return false;
            }

            //eventLog.WriteEntry("Validación de configuracion correcta.", EventLogEntryType.Information, idEvento, 999);

            return true;
        }

        private void EjecutarProcesoEnviarFacturasPendiente()
        {
            #region SETEAR CULTURA
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB", false);
            System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyGroupSeparator = ",";
            System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator = ",";
            #endregion

            #region DEFINICION DE VARIABLES

            string file = string.Empty;
            EventLog eventLog = new EventLog();
            eventLog.Source = "WinSerAppGFE -> EjecutarProcesoEnviarFacturasPendiente";
            eventLog.Log = sLog;
            #endregion

            try
            {
                DataTable dtCabeceraFactura = MetodoFactura.ObtenerCabeceraFacturaElectronicaPendienteEnvio();
                dtCabeceraFactura.TableName = "fehfe";
                string respuesta = string.Empty;

                if (dtCabeceraFactura.Rows.Count > 0)
                {
                    //Task.Run(() =>
                    //{
                    //    Parallel.Invoke(() =>
                    //    {
                    #region DETALLE DE LA FACTURA
                    foreach (DataRow cabeceraRow in dtCabeceraFactura.Rows)
                    {
#if DEBUG
                        Cronometro.Instancia.Reiniciar();
#endif
                        file = string.Empty;
                        file = Log.Instancia.GeneraNombreLog();
                        long idDocFiscalERP = long.Parse(cabeceraRow["fehfeiddf"].ToString());
                        file += idDocFiscalERP;
                        DataTable dtDetalleFactura = MetodoFactura.ObtenerDetalleFacturaElectronicaPendienteEnvio(idDocFiscalERP);
                        dtDetalleFactura.TableName = "fedfe";

                        if (dtDetalleFactura.Rows.Count > 0)
                        {
                            try
                            {
                                bool b = false;
                                string urlEndPoint = ConfigurationManager.AppSettings["urlEndPointEnviar"].ToString();

#warning ArmarFacturaElectronicaJson2() en progreso

                                //string facturaElectronicaJson = ArmarFacturaElectronicaJson(cabeceraRow, dtDetalleFactura);

                                string facturaElectronicaJson = ArmarFacturaElectronicaJson2(cabeceraRow, dtDetalleFactura);

                                ResponseFactura respuestaFacturaEstandar = ServicioRestFul(facturaElectronicaJson, urlEndPoint);
                                ActualizacionFactura oActualizarFactura = new ActualizacionFactura();

                                if (respuestaFacturaEstandar.Factura != null && respuestaFacturaEstandar.Proceso != null &&
                                    respuestaFacturaEstandar.Respuesta != null)
                                {
                                    string codRespuesta = respuestaFacturaEstandar.Respuesta.codRespuesta;
                                    string textoRespuesta = respuestaFacturaEstandar.Respuesta.txtRespuesta;

                                    if (codRespuesta == "0")
                                        b = oActualizarFactura.ActualizarDatosFacturaEstandar(respuestaFacturaEstandar);
                                    else
                                        b = oActualizarFactura.RespuestaErrorActualizarDatosFacturaEstandar(codRespuesta, textoRespuesta, idDocFiscalERP);
                                }
                                else
                                {
                                    Log.Instancia.LogWS_Mensaje_FSX(file, "La respuesta del FEEL fue nula en unos de los Sgtes. campos. \r\n" +
                                        "Factura: " + respuestaFacturaEstandar.Factura + "\r\n" +
                                        "Proceso: " + respuestaFacturaEstandar.Proceso + "\r\n" +
                                        "Respuesta: " + respuestaFacturaEstandar.Respuesta + "\r\n");
                                    eventLog.WriteEntry(file + ". La respuesta del FEEL fue nula en unos de los Sgtesl campos. Factura, Proceso o Respuesta", EventLogEntryType.Error, 990);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción: Salto una excepción, dentro del if (dtCabeceraFactura.Rows.Count > 0) : \r\n" +
                                     ex.Message);
                                eventLog.WriteEntry(file + "Excepción: Salto una excepción, dentro del if (dtCabeceraFactura.Rows.Count > 0)", EventLogEntryType.Error, 991);
                            }
                        }
                        else
                        {
                            if (!MetodoFactura.ActualizarFacturaSinDetalle(idDocFiscalERP))
                            {
                                Log.Instancia.LogWS_Mensaje_FSX(file, "Error no se pudo actualizar la factura con error. fehfeiddf: " + idDocFiscalERP + "\r\n");
                                eventLog.WriteEntry(file + "Error no se pudo actualizar la factura con error. fehfeiddf:" + idDocFiscalERP, EventLogEntryType.Error, 992);
                            }

                            Log.Instancia.LogWS_Mensaje_FSX(file, "Esta factura NO tiene un detalle de factura. fehfeiddf: " + idDocFiscalERP + "\r\n");
                            eventLog.WriteEntry(file + "Esta factura NO tiene un detalle de factura. fehfeiddf:" + idDocFiscalERP, EventLogEntryType.Error, 993);
                        }
                    }
                    #endregion
                    //    });
                    //});
                }
            }
            catch (Exception)
            {
                Log.Instancia.LogWS_Mensaje_FSX(file, "Error windows service OnStart -> EjecutarProcesoEnviarFacturasPendiente. \r\n");
                eventLog.WriteEntry("Error windows service OnStart -> EjecutarProcesoEnviarFacturasPendiente. ", EventLogEntryType.Error, idEvento, 992);
            }
        }

        /// <summary>
        /// ::
        /// </summary>
        /// <param name="request">Datos de la factura</param>
        /// <param name="urlEndPoint">url del servicio restful</param>
        /// <returns></returns>
        private ResponseFactura ServicioRestFul(string request, string urlEndPoint)
        {
            ResponseFactura respuesta = null;
            try
            {
                //string jsonrequest = JsonConvert.SerializeObject(request);
                string hoy = DateTime.Now.ToString("ddMMyyhhmm");
                File.WriteAllText(ConfigurationManager.AppSettings["RutaMensaje_FacturacionElectronica"] + "\\" + hoy + "req.json", request);
                var restClient = new RestClient(urlEndPoint);
                RestRequest restRequest = new RestRequest(Method.POST);
                //restRequest.AddHeader("cache-control", "no-cache");
                //restRequest.AddHeader("N-MS-AUTHCB", APIKEY);
                restRequest.AddParameter("application/json", request, ParameterType.RequestBody);

#warning Quitar para producción
                //#if DEBUG
                TimeSpan tgfe = Cronometro.Instancia.Detener();
                Debug.WriteLine("GFE:\t" + tgfe.Milliseconds.ToString());
                RegistroTiempos.Instancia.AddTiempoGFE(tgfe.Milliseconds);
                Stopwatch cronos = new Stopwatch();
                cronos.Start();
//#endif
                var resp = restClient.Execute(restRequest);
//#if DEBUG
                cronos.Stop();
                Debug.WriteLine("FEEL:\t" + cronos.ElapsedMilliseconds);
                RegistroTiempos.Instancia.AddTiempoFEEL(cronos.ElapsedMilliseconds);
//#endif

                File.WriteAllText(ConfigurationManager.AppSettings["RutaMensaje_FacturacionElectronica"] + "\\" + hoy + "resp.json", resp.Content);

                respuesta = new ResponseFactura(resp.Content); //JsonConvert.DeserializeObject<ResponseFactura>(resp.Content);
                //return resp.Content;
            }
            catch (Exception ex)
            {
                Log.Instancia.LogWS_Mensaje_FSX("Error en: ServicioRestFul", "Excepción en ServicioRestFul(string request, string urlEndPoint). \r\n" +
                    ex.Message);
                eventLog1.WriteEntry("Saltó una excepción en el servicio ServicioRestFul(string request, string urlEndPoint)", EventLogEntryType.Error, idEvento, 998);
            }

            return respuesta;
        }


        private ResponseFEEL ServicioRestFulAnular(string request, string urlEndPoint)
        {
            try
            {
                string hoy = DateTime.Now.ToString("ddMMyyhhmm");
                File.WriteAllText(ConfigurationManager.AppSettings["RutaMensaje_FacturacionElectronica"] + "\\" + hoy + "req.json", request);
                var restClient = new RestClient(urlEndPoint);
                RestRequest restRequest = new RestRequest(Method.POST);
                restRequest.AddParameter("application/json", request, ParameterType.RequestBody);

                var resp = restClient.Execute(restRequest);
                File.WriteAllText(ConfigurationManager.AppSettings["RutaMensaje_FacturacionElectronica"] + "\\" + hoy + "resp.json", resp.Content);
                return JsonConvert.DeserializeObject<ResponseFEEL>(resp.Content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// ::
        /// </summary>
        /// <param name="drCabezeraFactura"></param>
        /// <param name="dtDetalleFactura"></param>
        /// <returns></returns>
        public string ArmarFacturaElectronicaJson(DataRow drCabezeraFactura, DataTable dtDetalleFactura)
        {
            string facturaJson = string.Empty;
            List<Detalle> ListadoDetalleFactura = new List<Detalle>();
            RequestFactura requestFactura = null;
            string file = Log.Instancia.GeneraNombreLog();
            file += drCabezeraFactura["fehfeiddf"].ToString(); //nro. correlativo interno.

#warning Se hicieron cambios a los mapeadores, actualizar conforme
            try
            {
                TipoFactura tipoFactura = (TipoFactura)int.Parse(drCabezeraFactura["fehfecdse"].ToString());

                switch (tipoFactura)
                {
                    case TipoFactura.facturaEstandar:  //1
                        #region FACTURA ESTANDAR
                        try
                        {
                            #region FACTURA ESTANDAR

                            requestFactura = new RequestFactura();
                            requestFactura.Factura = new FacturaEstandar();
                            //requestFactura.Factura.Cabecera = new CabeceraFacturaEstandar();
                            requestFactura.Factura.Cabecera.NumeroFactura = long.Parse(drCabezeraFactura["fehfenfac"].ToString());
                            requestFactura.Factura.Cabecera.Direccion = drCabezeraFactura["fehfedire"].ToString();
                            requestFactura.Factura.Cabecera.FechaEmision = drCabezeraFactura["fehfefemi"].ToString();
                            requestFactura.Factura.Cabecera.CodigoTipoDocumentoIdentidad = drCabezeraFactura["fehfectdi"].ToString();
                            requestFactura.Factura.Cabecera.CUF = string.Empty;
                            requestFactura.Factura.Cabecera.NumeroDocumento = drCabezeraFactura["fehfendoc"].ToString();
                            requestFactura.Factura.Cabecera.Complemento = string.Empty;
                            requestFactura.Factura.Cabecera.CodigoSucursal = int.Parse(drCabezeraFactura["fehfecsuc"].ToString());
                            requestFactura.Factura.Cabecera.CodigoPuntoVenta = Int32.Parse(drCabezeraFactura["fehfecpve"].ToString());
                            requestFactura.Factura.Cabecera.NombreRazonSocial = drCabezeraFactura["fehfersoc"].ToString();
                            requestFactura.Factura.Cabecera.MontoTotal = decimal.Parse(drCabezeraFactura["fehfemtot"].ToString());

                            if (!string.IsNullOrEmpty(drCabezeraFactura["fehfemdes"].ToString()))
                                requestFactura.Factura.Cabecera.MontoDescuento = decimal.Parse(drCabezeraFactura["fehfemdes"].ToString());
                            else
                                requestFactura.Factura.Cabecera.MontoDescuento = null;

                            requestFactura.Factura.Cabecera.CodigoCliente = drCabezeraFactura["fehfeccli"].ToString();
                            requestFactura.Factura.Cabecera.CodigoDocumentoSector = string.IsNullOrEmpty(drCabezeraFactura["fehfecdse"].ToString()) ? int.Parse(drCabezeraFactura["fehfecdse"].ToString()) : 1;
                            requestFactura.Factura.Cabecera.NITEmisor = long.Parse(drCabezeraFactura["fehfenemi"].ToString());
                            requestFactura.Factura.Cabecera.CodigoMetodoPago = int.Parse(drCabezeraFactura["fehfecmpa"].ToString());

                            if (!string.IsNullOrEmpty(drCabezeraFactura["fehfentar"].ToString()))
                                ((CabeceraFacturaEstandar)requestFactura.Factura.Cabecera).NumeroTarjeta = long.Parse(drCabezeraFactura["fehfentar"].ToString());
                            else
                                ((CabeceraFacturaEstandar)requestFactura.Factura.Cabecera).NumeroTarjeta = null;

                            requestFactura.Factura.Cabecera.Leyenda = drCabezeraFactura["fehfeleye"].ToString();
                            requestFactura.Factura.Cabecera.Usuario = drCabezeraFactura["fehfeusua"].ToString();
                            requestFactura.Factura.Cabecera.CodigoMoneda = int.Parse(drCabezeraFactura["fehfecmon"].ToString());
                            requestFactura.Factura.Cabecera.MontoTotalMoneda = decimal.Parse(drCabezeraFactura["fehfemtmo"].ToString());
                            requestFactura.Factura.Cabecera.TipoCambio = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString());

                            //((FacturaEstandar)requestFactura.Factura).ListaDetalle = new List<DetalleFacturaEstandar>();

                            foreach (DataRow dataRow in dtDetalleFactura.Rows)
                            {
                                DetalleFacturaEstandar oDetalleFacturaEstandar = new DetalleFacturaEstandar();

                                if (!string.IsNullOrEmpty(dataRow["fedfeaeco"].ToString()))
                                    oDetalleFacturaEstandar.ActividadEconomica = int.Parse(dataRow["fedfeaeco"].ToString());
                                else
                                    oDetalleFacturaEstandar.ActividadEconomica = null;

                                if (!string.IsNullOrEmpty(dataRow["fedfecpsi"].ToString()))
                                    oDetalleFacturaEstandar.CodigoProductoSIN = int.Parse(dataRow["fedfecpsi"].ToString());
                                else
                                    oDetalleFacturaEstandar.CodigoProductoSIN = null;

                                oDetalleFacturaEstandar.CodigoProducto = int.Parse(dataRow["fedfecpro"].ToString());
                                oDetalleFacturaEstandar.Descripcion = dataRow["fedfedesc"].ToString();
                                oDetalleFacturaEstandar.Cantidad = Int64.Parse(dataRow["fedfecant"].ToString());
                                oDetalleFacturaEstandar.PrecioUnitario = decimal.Parse(dataRow["fedfepuni"].ToString());

                                if (!string.IsNullOrEmpty(dataRow["fedfemdes"].ToString()))
                                    oDetalleFacturaEstandar.MontoDescuentoDetalle = decimal.Parse(dataRow["fedfemdes"].ToString());
                                else
                                    oDetalleFacturaEstandar.CodigoProductoSIN = null;
                                oDetalleFacturaEstandar.SubTotal = decimal.Parse(dataRow["fedfestot"].ToString());
                                oDetalleFacturaEstandar.NumeroSerie = dataRow["fedfenser"].ToString();
                                oDetalleFacturaEstandar.UnidadMedida = dataRow["fedfeumed"].ToString();

                                ((FacturaEstandar)requestFactura.Factura).ListaDetalle.Add(oDetalleFacturaEstandar);
                            }

                            requestFactura.IdDocFiscalERP = drCabezeraFactura["fehfeiddf"].ToString();
                            requestFactura.Cufd = null;

                            if ((drCabezeraFactura["fehfecont"].ToString()) == "0")
                                requestFactura.Contingencia = false;
                            else
                                requestFactura.Contingencia = true;

                            if ((drCabezeraFactura["fehfelote"].ToString()) == "0")
                                requestFactura.EsLote = false;
                            else
                                requestFactura.EsLote = true;

                            requestFactura.IdLoteERP = drCabezeraFactura["fehfeidlo"].ToString();

                            if ((drCabezeraFactura["fehfeufac"].ToString()) == "0")
                                requestFactura.UltFacturaLote = false;
                            else
                                requestFactura.UltFacturaLote = true;

                            requestFactura.CodigoTipoFactura = int.Parse(drCabezeraFactura["fehfectip"].ToString());

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción ArmarFacturaElectronicaJson -> TIPO_FACTURA.FACTURA_ESTANDAR \r\n" +
                                ex.Message + "\r\n");
                        }
                        #endregion
                        break;

                    case TipoFactura.facturaColegio: //2
                        #region FACTURA_SECTORES_EDUCATIVOS
                        try
                        {
                            #region FACTURA SECTOR EDUCATIVO
                            requestFactura = new RequestFactura();
                            requestFactura.Factura = new FacturaColegio();
                            requestFactura.Factura.Cabecera.NumeroFactura = long.Parse(drCabezeraFactura["fehfenfac"].ToString());
                            requestFactura.Factura.Cabecera.Direccion = drCabezeraFactura["fehfedire"].ToString();
                            requestFactura.Factura.Cabecera.FechaEmision = drCabezeraFactura["fehfefemi"].ToString();
                            requestFactura.Factura.Cabecera.CodigoTipoDocumentoIdentidad = drCabezeraFactura["fehfectdi"].ToString();
                            requestFactura.Factura.Cabecera.CUF = string.Empty;
                            requestFactura.Factura.Cabecera.NumeroDocumento = drCabezeraFactura["fehfendoc"].ToString();
                            requestFactura.Factura.Cabecera.Complemento = string.Empty;
                            requestFactura.Factura.Cabecera.CodigoSucursal = int.Parse(drCabezeraFactura["fehfecsuc"].ToString());
                            requestFactura.Factura.Cabecera.CodigoPuntoVenta = Int32.Parse(drCabezeraFactura["fehfecpve"].ToString());
                            requestFactura.Factura.Cabecera.NombreRazonSocial = drCabezeraFactura["fehfersoc"].ToString();
                            requestFactura.Factura.Cabecera.MontoTotal = decimal.Parse(drCabezeraFactura["fehfemtot"].ToString());

                            if (!string.IsNullOrEmpty(drCabezeraFactura["fehfemdes"].ToString()))
                                requestFactura.Factura.Cabecera.MontoDescuento = decimal.Parse(drCabezeraFactura["fehfemdes"].ToString());
                            else
                                requestFactura.Factura.Cabecera.MontoDescuento = null;

                            requestFactura.Factura.Cabecera.CodigoCliente = drCabezeraFactura["fehfeccli"].ToString();
                            requestFactura.Factura.Cabecera.CodigoDocumentoSector = string.IsNullOrEmpty(drCabezeraFactura["fehfecdse"].ToString()) ? int.Parse(drCabezeraFactura["fehfecdse"].ToString()) : 1;
                            requestFactura.Factura.Cabecera.NITEmisor = long.Parse(drCabezeraFactura["fehfenemi"].ToString());
                            requestFactura.Factura.Cabecera.CodigoMetodoPago = int.Parse(drCabezeraFactura["fehfecmpa"].ToString());

                            if (!string.IsNullOrEmpty(drCabezeraFactura["fehfentar"].ToString()))
                                ((CabeceraFacturaColegio)requestFactura.Factura.Cabecera).NumeroTarjeta = long.Parse(drCabezeraFactura["fehfentar"].ToString());
                            else
                                ((CabeceraFacturaColegio)requestFactura.Factura.Cabecera).NumeroTarjeta = null;

                            requestFactura.Factura.Cabecera.Leyenda = drCabezeraFactura["fehfeleye"].ToString();
                            requestFactura.Factura.Cabecera.Usuario = drCabezeraFactura["fehfeusua"].ToString();
                            requestFactura.Factura.Cabecera.CodigoMoneda = int.Parse(drCabezeraFactura["fehfecmon"].ToString());
                            requestFactura.Factura.Cabecera.MontoTotalMoneda = decimal.Parse(drCabezeraFactura["fehfemtmo"].ToString());
                            requestFactura.Factura.Cabecera.TipoCambio = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString());

                            //((FacturaEstandar)requestFactura.Factura).ListaDetalle = new List<DetalleFacturaEstandar>();

                            foreach (DataRow dataRow in dtDetalleFactura.Rows)
                            {
                                DetalleFacturaColegio oDetalleFacturaColegio = new DetalleFacturaColegio();

                                if (!string.IsNullOrEmpty(dataRow["fedfeaeco"].ToString()))
                                    oDetalleFacturaColegio.ActividadEconomica = int.Parse(dataRow["fedfeaeco"].ToString());
                                else
                                    oDetalleFacturaColegio.ActividadEconomica = null;

                                if (!string.IsNullOrEmpty(dataRow["fedfecpsi"].ToString()))
                                    oDetalleFacturaColegio.CodigoProductoSIN = int.Parse(dataRow["fedfecpsi"].ToString());
                                else
                                    oDetalleFacturaColegio.CodigoProductoSIN = null;

                                oDetalleFacturaColegio.CodigoProducto = int.Parse(dataRow["fedfecpro"].ToString());
                                oDetalleFacturaColegio.Descripcion = dataRow["fedfedesc"].ToString();
                                oDetalleFacturaColegio.Cantidad = Int64.Parse(dataRow["fedfecant"].ToString());
                                //oDetalleFacturaEstandar.PrecioUnitario = decimal.Parse(dataRow["fedfepuni"].ToString());

                                if (!string.IsNullOrEmpty(dataRow["fedfemdes"].ToString()))
                                    oDetalleFacturaColegio.MontoDescuentoDetalle = decimal.Parse(dataRow["fedfemdes"].ToString());
                                else
                                    oDetalleFacturaColegio.CodigoProductoSIN = null;
                                oDetalleFacturaColegio.SubTotal = decimal.Parse(dataRow["fedfestot"].ToString());
                                //oDetalleFacturaEstandar.NumeroSerie = dataRow["fedfenser"].ToString();
                                oDetalleFacturaColegio.UnidadMedida = dataRow["fedfeumed"].ToString();

                                ((FacturaColegio)requestFactura.Factura).ListaDetalle.Add(oDetalleFacturaColegio);
                            }

                            requestFactura.IdDocFiscalERP = drCabezeraFactura["fehfeiddf"].ToString();
                            requestFactura.Cufd = null;

                            if ((drCabezeraFactura["fehfecont"].ToString()) == "0")
                                requestFactura.Contingencia = false;
                            else
                                requestFactura.Contingencia = true;

                            if ((drCabezeraFactura["fehfelote"].ToString()) == "0")
                                requestFactura.EsLote = false;
                            else
                                requestFactura.EsLote = true;

                            requestFactura.IdLoteERP = drCabezeraFactura["fehfeidlo"].ToString();

                            if ((drCabezeraFactura["fehfeufac"].ToString()) == "0")
                                requestFactura.UltFacturaLote = false;
                            else
                                requestFactura.UltFacturaLote = true;

                            requestFactura.CodigoTipoFactura = int.Parse(drCabezeraFactura["fehfectip"].ToString());

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción ArmarFacturaElectronicaJson -> TIPO_FACTURA.FACTURA_SECTORES_EDUCATIVOS \r\n" +
                                ex.Message + "\r\n");
                        }
                        #endregion
                        break;

                    case TipoFactura.facturaAlquiler:  //3
                        #region FACTURA DE ALQUILER DE BIENES INMUEBLES
                        try
                        {
                            #region FACTURA DE ALQUILER
                            requestFactura = new RequestFactura();
                            requestFactura.Factura = new FacturaAlquiler();
                            requestFactura.Factura.Cabecera.NumeroFactura = long.Parse(drCabezeraFactura["fehfenfac"].ToString());
                            requestFactura.Factura.Cabecera.Direccion = drCabezeraFactura["fehfedire"].ToString();
                            requestFactura.Factura.Cabecera.FechaEmision = drCabezeraFactura["fehfefemi"].ToString();
                            requestFactura.Factura.Cabecera.CodigoTipoDocumentoIdentidad = drCabezeraFactura["fehfectdi"].ToString();
                            requestFactura.Factura.Cabecera.CUF = string.Empty;
                            requestFactura.Factura.Cabecera.NumeroDocumento = drCabezeraFactura["fehfendoc"].ToString();
                            requestFactura.Factura.Cabecera.Complemento = string.Empty;
                            requestFactura.Factura.Cabecera.CodigoSucursal = int.Parse(drCabezeraFactura["fehfecsuc"].ToString());
                            requestFactura.Factura.Cabecera.CodigoPuntoVenta = Int32.Parse(drCabezeraFactura["fehfecpve"].ToString());
                            requestFactura.Factura.Cabecera.NombreRazonSocial = drCabezeraFactura["fehfersoc"].ToString();
                            requestFactura.Factura.Cabecera.MontoTotal = decimal.Parse(drCabezeraFactura["fehfemtot"].ToString());

                            if (!string.IsNullOrEmpty(drCabezeraFactura["fehfemdes"].ToString()))
                                requestFactura.Factura.Cabecera.MontoDescuento = decimal.Parse(drCabezeraFactura["fehfemdes"].ToString());
                            else
                                requestFactura.Factura.Cabecera.MontoDescuento = null;

                            requestFactura.Factura.Cabecera.CodigoCliente = drCabezeraFactura["fehfeccli"].ToString();
                            requestFactura.Factura.Cabecera.CodigoDocumentoSector = string.IsNullOrEmpty(drCabezeraFactura["fehfecdse"].ToString()) ? int.Parse(drCabezeraFactura["fehfecdse"].ToString()) : 1;
                            requestFactura.Factura.Cabecera.NITEmisor = long.Parse(drCabezeraFactura["fehfenemi"].ToString());
                            requestFactura.Factura.Cabecera.CodigoMetodoPago = int.Parse(drCabezeraFactura["fehfecmpa"].ToString());

                            if (!string.IsNullOrEmpty(drCabezeraFactura["fehfentar"].ToString()))
                                ((CabeceraFacturaAlquiler)requestFactura.Factura.Cabecera).NumeroTarjeta = long.Parse(drCabezeraFactura["fehfentar"].ToString());
                            else
                                ((CabeceraFacturaAlquiler)requestFactura.Factura.Cabecera).NumeroTarjeta = null;

                            requestFactura.Factura.Cabecera.Leyenda = drCabezeraFactura["fehfeleye"].ToString();
                            requestFactura.Factura.Cabecera.Usuario = drCabezeraFactura["fehfeusua"].ToString();
                            requestFactura.Factura.Cabecera.CodigoMoneda = int.Parse(drCabezeraFactura["fehfecmon"].ToString());
                            requestFactura.Factura.Cabecera.MontoTotalMoneda = decimal.Parse(drCabezeraFactura["fehfemtmo"].ToString());
                            requestFactura.Factura.Cabecera.TipoCambio = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString());
                            //NUEVO
                            ((CabeceraFacturaAlquiler)requestFactura.Factura.Cabecera).PeriodoFacturado = drCabezeraFactura["fehfepfac"].ToString();

                            foreach (DataRow dataRow in dtDetalleFactura.Rows)
                            {
                                DetalleFacturaAlquiler oDetalleFacturaAlquiler = new DetalleFacturaAlquiler();

                                if (!string.IsNullOrEmpty(dataRow["fedfeaeco"].ToString()))
                                    oDetalleFacturaAlquiler.ActividadEconomica = int.Parse(dataRow["fedfeaeco"].ToString());
                                else
                                    oDetalleFacturaAlquiler.ActividadEconomica = null;

                                if (!string.IsNullOrEmpty(dataRow["fedfecpsi"].ToString()))
                                    oDetalleFacturaAlquiler.CodigoProductoSIN = int.Parse(dataRow["fedfecpsi"].ToString());
                                else
                                    oDetalleFacturaAlquiler.CodigoProductoSIN = null;

                                oDetalleFacturaAlquiler.CodigoProducto = int.Parse(dataRow["fedfecpro"].ToString());
                                oDetalleFacturaAlquiler.Descripcion = dataRow["fedfedesc"].ToString();
                                oDetalleFacturaAlquiler.Cantidad = Int64.Parse(dataRow["fedfecant"].ToString());
                                //oDetalleFacturaEstandar.PrecioUnitario = decimal.Parse(dataRow["fedfepuni"].ToString());

                                if (!string.IsNullOrEmpty(dataRow["fedfemdes"].ToString()))
                                    oDetalleFacturaAlquiler.MontoDescuentoDetalle = decimal.Parse(dataRow["fedfemdes"].ToString());
                                else
                                    oDetalleFacturaAlquiler.CodigoProductoSIN = null;
                                oDetalleFacturaAlquiler.SubTotal = decimal.Parse(dataRow["fedfestot"].ToString());
                                //oDetalleFacturaEstandar.NumeroSerie = dataRow["fedfenser"].ToString();
                                oDetalleFacturaAlquiler.UnidadMedida = dataRow["fedfeumed"].ToString();

                                ((FacturaAlquiler)requestFactura.Factura).ListaDetalle.Add(oDetalleFacturaAlquiler);
                            }

                            requestFactura.IdDocFiscalERP = drCabezeraFactura["fehfeiddf"].ToString();
                            requestFactura.Cufd = null;

                            if ((drCabezeraFactura["fehfecont"].ToString()) == "0")
                                requestFactura.Contingencia = false;
                            else
                                requestFactura.Contingencia = true;

                            if ((drCabezeraFactura["fehfelote"].ToString()) == "0")
                                requestFactura.EsLote = false;
                            else
                                requestFactura.EsLote = true;

                            requestFactura.IdLoteERP = drCabezeraFactura["fehfeidlo"].ToString();

                            if ((drCabezeraFactura["fehfeufac"].ToString()) == "0")
                                requestFactura.UltFacturaLote = false;
                            else
                                requestFactura.UltFacturaLote = true;

                            requestFactura.CodigoTipoFactura = int.Parse(drCabezeraFactura["fehfectip"].ToString());

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción ArmarFacturaElectronicaJson -> TIPO_FACTURA.FACTURA_DE_ALQUILER_DE_BIENES_INMUEBLES \r\n" +
                                ex.Message + "\r\n");
                        }
                        #endregion
                        break;

                    case TipoFactura.facturaServicios: //5
                        #region FACTURA_DE_SERVICIOS_BASICOS
                        try
                        {
                            #region FACTURA SERVICIOS BASICOS
                            requestFactura = new RequestFactura()
                            {
                                Factura = new FacturaServiciosBasicos()
                                {
                                    Cabecera = new CabeceraFacturaServiciosBasicos()
                                    {
                                        NumeroFactura = Int64.Parse(drCabezeraFactura["fehfenfac"].ToString()),
                                        Direccion = drCabezeraFactura["fehfedire"].ToString(),
                                        FechaEmision = drCabezeraFactura["fehfefemi"].ToString(),
                                        CodigoTipoDocumentoIdentidad = drCabezeraFactura["fehfectdi"].ToString(),
                                        CUF = string.Empty,
                                        NumeroDocumento = drCabezeraFactura["fehfendoc"].ToString(),
                                        Complemento = string.Empty,
                                        CodigoSucursal = int.Parse(drCabezeraFactura["fehfecsuc"].ToString()),
                                        CodigoPuntoVenta = Int32.Parse(drCabezeraFactura["fehfecpve"].ToString()),
                                        NombreRazonSocial = drCabezeraFactura["fehfersoc"].ToString(),
                                        MontoTotal = decimal.Parse(drCabezeraFactura["fehfemtot"].ToString()),
                                        MontoDescuento = decimal.Parse(drCabezeraFactura["fehfemdes"].ToString()),
                                        CodigoCliente = drCabezeraFactura["fehfeccli"].ToString(),
                                        CodigoDocumentoSector = string.IsNullOrEmpty(drCabezeraFactura["fehfecdse"].ToString()) ? int.Parse(drCabezeraFactura["fehfecdse"].ToString()) : 1,
                                        NITEmisor = 305080026,
                                        CodigoMetodoPago = int.Parse(drCabezeraFactura["fehfecmpa"].ToString()),
                                        //NumeroTarjeta = !string.IsNullOrEmpty(dtCabezeraFactura["fehfentar"].ToString()) ? Int64.Parse(dtCabezeraFactura["fehfentar"].ToString()) : 0,
                                        Leyenda = drCabezeraFactura["fehfeleye"].ToString(),
                                        Usuario = "FTL",
                                        CodigoMoneda = 1,
                                        MontoTotalMoneda = decimal.Parse(drCabezeraFactura["fehfemtmo"].ToString()),
                                        TipoCambio = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString()),
                                        //CAMPOS NUEVOS
                                        Ciudad = drCabezeraFactura["fehfetcam"].ToString(),
                                        Zona = drCabezeraFactura["fehfetcam"].ToString(),
                                        NumeroMedidor = drCabezeraFactura["fehfetcam"].ToString(),
                                        Gestion = drCabezeraFactura["fehfetcam"].ToString(),
                                        Mes = drCabezeraFactura["fehfetcam"].ToString(),
                                        DomicilioComprador = drCabezeraFactura["fehfetcam"].ToString(),
                                        MontoTotalSujetoIVA = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString()),
                                        ConsumoKwh = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString()),
                                        ConsumoMetrosCubicos = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString()),
                                        MontoDescuentoLey1886 = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString()),
                                        TasaAseo = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString()),
                                        TasaAlumbrado = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString())
                                    },
                                    //ListaDetalle = new List<Detalle>()
                                },
                                IdDocFiscalERP = drCabezeraFactura["fehfeiddf"].ToString(),
                                Cufd = null,
                                Contingencia = !(drCabezeraFactura["fehfecont"].ToString() == "0") ? true : false,
                                EsLote = !(drCabezeraFactura["fehfelote"].ToString() == "0") ? true : false,
                                IdLoteERP = drCabezeraFactura["fehfeidlo"].ToString(),
                                UltFacturaLote = !(drCabezeraFactura["fehfeufac"].ToString() == "0") ? true : false,
                                CodigoTipoFactura = 1
                            };

                            foreach (DataRow dataRow in dtDetalleFactura.Rows)
                            {
                                requestFactura.Factura.ListaDetalle.Add(new DetalleFacturaEstandar()
                                {
                                    ActividadEconomica = int.Parse(dataRow["fedfeaeco"].ToString()),
                                    CodigoProductoSIN = !string.IsNullOrEmpty(dataRow["fedfecpsi"].ToString()) ? int.Parse(dataRow["fedfecpsi"].ToString()) : 0,
                                    CodigoProducto = int.Parse(dataRow["fedfecpro"].ToString()),
                                    Descripcion = dataRow["fedfedesc"].ToString(),
                                    Cantidad = Int64.Parse(dataRow["fedfecant"].ToString()),
                                    PrecioUnitario = decimal.Parse(dataRow["fedfepuni"].ToString()),
                                    MontoDescuentoDetalle = decimal.Parse(dataRow["fedfemdes"].ToString()),
                                    SubTotal = decimal.Parse(dataRow["fedfestot"].ToString()),
                                    NumeroSerie = dataRow["fedfenser"].ToString(),
                                    UnidadMedida = dataRow["fedfeumed"].ToString()
                                });
                            }

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción ArmarFacturaElectronicaJson -> TIPO_FACTURA.FACTURA_DE_SERVICIOS_BASICOS \r\n" +
                                ex.Message + "\r\n");
                        }
                        #endregion
                        break;

                    case TipoFactura.facturaHoteles:  //8
                        #region FACTURA ESTANDAR
                        try
                        {
                            #region FACTURA ESTANDAR

                            requestFactura = new RequestFactura();
                            requestFactura.Factura = new FacturaEstandar();
                            requestFactura.Factura.Cabecera = new CabeceraFacturaEstandar();
                            requestFactura.Factura.Cabecera.NumeroFactura = long.Parse(drCabezeraFactura["fehfenfac"].ToString());
                            requestFactura.Factura.Cabecera.Direccion = drCabezeraFactura["fehfedire"].ToString();
                            requestFactura.Factura.Cabecera.FechaEmision = drCabezeraFactura["fehfefemi"].ToString();
                            requestFactura.Factura.Cabecera.CodigoTipoDocumentoIdentidad = drCabezeraFactura["fehfectdi"].ToString();
                            requestFactura.Factura.Cabecera.CUF = string.Empty;
                            requestFactura.Factura.Cabecera.NumeroDocumento = drCabezeraFactura["fehfendoc"].ToString();
                            requestFactura.Factura.Cabecera.Complemento = string.Empty;
                            requestFactura.Factura.Cabecera.CodigoSucursal = int.Parse(drCabezeraFactura["fehfecsuc"].ToString());
                            requestFactura.Factura.Cabecera.CodigoPuntoVenta = Int32.Parse(drCabezeraFactura["fehfecpve"].ToString());
                            requestFactura.Factura.Cabecera.NombreRazonSocial = drCabezeraFactura["fehfersoc"].ToString();
                            requestFactura.Factura.Cabecera.MontoTotal = decimal.Parse(drCabezeraFactura["fehfemtot"].ToString());

                            if (!string.IsNullOrEmpty(drCabezeraFactura["fehfemdes"].ToString()))

                                requestFactura.Factura.Cabecera.MontoDescuento = decimal.Parse(drCabezeraFactura["fehfemdes"].ToString());

                            else
                                requestFactura.Factura.Cabecera.MontoDescuento = null;


                            requestFactura.Factura.Cabecera.CodigoCliente = drCabezeraFactura["fehfeccli"].ToString();
                            requestFactura.Factura.Cabecera.CodigoDocumentoSector = string.IsNullOrEmpty(drCabezeraFactura["fehfecdse"].ToString()) ? int.Parse(drCabezeraFactura["fehfecdse"].ToString()) : 1;
                            requestFactura.Factura.Cabecera.NITEmisor = long.Parse(drCabezeraFactura["fehfenemi"].ToString());
                            requestFactura.Factura.Cabecera.CodigoMetodoPago = int.Parse(drCabezeraFactura["fehfecmpa"].ToString());

                            if (!string.IsNullOrEmpty(drCabezeraFactura["fehfentar"].ToString()))

                                ((CabeceraFacturaEstandar)requestFactura.Factura.Cabecera).NumeroTarjeta = long.Parse(drCabezeraFactura["fehfentar"].ToString());
                            else
                                ((CabeceraFacturaEstandar)requestFactura.Factura.Cabecera).NumeroTarjeta = null;

                            requestFactura.Factura.Cabecera.Leyenda = drCabezeraFactura["fehfeleye"].ToString();
                            requestFactura.Factura.Cabecera.Usuario = drCabezeraFactura["fehfeusua"].ToString();
                            requestFactura.Factura.Cabecera.CodigoMoneda = int.Parse(drCabezeraFactura["fehfecmon"].ToString());
                            requestFactura.Factura.Cabecera.MontoTotalMoneda = decimal.Parse(drCabezeraFactura["fehfemtmo"].ToString());
                            requestFactura.Factura.Cabecera.TipoCambio = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString());

                            requestFactura.Factura.ListaDetalle = new List<Detalle>();

                            foreach (DataRow dataRow in dtDetalleFactura.Rows)
                            {
                                DetalleFacturaEstandar oDetalleFacturaEstandar = new DetalleFacturaEstandar();

                                if (!string.IsNullOrEmpty(dataRow["fedfeaeco"].ToString()))
                                    oDetalleFacturaEstandar.ActividadEconomica = int.Parse(dataRow["fedfeaeco"].ToString());
                                else
                                    oDetalleFacturaEstandar.ActividadEconomica = null;

                                if (!string.IsNullOrEmpty(dataRow["fedfecpsi"].ToString()))
                                    oDetalleFacturaEstandar.CodigoProductoSIN = int.Parse(dataRow["fedfecpsi"].ToString());
                                else
                                    oDetalleFacturaEstandar.CodigoProductoSIN = null;

                                oDetalleFacturaEstandar.CodigoProducto = int.Parse(dataRow["fedfecpro"].ToString());
                                oDetalleFacturaEstandar.Descripcion = dataRow["fedfedesc"].ToString();
                                oDetalleFacturaEstandar.Cantidad = Int64.Parse(dataRow["fedfecant"].ToString());
                                oDetalleFacturaEstandar.PrecioUnitario = decimal.Parse(dataRow["fedfepuni"].ToString());

                                if (!string.IsNullOrEmpty(dataRow["fedfemdes"].ToString()))
                                    oDetalleFacturaEstandar.MontoDescuentoDetalle = decimal.Parse(dataRow["fedfemdes"].ToString());
                                else
                                    oDetalleFacturaEstandar.CodigoProductoSIN = null;
                                oDetalleFacturaEstandar.SubTotal = decimal.Parse(dataRow["fedfestot"].ToString());
                                oDetalleFacturaEstandar.NumeroSerie = dataRow["fedfenser"].ToString();
                                oDetalleFacturaEstandar.UnidadMedida = dataRow["fedfeumed"].ToString();

                                requestFactura.Factura.ListaDetalle.Add(oDetalleFacturaEstandar);
                            }

                            requestFactura.IdDocFiscalERP = drCabezeraFactura["fehfeiddf"].ToString();
                            requestFactura.Cufd = null;

                            if ((drCabezeraFactura["fehfecont"].ToString()) == "0")

                                requestFactura.Contingencia = false;
                            else
                                requestFactura.Contingencia = true;

                            if ((drCabezeraFactura["fehfelote"].ToString()) == "0")
                                requestFactura.EsLote = false;
                            else
                                requestFactura.EsLote = true;

                            requestFactura.IdLoteERP = drCabezeraFactura["fehfeidlo"].ToString();

                            if ((drCabezeraFactura["fehfeufac"].ToString()) == "0")
                                requestFactura.UltFacturaLote = false;
                            else
                                requestFactura.UltFacturaLote = true;

                            requestFactura.CodigoTipoFactura = int.Parse(drCabezeraFactura["fehfectip"].ToString());

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción ArmarFacturaElectronicaJson -> TIPO_FACTURA.FACTURA_ESTANDAR \r\n" +
                                ex.Message + "\r\n");
                        }
                        #endregion
                        break;

                    case TipoFactura.facturaHospitales:  //9
                        #region FACTURAS_DE_HOSPITALES_CLINICAS  
                        try
                        {
                            #region FACTURA HOSPITALES

                            requestFactura = new RequestFactura();
                            requestFactura.Factura = new FacturaHospital();
                            requestFactura.Factura.Cabecera = new CabeceraFacturaHospital();
                            requestFactura.Factura.Cabecera.NumeroFactura = long.Parse(drCabezeraFactura["fehfenfac"].ToString());
                            requestFactura.Factura.Cabecera.Direccion = drCabezeraFactura["fehfedire"].ToString();
                            requestFactura.Factura.Cabecera.FechaEmision = drCabezeraFactura["fehfefemi"].ToString();
                            requestFactura.Factura.Cabecera.CodigoTipoDocumentoIdentidad = drCabezeraFactura["fehfectdi"].ToString();
                            requestFactura.Factura.Cabecera.CUF = string.Empty;
                            requestFactura.Factura.Cabecera.NumeroDocumento = drCabezeraFactura["fehfendoc"].ToString();
                            requestFactura.Factura.Cabecera.Complemento = string.Empty;
                            requestFactura.Factura.Cabecera.CodigoSucursal = int.Parse(drCabezeraFactura["fehfecsuc"].ToString());
                            requestFactura.Factura.Cabecera.CodigoPuntoVenta = Int32.Parse(drCabezeraFactura["fehfecpve"].ToString());
                            requestFactura.Factura.Cabecera.NombreRazonSocial = drCabezeraFactura["fehfersoc"].ToString();
                            requestFactura.Factura.Cabecera.MontoTotal = decimal.Parse(drCabezeraFactura["fehfemtot"].ToString());
                            requestFactura.Factura.Cabecera.MontoDescuento = decimal.Parse(drCabezeraFactura["fehfemdes"].ToString());
                            requestFactura.Factura.Cabecera.CodigoCliente = drCabezeraFactura["fehfeccli"].ToString();
                            requestFactura.Factura.Cabecera.CodigoDocumentoSector = string.IsNullOrEmpty(drCabezeraFactura["fehfecdse"].ToString()) ? int.Parse(drCabezeraFactura["fehfecdse"].ToString()) : 1;
                            requestFactura.Factura.Cabecera.NITEmisor = long.Parse(drCabezeraFactura["fehfenemi"].ToString());
                            requestFactura.Factura.Cabecera.CodigoMetodoPago = int.Parse(drCabezeraFactura["fehfecmpa"].ToString());

                            if (!string.IsNullOrEmpty(drCabezeraFactura["fehfentar"].ToString()))

                                ((CabeceraFacturaHospital)requestFactura.Factura.Cabecera).NumeroTarjeta = long.Parse(drCabezeraFactura["fehfentar"].ToString());
                            else
                                ((CabeceraFacturaHospital)requestFactura.Factura.Cabecera).NumeroTarjeta = null;

                            requestFactura.Factura.Cabecera.Leyenda = drCabezeraFactura["fehfeleye"].ToString();
                            requestFactura.Factura.Cabecera.Usuario = drCabezeraFactura["fehfeusua"].ToString();
                            requestFactura.Factura.Cabecera.CodigoMoneda = int.Parse(drCabezeraFactura["fehfecmon"].ToString());
                            requestFactura.Factura.Cabecera.MontoTotalMoneda = decimal.Parse(drCabezeraFactura["fehfemtmo"].ToString());
                            requestFactura.Factura.Cabecera.TipoCambio = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString());
                            //NUEVO
                            ((CabeceraFacturaHospital)requestFactura.Factura.Cabecera).ModalidadServicio = drCabezeraFactura["fehfemser"].ToString();


                            requestFactura.Factura.ListaDetalle = new List<Detalle>();

                            foreach (DataRow dataRow in dtDetalleFactura.Rows)
                            {
                                DetalleFacturaHospital oDetalleFacturaEstandar = new DetalleFacturaHospital();

                                if (!string.IsNullOrEmpty(dataRow["fedfeaeco"].ToString()))
                                    oDetalleFacturaEstandar.ActividadEconomica = int.Parse(dataRow["fedfeaeco"].ToString());
                                else
                                    oDetalleFacturaEstandar.ActividadEconomica = null;

                                if (!string.IsNullOrEmpty(dataRow["fedfecpsi"].ToString()))
                                    oDetalleFacturaEstandar.CodigoProductoSIN = int.Parse(dataRow["fedfecpsi"].ToString());
                                else
                                    oDetalleFacturaEstandar.CodigoProductoSIN = null;

                                oDetalleFacturaEstandar.CodigoProducto = int.Parse(dataRow["fedfecpro"].ToString());
                                oDetalleFacturaEstandar.Descripcion = dataRow["fedfedesc"].ToString();
                                //oDetalleFacturaEstandar.Cantidad = Int64.Parse(dataRow["fedfecant"].ToString());
                                //oDetalleFacturaEstandar.PrecioUnitario = decimal.Parse(dataRow["fedfepuni"].ToString());
                                oDetalleFacturaEstandar.MontoDescuentoDetalle = decimal.Parse(dataRow["fedfemdes"].ToString());
                                oDetalleFacturaEstandar.SubTotal = decimal.Parse(dataRow["fedfestot"].ToString());
                                //oDetalleFacturaEstandar.NumeroSerie = dataRow["fedfenser"].ToString();
                                oDetalleFacturaEstandar.UnidadMedida = dataRow["fedfeumed"].ToString();
                                //NUEVO                                                             
                                oDetalleFacturaEstandar.Especialidad = dataRow["fedfeespe"].ToString();
                                oDetalleFacturaEstandar.EspecialidadDetalle = dataRow["fedfeedet"].ToString();
                                oDetalleFacturaEstandar.NroQuirofanoSalaOperaciones = int.Parse(dataRow["fedfenqso"].ToString());
                                oDetalleFacturaEstandar.EspecialidadMedico = dataRow["fedfeemed"].ToString();
                                oDetalleFacturaEstandar.NombreApellidoMedico = dataRow["fedfename"].ToString();
                                oDetalleFacturaEstandar.NitDocumentoMedico = dataRow["fedfenifm"].ToString();
                                oDetalleFacturaEstandar.NroMatriculaMedico = dataRow["fedfenofm"].ToString();
                                oDetalleFacturaEstandar.NroFacturaMedico = int.Parse(dataRow["fedfefmed"].ToString());

                                requestFactura.Factura.ListaDetalle.Add(oDetalleFacturaEstandar);
                            }

                            requestFactura.IdDocFiscalERP = drCabezeraFactura["fehfeiddf"].ToString();
                            requestFactura.Cufd = null;

                            if ((drCabezeraFactura["fehfecont"].ToString()) == "0")
                                requestFactura.Contingencia = false;
                            else
                                requestFactura.Contingencia = true;

                            if ((drCabezeraFactura["fehfelote"].ToString()) == "0")
                                requestFactura.EsLote = false;
                            else
                                requestFactura.EsLote = true;

                            requestFactura.IdLoteERP = drCabezeraFactura["fehfeidlo"].ToString();

                            if ((drCabezeraFactura["fehfeufac"].ToString()) == "0")
                                requestFactura.UltFacturaLote = false;
                            else
                                requestFactura.UltFacturaLote = true;

                            requestFactura.CodigoTipoFactura = int.Parse(drCabezeraFactura["fehfectip"].ToString());

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción ArmarFacturaElectronicaJson -> TIPO_FACTURA.FACTURAS_DE_HOSPITALES_CLINICAS \r\n" +
                                ex.Message + "\r\n");
                        }
                        #endregion
                        break;

                    case TipoFactura.notaExportacion:   //12
                        #region FACTURA_COMERCIAL_DE_EXPORTACION
                        try
                        {
                            #region FACTURA_COMERCIAL_DE_EXPORTACION

                            requestFactura = new RequestFactura()
                            {
                                Factura = new NotaExportacion()
                                {
                                    Cabecera = new CabeceraNotaExportacion()
                                    {
                                        //Agregar los nuevo campos...n
                                        NumeroFactura = long.Parse(drCabezeraFactura["fehfenfac"].ToString()),
                                        Direccion = drCabezeraFactura["fehfedire"].ToString(),
                                        FechaEmision = drCabezeraFactura["fehfefemi"].ToString(),
                                        CodigoTipoDocumentoIdentidad = drCabezeraFactura["fehfectdi"].ToString(),
                                        CUF = string.Empty,
                                        NumeroDocumento = drCabezeraFactura["fehfendoc"].ToString(),
                                        Complemento = string.Empty,
                                        CodigoSucursal = int.Parse(drCabezeraFactura["fehfecsuc"].ToString()),
                                        CodigoPuntoVenta = Int32.Parse(drCabezeraFactura["fehfecpve"].ToString()),
                                        NombreRazonSocial = drCabezeraFactura["fehfersoc"].ToString(),
                                        MontoTotal = decimal.Parse(drCabezeraFactura["fehfemtot"].ToString()),
                                        MontoDescuento = decimal.Parse(drCabezeraFactura["fehfemdes"].ToString()),
                                        CodigoCliente = drCabezeraFactura["fehfeccli"].ToString(),
                                        CodigoDocumentoSector = string.IsNullOrEmpty(drCabezeraFactura["fehfecdse"].ToString()) ? int.Parse(drCabezeraFactura["fehfecdse"].ToString()) : 1,
                                        NITEmisor = 305080026,
                                        CodigoMetodoPago = int.Parse(drCabezeraFactura["fehfecmpa"].ToString()),
                                        //NumeroTarjeta = !string.IsNullOrEmpty(dtCabezeraFactura["fehfentar"].ToString()) ? Int64.Parse(dtCabezeraFactura["fehfentar"].ToString()) : 0,
                                        Leyenda = drCabezeraFactura["fehfeleye"].ToString(),
                                        Usuario = "FTL",
                                        CodigoMoneda = 1,
                                        MontoTotalMoneda = decimal.Parse(drCabezeraFactura["fehfemtmo"].ToString()),
                                        TipoCambio = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString()),
                                        //IngresoDiferenciaCambio = decimal.Parse(dtCabezeraFactura["fehfeidca"].ToString()),
                                        //NUEVOS CAMBIOS
                                        CodigoPais = int.Parse(drCabezeraFactura["fehfecpai"].ToString()),            //cambiar por campo correcto
                                        MontoTotalPuerto = decimal.Parse(drCabezeraFactura["fehfemtpu"].ToString()),  //cambiar por campo correcto
                                        OtrosMontos = decimal.Parse(drCabezeraFactura["fehfeomon"].ToString()),  //cambiar por campo correcto
                                        IncoTerm = drCabezeraFactura["fehfeinco"].ToString(),  //cambiar por campo correcto
                                        PuertoDestino = drCabezeraFactura["fehfepdes"].ToString(),  //cambiar por campo correcto
                                        LugarDestino = drCabezeraFactura["fehfeldes"].ToString(),  //cambiar por campo correcto
                                        PrecioValorBruto = decimal.Parse(drCabezeraFactura["fehfepvbr"].ToString()),  //cambiar por campo correcto
                                        GastosTransporteFrontera = decimal.Parse(drCabezeraFactura["fehfegtfr"].ToString()),  //cambiar por campo correcto
                                        GastosSeguroFrontera = decimal.Parse(drCabezeraFactura["fehfesfro"].ToString()),  //cambiar por campo correcto
                                        TotalFobFrontera = decimal.Parse(drCabezeraFactura["fehfetffr"].ToString()),  //cambiar por campo correcto
                                        MontoTransporteFrontera = decimal.Parse(drCabezeraFactura["fehfemtfr"].ToString()),  //cambiar por campo correcto
                                        MontoSeguroInternacional = decimal.Parse(drCabezeraFactura["fehfemsin"].ToString()),  //cambiar por campo correcto
                                    },
                                    //ListaDetalle = new List<Detalle>()
                                },
                                IdDocFiscalERP = drCabezeraFactura["fehfeiddf"].ToString(),
                                Cufd = null,
                                Contingencia = !(drCabezeraFactura["fehfecont"].ToString() == "0") ? true : false,
                                EsLote = !(drCabezeraFactura["fehfelote"].ToString() == "0") ? true : false,
                                IdLoteERP = drCabezeraFactura["fehfeidlo"].ToString(),
                                UltFacturaLote = !(drCabezeraFactura["fehfeufac"].ToString() == "0") ? true : false,
                                CodigoTipoFactura = 1
                            };

                            foreach (DataRow dataRow in dtDetalleFactura.Rows)
                            {
                                requestFactura.Factura.ListaDetalle.Add(new DetalleNotaExportacion()
                                {
                                    ActividadEconomica = int.Parse(dataRow["fedfeaeco"].ToString()),
                                    CodigoProductoSIN = !string.IsNullOrEmpty(dataRow["fedfecpsi"].ToString()) ? int.Parse(dataRow["fedfecpsi"].ToString()) : 0,
                                    CodigoProducto = int.Parse(dataRow["fedfecpro"].ToString()),
                                    Descripcion = dataRow["fedfedesc"].ToString(),
                                    //Cantidad = Int64.Parse(dataRow["fedfecant"].ToString()),
                                    //PrecioUnitario = decimal.Parse(dataRow["fedfepuni"].ToString()),
                                    MontoDescuentoDetalle = decimal.Parse(dataRow["fedfemdes"].ToString()),
                                    SubTotal = decimal.Parse(dataRow["fedfestot"].ToString()),
                                    //NumeroSerie = dataRow["fedfenser"].ToString(),
                                    UnidadMedida = dataRow["fedfeumed"].ToString(),
                                    //NUEVO
                                    CodigoNandina = int.Parse(dataRow["fedfecnan"].ToString())
                                });
                            }

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción ArmarFacturaElectronicaJson -> TIPO_FACTURA.FACTURA_COMERCIAL_DE_EXPORTACION \r\n" +
                                ex.Message + "\r\n");
                        }
                        #endregion
                        break;

                    case TipoFactura.notaLibreConsignacion:   //13
                        #region FACTURA_COMERCIAL_DE_EXPORTACION_EN_LIBRE_CONSIGNACION :
                        try
                        {
                            #region FACTURA_COMERCIAL_DE_EXPORTACION_EN_LIBRE_CONSIGNACION :

                            requestFactura = new RequestFactura()
                            {
                                Factura = new NotaLibreConsignacion()
                                {
                                    Cabecera = new CabeceraNotaLibreConsignacion()
                                    {
                                        //Agregar los nuevo campos...n
                                        NumeroFactura = long.Parse(drCabezeraFactura["fehfenfac"].ToString()),
                                        Direccion = drCabezeraFactura["fehfedire"].ToString(),
                                        FechaEmision = drCabezeraFactura["fehfefemi"].ToString(),
                                        CodigoTipoDocumentoIdentidad = drCabezeraFactura["fehfectdi"].ToString(),
                                        CUF = string.Empty,
                                        NumeroDocumento = drCabezeraFactura["fehfendoc"].ToString(),
                                        Complemento = string.Empty,
                                        CodigoSucursal = int.Parse(drCabezeraFactura["fehfecsuc"].ToString()),
                                        CodigoPuntoVenta = Int32.Parse(drCabezeraFactura["fehfecpve"].ToString()),
                                        NombreRazonSocial = drCabezeraFactura["fehfersoc"].ToString(),
                                        MontoTotal = decimal.Parse(drCabezeraFactura["fehfemtot"].ToString()),
                                        MontoDescuento = decimal.Parse(drCabezeraFactura["fehfemdes"].ToString()),
                                        CodigoCliente = drCabezeraFactura["fehfeccli"].ToString(),
                                        CodigoDocumentoSector = string.IsNullOrEmpty(drCabezeraFactura["fehfecdse"].ToString()) ? int.Parse(drCabezeraFactura["fehfecdse"].ToString()) : 1,
                                        NITEmisor = 305080026,
                                        CodigoMetodoPago = int.Parse(drCabezeraFactura["fehfecmpa"].ToString()),
                                        NumeroTarjeta = 1234567,
                                        Leyenda = drCabezeraFactura["fehfeleye"].ToString(),
                                        Usuario = "FTL",
                                        CodigoMoneda = 1,
                                        MontoTotalMoneda = decimal.Parse(drCabezeraFactura["fehfemtmo"].ToString()),
                                        TipoCambio = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString()),
                                        //IngresoDiferenciaCambio = decimal.Parse(dtCabezeraFactura["fehfeidca"].ToString()),
                                        //NUEVOS CAMBIOS
                                        CodigoPais = int.Parse(drCabezeraFactura["fehfecpai"].ToString()),            //cambiar por campo correcto
                                        MontoTotalPuerto = decimal.Parse(drCabezeraFactura["fehfemtpu"].ToString()),  //cambiar por campo correcto                                 
                                        PuertoDestino = drCabezeraFactura["fehfepdes"].ToString(),  //cambiar por campo correcto
                                        LugarDestino = drCabezeraFactura["fehfeldes"].ToString(),  //cambiar por campo correcto
                                        Remitente = drCabezeraFactura["fehfeldes"].ToString(),  //cambiar por campo correcto
                                        Consignatario = drCabezeraFactura["fehfecons"].ToString(),  //cambiar por campo correcto
                                        LugarAcopioPuerto = drCabezeraFactura["fehfelapu"].ToString(),  //cambiar por campo correcto

                                    },
                                    //ListaDetalle = new List<Detalle>()
                                },
                                IdDocFiscalERP = drCabezeraFactura["fehfeiddf"].ToString(),
                                Cufd = null,
                                Contingencia = !(drCabezeraFactura["fehfecont"].ToString() == "0") ? true : false,
                                EsLote = !(drCabezeraFactura["fehfelote"].ToString() == "0") ? true : false,
                                IdLoteERP = drCabezeraFactura["fehfeidlo"].ToString(),
                                UltFacturaLote = !(drCabezeraFactura["fehfeufac"].ToString() == "0") ? true : false,
                                CodigoTipoFactura = 1
                            };

                            foreach (DataRow dataRow in dtDetalleFactura.Rows)
                            {
                                requestFactura.Factura.ListaDetalle.Add(new Detalle()
                                {
                                    ActividadEconomica = int.Parse(dataRow["fedfeaeco"].ToString()),
                                    CodigoProductoSIN = !string.IsNullOrEmpty(dataRow["fedfecpsi"].ToString()) ? int.Parse(dataRow["fedfecpsi"].ToString()) : 0,
                                    CodigoProducto = int.Parse(dataRow["fedfecpro"].ToString()),
                                    Descripcion = dataRow["fedfedesc"].ToString(),
                                    MontoDescuentoDetalle = decimal.Parse(dataRow["fedfemdes"].ToString()),
                                    SubTotal = decimal.Parse(dataRow["fedfestot"].ToString()),
                                    UnidadMedida = dataRow["fedfeumed"].ToString(),
                                });
                            }

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción ArmarFacturaElectronicaJson -> TIPO_FACTURA.FACTURA_COMERCIAL_DE_EXPORTACIÓN_EN_LIBRE_CONSIGNACION \r\n" +
                                ex.Message + "\r\n");
                        }
                        #endregion
                        break;

                    case TipoFactura.notaZonaFranca:  //14
                        #region NOTA_FISCAL_DE_ZONA_FRANCA
                        try
                        {
                            #region FACTURA NOTA FISCAL ZONA FRANCA
                            requestFactura = new RequestFactura()
                            {
                                Factura = new NotaZonaFranca()
                                {
                                    Cabecera = new CabeceraNotaZonaFranca()
                                    {
                                        NumeroFactura = long.Parse(drCabezeraFactura["fehfenfac"].ToString()),
                                        Direccion = drCabezeraFactura["fehfedire"].ToString(),
                                        FechaEmision = drCabezeraFactura["fehfefemi"].ToString(),
                                        CodigoTipoDocumentoIdentidad = drCabezeraFactura["fehfectdi"].ToString(),
                                        CUF = string.Empty,
                                        NumeroDocumento = drCabezeraFactura["fehfendoc"].ToString(),
                                        Complemento = string.Empty,
                                        CodigoSucursal = int.Parse(drCabezeraFactura["fehfecsuc"].ToString()),
                                        CodigoPuntoVenta = Int32.Parse(drCabezeraFactura["fehfecpve"].ToString()),
                                        NombreRazonSocial = drCabezeraFactura["fehfersoc"].ToString(),
                                        MontoTotal = decimal.Parse(drCabezeraFactura["fehfemtot"].ToString()),
                                        MontoDescuento = decimal.Parse(drCabezeraFactura["fehfemdes"].ToString()),
                                        CodigoCliente = drCabezeraFactura["fehfeccli"].ToString(),
                                        CodigoDocumentoSector = string.IsNullOrEmpty(drCabezeraFactura["fehfecdse"].ToString()) ? int.Parse(drCabezeraFactura["fehfecdse"].ToString()) : 1,
                                        NITEmisor = 305080026,
                                        CodigoMetodoPago = int.Parse(drCabezeraFactura["fehfecmpa"].ToString()),
                                        //NumeroTarjeta = !string.IsNullOrEmpty(dtCabezeraFactura["fehfentar"].ToString()) ? Int64.Parse(dtCabezeraFactura["fehfentar"].ToString()) : 0,
                                        Leyenda = drCabezeraFactura["fehfeleye"].ToString(),
                                        Usuario = "FTL",
                                        CodigoMoneda = 1,
                                        MontoTotalMoneda = decimal.Parse(drCabezeraFactura["fehfemtmo"].ToString()),
                                        TipoCambio = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString()),
                                        //NUEVO
                                        NumeroParteRecepcion = long.Parse(drCabezeraFactura["fehfenpre"].ToString())
                                    },
                                    //ListaDetalle = new List<Detalle>()
                                },
                                IdDocFiscalERP = drCabezeraFactura["fehfeiddf"].ToString(),
                                Cufd = null,
                                Contingencia = !(drCabezeraFactura["fehfecont"].ToString() == "0") ? true : false,
                                EsLote = !(drCabezeraFactura["fehfelote"].ToString() == "0") ? true : false,
                                IdLoteERP = drCabezeraFactura["fehfeidlo"].ToString(),
                                UltFacturaLote = !(drCabezeraFactura["fehfeufac"].ToString() == "0") ? true : false,
                                CodigoTipoFactura = 1
                            };

                            foreach (DataRow dataRow in dtDetalleFactura.Rows)
                            {
                                requestFactura.Factura.ListaDetalle.Add(new DetalleFacturaEstandar()
                                {
                                    ActividadEconomica = int.Parse(dataRow["fedfeaeco"].ToString()),
                                    CodigoProductoSIN = !string.IsNullOrEmpty(dataRow["fedfecpsi"].ToString()) ? int.Parse(dataRow["fedfecpsi"].ToString()) : 0,
                                    CodigoProducto = int.Parse(dataRow["fedfecpro"].ToString()),
                                    Descripcion = dataRow["fedfedesc"].ToString(),
                                    Cantidad = Int64.Parse(dataRow["fedfecant"].ToString()),
                                    PrecioUnitario = decimal.Parse(dataRow["fedfepuni"].ToString()),
                                    MontoDescuentoDetalle = decimal.Parse(dataRow["fedfemdes"].ToString()),
                                    SubTotal = decimal.Parse(dataRow["fedfestot"].ToString()),
                                    NumeroSerie = dataRow["fedfenser"].ToString(),
                                    UnidadMedida = dataRow["fedfeumed"].ToString()
                                });
                            }

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción ArmarFacturaElectronicaJson -> TIPO_FACTURA.NOTA_FISCAL_DE_ZONA_FRANCA \r\n" +
                                ex.Message + "\r\n");
                        }
                        #endregion
                        break;

                    case TipoFactura.notaMonedaExtranjera:  //17
                        #region NOTA_FISCAL_DE_COMPRA_Y_VENTA_DE_MONEDA_EXTRANJERA
                        try
                        {
                            #region FACTURA NOTA FISCAL DE COMPRA VENTA DE MONEDA EXTRANJERA

                            requestFactura = new RequestFactura()
                            {
                                Factura = new NotaMonedaExtranjera()
                                {
                                    Cabecera = new CabeceraNotaMonedaExtranjera()
                                    {
                                        NumeroFactura = long.Parse(drCabezeraFactura["fehfenfac"].ToString()),
                                        Direccion = drCabezeraFactura["fehfedire"].ToString(),
                                        FechaEmision = drCabezeraFactura["fehfefemi"].ToString(),
                                        CodigoTipoDocumentoIdentidad = drCabezeraFactura["fehfectdi"].ToString(),
                                        CUF = string.Empty,
                                        NumeroDocumento = drCabezeraFactura["fehfendoc"].ToString(),
                                        Complemento = string.Empty,
                                        CodigoSucursal = int.Parse(drCabezeraFactura["fehfecsuc"].ToString()),
                                        CodigoPuntoVenta = Int32.Parse(drCabezeraFactura["fehfecpve"].ToString()),
                                        NombreRazonSocial = drCabezeraFactura["fehfersoc"].ToString(),
                                        MontoTotal = decimal.Parse(drCabezeraFactura["fehfemtot"].ToString()),
                                        MontoDescuento = decimal.Parse(drCabezeraFactura["fehfemdes"].ToString()),
                                        CodigoCliente = drCabezeraFactura["fehfeccli"].ToString(),
                                        CodigoDocumentoSector = string.IsNullOrEmpty(drCabezeraFactura["fehfecdse"].ToString()) ? int.Parse(drCabezeraFactura["fehfecdse"].ToString()) : 1,
                                        NITEmisor = 305080026,
                                        CodigoMetodoPago = int.Parse(drCabezeraFactura["fehfecmpa"].ToString()),
                                        NumeroTarjeta = !string.IsNullOrEmpty(drCabezeraFactura["fehfentar"].ToString()) ? Int64.Parse(drCabezeraFactura["fehfentar"].ToString()) : 0,
                                        Leyenda = drCabezeraFactura["fehfeleye"].ToString(),
                                        Usuario = "FTL",
                                        CodigoMoneda = 1,
                                        MontoTotalMoneda = decimal.Parse(drCabezeraFactura["fehfemtmo"].ToString()),
                                        TipoCambio = decimal.Parse(drCabezeraFactura["fehfetcam"].ToString()),
                                        IngresoDiferenciaCambio = decimal.Parse(drCabezeraFactura["fehfeidca"].ToString()),
                                    },
                                    //ListaDetalle = new List<Detalle>()
                                },
                                IdDocFiscalERP = drCabezeraFactura["fehfeiddf"].ToString(),
                                Cufd = null,
                                Contingencia = !(drCabezeraFactura["fehfecont"].ToString() == "0") ? true : false,
                                EsLote = !(drCabezeraFactura["fehfelote"].ToString() == "0") ? true : false,
                                IdLoteERP = drCabezeraFactura["fehfeidlo"].ToString(),
                                UltFacturaLote = !(drCabezeraFactura["fehfeufac"].ToString() == "0") ? true : false,
                                CodigoTipoFactura = 1
                            };

                            foreach (DataRow dataRow in dtDetalleFactura.Rows)
                            {
                                requestFactura.Factura.ListaDetalle.Add(new Detalle()
                                {
                                    ActividadEconomica = int.Parse(dataRow["fedfeaeco"].ToString()),
                                    CodigoProductoSIN = !string.IsNullOrEmpty(dataRow["fedfecpsi"].ToString()) ? int.Parse(dataRow["fedfecpsi"].ToString()) : 0,
                                    CodigoProducto = int.Parse(dataRow["fedfecpro"].ToString()),
                                    Descripcion = dataRow["fedfedesc"].ToString(),
                                    //Cantidad = Int64.Parse(dataRow["fedfecant"].ToString()),
                                    //PrecioUnitario = decimal.Parse(dataRow["fedfepuni"].ToString()),
                                    MontoDescuentoDetalle = decimal.Parse(dataRow["fedfemdes"].ToString()),
                                    SubTotal = decimal.Parse(dataRow["fedfestot"].ToString()),
                                    //NumeroSerie = dataRow["fedfenser"].ToString(),
                                    UnidadMedida = dataRow["fedfeumed"].ToString()
                                });
                            }

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción ArmarFacturaElectronicaJson -> TIPO_FACTURA.NOTA_FISCAL_DE_COMPRA_Y_VENTA_DE_MONEDA_EXTRANJERA \r\n" +
                                ex.Message + "\r\n");
                        }
                        #endregion
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción: ArmarFacturaElectronicaJson(DataTable dtCabezeraFactura, DataTable dtDetalleFactura) \r\n" +
                    ex.Message);
            }

            return requestFactura.ToString();
        }

        /// <summary>
        /// ::Método para cancelar/anular un Documento Fiscal
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public void EjecutarProcesoFacturasXAnular()
        {
            RequestAnularDocumentoFiscal requestAnularDocFiscal = new RequestAnularDocumentoFiscal();
            Respuesta respuestaAnularDocumentoFiscal = new Respuesta();
            DataTable dtAnularDocumentoFiscal = new DataTable();

            try
            {
                dtAnularDocumentoFiscal = MetodoFactura.AnularDocumentoFiscal();

                if (dtAnularDocumentoFiscal.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dtAnularDocumentoFiscal.Rows)
                    {
                        try
                        {
                            string urlEndPoint = ConfigurationManager.AppSettings["urlEndPointAnular"].ToString();
                            long idDocFiscalERP = long.Parse(dataRow["fehfeiddf"].ToString());
                            string anularFacturaJson = ArmarAnularFacturaElectronicaJson(dataRow);
                            ResponseFEEL respuestaAnularFactura = ServicioRestFulAnular(anularFacturaJson, urlEndPoint);

                            string codRespuesta = respuestaAnularFactura.respuesta.codRespuesta;
                            string txtRespuesta = respuestaAnularFactura.respuesta.txtRespuesta;
                            bool bandera = MetodoFactura.ActualizarRespuestaAnularDocumentoFiscal(codRespuesta, txtRespuesta, idDocFiscalERP);

                            if (!bandera)
                            {
                                string file = Log.Instancia.GeneraNombreLog();
                                file += idDocFiscalERP;
                                Log.Instancia.LogWS_Mensaje_FSX(file, "El método ActualizarRespuestaAnularDocumentoFiscal(...). No se pudo Actualizar. \r\n");
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string ArmarAnularFacturaElectronicaJson(DataRow drAnularDocumentoFiscal)
        {
            string facturaJson = string.Empty;
            RequestAnularDocumentoFiscal requestAnularDocumentoFiscal = null;

            try
            {
                requestAnularDocumentoFiscal = new RequestAnularDocumentoFiscal()
                {
                    Cuf = drAnularDocumentoFiscal["fehfeccuf"].ToString(),
                    IdDocFiscalERP = drAnularDocumentoFiscal["fehfeiddf"].ToString(),
                    NitEmisor = long.Parse(drAnularDocumentoFiscal["fehfenemi"].ToString()),
                    NumeroFactura = drAnularDocumentoFiscal["fehfenfac"].ToString()
                    //requestAnularDocFiscal.TokenCliente = dataRow[""].ToString();
                };

                return requestAnularDocumentoFiscal.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region Publicar Nota Fiscal
        public void EjecutarProcesoPublicarNotaFiscal()
        {

            //Respuesta respuestaAnularDocumentoFiscal = new Respuesta();
            RequestPublicarNF requestPublicarNotaFiscal = new RequestPublicarNF();
            DataTable dtPublicarNota = new DataTable();

            try
            {
                dtPublicarNota = MetodoPublicacionNF.ObtenerPublicacionNF();
                //string respuesta = string.Empty;
                if (dtPublicarNota.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dtPublicarNota.Rows)
                    {
                        try
                        {
                            string urlEndPoint = ConfigurationManager.AppSettings["urlEndPointPublicarNota"].ToString();
                            string publicarNota = ArmarPublicarNotaFiscalJson(dataRow);
                            ResponsePublicarNF responsePublicarNF = ServicioRestFulPublicar(publicarNota, urlEndPoint);

                            //  ActualizarNF oActualizarNF = new ActualizarNF();
                            int codigoEstadoPNF = int.Parse(dataRow["fehfepnfe"].ToString());
                            long idDocumentoFiscal = long.Parse(dataRow["fehfeiddf"].ToString());
                            string codRespuesta = responsePublicarNF.respuesta.codRespuesta;
                            string textoRespuesta = responsePublicarNF.respuesta.txtRespuesta;

                            if (codRespuesta == "0")
                            {
                                codigoEstadoPNF = 2; //el codigo 2 es enviada OK
                                MetodoPublicacionNF.ActualizarDatosFactura(responsePublicarNF, codigoEstadoPNF, idDocumentoFiscal);
                            }
                            else
                            {
                                codigoEstadoPNF = 3; //el codigo 3 es enviada con Error
                                MetodoPublicacionNF.ActualizarDatosFactura(responsePublicarNF, codigoEstadoPNF, idDocumentoFiscal);
                            }

                        }
                        catch (Exception ex)
                        {
                            throw;
                        }



                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private string ArmarPublicarNotaFiscalJson(DataRow drPublicarNota)
        {
            string facturaJson = string.Empty;
            RequestPublicarNF requestPublicarNotaFiscal = null;

            try
            {

                {

                    String directorioPDFs = (ConfigurationManager.AppSettings["DirectorioPDF"].ToString() + "Publicarnotafiscal.pdf");
                    byte[] pdfBytes = File.ReadAllBytes(directorioPDFs);
                    string pdfBase64 = Convert.ToBase64String(pdfBytes);

                    requestPublicarNotaFiscal = new RequestPublicarNF()
                    {
                        NitEmisor = long.Parse(drPublicarNota["fehfenemi"].ToString()),
                        Cuf = drPublicarNota["fehfeccuf"].ToString(),
                        Archivo = pdfBase64

                    };


                }

                return requestPublicarNotaFiscal.ToString();


            }
            catch (Exception)
            {
                throw;
            }

        }

        private ResponsePublicarNF ServicioRestFulPublicar(string request, string urlEndPoint)
        {
            try
            {
                var restClient = new RestClient(urlEndPoint);
                RestRequest restRequest = new RestRequest(Method.POST);
                restRequest.AddParameter("application/json", request, ParameterType.RequestBody);
                var resp = restClient.Execute(restRequest);

                return JsonConvert.DeserializeObject<ResponsePublicarNF>(resp.Content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        protected override void OnStop()
        {
        }

    }
}
