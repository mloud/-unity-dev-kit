#if USE_FIREBASE_ANALYTICS
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using OneDay.Core.Debugging;

namespace OneDay.Core.Modules.Analytics
{
    [LogSection("Analytics")]
    public class FirebaseAnalyticsService : IAnalyticsService
    {
        public bool IsInitialized { get; private set; }
        
        private Queue<(string eventName, (string parameterName, string parameterValue)[] parameters)> eventQueue= new();
        
        public UniTask Initialize()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    IsInitialized = true;
                    // Firebase is ready to use
                    D.LogInfo("Firebase initialized successfully.", this);
                }
                else
                {
                    IsInitialized = true;
                    D.LogError($"Could not resolve Firebase dependencies: {task.Result}",this);
                }
                ProcessEventQueue();
            });
            return UniTask.CompletedTask;
        }

        public void TrackEvent(string eventName, params (string parameterName, string parameterValue)[] parameters)
        {
            if (IsInitialized)
            {
                if (parameters.Length == 0)
                {
                    FirebaseAnalytics.LogEvent(eventName);
                }
                else
                {
                    LogToFirebaseWithParameters(eventName, parameters);
                }
            }
            else
            {
                eventQueue.Enqueue((eventName, parameters));
            }
        }

        private static void LogToFirebaseWithParameters(string eventName, (string parameterName, string parameterValue)[] parameters)
        {
            var firebaseParameters = new Parameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                firebaseParameters[i] = new Parameter(parameters[i].parameterName, parameters[i].parameterValue);
            }
            FirebaseAnalytics.LogEvent(eventName, firebaseParameters);
        }

        private void ProcessEventQueue()
        {
            while (eventQueue.Count > 0)
            {
                var @event = eventQueue.Dequeue();
                LogToFirebaseWithParameters(@event.eventName, @event.parameters);
            }
        }
    }
}
#endif