using System;

namespace Core
{
    public class Exchange
    {
        public Exchange(string name, string type, bool durable = true)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Durable = durable;
        }

        public string Name { get; }
        public string Type { get; }
        public bool Durable { get; }
    }
}