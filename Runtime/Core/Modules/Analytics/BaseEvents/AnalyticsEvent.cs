namespace OneDay.Core.Modules.Analytics.BaseEvents
{
    public class AnalyticsEvent
    {
        public string Name { get; }
        public (string name, string value)[] Parameters { get; }

        public AnalyticsEvent(string name, params (string name, string value)[] parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }
}