namespace OneDay.Core.Modules.Analytics.BaseEvents
{
    public class AppBootStartedEvent : AnalyticsEvent
    {
        public AppBootStartedEvent() : base("app_boot_started")
        { }
    }
}