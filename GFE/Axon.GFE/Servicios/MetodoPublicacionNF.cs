using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Axon.GFE.Mapeadores;

namespace Axon.GFE.Servicios
{
    public class MetodoPublicacionNF
    {
        
        public static DataTable ObtenerPublicacionNF()
        {
            DBAxon db = new DBAxon();
            DataTable dtPublicacionPendienteEnvio = new DataTable();

            try
            {
                db.OpenFactoryConnection();

                string query = "SELECT * FROM fehfe " +

                         "WHERE fehfepnfe = 1 ";

                dtPublicacionPendienteEnvio = db.DataAdapter(CommandType.Text, query);

                return dtPublicacionPendienteEnvio;
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

        public static void ActualizarDatosFactura(ResponsePublicarNF responseNF, Int32 nuevoEstado, long idDocumentoFiscalERP)
        {
            DBAxon db = new DBAxon();
            db.OpenFactoryConnection();

            try
            {
                db.BeginTransaction();
                db.SetLockModeToWait();
                db.SetDateFormat();

                if (responseNF != null)
                {
                    #region RESPUESTA
                    int responsequery = 0;

                    string codRespuesta = responseNF.respuesta.codRespuesta;

                    string txtRespuesta = responseNF.respuesta.txtRespuesta;
                    #endregion

                    DBAxon.Parameters[] prs = new DBAxon.Parameters[4]
               {
                   new DBAxon.Parameters("nuevoEstado", nuevoEstado, ParameterDirection.Input, DbType.Int32),
                   new DBAxon.Parameters("codRespuesta", codRespuesta, ParameterDirection.Input, DbType.String , 10),
                    new DBAxon.Parameters("txtRespuesta", txtRespuesta, ParameterDirection.Input, DbType.String , 200),
                    new DBAxon.Parameters("idDocumentoFiscalERP", idDocumentoFiscalERP, ParameterDirection.Input, DbType.Int32)

               };
                    string queryUpdate = "UPDATE fehfe SET fehfepnfe=?, fehfepnfc=? , fehfepnft=? " +
                        "WHERE fehfeiddf=?";

                    responsequery = db.ExecuteNonQuery(CommandType.Text, queryUpdate, prs);
                    if (responsequery > 0)
                    {
                        db.CommitTransaction();
                    }
                }

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



    }
   
}
