using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Core
{
    public class RabbitConnection : IDisposable
    {
        private static volatile object sync = new object();
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IDictionary<Queue, EventingBasicConsumer> _consumers;
        private readonly IDictionary<Queue, List<IRabbitSubscription>> _subscriptions;
        private bool _disposed;


        public RabbitConnection(RabbitConnectionString connectionString)
        {
            var factory = new ConnectionFactory
            {
                AutomaticRecoveryEnabled = true, Uri = connectionString.Uri
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _consumers = new ConcurrentDictionary<Queue, EventingBasicConsumer>();
            _subscriptions = new ConcurrentDictionary<Queue, List<IRabbitSubscription>>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Publish(Exchange exchange, Queue queue, RabbitMessage message)
        {
            DeclareExchangeAndQueue(exchange, queue);

            var props = _channel.CreateBasicProperties();
            props.ContentType = message.ContentType;
            props.DeliveryMode = message.Persistent;
            props.Type = message.Type;
            foreach (var header in message.Headers)
            {
                props.Headers.Add(header);
            }

            _channel.BasicPublish(exchange.Name, queue.Routing, props, message.Message);
        }

        public void Subscribe<T>(RabbitSubscription<T> subscription) where T : class
        {
            DeclareExchangeAndQueue(subscription.Exchange, subscription.Queue);

            List<IRabbitSubscription> subscriptions;
            lock (sync)
            {
                if (_subscriptions.TryGetValue(subscription.Queue, out subscriptions))
                {
                    subscriptions.Add(subscription);
                }
                else
                {
                    subscriptions = new List<IRabbitSubscription>();
                    subscriptions.Add(subscription);
                    _subscriptions.Add(subscription.Queue, subscriptions);
                }
            }

            subscription.SetUnsubscribe(Unsubscribe);

            lock (sync)
            {
                if (_consumers.ContainsKey(subscription.Queue) == false)
                {
                    var consumer = new EventingBasicConsumer(_channel);
                    consumer.Received += async (model, @event) =>
                    {
                        var json = Encoding.UTF8.GetString(@event.Body);
                        // ToDo: Remove subscription.Type dependency
                        var obj = JsonConvert.DeserializeObject(json, subscription.Type);
                        var tasks = subscriptions.Select(s => s.Handle(obj));
                        try
                        {
                            await Task.WhenAll(tasks);
                            _channel.BasicAck(@event.DeliveryTag, false);
                        }
                        catch
                        {
                            _channel.BasicNack(@event.DeliveryTag, false, true);
                        }
                    };
                    _consumers.Add(subscription.Queue, consumer);
                    _channel.BasicConsume(subscription.Queue.Name, false, consumer);
                }
            }
        }

        ~RabbitConnection()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _channel.Close();
                _connection.Close();
                _channel.Dispose();
                _connection.Dispose();
            }

            _disposed = true;
        }

        private void DeclareExchangeAndQueue(Exchange exchange, Queue queue)
        {
            _channel.ExchangeDeclare(exchange.Name, exchange.Type);
            _channel.QueueDeclare(queue.Name, queue.Durable, false, false, null);
            _channel.QueueBind(queue.Name, exchange.Name, queue.Routing);
        }

        private void Unsubscribe(IRabbitSubscription subscription)
        {
            lock (sync)
            {
                if (_subscriptions.TryGetValue(subscription.Queue, out var subscriptions))
                {
                    subscriptions.Remove(subscription);
                }
            }
        }
    }
}