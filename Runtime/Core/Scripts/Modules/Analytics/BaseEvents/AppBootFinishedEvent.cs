namespace OneDay.Core.Modules.Analytics.BaseEvents
{
    public class AppBootFinishedEvent : AnalyticsEvent
    {
        public AppBootFinishedEvent() : base("app_boot_finished")
        { }
    }
}