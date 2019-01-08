using System;
using System.Threading.Tasks;

namespace Core
{
    internal interface IRabbitSubscription
    {
        Exchange Exchange { get; }
        Queue Queue { get; }
        ushort PrefetchCount { get; }
        Type Type { get; }

        Task Handle(object message);

        void Unsubscribe();
    }

    public abstract class RabbitSubscription<T> : IRabbitSubscription
    {
        private Action<IRabbitSubscription> _unsubscribe;

        public RabbitSubscription(Exchange exchange, Queue queue, ushort prefetchCount = 1)
        {
            Type = typeof(T);
            Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            Queue = queue ?? throw new ArgumentNullException(nameof(queue));
            PrefetchCount = prefetchCount;
        }

        public Exchange Exchange { get; }
        public Queue Queue { get; }
        public ushort PrefetchCount { get; }
        public Type Type { get; }

        public Task Handle(object message)
        {
            return Handle((T) message);
        }

        public void Unsubscribe()
        {
            _unsubscribe?.Invoke(this);
        }

        protected abstract Task Handle(T message);

        internal void SetUnsubscribe(Action<IRabbitSubscription> @delegate)
        {
            _unsubscribe = @delegate;
        }
    }
}