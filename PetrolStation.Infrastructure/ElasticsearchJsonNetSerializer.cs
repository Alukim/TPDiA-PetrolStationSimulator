using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace PetrolStation.Infrastructure
{
    public class ElasticsearchJsonNetSerializer : JsonNetSerializer
    {
        public ElasticsearchJsonNetSerializer(IConnectionSettingsValues settings) : base(settings) { }

        protected override IList<Func<Type, JsonConverter>> ContractConverters =>
            new List<Func<Type, JsonConverter>>
    {
        type =>
        {
            return type == typeof(ExpandoObject)
                ? new CamelcaseExpandoObjectConverter()
                : null;
        }
    };
    }

    internal class CamelcaseExpandoObjectConverter : ExpandoObjectConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            base.WriteJson(writer, value, serializer);
        }
    }
}
