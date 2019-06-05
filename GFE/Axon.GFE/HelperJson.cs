using Axon.GFE.Mapeadores;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Axon.GFE
{
    public static class HelperJson {
        public static string IgnorarPropiedades(Type tipoMapeador, object objetoMapeado, params string[] propiedadesJson)
        {
            var jsonResolver = new IgnorarRenombrarPropiedadJson();
            jsonResolver.IgnorarPropiedades(tipoMapeador, propiedadesJson);
            var jsonSerializer = new JsonSerializerSettings();
            jsonSerializer.ContractResolver = jsonResolver;
            return JsonConvert.SerializeObject(objetoMapeado, jsonSerializer);
        }

        public static string RenombrarPropiedad(Type tipoMapeador, object objetoMapeado, string propiedad, string nuevoNombre) {
            var jsonResolver = new IgnorarRenombrarPropiedadJson();
            jsonResolver.RenombrarPropiedad(tipoMapeador, propiedad, nuevoNombre);
            var jsonSerializer = new JsonSerializerSettings();
            jsonSerializer.ContractResolver = jsonResolver;
            return JsonConvert.SerializeObject(objetoMapeado, jsonSerializer);
        }
    }

    public class IgnorarRenombrarPropiedadJson : DefaultContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> _ignorados;
        private readonly Dictionary<Type, Dictionary<string, string>> _renombrados;

        public IgnorarRenombrarPropiedadJson()
        {
            _ignorados = new Dictionary<Type, HashSet<string>>();
            _renombrados = new Dictionary<Type, Dictionary<string, string>>();
        }

        public void IgnorarPropiedades(Type tipo, params string[] propiedadesJson)
        {
            if (!_ignorados.ContainsKey(tipo))
                _ignorados[tipo] = new HashSet<string>();

            foreach (var prop in propiedadesJson)
                _ignorados[tipo].Add(prop);
        }

        public void RenombrarPropiedad(Type tipo, string nombrePropiedad, string nuevoNombrePropiedad)
        {
            if (!_renombrados.ContainsKey(tipo))
                _renombrados[tipo] = new Dictionary<string, string>();

            _renombrados[tipo][nombrePropiedad] = nuevoNombrePropiedad;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var propiedad = base.CreateProperty(member, memberSerialization);

            if (EsIgnorado(propiedad.DeclaringType, propiedad.PropertyName))
            {
                propiedad.ShouldSerialize = i => false;
                propiedad.Ignored = true;
            }

            if (EsRenombrado(propiedad.DeclaringType, propiedad.PropertyName, out var newJsonPropertyName))
                propiedad.PropertyName = newJsonPropertyName;

            return propiedad;
        }

        private bool EsIgnorado(Type tipo, string nombrePropiedadJson)
        {
            if (!_ignorados.ContainsKey(tipo))
                return false;

            return _ignorados[tipo].Contains(nombrePropiedadJson);
        }

        private bool EsRenombrado(Type tipo, string nombrePropiedadJson, out string nuevoNombrePropiedadJson)
        {
            Dictionary<string, string> renames;

            if (!_renombrados.TryGetValue(tipo, out renames) || !renames.TryGetValue(nombrePropiedadJson, out nuevoNombrePropiedadJson))
            {
                nuevoNombrePropiedadJson = null;
                return false;
            }

            return true;
        }
    }
}
