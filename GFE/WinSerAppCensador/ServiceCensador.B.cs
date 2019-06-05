using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Axon.GFE;
using Axon.GFE.Mapeadores;
using RestSharp;
using System.Configuration;
using Newtonsoft.Json;
using System.Reflection;
using System.Data.Common;

namespace Axon.WinSerAppCensador
{
    public partial class ServiceCensador
    {
        public void EjecutarProcesoConsultaDocumentoFiscal() {
            DataTable dt = null;
            try
            {
                dt = ObtenerFacturasAConsultar();
            }
            catch (Exception ex)
            {
                Log.Instancia.LogWS_Mensaje_FSX(Log.Instancia.GeneraNombreLog(), "EjecutarProcesoConsultaDocumentoFiscal()|" + ex.Message);
            }
            if (dt != null && dt.Rows.Count > 0) {
                foreach (DataRow dr in dt.Rows)
                {
                    RequestConsultaDocumentoFiscal req = new RequestConsultaDocumentoFiscal()
                    {
                        NITEmisor = long.Parse(dr["fehfenemi"].ToString()),
                        CUF = dr["fehfeccuf"].ToString()
                    };

                    ResponseConsultaDocumentoFiscal res = null;
                    try
                    {
                        CambiarEstadoConsultaFactura(Convert.ToInt32(dr["fehfeiddf"].ToString()), EstadoDocumentoFiscal.E500_PendienteDeConsulta);
#warning Quitar para producción
//#if DEBUG
                        res = ConsultarDocumentoFiscalAFEEL(req);
//#else
//                        res = new ResponseConsultaDocumentoFiscal(@"{""respuesta"": {""codRespuesta"": ""0"",""txtRespuesta"": ""Exito""},""estado"": ""D1"",""facturaEstandar"": {""cabecera"": {""numeroTarjeta"": 34567,""numeroFactura"": 200,""direccion"": ""Gualberto villarroel 123"",""fechaEmision"": ""20190101180000"",""codigoTipoDocumentoIdentidad"": ""1"",""cuf"": ""357042F1C7B1E5730E7371B6EDAB9F7B4CB54D06"",""numeroDocumento"": ""5642111"",""complemento"": """",""codigoSucursal"": 0,""codigoPuntoVenta"": 0,""nombreRazonSocial"": ""Juan Perez"",""codigoMoneda"": 1,""montoTotal"": 50.5,""codigoCliente"": ""55421"",""montoTotalMoneda"": 50.5,""tipoCambio"": 6.97,""codigoDocumentoSector"": 1,""nitEmisor"": 305080026,""codigoMetodoPago"": 1,""montoDescuento"": 1.0,""leyenda"": ""Ley N° 453: Los medios de comunicación deben promover el respeto de los derechos de los usuarios y consumidores."",""usuario"": ""FTL""},""detalle"": [{""actividadEconomica"": 123123,""codigoProductoSin"": 123,""codigoProducto"": 123,""descripcion"": ""Fanta 1 litro"",""subTotal"": 10.0,""montoDescuento"": null,""unidadMedida"": ""botella""},{""actividadEconomica"": 123123,""codigoProductoSin"": 124,""codigoProducto"": 124,""descripcion"": ""lenteja vegana"",""subTotal"": 10.0,""montoDescuento"": null,""unidadMedida"": ""lenteja""}]}}");
//                        //res = new ResponseConsultaDocumentoFiscal(@"{""respuesta"": {""codRespuesta"": ""0"",""txtRespuesta"": ""Exito""},""estado"": ""D1"",""facturaEstandar"": {""cabecera"": {""numeroTarjeta"": 1467,""numeroFactura"": 200,""direccion"": ""Gualberto villarroel 123"",""fechaEmision"": ""20190101180000"",""codigoTipoDocumentoIdentidad"": ""1"",""cuf"": ""357042F1C7B1E5730E7371B6EDAB9F7B4CB54D06"",""numeroDocumento"": ""5642111"",""complemento"": """",""codigoSucursal"": 0,""codigoPuntoVenta"": 0,""nombreRazonSocial"": ""Juan Perez"",""codigoMoneda"": 1,""montoTotal"": 50.5,""codigoCliente"": ""55421"",""montoTotalMoneda"": 50.5,""tipoCambio"": 6.97,""codigoDocumentoSector"": 1,""nitEmisor"": 305080026,""codigoMetodoPago"": 1,""montoDescuento"": 1.0,""leyenda"": ""Ley N° 453: Los medios de comunicación deben promover el respeto de los derechos de los usuarios y consumidores."",""usuario"": ""FTL""},""detalle"": [{""actividadEconomica"": 123123,""codigoProductoSin"": 123,""codigoProducto"": 123,""descripcion"": ""Fanta 1 litro"",""subTotal"": 10.0,""montoDescuento"": null,""unidadMedida"": ""botella""},{""actividadEconomica"": 123123,""codigoProductoSin"": 124,""codigoProducto"": 124,""descripcion"": ""lenteja vegana"",""subTotal"": 10.0,""montoDescuento"": null,""unidadMedida"": ""lenteja""},{""actividadEconomica"": 123123,""codigoProductoSin"": 124,""codigoProducto"": 124,""descripcion"": ""beyond meat"",""subTotal"": 10.0,""montoDescuento"": null,""unidadMedida"": ""microgramo""}]}}");
//#endif
                        if (res.Respuesta.CodRespuesta == "0")
                            ActualizarDocumentoFiscal(res, dr);
                        else
                        {
                            CambiarEstadoConsultaFactura(Convert.ToInt32(dr["fehfeiddf"].ToString()), EstadoDocumentoFiscal.E502_RespuestaConsultaError);

                            Log.Instancia.LogWS_Mensaje_FSX(Log.Instancia.GeneraNombreLog(), "EjecutarProcesoConsultaDocumentoFiscal()|Ocurrio un error en FEEL al solicitar una factura|Request:" + req.ToString() + "|Response:" + res.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Instancia.LogWS_Mensaje_FSX(Log.Instancia.GeneraNombreLog(), "EjecutarProcesoConsultaDocumentoFiscal()|" + ex.Message + "|Request:" + req.ToString() + ((res != null) ? "|Response:" + res.ToString() : ""));
                    }
                    
                }
            }
        }

        void CambiarEstadoConsultaFactura(int idFactura, int estado)
        {
            DBAxon db = new DBAxon();
            try
            {
                db.OpenFactoryConnection();
                db.SetLockModeToWait();
                string update = "UPDATE fehfe SET fehfeecdf = " + estado + " WHERE fehfeiddf = ?";

                db.ExecuteNonQuery(CommandType.Text, update, new DBAxon.Parameters[] {
                    new DBAxon.Parameters("iddf", idFactura, ParameterDirection.Input, DbType.Int32)
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Excepción: MarcarComoConsultaEnviada()|" + ex.Message);
            }
            finally
            {
                db.CloseFactoryConnection();
                db = null;
            }
        }

        DataTable ObtenerFacturasAConsultar() {
            DBAxon db = new DBAxon();
            try
            {
                db.OpenFactoryConnection();
                db.SetLockModeToWait();
                string query = "SELECT ";
                query += string.Join(", ", BD.Instancia.CamposFacturaCabeceraConsolidado);
                query += ", fehfecsta FROM fehfe WHERE fehfeecdf = " + EstadoDocumentoFiscal.E500_PendienteDeConsulta;
                return db.DataAdapter(CommandType.Text, query);
            }
            catch (Exception ex)
            {
                throw new Exception("Excepción: ObtenerFacturasAConsultar()|" + ex.Message);
            }
            finally
            {
                db.CloseFactoryConnection();
                db = null;
            }
        }

        DataTable ObtenerListaDetalle(string idFactura)
        {
            DBAxon db = new DBAxon();
            try
            {
                db.OpenFactoryConnection();
                db.SetLockModeToWait();
                string query = "SELECT * FROM fedfe WHERE fedfeiddf=" + idFactura;
                return db.DataAdapter(CommandType.Text, query);
            }
            catch (Exception ex)
            {
                throw new Exception("Excepción: ObtenerListaDetalle()|" + ex.Message);
            }
            finally
            {
                db.CloseFactoryConnection();
                db = null;
            }
        }

        ResponseConsultaDocumentoFiscal ConsultarDocumentoFiscalAFEEL(RequestConsultaDocumentoFiscal request) {
            var restClient = new RestClient(ConfigurationManager.AppSettings["urlEndPointFind"]);
            var rr = new RestRequest(Method.POST);
            rr.AddParameter("application/json", request.ToString(), ParameterType.RequestBody);
            var response = restClient.Execute(rr);
            return new ResponseConsultaDocumentoFiscal(response.Content);
        }

        void ActualizarDocumentoFiscal(ResponseConsultaDocumentoFiscal responseConsultaDocumentoFiscal, DataRow facturaLocal)
        {
            dynamic facturaFEEL = Convert.ChangeType(responseConsultaDocumentoFiscal.Factura,responseConsultaDocumentoFiscal.Factura.GetType());

            List<DBAxon.Parameters> paramsCabecera = new List<DBAxon.Parameters>();
            string updCabecera = ConstruirUpdate(facturaFEEL.Cabecera, facturaFEEL.CamposDB, facturaLocal, "fehfe", "fehfeiddf", facturaLocal["fehfeiddf"].ToString(), out paramsCabecera);

            //Estado
            string updEstado = ConstruirUpdateEstado(responseConsultaDocumentoFiscal.Estado, facturaLocal["fehfecsta"].ToString(), facturaLocal["fehfeiddf"].ToString());

            //Detalle
            List<string> updsDetalle = new List<string>();
            DataTable dt = ObtenerListaDetalle(facturaLocal["fehfeiddf"].ToString());
            List<DBAxon.Parameters>[] paramsDetalles = new List<DBAxon.Parameters>[facturaFEEL.ListaDetalle.Count];
            
            if (dt != null && facturaFEEL.ListaDetalle.Count == dt.Rows.Count) {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    paramsDetalles[i] = new List<DBAxon.Parameters>();
                    updsDetalle.Add(ConstruirUpdate(facturaFEEL.ListaDetalle[i], facturaFEEL.CamposDB, dt.Rows[i], "fedfe", "fedfeiddf", dt.Rows[i]["fedfeiddf"].ToString(), out paramsDetalles[i]));
                }
            }
            else {
                if (facturaFEEL.ListaDetalle.Count > 0) { //en caso de que haya una cantidad diferente en las listas de detalle
                    updCabecera += "DELETE FROM fedfe WHERE fedfeiddf = ?;";
                    paramsCabecera.Add(new DBAxon.Parameters("iddf", facturaLocal["fehfeiddf"].ToString(), ParameterDirection.Input, DbType.Int32));

                    for (int i = 0; i < facturaFEEL.ListaDetalle.Count; i++)
                    {
                        paramsDetalles[i] = new List<DBAxon.Parameters>();
                        updsDetalle.Add(ConstruirInsert(facturaFEEL.ListaDetalle[i], facturaFEEL.CamposDB, "fedfe", "fedfeiddf", facturaLocal["fehfeiddf"].ToString(), out paramsDetalles[i]));
                    }

                }
            }

            bool huboCambios = false;
            string sql = string.Empty;
            List<DBAxon.Parameters> paramsGral = new List<DBAxon.Parameters>();
            DBAxon db = new DBAxon();
            try
            {
                db.OpenFactoryConnection();
                db.BeginTransaction();                
                db.SetLockModeToWait();
                
                if (paramsCabecera.Count > 1) //Si hay diferencias, guardar
                {
                    paramsGral.AddRange(paramsCabecera.ToArray());
                    sql += updCabecera.ToString();
                    huboCambios = true;
                }

                for (int i = 0; i < updsDetalle.Count; i++)
                {
                    if (paramsDetalles[i].Count > 1)
                    {
                        paramsGral.AddRange(paramsDetalles[i].ToArray());
                        sql += updsDetalle[i].ToString();
                        huboCambios = true;
                    }
                }

                huboCambios = !string.IsNullOrEmpty(updEstado);

                if (huboCambios) { //agregar hora y fecha de corrección
                    sql += "UPDATE fehfe SET fehfevfco = ?, fehfevhco = ?, fehfeecdf = ? WHERE fehfeiddf = ?;";
                    paramsGral.AddRange(new DBAxon.Parameters[] {
                        new DBAxon.Parameters("vfco", DateTime.Now, ParameterDirection.Input, DbType.Date),
                        new DBAxon.Parameters("vhco", DateTime.Now.ToString("HH:mm:ss"), ParameterDirection.Input, DbType.String, 8),
                        new DBAxon.Parameters("ecdf", EstadoDocumentoFiscal.E503_Actualizada,ParameterDirection.Input, DbType.Int32),
                        new DBAxon.Parameters("iddf", Convert.ToInt32(facturaLocal["fehfeiddf"].ToString()), ParameterDirection.Input, DbType.Int32)
                    });
                    if (!string.IsNullOrEmpty(updEstado)) sql += updEstado;
                    db.PrepareCommand(true, CommandType.Text, sql, paramsGral.ToArray());
                    db.command.ExecuteNonQuery();

                    GuardarLogDeCambios(sql, paramsGral, facturaLocal,dt);
                }
                db.CommitTransaction();
            }
            catch (Exception ex)
            {
                db.RollbackTransaction();
                throw new Exception("Excepción: ActualizarDocumentoFiscales()" + ex.Message);
            }
            finally
            {
                db.CloseFactoryConnection();
                db = null;
            }
        }

        private string ConstruirUpdateEstado(EstadoFacturaFEEL estadoFEEL, string estadoLocal, string idFactura)
        {
            string upd = "UPDATE fehfe SET fehfecsta =";
            string est = string.Empty;
            switch (estadoFEEL)
            {
                case EstadoFacturaFEEL.FacturaRecibidaDeOrigen: if (estadoLocal != "D1") est = "'D1'"; break;
                case EstadoFacturaFEEL.FacturaEnviadaAlSIN: if (estadoLocal != "D2") est = "'D2'"; break;
                case EstadoFacturaFEEL.FacturarValidadaPorElSIN: if (estadoLocal != "D3") est = "'D3'"; break;
                case EstadoFacturaFEEL.FacturaNotificadaAOrigen: if (estadoLocal != "D4") est = "'D4'"; break;
                case EstadoFacturaFEEL.AnulacionRecibidaDeOrigen: if (estadoLocal != "DA1") est = "'DA1'"; break;
                case EstadoFacturaFEEL.AnulacionEnviadaAlSIN: if (estadoLocal != "DA2") est = "'DA2'"; break;
                case EstadoFacturaFEEL.AnulacionValidadaPorSIN: if (estadoLocal != "DA3") est = "'DA3'"; break;
                case EstadoFacturaFEEL.AnulacionNotificadaAOrigen: if (estadoLocal != "DA4") est = "'DA4'"; break;
            }

            return (string.IsNullOrEmpty(est)) ? est : upd + est + " WHERE fehfeiddf = " + idFactura + ";";
        }

        private void GuardarLogDeCambios(string sql, List<DBAxon.Parameters> paramsGral, DataRow facturaLocal,DataTable detalles)
        {
            int i = 0;
            sql = sql.Replace('?', '~');
            foreach (DBAxon.Parameters p in paramsGral)
            {
                 i = sql.IndexOf('~');
                sql = sql.Remove(i, 1);
                sql = sql.Insert(i, "[" + p.paramValue.ToString() + "]");
            }

            string cabecera = "CABECERA ANTERIOR:" + string.Join("|", facturaLocal.ItemArray);
            string detalle = "DETALLE ANTERIOR:";
            foreach (DataRow d in detalles.Rows)
            {
                detalle += string.Join("|", d.ItemArray) + Environment.NewLine;
            }

            try
            {
                Log.Instancia.LogWS_Mensaje_FSX("correccion" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + facturaLocal["fehfeiddf"].ToString() + ".log", cabecera + Environment.NewLine + detalle + Environment.NewLine + "CAMBIOS:" + sql);
            }
            catch { }
        }

        string ConstruirUpdate(object obj, Dictionary<string,string> camposDB, DataRow facturaLocal, string tabla, string campowhere, string valorwhere, out List<DBAxon.Parameters> paramsUPD) {
            StringBuilder sql = new StringBuilder();
            sql.Append("UPDATE "+tabla+" SET ");
            paramsUPD = new List<DBAxon.Parameters>();
            
            foreach (PropertyInfo propiedad in obj.GetType().GetProperties())
            {
                var valForiginal = propiedad.GetValue(obj);
                var valFlocal = facturaLocal[camposDB[propiedad.Name]];
                if (valForiginal == null && valFlocal == DBNull.Value) continue;

                if (propiedad.PropertyType.ToString().Contains("Int16"))
                {
                    if (valForiginal == null || valFlocal == DBNull.Value || Convert.ToInt16(valForiginal) != Convert.ToInt16(valFlocal.ToString()))
                    {
                        sql.Append(camposDB[propiedad.Name] + " = ?, ");
                        paramsUPD.Add(new DBAxon.Parameters(camposDB[propiedad.Name], valForiginal ?? DBNull.Value, ParameterDirection.Input, DbType.Int16));
                    }
                }
                else if (propiedad.PropertyType.ToString().Contains("Int32"))
                {
                    if (valForiginal == null || valFlocal == DBNull.Value || Convert.ToInt32(valForiginal) != Convert.ToInt32(facturaLocal[camposDB[propiedad.Name]].ToString()))
                    {
                        sql.Append(camposDB[propiedad.Name] + " = ?, ");
                        paramsUPD.Add(new DBAxon.Parameters(camposDB[propiedad.Name], valForiginal ?? DBNull.Value, ParameterDirection.Input, DbType.Int32));
                    }
                }
                else if (propiedad.PropertyType.ToString().Contains("Int64"))
                {
                    if (valForiginal == null || valFlocal == DBNull.Value || Convert.ToInt64(valForiginal) != Convert.ToInt64(facturaLocal[camposDB[propiedad.Name]].ToString()))
                    {
                        sql.Append(camposDB[propiedad.Name] + " = ?, ");
                        paramsUPD.Add(new DBAxon.Parameters(camposDB[propiedad.Name], valForiginal ?? DBNull.Value, ParameterDirection.Input, DbType.Int64));
                    }
                }
                else if (propiedad.PropertyType.ToString().Contains("Decimal"))
                {
                    if (valForiginal == null || valFlocal == DBNull.Value || Convert.ToDecimal(valForiginal) != Convert.ToDecimal(facturaLocal[camposDB[propiedad.Name]].ToString()))
                    {
                        sql.Append(camposDB[propiedad.Name] + " = ?, ");
                        paramsUPD.Add(new DBAxon.Parameters(camposDB[propiedad.Name], valForiginal ?? DBNull.Value, ParameterDirection.Input, DbType.Decimal));
                    }
                }
                else if (propiedad.PropertyType.ToString().Contains("String"))
                {
                    if (valForiginal == null || valFlocal == DBNull.Value || valForiginal.ToString() != facturaLocal[camposDB[propiedad.Name]].ToString())
                    {
                        sql.Append(camposDB[propiedad.Name] + " = ?, ");
                        paramsUPD.Add(new DBAxon.Parameters(camposDB[propiedad.Name], valForiginal ?? DBNull.Value, ParameterDirection.Input, DbType.String));
                    }
                }
            }
            sql.Remove(sql.ToString().LastIndexOf(','), 1); //quitar la ultima coma
            sql.Append(" WHERE " + campowhere + " = ?;");
            paramsUPD.Add(new DBAxon.Parameters("iddf", valorwhere, ParameterDirection.Input, DbType.Int32));
            return sql.ToString();
        }

        string ConstruirInsert(object obj, Dictionary<string, string> camposDB, string tabla, string campoFK, string valorFK, out List<DBAxon.Parameters> paramsUPD)
        {
            StringBuilder sql = new StringBuilder();
            string campos = campoFK + ", ", values = "?, ";
            sql.Append("INSERT INTO " + tabla + " ");
            paramsUPD = new List<DBAxon.Parameters>();
            paramsUPD.Add(new DBAxon.Parameters(campoFK, valorFK, ParameterDirection.Input, DbType.Int32));

            foreach (PropertyInfo propiedad in obj.GetType().GetProperties())
            {
                var valForiginal = propiedad.GetValue(obj);
                campos += camposDB[propiedad.Name];
                campos += ", ";
                values += "?, ";
                if (propiedad.PropertyType.ToString().Contains("Int16"))
                    paramsUPD.Add(new DBAxon.Parameters(camposDB[propiedad.Name], valForiginal ?? DBNull.Value, ParameterDirection.Input, DbType.Int16));
                else if (propiedad.PropertyType.ToString().Contains("Int32"))
                    paramsUPD.Add(new DBAxon.Parameters(camposDB[propiedad.Name], valForiginal ?? DBNull.Value, ParameterDirection.Input, DbType.Int32));
                else if (propiedad.PropertyType.ToString().Contains("Int64"))
                    paramsUPD.Add(new DBAxon.Parameters(camposDB[propiedad.Name], valForiginal ?? DBNull.Value, ParameterDirection.Input, DbType.Int64));
                else if (propiedad.PropertyType.ToString().Contains("Decimal"))
                    paramsUPD.Add(new DBAxon.Parameters(camposDB[propiedad.Name], valForiginal ?? DBNull.Value, ParameterDirection.Input, DbType.Decimal));
                else if (propiedad.PropertyType.ToString().Contains("String"))
                    paramsUPD.Add(new DBAxon.Parameters(camposDB[propiedad.Name], valForiginal ?? DBNull.Value, ParameterDirection.Input, DbType.String));                
            }
            sql.Append("(" + campos.Remove(campos.LastIndexOf(','), 1) + ") VALUES(" + values.Remove(values.LastIndexOf(','), 1) + ");");
            return sql.ToString();
        }
    }
}
