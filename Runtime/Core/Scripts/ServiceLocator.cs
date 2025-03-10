using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core
{
    public interface IService
    {
        UniTask Initialize();
        UniTask PostInitialize();
    }

    public static class ServiceLocator
    {
        private static Dictionary<Type, IService> services = new();

        public static void Register<T>(IService service)
        {
            Debug.Assert(service != null);
            services.Add(typeof(T), service);
        }
        
        public static T Get<T>()
        {
            if (services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            return default;
        }

        public static IEnumerable<IService> GetAll()
        {
            foreach (var keyValuePair in services)
            {
                yield return keyValuePair.Value;
            }
        }
        
        public static void ForEach(Action<IService> action)
        {
            foreach (var keyValuePair in services)
            {
                action(keyValuePair.Value);
            }
        }

    }
}