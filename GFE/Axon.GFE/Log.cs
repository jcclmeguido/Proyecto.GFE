using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE
{
    public sealed class Log
    {
        private static readonly Lazy<Log> lazy = new Lazy<Log>(() => new Log());

        public static Log Instancia { get { return lazy.Value; } }

        private Log() { }

        public void LogWS_Mensaje_FSX(string file, string datos)
        {
#warning Descomentar para producción
            /*
            string path = ConfigurationManager.AppSettings["RutaFacturacionElectronicaGFE"].ToString();
            //string file = "MENSAJE-" + DateTime.Now.ToString("ddMMyyyy") + "-" + valor.ToString();
            string pathFile = path + "\\" + file + ".txt";

            if (!Directory.Exists(pathFile))
            {
                //El directorio se creo
                DirectoryInfo directoryInfo = Directory.CreateDirectory(path);
            }

            string datosCompletos = DateTime.Now.ToString("HH:mm:ss") + "|" + datos;
            byte[] datosArray = Encoding.UTF8.GetBytes(datosCompletos);

            try
            {
                using (FileStream fs = new FileStream(pathFile, FileMode.Append, FileAccess.Write))
                {
                    fs.Write(datosArray, 0, datosArray.Length);
                    //fs.Close();
                }
            }
            catch (Exception exe)
            {
                throw new Exception(exe.Message);
            }
            finally
            {

            
    */
        }

        public void ErrorRestService(string metodo, string msj)
        {
            string errordb = AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + "\\error" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            using (StreamWriter sw = File.AppendText(errordb))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "|" + metodo + "|" + msj);
            }
        }

        public string GeneraNombreLog()
        {
            Random random = new Random();
            int minValue = 0;
            int maxValue = 100000000;
            int valor = random.Next(minValue, maxValue);

            string file = DateTime.Now.ToString("yyyyMMdd") + "-" + valor + "-";
            return file;
        }

    }
}
