﻿using BookStore.Caches.Settings;
using BookStore.MessagePack;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WebAPI.Models;

namespace BookStore.Caches
{
    public class KafkaHostedService<Key, Value>  where Value : ICacheItem<Key>
    {

        private IOptions<KafkaSettingsConsumer> _kafkaSettings;
        public IConsumer<Key, Value> _consumer;
        private ConsumerConfig _consumerConfig;

        public KafkaHostedService(IOptions<KafkaSettingsConsumer> kafkaSettings)
        {

            _kafkaSettings = kafkaSettings;

            _consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = _kafkaSettings.Value.BootstrapServers,
                AutoOffsetReset = (AutoOffsetReset?)_kafkaSettings.Value.AutoOffsetReset,
                GroupId = _kafkaSettings.Value.GroupId
            };

            _consumer = new ConsumerBuilder<Key, Value>(_consumerConfig)
                    .SetKeyDeserializer(new MsgPackDeserializer<Key>())
                    .SetValueDeserializer(new MsgPackDeserializer<Value>())
                    .Build();

            _consumer.Subscribe(typeof(Value).Name);
        }


        public Task<List<Value>> StartAsync(List<Value> list ,CancellationToken cancellationToken)
        {

                while (!cancellationToken.IsCancellationRequested)
                {
                    var cr = _consumer.Consume();

                    list.Add(cr.Value); 
                    Console.WriteLine($"Recieved msg with key:{cr.Value.GetKey()} value:{cr.Value}");
                };

            return Task.FromResult(list); //Tsak.Completed
        }
    }
}
