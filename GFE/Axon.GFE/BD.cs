using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE
{
    public sealed class BD
    {
        private static readonly Lazy<BD> lazy = new Lazy<BD>(() => new BD());

        public static BD Instancia { get { return lazy.Value; } }

        readonly string[] _camposFacturaCabeceraConsolidado;
        public string[] CamposFacturaCabeceraConsolidado { get { return _camposFacturaCabeceraConsolidado; } }

        private BD()
        {
            _camposFacturaCabeceraConsolidado = new string[]{
                "fehfeiddf",
                "fehfenfac",
                "fehfedire",
                "fehfeciud",
                "fehfezona",
                "fehfenmed",
                "fehfefemi",
                "fehfegest",
                "fehfemmes",
                "fehfectdi",
                "fehfeccuf",
                "fehfendoc",
                "fehfecomp",
                "fehfecsuc",
                "fehfecpve",
                "fehfenest",
                "fehfersoc",
                "fehfeteve",
                "fehfeleve",
                "fehfefeve",
                "fehfeaeve",
                "fehfensal",
                "fehfedsal",
                "fehfecmon",
                "fehfemser",
                "fehfedoco",
                "fehfedico",
                "fehfenpre",
                "fehfemtot",
                "fehfemtaf",
                "fehfemled",
                "fehfeccli",
                "fehfel317",
                "fehfemtsi",
                "fehfemtmo",
                "fehfetcam",
                "fehfemtoj",
                "fehfemtsl",
                "fehfecdse",
                "fehfenemi",
                "fehfecmpa",
                "fehfemdes",
                "fehfeleye",
                "fehfeusua",
                "fehfentar",
                "fehfepfac",
                "fehfecpai",
                "fehfepveh",
                "fehfetenv",
                "fehfemice",
                "fehfenpro",
                "fehfenrle",
                "fehfecpag",
                "fehfepent",
                "fehfechue",
                "fehfechab",
                "fehfecmay",
                "fehfecmen",
                "fehfefiho",
                "fehfenotu",
                "fehfersot",
                "fehfeckwh",
                "fehfecmcu",
                "fehfedley",
                "fehfetase",
                "fehfetalu",
                "fehfeidca",
                "fehfemtpu",
                "fehfeomon",
                "fehfeinco",
                "fehfepdes",
                "fehfeldes",
                "fehfepvbr",
                "fehfegtfr",
                "fehfesfro",
                "fehfetffr",
                "fehfemtfr",
                "fehfemsin",
                "fehferemi",
                "fehfecons",
                "fehfelapu",
                "fehfeidbd",
                "fehfemrcb",
                "fehfestat",
                "fehfecres",
                "fehfetres",
                "fehfecufd",
                "fehfecont",
                "fehfelote",
                "fehfeidlo",
                "fehfeufac",
                "fehfectip"
                 };
        }


        
        
    }
}
