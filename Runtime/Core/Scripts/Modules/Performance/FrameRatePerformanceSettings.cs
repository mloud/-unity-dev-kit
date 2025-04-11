using UnityEngine;

namespace OneDay.Core.Modules.Performance
{
    public class FrameRatePerformanceSettings : APerformanceSettings
    {
        [SerializeField] private int targetFps;
        
        public override void Apply() => Application.targetFrameRate = targetFps;
    }
}