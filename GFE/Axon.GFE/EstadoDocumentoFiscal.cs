using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE
{
    public static class EstadoDocumentoFiscal
    {
        //Envío de Facturas
        public const int E100_PendienteDeEnvio = 100;
        public const int E101_EnviadoAlFEEL = 101;
        public const int E102_ProcesadaCorrectamente = 102;
        public const int E103_RespuestaEnviarConError = 103;

        //Anulación de Facturas
        public const int E200_PendienteDeAnular = 200;
        public const int E201_PendienteDeAnularEnviadoAlFEEL = 201;
        public const int E202_AnulacionProcesada = 202;
        public const int E204_AnulacionConError = 204;

        //Validación
        public const int E400_ValidadoOK = 400;
        public const int E401_FacturaObservada = 401;
        public const int E903_Procesada = 903;
        public const int E904_Observada = 904;

        //Consulta
        public const int E500_PendienteDeConsulta = 500;
        public const int E501_ConsultaEnviada = 501;
        public const int E502_RespuestaConsultaError = 502;
        public const int E503_Actualizada = 503;
    }
}
