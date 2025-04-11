using System.Collections.Generic;
using UnityEngine;

namespace OneDay.Core.Modules.Performance
{
    public class PerformanceMode : MonoBehaviour
    {
        public string Name;
        public List<APerformanceSettings> PerformanceSettings;
        public void Apply() => PerformanceSettings.ForEach(x => x.Apply());
    }
}