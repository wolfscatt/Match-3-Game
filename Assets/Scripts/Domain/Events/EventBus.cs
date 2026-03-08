
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3Game.Domain.Events
{
    /// <summary>
    /// Tip-güvenli, allocation-minimal event bus.
    /// Tüm sistemler arası iletişim buradan geçer - direct reference yok.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if(!_handlers.ContainsKey(type))
                _handlers[type] = new List<Delegate>();

            _handlers[type].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if(_handlers.TryGetValue(type, out var list))
                list.Remove(handler);
        }

        public void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if(!_handlers.TryGetValue(type, out var list)) return;

            // Iterate kopyası - publish sırasında unsubscribe güvenli
            var snapshot = new List<Delegate>(list);
            foreach(var handler in snapshot)
                ((Action<T>)handler).Invoke(eventData);
        }
    }

    public interface IEventBus
    {
        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>(Action<T> handler);
        void Publish<T>(T eventData);
    }

}
