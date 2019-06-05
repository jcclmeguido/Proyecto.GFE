using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    /// <summary>
    /// ::C1 -> ResponseAnular_DocumentoFiscal
    /// </summary>
    public class C1
    {
        //Obligatorio:Si
        public Int64 NitEmisor { get; set; }

        //Obligatorio:No
        public string Cuf { get; set; }

        //Obligatorio:No
        public string NumeroFactura { get; set; }

        //Obligatorio:No
        public string IdDocFiscalERP { get; set; }

        //Obligatorio:No
        public string TokenCliente { get; set; }

    }
}
