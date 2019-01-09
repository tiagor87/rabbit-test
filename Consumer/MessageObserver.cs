using System;
using System.Threading.Tasks;
using Core;
using RabbitMQ.Client;

namespace Consumer
{
    public class MessageObserver : RabbitObserver<MessageDto>
    {
        public MessageObserver() : base(new Exchange("com.rabbit-test", ExchangeType.Topic),
            new Queue("com.rabbit-test.message", "com.rabbit-test.#"), 1)
        {
        }

        protected override Task Next(MessageDto message)
        {
            Console.WriteLine($@"Message ""{message.Message}"" received.");
            return Task.CompletedTask;
        }
    }
}