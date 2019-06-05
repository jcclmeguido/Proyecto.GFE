using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE.Mapeadores
{
    //Es igual al internacional

    public class NotaEspectaculoPublicoNacional : FacturaEspectaculoPublicoInternacional
    {
        public NotaEspectaculoPublicoNacional():base() {
            Tipo = TipoFactura.notaEspectaculoPublicoNacional;
        }
    }
}
