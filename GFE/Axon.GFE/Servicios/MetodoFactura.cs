using Axon.GFE.Mapeadores;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Axon.GFE.Servicios
{
    #region ESTADOS FACTURACION ELECTRONICA COPIADOS AL GESTOR SQL

    public enum ESTADOS_FACTURACION_ELECTRONICA : int
    {
        PENDIENTES_DE_ENVIO = 100, //PENDIENTE DE ENVIO
        ENVIADA_AL_FEEL = 101, // ENVIADA AL FEEL
        PROCESADA_CORRECTAMENTE = 102, // PROCESADA CORRECTAMENTE
        ERROR = 103, // ERROR
        FACTURA_SIN_DETALLE = 104, // FACTURA SIN DETALLE
        PENDIENTE_DE_ANULAR = 200, //
        ENVIADO_AL_FEEL_PENDIENTE_DE_ANULAR = 201, //ENVIADO AL FEEL PENDIENTE DE ANULAR
        ANULACION_PROCESADA = 202, //ANULACION PROCESADA
        ANULACION_CON_ERROR = 204 //ANULACION CON ERROR
    };

    #endregion

    public static class MetodoFactura
    {
        public static bool ActualizarFacturaSinDetalle(long idDocFiscalERP)
        {
            DBAxon db = new DBAxon();
            bool bandera = false;
            DataTable dtFacturaPendienteEnvio = new DataTable();
            int respuestaFacturaSinDetalle = 0;

            try
            {
                db.OpenFactoryConnection();
                db.SetLockModeToWait();

                string queryFacturaSinDetalle = "UPDATE fehfe SET fehfestat=" + (int)ESTADOS_FACTURACION_ELECTRONICA.FACTURA_SIN_DETALLE +
                                        " WHERE fehfemrcb=0 AND fehfeiddf=" + idDocFiscalERP;

                respuestaFacturaSinDetalle = db.ExecuteNonQuery(CommandType.Text, queryFacturaSinDetalle);
            }
            catch (Exception ex)
            {
                throw new Exception("Excepción: ActualizarFacturaSinDetalle(long idDocFiscalERP) " + ex.Message);
            }
            finally
            {
                db.CloseFactoryConnection();
                db = null;
            }

            return respuestaFacturaSinDetalle > 0;
        }

        public static DataTable ObtenerCabeceraFacturaElectronicaPendienteEnvio()
        {
            DBAxon db = new DBAxon();
            DataTable dtFacturaPendienteEnvio = new DataTable();
            int respuestaUpdate = 0;

            try
            {
                db.OpenFactoryConnection();
                db.SetLockModeToWait();

                string query = "SELECT * FROM fehfe " +
                                    "WHERE fehfemrcb=0 AND fehfelote=0 AND fehfestat=" + (int)ESTADOS_FACTURACION_ELECTRONICA.PENDIENTES_DE_ENVIO;

                dtFacturaPendienteEnvio = db.DataAdapter(CommandType.Text, query);

                if (dtFacturaPendienteEnvio.Rows.Count > 0)
                {

                    string queryUpdate = "UPDATE fehfe SET fehfestat=" + (int)ESTADOS_FACTURACION_ELECTRONICA.ENVIADA_AL_FEEL +
                                            " WHERE fehfemrcb=0 AND fehfelote=0 AND fehfestat=" + (int)ESTADOS_FACTURACION_ELECTRONICA.PENDIENTES_DE_ENVIO;

                    respuestaUpdate = db.ExecuteNonQuery(CommandType.Text, queryUpdate);

                }
                else
                {
                    return new DataTable();
                }

                return dtFacturaPendienteEnvio;
            }
            catch (Exception ex)
            {
                throw new Exception("Excepción: ObtenerCabezeraFacturaElectronica() en el try catch mas grande." + ex.Message);
            }
            finally
            {
                db.CloseFactoryConnection();
                db = null;
            }
        }

        public static DataTable ObtenerDetalleFacturaElectronicaPendienteEnvio(Int64 idDocFiscalERP)
        {
            DBAxon db = new DBAxon();
            DataTable dtDetalleFacturaPendienteEnvio = new DataTable();

            try
            {
                db.OpenFactoryConnection();

                string query = "SELECT * FROM fedfe WHERE fedfeiddf=" + idDocFiscalERP;
                dtDetalleFacturaPendienteEnvio = db.DataAdapter(CommandType.Text, query);

                return dtDetalleFacturaPendienteEnvio;
            }
            catch (Exception ex)
            {
                throw new Exception("Excepción: ObtenerDetalleFacturaElectronica(Int64 correlativo)" + ex.Message);
            }
            finally
            {
                db.CloseFactoryConnection();
                db = null;
            }
        }

        public static DataTable AnularDocumentoFiscal()
        {
            DBAxon db = new DBAxon();
            string file = Log.Instancia.GeneraNombreLog();
            DataTable dtAnularDocumentoFiscal = new DataTable();
            int respuestaUpdate = 0;

            try
            {
                db.OpenFactoryConnection();
                db.SetLockModeToWait();

                string query = "SELECT * FROM fehfe " +
                                    "WHERE fehfestat=" + (int)ESTADOS_FACTURACION_ELECTRONICA.PENDIENTE_DE_ANULAR + " AND fehfemrcb=0";

                dtAnularDocumentoFiscal = db.DataAdapter(CommandType.Text, query);

                if (dtAnularDocumentoFiscal.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dtAnularDocumentoFiscal.Rows)
                    {
                        try
                        {
                            Int64 idDocFiscalERP = Int64.Parse(dataRow["fehfeiddf"].ToString());
                            file += idDocFiscalERP.ToString();
                            string queryUpdate = "UPDATE fehfe SET fehfestat=" + (int)ESTADOS_FACTURACION_ELECTRONICA.ENVIADO_AL_FEEL_PENDIENTE_DE_ANULAR +
                                                    " WHERE fehfemrcb=0 AND fehfeiddf=" + idDocFiscalERP;
                            respuestaUpdate = db.ExecuteNonQuery(CommandType.Text, queryUpdate);
                        }
                        catch (Exception ex)
                        {
                            Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción en: AnularDocumentoFiscal() al hacer un UPDATE fehfe" +
                                ex.Message);
                        }
                    }
                }
                else
                {
                    return new DataTable();
                }

                return dtAnularDocumentoFiscal;
            }
            catch (Exception ex)
            {
                throw new Exception("Excepción: AnularDocumentoFiscal() en el try catch mas grande." + ex.Message);
            }
            finally
            {
                db.CloseFactoryConnection();
                db = null;
            }
        }

        /// <summary>
        /// ::Metodo que actualiza los datos de la solicitud que se hace al anular un documento fiscal,
        /// ::esta respuesta se actualiza en la tabla fehfe.
        /// </summary>
        /// <returns></returns>
        public static bool ActualizarRespuestaAnularDocumentoFiscal(string codRespuesta, string txtRespuesta, long idDocFiscalERP)
        {
            DBAxon db = new DBAxon();
            string file = Log.Instancia.GeneraNombreLog() + idDocFiscalERP;
            DataTable dtActualizarRespuestaAnularDocFiscal = new DataTable();
            int respuestaUpdate = 0;

            try
            {
                db.OpenFactoryConnection();
                db.SetLockModeToWait();

                string queryUpdate = "UPDATE fehfe SET fehfestat=" + (int)ESTADOS_FACTURACION_ELECTRONICA.ANULACION_PROCESADA +
                                        ", fehfecres='" + codRespuesta + "', fehfetres='" + txtRespuesta + "'" +
                                        " WHERE fehfemrcb=0 AND fehfeiddf=" + idDocFiscalERP;

                respuestaUpdate = db.ExecuteNonQuery(CommandType.Text, queryUpdate);
            }
            catch (Exception ex)
            {
                Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción en: ActualizarRespuestaAnularDocumentoFiscal() al hacer un UPDATE fehfe" +
                        ex.Message);
            }
            finally
            {
                db.CloseFactoryConnection();
                db = null;
            }

            return respuestaUpdate > 0;

        }

        public static Factura ConsultarDocumentoFiscal()
        {
            return null;
        }


        public static bool ActualizarAlEstadoOrigen(long idDocFiscalERP)
        {
            bool b;
            DBAxon db = new DBAxon();
            db.OpenFactoryConnection();
            db.SetLockModeToWait();

            try
            {
                int update = 0;
                string queryUpdate = "UPDATE fehfe SET fehfestat=" + int.Parse(EstadoDocumentoFiscal.E100_PendienteDeEnvio.ToString()) +
                                         " WHERE fehfeiddf=" + idDocFiscalERP;

                update = db.ExecuteNonQuery(CommandType.Text, queryUpdate);

                b = update > 0;
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
