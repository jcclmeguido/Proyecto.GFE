using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Axon.GFE.Mapeadores
{
            public class Publicacion
        {
            [JsonProperty("nitEmisor")] public long NitEmisor { get; set; }
            [JsonProperty("cuf")] public string Cuf { get; set; }
            [JsonProperty("archivo")] public string Archivo { get; set; }

        }
   
}
