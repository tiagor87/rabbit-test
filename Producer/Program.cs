using System;
using Core;
using RabbitMQ.Client;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new RabbitConnection("amqp://guest:guest@localhost:5672"))
            {
                var i = 0;
                while (true)
                {
                    i++;
                    Console.WriteLine("Press ENTER to send message.");
                    Console.ReadLine();
                    var obj = new
                    {
                        Message = $"Message {i}"
                    };

                    connection.Publish(new Exchange("com.rabbit-test", ExchangeType.Topic),
                        new Queue("com.rabbit-test.message", "com.rabbit-test.#"), RabbitMessage.AddJson(obj));

                    Console.WriteLine($@"Message ""{obj.Message}"" sent.");
                }
            }
        }
    }
}