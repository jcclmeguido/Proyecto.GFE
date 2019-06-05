using Axon.GFE.Mapeadores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE
{
    public static class FabricaFactura
    {
        public static Factura Fabricar(TipoFactura tipo) {
#warning Completar este switch
            switch (tipo)
            {
                case TipoFactura.facturaEstandar: return new FacturaEstandar();
                case TipoFactura.facturaColegio: return new FacturaColegio();
                case TipoFactura.facturaAlquiler: return new FacturaAlquiler();
                case TipoFactura.facturaCombustible: return new FacturaCombustible();
                case TipoFactura.facturaServicios: return new FacturaServiciosBasicos();
                case TipoFactura.facturaEmbotelladoras: return new FacturaEmbotelladora();
                case TipoFactura.facturaBancos: return new FacturaBanco();
                case TipoFactura.facturaHoteles: return new FacturaHotel();
                case TipoFactura.facturaHospitales: return new FacturaHospital();
                case TipoFactura.facturaJuegos: return new FacturaJuego();
                case TipoFactura.facturaEspectaculoPublicoInternacional: return new FacturaEspectaculoPublicoInternacional();
                case TipoFactura.notaExportacion: return new NotaExportacion();
                case TipoFactura.notaLibreConsignacion: return new NotaLibreConsignacion();
                case TipoFactura.notaZonaFranca: return new NotaZonaFranca();
                case TipoFactura.notaEspectaculoPublicoNacional: return new NotaEspectaculoPublicoNacional();
                case TipoFactura.notaSeguridadAlimentaria: return new NotaSeguridadAlimentaria();
                case TipoFactura.notaMonedaExtranjera: return new NotaMonedaExtranjera();
                //case TipoFactura.NOTA_DE_CREDITO_DEBITO:
                //    break;
                //case TipoFactura.NOTA_DE_CONCILIACION:
                //    break;
                //case TipoFactura.BOLETO_AEREO:
                //    break;
                case TipoFactura.notaTurismoReceptivo: return new NotaTurismoReceptivo();
                case TipoFactura.notaTasaCero: return new Factura();   //sin campos particulares
                case TipoFactura.facturaHidrocarburos: return new FacturaHidrocarburos();
            }
            return null;
        }

        public static Detalle FabricarDetalle(TipoFactura tipo) {
#warning Completar este switch
            switch (tipo)
            {
                case TipoFactura.facturaEstandar: return new DetalleFacturaEstandar();
                case TipoFactura.facturaColegio: return new DetalleFacturaColegio();
                case TipoFactura.facturaAlquiler: return new DetalleFacturaAlquiler();
                case TipoFactura.facturaCombustible: return new DetalleFacturaCombustible();
                case TipoFactura.facturaServicios: return new DetalleFacturaServiciosBasicos();
                case TipoFactura.facturaEmbotelladoras: return new DetalleFacturaEmbotelladora();
                case TipoFactura.facturaBancos: return new DetalleFacturaBanco();
                case TipoFactura.facturaHoteles: return new DetalleFacturaHotel();
                case TipoFactura.facturaHospitales: return new DetalleFacturaHospital();
                case TipoFactura.facturaJuegos: return new DetalleFacturaJuego();
                case TipoFactura.facturaEspectaculoPublicoInternacional: return new DetalleFacturaEspectaculoPublicoInternacional();
                case TipoFactura.notaExportacion: return new DetalleNotaExportacion();
                case TipoFactura.notaLibreConsignacion: return new DetalleNotaLibreConsignacion();
                case TipoFactura.notaZonaFranca: return new DetalleNotaZonaFranca();
                case TipoFactura.notaEspectaculoPublicoNacional: return new DetalleFacturaEspectaculoPublicoInternacional();
                case TipoFactura.notaSeguridadAlimentaria: return new DetalleNotaSeguridadAlimentaria();
                case TipoFactura.notaMonedaExtranjera: return new DetalleNotaMonedaExtranjera();
                //case TipoFactura.NOTA_DE_CREDITO_DEBITO:
                //    break;
                //case TipoFactura.NOTA_DE_CONCILIACION:
                //    break;
                //case TipoFactura.BOLETO_AEREO:
                //    break;
                case TipoFactura.notaTurismoReceptivo: return new DetalleNotaTurismoReceptivo();
                case TipoFactura.notaTasaCero: return new Detalle();   //sin campos particulares
                case TipoFactura.facturaHidrocarburos: return new DetalleFacturaHidrocarburos();
            }
            return null;
        }
    }
}
