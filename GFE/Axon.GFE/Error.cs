using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE
{
    public static class Error
    {
        //Validación
        public const string E200 = "La cadena JSON de entrada no cumple con el formato exigido";
        public const string E201 = "Estructura JSON inválida para validación";
        public const string E202 = "Ocurrió un error en el proceso de almacenar respuesta de validación";
        public const string E203 = "No se encontró la factura solicitada";
        public const string E204 = "fechaRecepcion no cumple con el formato yyyyMMddHHmmss exigido";
        public const string E205 = "El campo estado posee un valor inválido";
        public const string E206 = "Se espera un campo listaMensajes o listaErrores para una validación observada";

        //Deserealización JSON
        public const string E300 = "La cadena JSON de entrada no contiene un tipo de estructura reconocida para este método";
        public const string E301 = "Valor inesperado para el campo 'estado'";

        //Consulta factura a FEEL
        public const string E400 = "ADVERTENCIA! - Se detecto una cantidad distinta de items en FEEL y en CORE en la factura: ";
    }
}
