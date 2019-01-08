using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Newtonsoft.Json;

namespace Core
{
    public enum DeliveryMode
    {
        NonPersistent = 1,
        Persistent = 2
    }

    public class RabbitMessage
    {
        private readonly IDictionary<string, object> _headers;

        public RabbitMessage(byte[] message, Type type, string contentType, DeliveryMode deliveryMode,
            KeyValuePair<string, object>[] headers)
        {
            Message = message;
            Type = type?.AssemblyQualifiedName;
            ContentType = contentType;
            Persistent = (byte) deliveryMode;
            _headers = new Dictionary<string, object>(headers);
        }

        public byte[] Message { get; }
        public string ContentType { get; }
        public byte Persistent { get; }
        public string Type { get; }

        public IReadOnlyDictionary<string, object> Headers
        {
            get => new ReadOnlyDictionary<string, object>(_headers);
        }

        public static RabbitMessage AddJson(object obj, DeliveryMode deliveryMode = DeliveryMode.Persistent,
            params KeyValuePair<string, object>[] headers)
        {
            var json = JsonConvert.SerializeObject(obj);
            var message = Encoding.UTF8.GetBytes(json);
            return new RabbitMessage(message, obj.GetType(), "application/json", deliveryMode, headers);
        }

        public void AddHeader(string key, object value) => _headers.Add(key, value);
        public void RemoveHeader(string key) => _headers.Remove(key);
    }
}