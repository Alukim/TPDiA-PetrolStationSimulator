using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PetrolStation.Infrastructure
{
    public class KafkaProducer
    {
        private readonly JsonSerializerSettingsProvider jsonSerializerSettingsProvider;
        private readonly KafkaTopicProvider kafkaTopicProvider;
        private readonly IOptions<GlobalSettings> globalSettings;
        private readonly ILogger<KafkaProducer> logger;

        private ISerializer<string> stringSerializer = new StringSerializer(Encoding.UTF8);

        public KafkaProducer(JsonSerializerSettingsProvider jsonSerializerSettingsProvider, KafkaTopicProvider kafkaTopicProvider, IOptions<GlobalSettings> globalSettings, ILogger<KafkaProducer> logger)
        {
            this.jsonSerializerSettingsProvider = jsonSerializerSettingsProvider;
            this.kafkaTopicProvider = kafkaTopicProvider;
            this.globalSettings = globalSettings;
            this.logger = logger;
        }

        public async Task ProduceEvent(IEvent @event)
        {
            var eventEnvelope = MapEventToEnvelope(@event);
            var serializedEvent = SerializeEventEnvelope(eventEnvelope);

            var headers = new Headers { { HeadersValue.HeaderName, stringSerializer.Serialize(string.Empty, @event.GetType().GetFriendlyTypeName()) } };
            var partitionKey = $"A:{globalSettings.Value.ServiceId}";

            var message = new Message<byte[], byte[]>
            {
                Value = stringSerializer.Serialize(kafkaTopicProvider.GetTopicName(), serializedEvent),
                Key = stringSerializer.Serialize(kafkaTopicProvider.GetTopicName(), partitionKey),
                Headers = headers
            };

            logger.LogDebug($"Event produced: {@event.GetType().Name}");

            using (var producer = new Producer<byte[], byte[]>(GetKafkaConfigs(), new ByteArraySerializer(), new ByteArraySerializer()))
            {
                await producer.ProduceAsync(kafkaTopicProvider.GetTopicName(), message);
            }
        }

        private EventEnvelope MapEventToEnvelope(IEvent @event)
        {
            var callContext = new CallContext(globalSettings.Value.ServiceId, globalSettings.Value.ServiceName);
            return new EventEnvelope(callContext, @event);
        }
        private string SerializeEventEnvelope(EventEnvelope eventEnvelope)
        {
            var jsonSerializerSettings = jsonSerializerSettingsProvider.GetJsonSerializerSettings();
            return JsonConvert.SerializeObject(eventEnvelope, jsonSerializerSettings);
        }

        private Dictionary<string, object> GetKafkaConfigs()
            => new Dictionary<string, object>
                {
                    { "bootstrap.servers", "kafka:9092" }
                };

}
}
