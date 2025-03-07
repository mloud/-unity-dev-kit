using System;
using Cysharp.Threading.Tasks;
using OneDay.Core.Debugging;
using UnityEngine;

namespace OneDay.Core.Modules.Analytics
{
    public interface IRemoteConfigManager
    {
        void SetRemoteConfigServiceServices(IRemoteConfigService service);
        UniTask<T> Fetch<T>(string key);
    }

    public interface IRemoteConfigService
    {
        bool IsInitialized { get; }
        UniTask Initialize();
        
        UniTask<T> Fetch<T>(string key);
    }
    
    [LogSection("RemoteConfig")]
    public class RemoteConfigManager : MonoBehaviour, IService, IRemoteConfigManager
    {
        private IRemoteConfigService service;
        public UniTask Initialize()
        {
            return UniTask.CompletedTask;
        }

        public void SetRemoteConfigServiceServices(IRemoteConfigService service)
        {
            this.service = service;
            this.service.Initialize();
        }

        public UniTask<T> Fetch<T>(string key)
        {
            if (service == null)
            {
                throw new InvalidOperationException("Remote Config service is not set");
            }

            var result = service.Fetch<T>(key);
            return result;
        }


        public UniTask PostInitialize() => UniTask.CompletedTask;
    }
}