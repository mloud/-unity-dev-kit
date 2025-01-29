using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Analytics.BaseEvents;
using UnityEngine;

namespace OneDay.Core.Modules.Analytics
{
    public interface IAnalyticsManager
    {
        void RegisterAnalyticsServices(params IAnalyticsService[] services);
        void TrackEvent(string eventName, params (string parameterName, string parameterValue)[] parameters);
        void TrackEvent(AnalyticsEvent analyticsEvent);
    }

    public interface IAnalyticsService
    {
        bool IsInitialized { get; }
        UniTask Initialize();
        void TrackEvent(string eventName, params (string parameterName, string parameterValue)[] parameters);
    }
    public class AnalyticsManager : MonoBehaviour, IService, IAnalyticsManager
    {
        private List<IAnalyticsService> services;
        public UniTask Initialize()
        {
            services ??= new List<IAnalyticsService>();
            return UniTask.CompletedTask;
        }

        public void RegisterAnalyticsServices(params IAnalyticsService[] analyticsServices)
        {
            services ??= new List<IAnalyticsService>();
            analyticsServices.ForEach(x=>
            {
                services.Add(x);
                x.Initialize();
            });
        }

        public void TrackEvent(string eventName, params (string parameterName, string parameterValue)[] parameters)
        {
            services.ForEach(x=>x.TrackEvent(eventName, parameters));
        }

        public void TrackEvent(AnalyticsEvent analyticsEvent)
        {
            services.ForEach(x=>x.TrackEvent(analyticsEvent.Name, analyticsEvent.Parameters));
        }

        public UniTask PostInitialize() => UniTask.CompletedTask;
    }
}