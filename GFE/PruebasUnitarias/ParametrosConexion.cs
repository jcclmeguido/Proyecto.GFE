using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasUnitarias
{
    class ParametrosConexion
    {
        private static string sDATABASE = "";				//Nombre de la base de datos "tbsai"
        private static string sSERVERNAME = "";	            //Nombre del servidor
        private static string sLogin = "";					//Login al servidor unix,etc.
        private static string sPassword = "";				//Password al servidor unix,etc.

        private static string sStringConn = "";			    //cadena de conexion
        private static string sUser = "";                   //Usuario
        private static string sClav = "";                   //Clave

        private static string sIntervalo = "";              //Intervalo en minutos
        private static string sHorasejecucionsw = "";       //Horas ejecucion

        public ParametrosConexion()
        {
        }
        public ParametrosConexion(string _sDATABASE, string _sSERVERNAME,
                          string _sUser, string _sClav, string _sIntervalo,
                          string _sHorasEjecucionsw)
        {
            sDATABASE = _sDATABASE;
            sSERVERNAME = _sSERVERNAME;
            sUser = _sUser;
            sClav = _sClav;
            sIntervalo = _sIntervalo;
            sHorasejecucionsw = _sHorasEjecucionsw;
        }
        public static string Horasejecucionsw
        {
            get
            {
                return sHorasejecucionsw;
            }
            set
            {
                sHorasejecucionsw = value;
            }
        }
        public static string Intervalo
        {
            get
            {
                return sIntervalo;
            }
            set
            {
                sIntervalo = value;
            }
        }
        public static string DATABASE
        {
            get
            {
                return sDATABASE;
            }
            set
            {
                sDATABASE = value;
            }
        }
        public static string Login
        {
            get
            {
                return sLogin;
            }
            set
            {
                sLogin = value;
            }
        }
        public static string Password
        {
            get
            {
                return sPassword;
            }
            set
            {
                sPassword = value;
            }
        }
        public static string SERVERNAME
        {
            get
            {
                return sSERVERNAME;
            }
            set
            {
                sSERVERNAME = value;
            }
        }
        public static string User
        {
            get
            {
                return sUser;
            }
            set
            {
                sUser = value;
            }
        }
        public static string Clav
        {
            get
            {
                return sClav;
            }
            set
            {
                sClav = value;
            }
        }
        public static string StringConn
        {
            get
            {
                return sStringConn;
            }
            set
            {
                sStringConn = value;
            }
        }
        public static void ArmarConexion()
        {
            sStringConn = "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=" + sLogin + ";Password=" + sPassword + ";Initial Catalog=" + sDATABASE + ";Data Source=" + sSERVERNAME + ";Use Procedure for Prepare=1;Auto Translate=True;Packet Size=4096;Workstation ID=" + sSERVERNAME + ";Use Encryption for Data=False;Tag with column collation when possible=False";
        }
    }
}
