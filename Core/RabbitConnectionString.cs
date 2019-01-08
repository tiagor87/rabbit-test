using System;

namespace Core
{
    public class RabbitConnectionString
    {
        public RabbitConnectionString(string connectionString)
        {
            Uri = new Uri(connectionString ?? throw new ArgumentNullException(nameof(connectionString)));
        }

        public Uri Uri { get; }

        public static implicit operator RabbitConnectionString(string connectionString)
        {
            return new RabbitConnectionString(connectionString);
        }
    }
}