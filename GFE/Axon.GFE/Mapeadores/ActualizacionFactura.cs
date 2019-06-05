using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    public class ActualizacionFactura
    {
        public bool ActualizarDatosFactura(ResponseFactura respuestaFacturaEstandar)
        {
            DBAxon db = new DBAxon();
#if DEBUG
            System.Diagnostics.Stopwatch cronoDB = new System.Diagnostics.Stopwatch();
            cronoDB.Start();
#endif
            db.OpenFactoryConnection();
            int updateCabecera = 0;
            int updateItem = 0;

            try
            {
                db.BeginTransaction();
                db.SetLockModeToWait();
                db.SetDateFormat();

                if (respuestaFacturaEstandar != null && respuestaFacturaEstandar.Proceso != null && respuestaFacturaEstandar.Factura != null)
                {
                    #region RESPUESTA
                    string codRespuesta = respuestaFacturaEstandar.Respuesta.CodRespuesta;
                    string txtRespuesta = respuestaFacturaEstandar.Respuesta.TxtRespuesta;
                    #endregion

                    #region PROCESO
                    string idDocFiscalERP = respuestaFacturaEstandar.Proceso.IdDocFiscalERP;
                    string idDocFiscalFEEL = respuestaFacturaEstandar.Proceso.IdDocFiscalFEEL;
                    string cufd = respuestaFacturaEstandar.Proceso.CUFD;
                    string codEstado = respuestaFacturaEstandar.Proceso.CodEstado;
                    int codigoTipoFactura = respuestaFacturaEstandar.Proceso.CodigoTipoFactura;
                    #endregion

                    #region FACTURA ESTANDAR

                    //CABECERA
                    string fechaEmision = respuestaFacturaEstandar.Factura.Cabecera.FechaEmision;
                    string codigoTipoDocumentoIdentidad = respuestaFacturaEstandar.Factura.Cabecera.CodigoTipoDocumentoIdentidad;
                    string cuf = respuestaFacturaEstandar.Factura.Cabecera.CUF;
                    int codigoSucursal = respuestaFacturaEstandar.Factura.Cabecera.CodigoSucursal;
                    int codigoPuntoVenta = respuestaFacturaEstandar.Factura.Cabecera.CodigoPuntoVenta;
                    int codigoDocumentoSector = respuestaFacturaEstandar.Factura.Cabecera.CodigoDocumentoSector;
                    int codigoMetodoPago = respuestaFacturaEstandar.Factura.Cabecera.CodigoMetodoPago;
                    string leyenda = respuestaFacturaEstandar.Factura.Cabecera.Leyenda;
                    int codigoMoneda = respuestaFacturaEstandar.Factura.Cabecera.CodigoMoneda;

                    DBAxon.Parameters[] prs = new DBAxon.Parameters[16]
                {
                    new DBAxon.Parameters("codRespuesta", codRespuesta, ParameterDirection.Input, DbType.String , 10),
                    new DBAxon.Parameters("txtRespuesta", txtRespuesta, ParameterDirection.Input, DbType.String , 200),

                    new DBAxon.Parameters("idDocFiscalFEEL", idDocFiscalFEEL, ParameterDirection.Input,DbType.String , 20),
                    new DBAxon.Parameters("cufd", cufd, ParameterDirection.Input, DbType.String, 100),
                    new DBAxon.Parameters("codEstado", codEstado, ParameterDirection.Input,DbType.String , 10),
                    new DBAxon.Parameters("codigoTipoFactura", codigoTipoFactura, ParameterDirection.Input,DbType.Int32),

                    new DBAxon.Parameters("fechaEmision", fechaEmision, ParameterDirection.Input,DbType.String , 20),
                    new DBAxon.Parameters("codigoTipoDocumentoIdentidad", codigoTipoDocumentoIdentidad, ParameterDirection.Input, DbType.Int32),
                    new DBAxon.Parameters("cuf", cuf, ParameterDirection.Input,DbType.String , 50),
                    new DBAxon.Parameters("codigoSucursal", codigoSucursal, ParameterDirection.Input,DbType.Int32),
                    new DBAxon.Parameters("codigoPuntoVenta", codigoPuntoVenta, ParameterDirection.Input,DbType.Int32),
                    new DBAxon.Parameters("codigoDocumentoSector", codigoDocumentoSector, ParameterDirection.Input,DbType.Int32),
                    new DBAxon.Parameters("codigoMetodoPago", codigoMetodoPago, ParameterDirection.Input,DbType.Int16),
                    new DBAxon.Parameters("leyenda", leyenda, ParameterDirection.Input,DbType.String , 200),
                    new DBAxon.Parameters("codigoMoneda", codigoMoneda, ParameterDirection.Input,DbType.Int32),
                    new DBAxon.Parameters("idDocFiscalERP", idDocFiscalERP, ParameterDirection.Input,DbType.Int32)
                };

                    string queryUpdate = "UPDATE fehfe SET fehfecres=?, fehfetres=? , fehfeifee=?, fehfecufd=?, " +
                        "fehfecsta=?, fehfectip=?,  fehfefemi=?, fehfectdi=?, fehfeccuf=?, fehfecsuc=?, " +
                        "fehfecpve=?, fehfecdse=?, fehfecmpa=?, fehfeleye=?, fehfecmon=? , fehfestat=" + int.Parse(EstadoDocumentoFiscal.E102_ProcesadaCorrectamente.ToString()) +
                        " WHERE fehfeiddf=?";

                    updateCabecera = db.ExecuteNonQuery(CommandType.Text, queryUpdate, prs);

                    #endregion

                    if (updateCabecera > 0)
                    {
                        #region DETALLE
                        dynamic factura = Convert.ChangeType(respuestaFacturaEstandar.Factura, respuestaFacturaEstandar.Factura.GetType());

                        if (factura.Tipo != TipoFactura.notaExportacion)
                        {
                            foreach (var item in factura.ListaDetalle)
                            {
                                int? actividadEconomica = item.ActividadEconomica ?? null;
                                int? codigoProductoSin = item.CodigoProductoSIN ?? null;

                                DBAxon.Parameters[] prs2 = new DBAxon.Parameters[2]
                                    {
                                    new DBAxon.Parameters("actividadEconomica", actividadEconomica, ParameterDirection.Input, DbType.Int32),
                                    new DBAxon.Parameters("codigoProductoSin", codigoProductoSin, ParameterDirection.Input,DbType.Int32)
                                    };

                                updateItem = db.ExecuteNonQuery(CommandType.Text, "UPDATE fedfe SET fedfeaeco=?, fedfecpsi=? " +
                                                            "WHERE fedfeiddf =" + idDocFiscalERP, prs2);
                            }
                        }
                        else
                        {
                            foreach (var item in factura.ListaDetalle)
                            {
                                int? actividadEconomica = item.ActividadEconomica ?? null;
                                int? codigoProductoSin = item.CodigoProductoSIN ?? null;

                                DBAxon.Parameters[] prs2 = new DBAxon.Parameters[3]
                                {
                                    new DBAxon.Parameters("actividadEconomica", actividadEconomica, ParameterDirection.Input, DbType.Int32),
                                    new DBAxon.Parameters("codigoProductoSin", codigoProductoSin, ParameterDirection.Input,DbType.Int32),
                                    new DBAxon.Parameters("codigoNandina", item.CodigoNandina, ParameterDirection.Input,DbType.Int32)
                                };

                                updateItem = db.ExecuteNonQuery(CommandType.Text, "UPDATE fedfe SET fedfeaeco=?, fedfecpsi=?, fedfecnan=? " +
                                                            "WHERE fedfeiddf =" + idDocFiscalERP, prs2);
                            }
                        }

                        db.CommitTransaction();
                        #endregion

#if DEBUG
                        cronoDB.Stop();
                        Cronometro.Instancia.TotalMs += cronoDB.ElapsedMilliseconds;
#endif
                    }
                }
                else
                {
                    //string file = Log.Instancia.GeneraNombreLog();
                    //file += respuestaFacturaEstandar.Proceso.IdDocFiscalERP;
                    //Log.Instancia.LogWS_Mensaje_FSX(file, "Esta nulo o respuestaFacturaEstandar ó respuestaFacturaEstandar.Proceso ó respuestaFacturaEstandar.Factura");
                }

                return (updateCabecera + updateItem) >= 2;
            }
            catch (Exception eEx)
            {
                db.RollbackTransaction();
                throw new Exception(eEx.Message, eEx);
            }
            finally
            {
                db.CloseFactoryConnection();
                db = null;
            }
        }


        /// <summary>
        /// ::Método que actualiza la cabecera de la factura con error, dado que el servicio del FEEL respondio con un error,
        /// ya sea por validación, etc.
        /// ::Datos que se actualizan: codRespuesta, txtRespuesta y codEstado
        /// </summary>
        /// <param name="respuestaFacturaEstandar">Objeto respuesta</param>
        /// <returns>bool</returns>
        public bool RespuestaErrorActualizarDatosFacturaEstandar(string codRespuesta, string textoRespuesta, long idDocFiscalERP)
        {
            bool b;
            //string file = Log.Instancia.GeneraNombreLog();
            //file += respuestaFacturaEstandar.proceso.idDocFiscalERP;

            try
            {
                DBAxon db = new DBAxon();
                db.OpenFactoryConnection();
                int update = 0;

                if (!string.IsNullOrEmpty(codRespuesta))
                {
                    string queryUpdate = "UPDATE fehfe SET fehfecres='" + codRespuesta + "', fehfetres='" + textoRespuesta + "', " +
                                             "fehfestat=" + int.Parse(EstadoDocumentoFiscal.E103_RespuestaEnviarConError.ToString()) +
                                             " WHERE fehfeiddf=" + idDocFiscalERP;

                    update = db.ExecuteNonQuery(CommandType.Text, queryUpdate);

                    b = update > 0;
                }
                else
                {
                    b = false;
                }
            }
            catch (Exception ex)
            {
                //Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción: RespuestaErrorActualizarDatosFacturaEstandar \r\n" +
                //    ex.Message);
                b = false;
            }

            return b;
        }


        


    }
}
