using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Axon.WinSerAppCensador
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                ServiceCensador servicioCensador = new ServiceCensador();
                //ServiceCensadorsecuencial servicioCensador = new ServiceCensadorsecuencial();
                servicioCensador.TestStartupAndStop(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new ServiceCensador()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
