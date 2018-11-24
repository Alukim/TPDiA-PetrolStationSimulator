using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PetrolStation.Infrastructure
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly ILogger<KafkaConsumer> logger;
        private readonly KafkaTopicProvider kafkaTopicProvider;
        private readonly EventDispatcher eventDispatcher;
        private readonly JsonSerializerSettingsProvider jsonSerializerSettingsProvider;

        private Consumer<byte[], byte[]> consumer;

        private IDeserializer<string> stringDeserializer = new StringDeserializer(Encoding.UTF8);

        public KafkaConsumer(ILogger<KafkaConsumer> logger, KafkaTopicProvider kafkaTopicProvider, EventDispatcher eventDispatcher, JsonSerializerSettingsProvider jsonSerializerSettingsProvider)
        {
            this.logger = logger;
            this.kafkaTopicProvider = kafkaTopicProvider;
            this.eventDispatcher = eventDispatcher;
            this.jsonSerializerSettingsProvider = jsonSerializerSettingsProvider;

            consumer = new Consumer<byte[], byte[]>(config, new ByteArrayDeserializer(), new ByteArrayDeserializer());
            consumer.OnPartitionsAssigned += OnPartitionsAssigned;
            consumer.OnPartitionsRevoked += OnPartitionsRevoked;

            ResubscribeToTopics();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogDebug("Kafka consumer. Message poll working. Waiting for messages.");

            var consumeTimeout = TimeSpan.FromMilliseconds(100);
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if(consumer.Consume(out var message, consumeTimeout))
                    {
                        await HandleMessage(message);
                        consumer.Commit(message);
                    }
                }
                catch(Exception ex)
                {
                    ResubscribeToTopics();
                }
            }

            logger.LogDebug("Kafka consumer exited.");

            StopAndCleanUpConsumer();
        }

        private async Task HandleMessage(ConsumerRecord<byte[], byte[]> message)
        {
            var value = stringDeserializer.Deserialize(message.Topic, message.Value);
            var eventEnvelope = JsonConvert.DeserializeObject<EventEnvelope>(value, jsonSerializerSettingsProvider.GetJsonSerializerSettings());
            if (eventEnvelope != null)
            {
                try
                {
                    await eventDispatcher.Dispatch(eventEnvelope);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Error during event processing of event {0}. {1}", eventEnvelope.Event.GetType().Name, exception.Message);
                }
            }
        }

        private void ResubscribeToTopics()
        {
            consumer.Subscribe(kafkaTopicProvider.GetTopicName());

            logger.LogDebug($"Kafka Consumer. Subscribed to: [{string.Join(", ", consumer.Subscription)}]");
        }

        private void OnPartitionsRevoked(object _, List<TopicPartition> partitions)
        {
            logger.LogDebug($"Kafka Consumer. Revoked partitions: [{string.Join(", ", partitions)}]");
            consumer.Unassign();
        }

        private void OnPartitionsAssigned(object _, List<TopicPartition> partitions)
        {
            logger.LogDebug($"Kafka Consumer. Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
            consumer.Assign(partitions);
        }

        private void StopAndCleanUpConsumer()
        {
            logger.LogDebug("Kafka Consumer stopping!");

            if (consumer != null)
            {
                consumer.OnPartitionsAssigned -= OnPartitionsAssigned;
                consumer.OnPartitionsRevoked -= OnPartitionsRevoked;

                TryDisposeConsumer();

                consumer = null;
            }
        }

        private void TryDisposeConsumer()
        {
            try
            {
                consumer?.Unassign();
                consumer?.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Kafka Consumer. Failed to dispose Kafka consumer internal ({0}). {1}", ex.GetType().Name, ex.Message);
            }
        }

        private Dictionary<string, object> config = new Dictionary<string, object>
            {
                { "group.id", "sample-consumer" },
                { "bootstrap.servers", "kafka:9092" },
                { "enable.auto.commit", true },
                { "auto.commit.interval.ms", 5000 },
                { "enable.auto.offset.store", false },
                {
                    "default.topic.config", new Dictionary<string, object>
                    {
                        {"auto.offset.reset", "smallest"},
                    }
                },
            };
    }
}
