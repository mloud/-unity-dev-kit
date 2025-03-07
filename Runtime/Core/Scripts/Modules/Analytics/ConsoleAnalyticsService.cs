using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using OneDay.Core.Debugging;

namespace OneDay.Core.Modules.Analytics
{
    [LogSection("Analytics")]
    public class ConsoleAnalyticsService : IAnalyticsService
    {
        public bool IsInitialized { get; private set; }

        public UniTask Initialize()
        {
            IsInitialized = true;
            return UniTask.CompletedTask;
        }

        public void TrackEvent(string eventName, params (string parameterName, string parameterValue)[] parameters)
        {
            D.LogInfo($"Sending event: {eventName} {JsonConvert.SerializeObject(parameters)}");
        }
    }
}