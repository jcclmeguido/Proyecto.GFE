using Axon.GFE.Mapeadores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axon.GFE
{
    public sealed class Cronometro
    {
        private static readonly Lazy<Cronometro> lazy = new Lazy<Cronometro>(() => new Cronometro());

        public static Cronometro Instancia { get { return lazy.Value; } }

        private Cronometro()
        {
            _cronometro = new Stopwatch();
            TotalMs = 0;
        }

        Stopwatch _cronometro;

        public long TotalMs { get; set; }
        public void Iniciar() {
            if (!_cronometro.IsRunning) {
                _cronometro.Start();
            }
        }

        public TimeSpan Detener() {
            if (_cronometro.IsRunning) _cronometro.Stop();
            return _cronometro.Elapsed;
        }


        public void Reiniciar() {
            _cronometro = new Stopwatch();
            _cronometro.Start();
        }

        public ResponseFactura SimulacionFEEL(RequestFactura request) {
           // Thread.Sleep(500);
            request.Factura.Cabecera.CUF = "CUFprueba";
            ResponseFactura response = new ResponseFactura()
            {
                Factura = request.Factura,
                Proceso = new Proceso()
                {
                    CodEstado = "123",
                    CodigoTipoFactura = (int)request.Factura.Tipo,
                    CUFD = "CUFDPRUEBA",
                    IdDocFiscalERP = request.IdDocFiscalERP,
                    IdDocFiscalFEEL = request.IdDocFiscalERP
                },
                Respuesta = new Respuesta()
                {
                    CodRespuesta = "0",
                    TxtRespuesta = "Exito"
                }
            };
            return response;
        }
    }
}
