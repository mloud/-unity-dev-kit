using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core.Modules.Settings
{
    public interface ISettingsManager
    {
        T GetModule<T>() where T : ISettingsModule;
        void RegisterModule<T>(ISettingsModule module) where T : ISettingsModule;
    }

    public class SettingsManager : MonoBehaviour, ISettingsManager, IService
    {
        private Dictionary<Type, ISettingsModule> modules = new();

        public UniTask Initialize() => UniTask.CompletedTask;
        public UniTask PostInitialize() => UniTask.CompletedTask;

        public T GetModule<T>() where T : ISettingsModule
        {
            if (modules.TryGetValue(typeof(T), out var module))
                return (T)module;
            return default;
        }

        public void RegisterModule<T>(ISettingsModule module) where T : ISettingsModule
        {
            modules.Add(typeof(T), module);
        }
    }

    public interface ISettingsModule
    {
    }

    public interface IVolumeModule : ISettingsModule
    {
        public float MusicVolume { get; set; }
        public float SfxVolume { get; set; }

    }

    public class VolumeModule : IVolumeModule
    {
        public float MusicVolume
        {
            get => PlayerPrefs.GetFloat("MusicVolume", 100);
            set => PlayerPrefs.SetFloat("MusicVolume", value);
        }

        public float SfxVolume
        {
            get => PlayerPrefs.GetFloat("SfxVolume", 100);
            set => PlayerPrefs.SetFloat("SfxVolume", value);
        }
    }
}