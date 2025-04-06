using System.Collections.Generic;
using OneDay.Core.Modules.Pooling;

namespace Core.Modules.Ui.Loading
{
    public static class LoadingTrackerExtensions
    {
        public static void RegisterPreload(this LoadingTracker tracker, IPoolManager poolManager, 
            string key, int count, string message = null)
        {
            tracker.RegisterOperation(
                () => poolManager.PreloadAsync(key, count),
                message: message ?? $"Loading {key}..."
            );
        }
        
        public static void RegisterPreloadGroup(this LoadingTracker tracker, IPoolManager poolManager, 
            string groupName, Dictionary<string, int> items)
        {
            foreach (var item in items)
            {
                tracker.RegisterPreload(poolManager, item.Key, item.Value, 
                    $"Loading {groupName}: {item.Key}...");
            }
        }
    }
}