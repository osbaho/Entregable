using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Managers
{
    public class ServiceLocator
    {
        private static ServiceLocator instance;
        public static ServiceLocator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceLocator();
                }
                return instance;
            }
        }

        private Dictionary<Type, object> services = new Dictionary<Type, object>();

        public void RegisterService<T>(T service)
        {
            Type type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type.Name} is already registered.");
                return;
            }
            services.Add(type, service);
        }

        public T GetService<T>()
        {
            Type type = typeof(T);
            if (services.ContainsKey(type))
            {
                return (T)services[type];
            }
            Debug.LogError($"Service of type {type.Name} not found.");
            return default;
        }
    }
}
