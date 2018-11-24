using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PetrolStation.Infrastructure
{
    public class JsonSerializerSettingsProvider
    {
        private readonly JsonSerializerSettings jsonSerializerSettings;

        public JsonSerializerSettingsProvider()
        {
            this.jsonSerializerSettings = new JsonSerializerSettings();
            this.jsonSerializerSettings.Converters.Add(new StringEnumConverter());
            this.jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
        }

        public JsonSerializerSettings GetJsonSerializerSettings()
            => jsonSerializerSettings;
    }
}
