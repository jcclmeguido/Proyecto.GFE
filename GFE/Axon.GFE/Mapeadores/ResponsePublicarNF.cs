using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Axon.GFE.Mapeadores
{
    public class ResponsePublicarNF
    {
        public RespuestaNF respuesta { get; set; }

        public override string ToString()
        {

            return JObject.FromObject(this).ToString();
        }



    }

    public class RespuestaNF
    {

        public string codRespuesta { get; set; }         //fehfecres
        public string txtRespuesta { get; set; }        //fehfetres


        public override string ToString()
        {

            return JObject.FromObject(this).ToString();
        }
    }
}
