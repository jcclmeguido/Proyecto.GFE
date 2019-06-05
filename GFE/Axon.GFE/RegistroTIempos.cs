using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE
{
    public sealed class RegistroTiempos
    {
        private static readonly Lazy<RegistroTiempos> lazy = new Lazy<RegistroTiempos>(() => new RegistroTiempos());

        public static RegistroTiempos Instancia { get { return lazy.Value; } }

        private RegistroTiempos() { }

        public string RegGFE { get; set; }
        public string RegFEEL { get; set; }

        long totalgfe;
        public long TotalGFE { get => totalgfe; }
        long totalfeel;
        public long TotalFEEL { get => totalfeel; }
        

        DateTime dtIni;

        public void IniciarRegistros() {
            dtIni = DateTime.Now;
            RegGFE = RegFEEL = "INICIO: " + dtIni.ToString("HH:mm:ss") + Environment.NewLine;
        }

        public void AddTiempoGFE(long t)
        {
            totalgfe += t;
            RegGFE += t.ToString() + Environment.NewLine;
        }

        public void AddTiempoFEEL(long t)
        {
            totalfeel += t;
            RegFEEL += t.ToString() + Environment.NewLine;
        }

        public void GuardarRegistros() {
            TimeSpan ts = DateTime.Now - dtIni;
            TimeSpan tgfe = TimeSpan.FromMilliseconds(TotalGFE);
            TimeSpan tfeel = TimeSpan.FromMilliseconds(TotalFEEL);
            string fin = "FIN: " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine +
                "TIEMPO TRANSCURRIDO: " + string.Format("{0}:{1:00}", (int)ts.TotalMinutes, ts.Seconds) + Environment.NewLine;
            RegGFE += fin + "TOTAL GFE: " + TotalGFE + " = " + string.Format("{0}:{1:00}", (int)tgfe.TotalMinutes, tgfe.Seconds);
            RegFEEL += fin + "TOTAL FEEL: " + TotalFEEL + " = " + string.Format("{0}:{1:00}", (int)tfeel.TotalMinutes, tfeel.Seconds);

            using (StreamWriter swGFE = File.AppendText(ConfigurationManager.AppSettings["RutaFacturacionElectronicaGFE"] + "GFE-" + dtIni.ToString("yyyyMMddHHmmss") + ".txt")) {
                swGFE.Write(RegGFE);
                RegGFE = string.Empty;
            }
            using (StreamWriter swFEEL = File.AppendText(ConfigurationManager.AppSettings["RutaFacturacionElectronicaGFE"] + "FEEL-" + dtIni.ToString("yyyyMMddHHmmss")+".txt"))
            {
                swFEEL.Write(RegFEEL);
                RegFEEL = string.Empty;
            }
        }
    }
}
