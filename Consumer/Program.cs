using System;
using Core;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new RabbitConnection("amqp://guest:guest@localhost:5672"))
            {
                connection.Subscribe(new MessageObserver());
                Console.ReadLine();
            }
        }
    }
}