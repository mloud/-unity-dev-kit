#if USE_FIREBASE_REMOTE_CONFIG
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
using OneDay.Core.Debugging;


namespace OneDay.Core.Modules.Analytics
{
    [LogSection("RemoteConfig")]
    public class FirebaseRemoteConfigService : IRemoteConfigService
    {
        public bool IsInitialized { get; private set; }

        public UniTask Initialize()
        {
            IsInitialized = true;
            return UniTask.CompletedTask;
        }

        public async UniTask<T> Fetch<T>(string key)
        {
            var defaults = new Dictionary<string, object>
            {
                { key, null }
            };
            await FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
            var fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync();
            await fetchTask;

            if (fetchTask.IsCompletedSuccessfully && fetchTask.Result)
            {
                var json = FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;

                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                    catch (JsonException e)
                    {
                        D.LogError($"JSON deserialization error for key {key}: {e.Message}", this);
                    }
                }
                else
                {
                    D.LogError($"Remote Config value for {key} is empty or null", this);
                }
            }
            else
            {
                D.LogError($"Failed to fetch Remote Config for {key}", this);
            }

            return default;
        }
    }
}
#endif
