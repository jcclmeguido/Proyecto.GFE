using Axon.GFE;
using Axon.GFE.Mapeadores;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AxonGFEService.Controllers
{
    [RoutePrefix("axon-gfe")]
    public class GFEController : ApiController
    {
        [HttpPost]
        [Route("validacion")]
        public JObject PostValidacion(JObject jo)
        {
            #region Validar JSON de entrada
            Validacion v = null;
            if (!jo.ContainsKey("tipoValidacion") || !jo.ContainsKey("estado")) {
                Log.Instancia.ErrorRestService("PostValidacion", Error.E200 + "|"+jo.ToString());
                return JObject.Parse(new Respuesta() { CodRespuesta = "200", TxtRespuesta = Error.E200 }.ToString());
            }

            if (jo["estado"].ToString() != EstadoDocumentoFiscal.E903_Procesada.ToString() && jo["estado"].ToString() != EstadoDocumentoFiscal.E904_Observada.ToString())
            {
                Log.Instancia.ErrorRestService("PostValidacion", Error.E205 + "|" + jo.ToString());
                return JObject.Parse(new Respuesta() { CodRespuesta = "205", TxtRespuesta = Error.E205 }.ToString());
            }
            if (jo["estado"].ToString() == EstadoDocumentoFiscal.E904_Observada.ToString()) {
                if (!(jo.ContainsKey("listaMensajes") || jo.ContainsKey("listaErrores")) || (jo.ContainsKey("listaMensajes") && jo["listaMensajes"].ToString() == "") || (jo.ContainsKey("listaErrores") && jo["listaErrores"].ToString() == ""))
                {
                    Log.Instancia.ErrorRestService("PostValidacion", Error.E206 + "|" + jo.ToString());
                    return JObject.Parse(new Respuesta() { CodRespuesta = "206", TxtRespuesta = Error.E206 }.ToString());
                }
            }

            if (jo.ContainsKey("fechaRecepcion")){
                DateTime dt;
                if (!DateTime.TryParseExact(jo["fechaRecepcion"].ToString(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    Log.Instancia.ErrorRestService("PostValidacion", Error.E204 + "|" + jo.ToString());
                    return JObject.Parse(new Respuesta() { CodRespuesta = "204", TxtRespuesta = Error.E204 }.ToString());
                }
            }
            #endregion

            try
            {
                short tipo = Convert.ToInt16(jo["tipoValidacion"].ToString());
                if (tipo == 1 || tipo == 12)
                    v = JsonConvert.DeserializeObject<ValidacionFactura>(jo.ToString());
                else if ((tipo >= 8 && tipo <= 11) || (tipo >= 19 && tipo <= 22) || (tipo >= 25 && tipo <= 28))
                    v = JsonConvert.DeserializeObject<ValidacionPaquete>(jo.ToString());
                else v = JsonConvert.DeserializeObject<Validacion>(jo.ToString());
            }
            catch (Exception ex)
            {
                Log.Instancia.ErrorRestService("PostValidacion", Error.E201 + " - " + ex.Message + "|" + jo.ToString());
                return JObject.Parse(new Respuesta() { CodRespuesta = "201", TxtRespuesta = Error.E201 }.ToString());
            }

            try
            {
                int res = v.GuardarRespuestaValidacion();
                if (res == 0) {
                    Log.Instancia.ErrorRestService("PostValidacion", Error.E203 + " - IdDocFiscalFEEL:" + v.IdDocFiscalFEEL);
                    return JObject.Parse(new Respuesta() { CodRespuesta = "203", TxtRespuesta = Error.E203 + " " + v.IdDocFiscalFEEL }.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.Instancia.ErrorRestService("PostValidacion", Error.E202 + " - " + ex.Message + "|" + jo.ToString());
                return JObject.Parse(new Respuesta() { CodRespuesta = "202", TxtRespuesta = Error.E202 }.ToString());
            }

            return JObject.Parse(new Respuesta() { CodRespuesta = "0", TxtRespuesta = "EXITO" }.ToString());
        }
    }
}
