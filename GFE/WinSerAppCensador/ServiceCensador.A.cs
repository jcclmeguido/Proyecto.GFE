using Axon.GFE;
using Axon.GFE.Mapeadores;
using Axon.GFE.Servicios;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Axon.WinSerAppCensador
{
    public partial class ServiceCensador
    {
        public RequestFactura ArmarFacturaElectronicaJson2(DataRow drCabeceraFactura, DataTable dtDetalleFactura)
        {
            string facturaJson = string.Empty;
            List<Detalle> ListadoDetalleFactura = new List<Detalle>();
            RequestFactura requestFactura = new RequestFactura();
            //string file = Log.Instancia.GeneraNombreLog();
            //file += drCabeceraFactura["fehfeiddf"].ToString(); //nro. correlativo interno.

            try
            {
                Factura factura = FabricaFactura.Fabricar((TipoFactura)int.Parse(drCabeceraFactura["fehfecdse"].ToString()));
                CargarObjetoFactura(factura.Cabecera, drCabeceraFactura, factura.CamposDB);

                Detalle detalle = null;
                foreach (DataRow drDetalleFactura in dtDetalleFactura.Rows)
                {
                    detalle = FabricaFactura.FabricarDetalle(factura.Tipo);
                    CargarObjetoFactura(detalle, drDetalleFactura, factura.CamposDB);
#warning Completar este switch
                    switch (factura.Tipo)
                    {
                        case TipoFactura.facturaEstandar:
                            ((FacturaEstandar)factura).ListaDetalle.Add(detalle as DetalleFacturaEstandar);
                            break;
                        case TipoFactura.facturaColegio:
                            ((FacturaColegio)factura).ListaDetalle.Add(detalle as DetalleFacturaColegio);
                            break;
                        case TipoFactura.facturaAlquiler:
                            ((FacturaAlquiler)factura).ListaDetalle.Add(detalle as DetalleFacturaAlquiler);
                            break;
                        case TipoFactura.facturaCombustible:
                            ((FacturaCombustible)factura).ListaDetalle.Add(detalle as DetalleFacturaCombustible);
                            break;
                        case TipoFactura.facturaServicios:
                            ((FacturaServiciosBasicos)factura).ListaDetalle.Add(detalle as DetalleFacturaServiciosBasicos);
                            break;
                        case TipoFactura.facturaEmbotelladoras:
                            ((FacturaEmbotelladora)factura).ListaDetalle.Add(detalle as DetalleFacturaEmbotelladora);
                            break;
                        case TipoFactura.facturaBancos:
                            ((FacturaBanco)factura).ListaDetalle.Add(detalle as DetalleFacturaBanco);
                            break;
                        case TipoFactura.facturaHoteles:
                            ((FacturaHotel)factura).ListaDetalle.Add(detalle as DetalleFacturaHotel);
                            break;
                        case TipoFactura.facturaHospitales:
                            ((FacturaHospital)factura).ListaDetalle.Add(detalle as DetalleFacturaHospital);
                            break;
                        case TipoFactura.facturaJuegos:
                            ((FacturaJuego)factura).ListaDetalle.Add(detalle as DetalleFacturaJuego);
                            break;
                        case TipoFactura.facturaEspectaculoPublicoInternacional:
                            ((FacturaEspectaculoPublicoInternacional)factura).ListaDetalle.Add(detalle as DetalleFacturaEspectaculoPublicoInternacional);
                            break;
                        case TipoFactura.notaExportacion:
                            ((NotaExportacion)factura).ListaDetalle.Add(detalle as DetalleNotaExportacion);
                            break;
                        case TipoFactura.notaLibreConsignacion:
                            ((NotaLibreConsignacion)factura).ListaDetalle.Add(detalle as DetalleNotaLibreConsignacion);
                            break;
                        case TipoFactura.notaZonaFranca:
                            ((NotaZonaFranca)factura).ListaDetalle.Add(detalle as DetalleNotaZonaFranca);
                            break;
                        case TipoFactura.notaEspectaculoPublicoNacional:
                            ((NotaEspectaculoPublicoNacional)factura).ListaDetalle.Add(detalle as DetalleFacturaEspectaculoPublicoInternacional);
                            break;
                        case TipoFactura.notaSeguridadAlimentaria:
                            ((NotaSeguridadAlimentaria)factura).ListaDetalle.Add(detalle as DetalleNotaSeguridadAlimentaria);
                            break;
                        case TipoFactura.notaMonedaExtranjera:
                            ((NotaMonedaExtranjera)factura).ListaDetalle.Add(detalle as DetalleNotaMonedaExtranjera);
                            break;
                        //case TipoFactura.NOTA_DE_CREDITO_DEBITO:
                        //    break;
                        //case TipoFactura.NOTA_DE_CONCILIACION:
                        //    break;
                        //case TipoFactura.BOLETO_AEREO:
                        //    break;
                        case TipoFactura.notaTurismoReceptivo:
                            ((NotaTurismoReceptivo)factura).ListaDetalle.Add(detalle as DetalleNotaTurismoReceptivo);
                            break;
                        case TipoFactura.notaTasaCero:
                            factura.ListaDetalle.Add(detalle);
                            break;
                        case TipoFactura.facturaHidrocarburos:
                            ((FacturaHidrocarburos)factura).ListaDetalle.Add(detalle as DetalleFacturaHidrocarburos);
                            break;
                    }
                }

                requestFactura.Factura = factura;

                requestFactura.IdDocFiscalERP = drCabeceraFactura["fehfeiddf"].ToString();
                requestFactura.Cufd = null;
                requestFactura.Contingencia = (drCabeceraFactura["fehfecont"].ToString() != "0");
                requestFactura.EsLote = (drCabeceraFactura["fehfelote"].ToString() != "0");
                requestFactura.IdLoteERP = drCabeceraFactura["fehfeidlo"].ToString();
                requestFactura.UltFacturaLote = (drCabeceraFactura["fehfeufac"].ToString() != "0");
                requestFactura.CodigoTipoFactura = int.Parse(drCabeceraFactura["fehfectip"].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //Log.Instancia.LogWS_Mensaje_FSX(file, "Excepción: ArmarFacturaElectronicaJson2(DataTable dtCabezeraFactura, DataTable dtDetalleFactura) \r\n" +
                //    ex.Message);
            }

            return requestFactura;
        }

        private void CargarObjetoFactura(object obj, DataRow valores, Dictionary<string, string> camposDB)
        {
            foreach (PropertyInfo propiedad in obj.GetType().GetProperties())
            {
                var valor = valores[camposDB[propiedad.Name]];
                if (valor != null && valor != DBNull.Value)
                {
                    if (propiedad.PropertyType.ToString().Contains("Int16"))
                        propiedad.SetValue(obj, Convert.ToInt16(valor));
                    else if (propiedad.PropertyType.ToString().Contains("Int32"))
                        propiedad.SetValue(obj, Convert.ToInt32(valor));
                    else if (propiedad.PropertyType.ToString().Contains("Int64"))
                        propiedad.SetValue(obj, Convert.ToInt64(valor));
                    else if (propiedad.PropertyType.ToString().Contains("Decimal"))
                        propiedad.SetValue(obj, Convert.ToDecimal(valor));
                    else if (propiedad.PropertyType.ToString().Contains("String"))
                        propiedad.SetValue(obj, valor.ToString());
                }
            }
        }

        private void GuardarResponseFactura(ResponseFactura response)
        {
            StringBuilder sql = new StringBuilder("UPDATE fehfe SET ");
            List<DBAxon.Parameters> parametros = new List<DBAxon.Parameters>();

            //Respuesta
            sql.Append("fehfecres=?, fehfetres=?, ");
            parametros.Add(new DBAxon.Parameters("cres", response.Respuesta.CodRespuesta, ParameterDirection.Input, DbType.String, 10));
            parametros.Add(new DBAxon.Parameters("tres", response.Respuesta.TxtRespuesta, ParameterDirection.Input, DbType.String, 200));

            //Proceso
            sql.Append("fehfeifee = ?, fehfecufd = ?, fehfecsta = ?, fehfectip = ?, ");
            parametros.Add(new DBAxon.Parameters("ifee", response.Proceso.IdDocFiscalFEEL, ParameterDirection.Input, DbType.String, 20));
            parametros.Add(new DBAxon.Parameters("cufd", response.Proceso.CUFD, ParameterDirection.Input, DbType.String, 100));
            parametros.Add(new DBAxon.Parameters("csta", response.Proceso.CodEstado, ParameterDirection.Input, DbType.String, 10));
            parametros.Add(new DBAxon.Parameters("ctip", response.Proceso.CodigoTipoFactura, ParameterDirection.Input, DbType.Int32));

            //Factura - Cabecera
            sql.Append("fehfefemi=?, fehfectdi=?, fehfeccuf=?, fehfecsuc=?, fehfecpve=?, fehfecdse=?, fehfecmpa=?, fehfeleye=?, fehfecmon=? , fehfestat=" + EstadoDocumentoFiscal.E102_ProcesadaCorrectamente.ToString()+" ");
            parametros.Add(new DBAxon.Parameters("femi", response.Factura.Cabecera.FechaEmision, ParameterDirection.Input, DbType.String, 20));
            parametros.Add(new DBAxon.Parameters("ctdi", response.Factura.Cabecera.CodigoTipoDocumentoIdentidad, ParameterDirection.Input, DbType.Int32));
            parametros.Add(new DBAxon.Parameters("ccuf", response.Factura.Cabecera.CUF, ParameterDirection.Input, DbType.String, 50));
            parametros.Add(new DBAxon.Parameters("csuc", response.Factura.Cabecera.CodigoSucursal, ParameterDirection.Input, DbType.Int32));
            parametros.Add(new DBAxon.Parameters("cpve", response.Factura.Cabecera.CodigoPuntoVenta, ParameterDirection.Input, DbType.Int32));
            parametros.Add(new DBAxon.Parameters("cdse", response.Factura.Cabecera.CodigoDocumentoSector, ParameterDirection.Input, DbType.Int32));
            parametros.Add(new DBAxon.Parameters("cmpa", response.Factura.Cabecera.CodigoMetodoPago, ParameterDirection.Input, DbType.Int16));
            parametros.Add(new DBAxon.Parameters("leye", response.Factura.Cabecera.Leyenda, ParameterDirection.Input, DbType.String, 200));
            parametros.Add(new DBAxon.Parameters("cmon", response.Factura.Cabecera.CodigoMoneda, ParameterDirection.Input, DbType.Int32));

            sql.Append(" WHERE fehfeiddf= ?;");
            parametros.Add(new DBAxon.Parameters("iddf", response.Proceso.IdDocFiscalERP, ParameterDirection.Input, DbType.Int32));

            //Factura - ListaDetalle
            dynamic factura = Convert.ChangeType(response.Factura, response.Factura.GetType());
            //sql.Append("UPDATE fedfe SET ");
            if (factura.Tipo != TipoFactura.notaExportacion)
            {
                foreach (var detalle in factura.ListaDetalle)
                {
                    sql.Append("UPDATE fedfe SET fedfeaeco=?, fedfecpsi=? WHERE fedfeiddf = ?;");
                    parametros.Add(new DBAxon.Parameters("aeco", detalle.ActividadEconomica, ParameterDirection.Input, DbType.Int32));
                    parametros.Add(new DBAxon.Parameters("cpsi", detalle.CodigoProductoSIN, ParameterDirection.Input, DbType.Int32));
                    parametros.Add(new DBAxon.Parameters("iddf", response.Proceso.IdDocFiscalERP, ParameterDirection.Input, DbType.Int32));
                }
            }
            else {
                foreach (var detalle in factura.ListaDetalle)
                {
                    sql.Append("UPDATE fedfe SET fedfeaeco=?, fedfecpsi=?, fedfecnan=? WHERE fedfeiddf = ?;");
                    parametros.Add(new DBAxon.Parameters("aeco", detalle.ActividadEconomica, ParameterDirection.Input, DbType.Int32));
                    parametros.Add(new DBAxon.Parameters("cpsi", detalle.CodigoProductoSIN, ParameterDirection.Input, DbType.Int32));
                    parametros.Add(new DBAxon.Parameters("cnan", detalle.CodigoNandina, ParameterDirection.Input, DbType.Int32));
                    parametros.Add(new DBAxon.Parameters("iddf", response.Proceso.IdDocFiscalERP, ParameterDirection.Input, DbType.Int32));
                }
            }

            //BD
            DBAxon db = new DBAxon();
            try
            {
                db.OpenFactoryConnection();
                db.BeginTransaction();
                db.SetLockModeToWait();
                db.PrepareCommand(true, CommandType.Text, sql.ToString(), parametros.ToArray());
                db.command.ExecuteNonQuery();
                db.CommitTransaction();
            }
            catch (Exception ex)
            {
                db.RollbackTransaction();
                db.ExecuteNonQuery(CommandType.Text, "UPDATE fehfe SET fehfestat=" + int.Parse(EstadoDocumentoFiscal.E100_PendienteDeEnvio.ToString()) + " WHERE fehfeiddf=" + response.Proceso.IdDocFiscalERP);
                throw new Exception("Excepcion: GuardarResponseFactura() - " + ex.Message);
            }
            finally
            {
                db.CloseFactoryConnection();
                db = null;
            }
        }
    }
}
