using System;

namespace Core
{
    public class Queue
    {
        public Queue(string name, string routing, bool durable = true)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Routing = routing ?? throw new ArgumentNullException(nameof(routing));
            Durable = durable;
        }

        public string Name { get; }
        public string Routing { get; }
        public bool Durable { get; }
    }
}