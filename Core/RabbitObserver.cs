using System;
using System.Threading.Tasks;

namespace Core
{
    public abstract class RabbitObserver<T> : IRabbitObserver, IDisposable
    {
        private bool _disposed;
        private Action<IRabbitObserver> _unsubscribe;

        public RabbitObserver(Exchange exchange, Queue queue, ushort prefetchCount = 1)
        {
            Type = typeof(T);
            Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            Queue = queue ?? throw new ArgumentNullException(nameof(queue));
            PrefetchCount = prefetchCount;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Exchange Exchange { get; }
        public Queue Queue { get; }
        public ushort PrefetchCount { get; }
        public Type Type { get; }

        public Task Next(object message)
        {
            return Next((T) message);
        }

        public void Unsubscribe()
        {
            _unsubscribe?.Invoke(this);
        }

        protected abstract Task Next(T message);

        ~RabbitObserver()
        {
            Dispose(false);
        }

        internal void SetUnsubscribe(Action<IRabbitObserver> @delegate)
        {
            _unsubscribe = @delegate;
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Unsubscribe();
            }

            _disposed = true;
        }
    }
}