using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OneDay.Core.Debugging;
using OneDay.Core.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace OneDay.Core.Modules.Performance
{
    public interface IPerformanceManager
    {
        void SetScreenDimmed(bool isDimmed);
        void SwitchToMode(PerformanceMode mode);
        void SwitchToMode(string name);
    }
    
    [LogSection("Performance")]
    public class PerformanceManager : MonoBehaviour, IService, IPerformanceManager
    {
        [SerializeField] private PerformanceMode defaultPerformanceMode;
        [SerializeField] private List<PerformanceMode> performanceModes;
        
        [SerializeField] private Image dimLayer;

        public UniTask Initialize()
        {
            if (dimLayer != null)
            {
                dimLayer.SetAlpha(0);
            }

            if (defaultPerformanceMode != null)
            {
                defaultPerformanceMode.Apply();
            }
            return UniTask.CompletedTask;   
        }
        public UniTask PostInitialize() => UniTask.CompletedTask;

        public void SetScreenDimmed(bool isDimmed)
        {
            if (dimLayer == null)
            {
                D.LogError("DimLayer is not assigned", this);
                return;
            }
            dimLayer.DOFade(isDimmed ? 0.98f : 0, 1.0f);
        }

        public void SwitchToMode(PerformanceMode mode) => mode.Apply();

        public void SwitchToMode(string modeName)
        {
            var performanceMode = performanceModes.Find(x => x.Name == modeName);
            if (performanceMode != null)
            {
                SwitchToMode(performanceMode);
            }
            else
            {
                D.LogError($"Could not find Performance mode {modeName}", this);
            }
        }
    }
}