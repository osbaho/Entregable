using System;
using System.Collections.Generic;

namespace Core.Events
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Action<object>>> eventHandlers = 
            new Dictionary<Type, List<Action<object>>>();

        public static void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!eventHandlers.ContainsKey(type))
            {
                eventHandlers[type] = new List<Action<object>>();
            }
            eventHandlers[type].Add((obj) => handler((T)obj));
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (eventHandlers.ContainsKey(type))
            {
                var handlers = eventHandlers[type];
                handlers.RemoveAll(h => h.Target == handler.Target && h.Method == handler.Method);
            }
        }

        public static void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if (eventHandlers.ContainsKey(type))
            {
                foreach (var handler in eventHandlers[type])
                {
                    handler.Invoke(eventData);
                }
            }
        }

        public static void Clear()
        {
            eventHandlers.Clear();
        }
    }
}
