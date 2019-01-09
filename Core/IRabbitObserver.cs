using System;
using System.Threading.Tasks;

namespace Core
{
    internal interface IRabbitObserver
    {
        Exchange Exchange { get; }
        Queue Queue { get; }
        ushort PrefetchCount { get; }
        Type Type { get; }

        Task Next(object message);

        void Unsubscribe();
    }
}