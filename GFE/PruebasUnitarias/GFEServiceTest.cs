using System;
using System.IO;
using Axon.GFE.Mapeadores;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using Axon.GFE;
using System.Data;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Linq;
using Axon.WinSerAppCensador;
using System.Threading;

namespace PruebasUnitarias
{
    [TestClass]
    public class GFEServiceTest
    {
        [TestMethod]
        public void ListaRemove()
        {
            List<string> lista = new string[] { "1", "2", "3", "4", "5" }.ToList();
            Debug.WriteLine(string.Join("\t", lista.ToArray()));
            for (int i = 0; i < lista.Count; i++)
            {
                Debug.WriteLine(i+": "+string.Join("\t", lista.ToArray()));
                if (i == 2) lista.RemoveAt(i);
            }
        }

        [TestMethod]
        public void ServicioGFEvalidacion()
        {
            if (ValidarDatosConexion())
            {
                DBAxon db = new DBAxon();
                Axon.DAL.Conexion oConexion = new Axon.DAL.Conexion();
                string baseDatos = ConfigurationManager.AppSettings["BaseDatos"].ToString();
                string dataSource = ConfigurationManager.AppSettings["DataSource"].ToString();
                string userId = ConfigurationManager.AppSettings["UserId"].ToString();
                string password = ConfigurationManager.AppSettings["Password"].ToString();
                string dbLocale = ConfigurationManager.AppSettings["dbLocale"].ToString();
                string clientLocale = ConfigurationManager.AppSettings["clientLocale"].ToString();
                Axon.DAL.TipoConexion tipoConexion = (Axon.DAL.TipoConexion)Convert.ToInt32(ConfigurationManager.AppSettings["TipoConn"].ToString());
                oConexion.CargarDatosConfiguracion(tipoConexion, baseDatos, dataSource, userId, password, clientLocale, dbLocale);

                DataTable facturas = null;
                try
                {
                    db.OpenFactoryConnection();
                    db.SetLockModeToWait();
                    string query = "SELECT FIRST 500 fehfeifee, fehfenfac, fehfeccuf FROM fehfe WHERE fehfeifee IS NOT NULL AND fehfenfac IS NOT NULL AND fehfeccuf IS NOT NULL";
                    facturas = db.DataAdapter(CommandType.Text, query);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    db.CloseFactoryConnection();
                    db = null;
                }

                string request = @"{""tipoValidacion""= 12,""idDocFiscalFEEL"" = ""11"",""respuestaSIN"" = true,""codigoRecepcion"" = """",""estado"" = 903,""numeroFactura"" = 1,""cuf"" = ""BCA""}";
                Debug.WriteLine("Iniciando - " + DateTime.Now.ToString("HH:mm:ss"));
                Debug.WriteLine(facturas.Rows.Count + " facturas...");

                foreach (DataRow f in facturas.Rows)
                {
                    request = @"{""tipoValidacion"": 12,""idDocFiscalFEEL"" : ""[@idDoc]"",""respuestaSIN"" : true,""codigoRecepcion"": -1,""estado"": 903,""numeroFactura"": [@numFactu],""cuf"": ""[@cuf]""}";
                    request = request.Replace("[@idDoc]", f["fehfeifee"].ToString());
                    request = request.Replace("[@numFactu]", f["fehfenfac"].ToString());
                    request = request.Replace("[@cuf]", f["fehfeccuf"].ToString());

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    //var restClient = new RestClient("http://localhost:64918/axon-gfe");
                    var restClient = new RestClient("http://190.104.3.139:1580/GFE/axon-gfe");
                    var rr = new RestRequest("validacion", Method.POST);
                    rr.AddParameter("application/json", request, ParameterType.RequestBody);
                    var resp = restClient.Execute(rr);

                    sw.Stop();
                    Debug.WriteLine(sw.ElapsedMilliseconds);

                    JObject res = JObject.Parse(resp.Content);
                    if (res.ContainsKey("codRespuesta"))
                    {
                        if (res["codRespuesta"].ToString() != "0") throw new Exception("Retornó error: " + res["codRespuesta"].ToString() + " - " + res["txtRespuesta"]);
                    }
                    else throw new Exception("no hay codRespuesta" + Environment.NewLine + resp.Content);
                }

                Debug.WriteLine("FIN - " + DateTime.Now.ToString("HH:mm:ss"));
            }
        }

        [TestMethod]
        public void Chronos() {
            Cronometro.Instancia.Iniciar();
            RegistroTiempos.Instancia.IniciarRegistros();
            Thread.Sleep(1000);
            RegistroTiempos.Instancia.AddTiempoFEEL(1000);
            Thread.Sleep(500);
            RegistroTiempos.Instancia.AddTiempoGFE(500);
            TimeSpan ts = Cronometro.Instancia.Detener();
            RegistroTiempos.Instancia.GuardarRegistros();
            Debug.WriteLine(string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10));
        }

        [TestMethod]
        public void CambiarJsonNAMEprop()
        {
            Factura f1 = new FacturaEstandar()
            {
                Cabecera = new CabeceraFacturaEstandar()
                {
                    NumeroFactura = 102,
                    Direccion = "GV",
                    FechaEmision = "20190116180000",
                    CodigoTipoDocumentoIdentidad = "3",
                    CUF = null,
                    NumeroDocumento = "5642111",
                    Complemento = null,
                    CodigoSucursal = 1,
                    CodigoPuntoVenta = 123,
                    NombreRazonSocial = "Juan Perez",
                    MontoTotal = 50.5m,
                    MontoDescuento = 0,
                    CodigoCliente = "55421",
                    CodigoDocumentoSector = 1,
                    NITEmisor = 305080026,
                    CodigoMetodoPago = 1,
                    NumeroTarjeta = 414,
                    Leyenda = "Ley No 453: ",
                    Usuario = "FTL",
                    CodigoMoneda = 1,
                    MontoTotalMoneda = 50.5m,
                    TipoCambio = 6.96m
                },
                //ListaDetalle= new List<Detalle>()
            };
                
            ((FacturaEstandar)f1).ListaDetalle.Add(
                new DetalleFacturaEstandar()
                {
                    ActividadEconomica = 123123,
                    CodigoProductoSIN = 123,
                    CodigoProducto = 123,
                    Descripcion = "coca cola 1 litro",
                    Cantidad = 5,
                    PrecioUnitario = 10.1m,
                    MontoDescuentoDetalle = 0,
                    SubTotal = 50.5m,
                    NumeroSerie = "ABC345",
                    UnidadMedida = "botella"
                });

            Factura f2 = new FacturaAlquiler()
            {
                Cabecera = new CabeceraFacturaAlquiler()
                {
                    NumeroFactura = 102,
                    Direccion = "GV",
                    FechaEmision = "20190116180000",
                    CodigoTipoDocumentoIdentidad = "3",
                    CUF = null,
                    NumeroDocumento = "5642111",
                    Complemento = null,
                    CodigoSucursal = 1,
                    CodigoPuntoVenta = 123,
                    NombreRazonSocial = "Juan Perez",
                    MontoTotal = 50.5m,
                    MontoDescuento = 0,
                    CodigoCliente = "55421",
                    CodigoDocumentoSector = 1,
                    NITEmisor = 305080026,
                    CodigoMetodoPago = 1,
                    PeriodoFacturado = "string",
                    Leyenda = "Ley No 453: ",
                    Usuario = "FTL",
                    CodigoMoneda = 1,
                    MontoTotalMoneda = 50.5m,
                    TipoCambio = 6.96m
                },
                //ListaDetalle = new List<Detalle>()
            };

            f2.ListaDetalle.Add(
                new Detalle()
                {
                    ActividadEconomica = 123123,
                    CodigoProductoSIN = 123,
                    CodigoProducto = 123,
                    Descripcion = "coca cola 1 litro",
                    MontoDescuentoDetalle = 0,
                    SubTotal = 50.5m,
                    UnidadMedida = "botella"
                });



            string r1 = @"{ ""respuesta"": null, ""proceso"":null, ""facturaEstandar"": " + f1.ToString() + "}";
            string r2 = @"{ ""respuesta"": null, ""proceso"": null, ""facturaAlquiler"": " + f2.ToString() + "}";
            string r3 = @"{ ""respuesta"": null, ""proceso"": null, ""fac"": null }";
            string r4 = @"{ ""respuesta"": {
                ""codRespuesta"": ""0"",
                ""txtRespuesta"": ""Exito""
                },
                ""proceso"": {
                                ""idDocFiscalFEEL"": 11,
                ""cufd"": ""42bf9913ec06745b7f3c5cc62b32a59e"",
                ""codEstado"": ""D1"",
                ""idDocFiscalERP"": ""3"",
                ""codigoTipoFactura"": 1
                },
                ""facturaEstandar"": {
                    ""cabecera"": {
                        ""numeroFactura"": 3,
                        ""direccion"": ""Gualberto villarroel 123"",
                        ""fechaEmision"": 20190101180000,
                        ""codigoTipoDocumentoIdentidad"": 1,
                        ""cuf"": ""B41ECAD147A6BE9D2AD905278126E6FB4B88B3E3"",
                        ""numeroDocumento"": ""5642111"",
                        ""complemento"": """",
                        ""codigoSucursal"": 0,
                        ""nombreRazonSocial"": ""Juan Perez"",
                        ""montoTotal"": 50.5,
                        ""codigoCliente"": ""55421"",
                        ""codigoDocumentoSector"": 1,
                        ""nitEmisor"": 1028305029,
                        ""codigoMetodoPago"": 1,
                        ""leyenda"": ""Ley NÂ° 453: Los medios de comunicaciÃ³n deben promover el respeto de los derechos de los usuarios y consumidores."",
                        ""usuario"": ""FTL"",
                        ""codigoMoneda"": 1,
                        ""montoTotalMoneda"": 50.5,
                        ""tipoCambio"": 6.97,
                        ""numeroTarjeta"": 54321
                    },
                    ""detalle"": [
                        {
                            ""actividadEconomica"": 123123,
                            ""codigoProductoSin"": 123,
                            ""codigoProducto"": ""123"",
                            ""descripcion"": ""coca cola 1 litro"",
                            ""cantidad"": 5,
                            ""precioUnitario"": 10,
                            ""subTotal"": 10,
                            ""numeroSerie"": ""1234567ADC"",
                            ""unidadMedida"": ""botella""
                        },
                        {
                            ""actividadEconomica"": 123123,
                            ""codigoProductoSin"": 123,
                            ""codigoProducto"": ""123"",
                            ""descripcion"": ""Fanta 1 litro"",
                            ""cantidad"": 5,
                            ""precioUnitario"": 10,
                            ""subTotal"": 10,
                            ""numeroSerie"": ""1234567ADC"",
                            ""unidadMedida"": ""botella""
                        }
                    ]
                }
            }";
            string r5 = @"{ ""respuesta"": { ""codRespuesta"": ""1200"", ""txtRespuesta"": ""Error la facturaEstandar ya se encuentra registrada numeroFactura :3"" } }";

            RequestFactura reqq = new RequestFactura()
            {
                CodigoTipoFactura = 1,
                Contingencia = false,
                Cufd = "42bf9913ec06745b7f3c5cc62b32a59e",
                EsLote = false,
                IdDocFiscalERP = "3",
                IdLoteERP = "",
                UltFacturaLote = false,
                Factura = f1
            };
            Debug.WriteLine(reqq);
            Debug.WriteLine(JObject.FromObject(reqq).ToString());

            ResponseFactura resp4 = new ResponseFactura(r4);
            Debug.WriteLine(resp4);
            Debug.WriteLine(((CabeceraFacturaEstandar)resp4.Factura.Cabecera).NumeroTarjeta);

            ResponseFactura resp5 = new ResponseFactura(r5);
            Debug.WriteLine(resp5);
            //ResponseFactura resp1 = JsonConvert.DeserializeObject<ResponseFactura>(r1, HelperJson.AdaptarResponseFactura(r1));
            //ResponseFactura resp2 = JsonConvert.DeserializeObject<ResponseFactura>(r2, HelperJson.AdaptarResponseFactura(r2));
            ////RequestFactura req3 = JsonConvert.DeserializeObject<RequestFactura>(r3, HelperJson.AdaptarResponseFactura(r3));

            //Debug.WriteLine(resp1.Factura.GetType().ToString());
            //Debug.WriteLine(resp2.Factura.GetType().ToString());
            //Debug.WriteLine((FacturaEstandar)resp1.Factura);
            //Debug.WriteLine((FacturaAlquiler)resp2.Factura);
        }

        [TestMethod]
        public void ProcesoConsultaDocumentoFiscal()
        {
            if (ValidarDatosConexion())
            {
                DBAxon db = new DBAxon();
                Axon.DAL.Conexion oConexion = new Axon.DAL.Conexion();
                string baseDatos = ConfigurationManager.AppSettings["BaseDatos"].ToString();
                string dataSource = ConfigurationManager.AppSettings["DataSource"].ToString();
                string userId = ConfigurationManager.AppSettings["UserId"].ToString();
                string password = ConfigurationManager.AppSettings["Password"].ToString();
                string dbLocale = ConfigurationManager.AppSettings["dbLocale"].ToString();
                string clientLocale = ConfigurationManager.AppSettings["clientLocale"].ToString();
                Axon.DAL.TipoConexion tipoConexion = (Axon.DAL.TipoConexion)Convert.ToInt32(ConfigurationManager.AppSettings["TipoConn"].ToString());
                oConexion.CargarDatosConfiguracion(tipoConexion, baseDatos, dataSource, userId, password, clientLocale, dbLocale);

                ServiceCensador serviceCensador = new ServiceCensador();

                serviceCensador.EjecutarProcesoConsultaDocumentoFiscal();
            }
        }

        private bool ValidarDatosConexion()
        {
            int idEvento = 1;
            //EventLog eventLog1 = new EventLog();
            //string sSource = "WinSerAppCensadorTest";
            //string sLog = "Application";
            //if (!EventLog.SourceExists(sSource))
            //    EventLog.CreateEventSource(sSource, sLog);
            //eventLog1.Source = sSource;
            //eventLog1.Log = sLog;
            //string nombre = eventLog1.LogDisplayName;
            //eventLog1.WriteEntry("Validando datos archivo de configuracion.", EventLogEntryType.Information, idEvento, 999);

            //Nombre de la base de datos
            ParametrosConexion.DATABASE = ConfigurationManager.AppSettings["BaseDatos"].ToString();

            if (ParametrosConexion.DATABASE.Trim() == string.Empty)
            {
                idEvento = 4;
                //eventLog1.WriteEntry("Nombre de la base de datos no puede ser vacío/nulo.", EventLogEntryType.Error, idEvento, 999);
                return false;
            }

            ParametrosConexion.SERVERNAME = ConfigurationManager.AppSettings["DataSource"];     //Nombre del servidor
            if (ParametrosConexion.SERVERNAME.Trim() == string.Empty)
            {
                idEvento = 8;
                //eventLog1.WriteEntry("Nombre del servidor no puede ser vacío/nulo.", EventLogEntryType.Error, idEvento, 999);
                return false;
            }

            //ParametrosConexion.Intervalo = ConfigurationManager.AppSettings["Intervalo"];        //Intervalo
            //if (ParametrosConexion.Intervalo.Trim() == "")
            //{
            //    idEvento = 12;
            //    eventLog1.WriteEntry("Interalo no puede ser nulo.", EventLogEntryType.Error, idEvento, 999);
            //    return false;
            //}

            ParametrosConexion.User = ConfigurationManager.AppSettings["UserId"];     //Usuario
            if (ParametrosConexion.User.Trim() == string.Empty)
            {
                idEvento = 16;
                //eventLog1.WriteEntry("Usuario no puede ser vacío/nulo.", EventLogEntryType.Error, idEvento, 999);
                return false;
            }

            //ParametrosConexion.Clav = ConfigurationManager.AppSettings["Clav"];     //Clave
            //if (ParametrosConexion.Clav.Trim() == "")
            //{
            //    idEvento = 20;
            //    eventLog1.WriteEntry("Clave no puede ser vacía/nula.", EventLogEntryType.Error, idEvento, 999);
            //    return false;
            //}

            idEvento = 100;
            //eventLog1.WriteEntry("Validación de configuracion correcta.", EventLogEntryType.Information, idEvento, 999);

            return true;
        }

        [TestMethod]
        public void ReqFEELFactura()
        {
            RequestFactura req = new RequestFactura()
            {
                Factura = new FacturaEstandar()
            };

            RequestFactura req2 = new RequestFactura()
            {
                Factura = new NotaMonedaExtranjera()
            };

            Debug.WriteLine(req.ToString());
            Debug.WriteLine(req2.ToString());
        }

        [TestMethod]
        public void ConsultaDocFiscal() {
            RequestConsultaDocumentoFiscal req = new RequestConsultaDocumentoFiscal()
            {
                //FechaDesde = 20190101,
                //FechaHasta = 20190129,
                NITEmisor = 305080026,
                CUF = "357042F1C7B1E5730FC69697849D03D05DC2C3C7"
            };

            RequestConsultaDocumentoFiscal req3 = new RequestConsultaDocumentoFiscal()
            {
                NITEmisor = 1234567,
                CUF = "357042F1C7B1E5730E7371B6FB68CF3C83472B24"
            };

            RequestConsultaDocumentoFiscal req4 = new RequestConsultaDocumentoFiscal()
            {
                NITEmisor = 305080026,
                CUF = "357042F1C7B1E5730E7371B6EDAB9F7B4CB54D06"
            };

            Debug.WriteLine(req);

            var jsonResolver = new IgnorarRenombrarPropiedadJson();
            jsonResolver.IgnorarPropiedades(typeof(RequestConsultaDocumentoFiscal), "cufd");
            var ss = new JsonSerializerSettings();
            ss.ContractResolver = jsonResolver;
            var req2 = JsonConvert.SerializeObject(req, ss);

            Debug.WriteLine(req4);

            //string json = @"{""fechaDesde"":20190101, ""fechaHasta"":20190129,""nitEmisor"":305080026, ""cuf"": ""357042F1C7B1E5730FC69697849D03D05DC2C3C7""}";
            var restClient = new RestClient("http://190.171.205.89:8080/ws-find-docfiscal/fnd/docfiscal/find");
            var rr = new RestRequest(Method.POST);
            rr.AddParameter("application/json", req4.ToString(), ParameterType.RequestBody);
            var resp = restClient.Execute(rr);
            Debug.WriteLine(resp.Content);
        }

        [TestMethod]
        public void ValidacionPublished()
        {
            Validacion v1 = new Validacion()
            {
                CodigoRecepcion = 132,
                Estado = EstadoValidacion.Observada,
                IdDocFiscalFEEL = "12",
                ListaMensajes = new List<short>(),
                RespuestaSIN = true,
                Tipo = TipoValidacion.NotaCredDebElectronica
            };
            v1.ListaMensajes.Add(2);
            v1.ListaMensajes.Add(3);
            v1.ListaMensajes.Add(4);

            Validacion v2 = new ValidacionFactura()
            {
                CodigoRecepcion = 132,
                Estado = EstadoValidacion.Observada,
                IdDocFiscalFEEL = "10",
                ListaMensajes = null,
                RespuestaSIN = true,
                Tipo = TipoValidacion.FacturaElectronica,
                FechaRecepcion = "20190116180000"
            };

            Validacion v3 = new ValidacionPaquete()
            {
                CodigoRecepcion = 132,
                Estado = EstadoValidacion.Observada,
                IdDocFiscalFEEL = "123",
                ListaMensajes = v1.ListaMensajes,
                RespuestaSIN = true,
                Tipo = TipoValidacion.PaqueteFacturaComputarizada,
                ListaErrores = v1.ListaMensajes
            };

            

            string json = "{ " +
              "\"tipoValidacion\": 19," +
              "\"idDocFiscalFEEL\": \"2\"," +
              "\"respuestaSIN\": true," +
              "\"codigoRecepcion\": 132," +
              "\"estado\": 903," +
              "\"listaMensajes\": [5,3,6]," +
              //"\"numeroFactura\": 12345," +
              //"\"cuf\": \"string\"," +
              "\"listaErrores\": [2,3,4]," +
                "}";

            //Validacion vvv = JsonConvert.DeserializeObject<ValidacionPaquete>(json);

            var restClient = new RestClient("http://190.104.3.139:1580/GFE/axon-gfe");
            var rr = new RestRequest("validacion", Method.POST);
            //rr.AddParameter("application/json", v2, ParameterType.RequestBody);
            //var resp = restClient.Execute(rr);
            //Debug.WriteLine(resp.Content);

            ////restClient = new RestClient("http://190.104.3.139:1580/GFE/axon-gfe");
            //rr = new RestRequest("validacion", Method.POST);
            //rr.AddParameter("application/json", v1, ParameterType.RequestBody);
            //resp = restClient.Execute(rr);
            //Debug.WriteLine(resp.Content);

            //rr = new RestRequest("validacion", Method.POST);
            //rr.AddParameter("application/json", v3, ParameterType.RequestBody);
            //resp = restClient.Execute(rr);
            //Debug.WriteLine(resp.Content);

            //rr = new RestRequest("validacion", Method.POST);
            rr.AddParameter("application/json", json, ParameterType.RequestBody);
            var resp = restClient.Execute(rr);
            Debug.WriteLine(resp.Content);
        }

        [TestMethod]
        public void ValidacionTest()
        {
            Validacion v1 = new Validacion()
            {
                CodigoRecepcion = 132,
                Estado = EstadoValidacion.Observada,
                IdDocFiscalFEEL = "12",
                ListaMensajes = new List<short>(),
                RespuestaSIN = true,
                Tipo = TipoValidacion.NotaCredDebElectronica
            };
            v1.ListaMensajes.Add(2);
            v1.ListaMensajes.Add(3);
            v1.ListaMensajes.Add(4);

            Validacion v2 = new ValidacionFactura()
            {
                CodigoRecepcion = 132,
                Estado = EstadoValidacion.Observada,
                IdDocFiscalFEEL = "10",
                ListaMensajes = null,
                RespuestaSIN = true,
                Tipo = TipoValidacion.FacturaElectronica,
                FechaRecepcion = "20190116180000"
            };

            Validacion v3 = new ValidacionPaquete()
            {
                CodigoRecepcion = 132,
                Estado = EstadoValidacion.Observada,
                IdDocFiscalFEEL = "123",
                ListaMensajes = v1.ListaMensajes,
                RespuestaSIN = true,
                Tipo = TipoValidacion.PaqueteFacturaComputarizada,
                ListaErrores = v1.ListaMensajes
            };

            string json = "{ " +
              "\"tipoValidacion\": 19," +
              "\"idDocFiscalFEEL\": \"98\"," +
              "\"respuestaSIN\": true," +
              "\"codigoRecepcion\": 132," +
              "\"estado\": 903," +
              //"\"listaMensajes\": [5,3,6]," +
              "\"numeroFactura\": 12345," +
              "\"cuf\": \"string\"," +
                //"\"listaErrores\": [2,3,4]," +
                "}";

            Validacion vvv = JsonConvert.DeserializeObject<ValidacionPaquete>(json);

            //FechaRecepcion = "2019 01 16 18 00 00"
            //DateTime fechahora = DateTime.ParseExact(((ValidacionFactura)v2).FechaRecepcion, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            //Debug.WriteLine(fechahora.ToString("HH:mm:ss"));
            //string jsonrequest = @"{ ""prueba"": ""this is Sparta!!!"", ""voulez"": true }";
            var restClient = new RestClient("http://localhost:64918/axon-gfe");
            var rr = new RestRequest("validacion", Method.POST);
            rr.AddParameter("application/json", json, ParameterType.RequestBody);
            var resp = restClient.Execute(rr);
            Debug.WriteLine(resp.Content);

            rr = new RestRequest("validacion", Method.POST);
            rr.AddParameter("application/json", v1, ParameterType.RequestBody);
            resp = restClient.Execute(rr);
            Debug.WriteLine(resp.Content);

            rr = new RestRequest("validacion", Method.POST);
            rr.AddParameter("application/json", v2, ParameterType.RequestBody);
            resp = restClient.Execute(rr);
            Debug.WriteLine(resp.Content);

            rr = new RestRequest("validacion", Method.POST);
            rr.AddParameter("application/json", v3, ParameterType.RequestBody);
            resp = restClient.Execute(rr);
            Debug.WriteLine(resp.Content);

            //rr = new RestRequest("validacion", Method.POST);
            //rr.AddParameter("application/json", json, ParameterType.RequestBody);
            //resp = restClient.Execute(rr);
            //Debug.WriteLine(resp.Content);
        }

        JObject Validacion(JObject jo) {
            Validacion v = null;
            if (jo.ContainsKey("tipoValidacion"))
            {
                short tipo = Convert.ToInt16(jo["tipoValidacion"].ToString());
                if (tipo == 1 || tipo == 12)
                    v = JsonConvert.DeserializeObject<ValidacionFactura>(jo.ToString());
                else if ((tipo >= 8 && tipo <= 11) || (tipo >= 19 && tipo <= 22) || (tipo >= 25 && tipo <= 28))
                    v = JsonConvert.DeserializeObject<ValidacionPaquete>(jo.ToString());
                else v = JsonConvert.DeserializeObject<Validacion>(jo.ToString());
            }

            Debug.WriteLine(JObject.FromObject(v));

            return JObject.FromObject(v);
        }

        [TestMethod]
        public void ToValidacionTest() {
            Validacion v1 = new Validacion() {
                CodigoRecepcion = 132,
                Estado = EstadoValidacion.Procesada,
                IdDocFiscalFEEL = "123",
                ListaMensajes = new List<short>(),
                RespuestaSIN = true,
                Tipo = TipoValidacion.NotaCredDebElectronica
            };
            v1.ListaMensajes.Add(2);
            v1.ListaMensajes.Add(3);
            v1.ListaMensajes.Add(4);

            Validacion v2 = new ValidacionFactura()
            {
                CodigoRecepcion = 132,
                Estado = EstadoValidacion.Procesada,
                IdDocFiscalFEEL = "123",
                ListaMensajes = v1.ListaMensajes,
                RespuestaSIN = true,
                Tipo = TipoValidacion.FacturaElectronica,
                FechaRecepcion = "hoy/esteaño"
            };

            Validacion v3 = new ValidacionPaquete()
            {
                CodigoRecepcion = 132,
                Estado = EstadoValidacion.Procesada,
                IdDocFiscalFEEL = "123",
                ListaMensajes = v1.ListaMensajes,
                RespuestaSIN = true,
                Tipo = TipoValidacion.PaqueteFacturaComputarizada,
                ListaErrores = v1.ListaMensajes
            };
            string json = "{ " +
              "\"tipoValidacion\": 19," +
              "\"idDocFiscalFEEL\": \"123\"," +
              "\"respuestaSIN\": true," +
              "\"codigoRecepcion\": 132," +
              "\"estado\": 2," +
              "\"listaMensajes\": [5,3,6]," +
              "\"numeroFactura\": 12345," +
              "\"cuf\": \"string\"," +
              "\"listaErrores\": [2,3,4]," +
                "}";

            Validacion vvv = JsonConvert.DeserializeObject<ValidacionPaquete>(json);

            Debug.WriteLine((int)TipoValidacion.AnulacionFacturaElectronica);

            JObject jo = Validacion(JObject.FromObject(v1));
            jo = Validacion(JObject.FromObject(v2));
            jo = Validacion(JObject.FromObject(v3));

            Debug.WriteLine(jo.ToString());
            //Validacion v2 = JsonConvert.DeserializeObject<Validacion>(jo.ToString());

        }

        [TestMethod]
        public void PruebaWATest()
        {   
            string jsonrequest = @"{ ""prueba"": ""this is Sparta!!!"", ""voulez"": true }";
            var restClient = new RestClient("http://localhost:51620/api/prueba");
            var rr = new RestRequest("jjsonson", Method.POST);
            //var rr = new RestRequest("Products", Method.POST);
            //rr.AddHeader("cache-control", "no-cache");
            //rr.AddHeader("N-MS-AUTHCB", APIKEY);
            rr.AddParameter("application/json", jsonrequest, ParameterType.RequestBody);
            var resp = restClient.Execute(rr);
            Debug.WriteLine(resp.Content);
            //Assert.AreEqual(resp.Content, "Recibido:");
        }

        [TestMethod]
        public void GetProductTest()
        {
            string jsonrequest = @"{ ""prueba"": ""this is Sparta!!!"", ""voulez"": true }";
            var restClient = new RestClient("http://localhost:64918/api/prueba");
            var rr = new RestRequest("jjsonson", Method.POST);
            //var rr = new RestRequest("Products", Method.POST);
            //rr.AddHeader("cache-control", "no-cache");
            //rr.AddHeader("N-MS-AUTHCB", APIKEY);
            rr.AddParameter("application/json", jsonrequest, ParameterType.RequestBody);
            var resp = restClient.Execute(rr);
            System.Diagnostics.Debug.WriteLine(resp.Content);
            //Assert.AreEqual(resp.Content, "Recibido:");
        }

        [TestMethod]
        public void GetProductsTest() {
           var restClient = new RestClient("http://localhost:64918/api");
           RestRequest rr = new RestRequest("Products", Method.POST);
           var resp = restClient.Execute(rr);
           System.Diagnostics.Debug.WriteLine(resp.Content);

        }

        [TestMethod]
        public void ToJSON()
        {
            //Factura f = new Factura()
            //{
            //    facturaEstandar = new FacturaEstandar()
            //    {
            //        cabecera = new Cabecera()
            //        {
            //            numeroFactura = 102,
            //            direccion = "GV",
            //            fechaEmision = "20190116180000",
            //            codigoTipoDocumentoIdentidad = "3",
            //            cuf = null,
            //            numeroDocumento = "5642111",
            //            complemento = null,
            //            codigoSucursal = 1,
            //            codigoPuntoVenta = null,
            //            nombreRazonSocial = "Juan Perez",
            //            montoTotal = 50.5m,
            //            montoDescuento = 0,
            //            codigoCliente = "55421",
            //            codigoDocumentoSector = 1,
            //            nitEmisor = 305080026,
            //            codigoMetodoPago = 1,
            //            numeroTarjeta = null,
            //            leyenda = "Ley No 453: ",
            //            usuario = "FTL",
            //            codigoMoneda = 1,
            //            montoTotalMoneda = 50.5m,
            //            tipoCambio = null
            //        },
            //        detalle = new System.Collections.Generic.List<Detalle>()
            //    },
            //    idDocFiscalERP = "102",
            //    cufd = null,
            //    contingencia = false,
            //    esLote = false,
            //    idLoteERP = "0",
            //    ultFacturaLote = false,
            //    codigoTipoFactura = 1
            //};
            //f.facturaEstandar.detalle.Add(
            //    new Detalle()
            //    {
            //        actividadEconomica = 123123,
            //        codigoProductoSin = 123,
            //        codigoProducto = 123,
            //        descripcion = "coca cola 1 litro",
            //        cantidad = 5,
            //        precioUnitario = 10.1m,
            //        montoDescuentoDetalle = 0,
            //        subTotal = 50.5m,
            //        numeroSerie = "ABC345",
            //        unidadMedida = "botella"
            //    });

            //foreach (PropertyInfo propertyInfo in f.facturaEstandar.cabecera.GetType().GetProperties())
            //{
            //    Debug.WriteLine(propertyInfo.Name + ": " + propertyInfo.PropertyType);
            //    Type t = propertyInfo.PropertyType;
            //    switch (Type.GetTypeCode(t))
            //    {
            //        case TypeCode.Empty:
            //            break;
            //        case TypeCode.Object:
            //            break;
            //        case TypeCode.DBNull:
            //            break;
            //        case TypeCode.Boolean:
            //            break;
            //        case TypeCode.Char:
            //            break;
            //        case TypeCode.SByte:
            //            break;
            //        case TypeCode.Byte:
            //            break;
            //        case TypeCode.Int16:
            //            break;
            //        case TypeCode.UInt16:
            //            break;
            //        case TypeCode.Int32:
            //            break;
            //        case TypeCode.UInt32:
            //            break;
            //        case TypeCode.Int64:
            //            Debug.WriteLine("long " + propertyInfo.Name);
            //            break;
            //        case TypeCode.UInt64:
            //            break;
            //        case TypeCode.Single:
            //            break;
            //        case TypeCode.Double:
            //            break;
            //        case TypeCode.Decimal:
            //            break;
            //        case TypeCode.DateTime:
            //            break;
            //        case TypeCode.String:
            //            Debug.WriteLine("string " + propertyInfo.Name);
            //            break;
            //        default:
            //            break;
            //    }
            //}



            //Debug.WriteLine(typeof(string));

            //string jsonrequest = JsonConvert.SerializeObject(f);
            //System.Diagnostics.Debug.WriteLine(jsonrequest);
        }
    }
}
