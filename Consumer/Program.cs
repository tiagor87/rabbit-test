using System;
using System.Threading.Tasks;
using Core;
using RabbitMQ.Client;

namespace Consumer
{
    public class MessageDto
    {
        public string Message { get; set; }
    }

    public class MessageSubscription : RabbitSubscription<MessageDto>
    {
        public MessageSubscription() : base(new Exchange("com.rabbit-test", ExchangeType.Topic),
            new Queue("com.rabbit-test.message", "com.rabbit-test.#"), 1)
        {
        }

        protected override Task Handle(MessageDto message)
        {
            Console.WriteLine($@"Message ""{message.Message}"" received.");
            return Task.CompletedTask;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new RabbitConnection("amqp://guest:guest@localhost:5672"))
            {
                connection.Subscribe(new MessageSubscription());
                while (true)
                {
                    Console.WriteLine("Press ENTER to read messages.");
                    Console.ReadLine();
//                    Thread.Sleep(10000);
//                    var @event2 = subscription2.Next();
//                    var json2 = Encoding.UTF8.GetString(@event2.Body);
//                    var obj2 = JsonConvert.DeserializeObject<MessageDto>(json2);
//                    Console.WriteLine($@"Message ""{obj2.Message}"" received.");
//                    subscription.Ack();
//                    subscription2.Ack();
                }
            }
        }
    }
}