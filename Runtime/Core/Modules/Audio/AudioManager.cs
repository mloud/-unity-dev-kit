using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Settings;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace OneDay.Core.Modules.Audio
{
    public interface IAudioManager
    {
        Action<bool> MusicStateChanged { get; set; }
        
        float SfxVolume { get; set;}
        float MusicVolume { get; set;}
        UniTask PlayMusic(string musicName);
        UniTask PlaySfx(AudioClip clip);
        UniTask PlaySfx(string clipId);
        UniTask StopMusic();
        bool MusicEnabled { get; set; }
        bool SfxEnabled { get; set; }
    }
    public class AudioManager : MonoBehaviour, IService, IAudioManager
    {
        public Action<bool> MusicStateChanged { get; set; }

        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private List<string> musicNames;
        
        [Range(0,1)] [SerializeField] private float musicVolume = 0.5f;
        [Range(0,1)] [SerializeField] private float sfxVolume = 0.5f;

        [SerializeField] private AudioMixer audioMixer;
        
        private ISettingsManager _settingsManager;

        private string currentMusic;

        public float SfxVolume
        {
            get => _settingsManager.GetModule<IVolumeModule>().SfxVolume;
            set
            {
                _settingsManager.GetModule<IVolumeModule>().SfxVolume = value;
                audioMixer.SetFloat("SfxVolume", value);
            }
        }
        
        public float MusicVolume
        {
            get => _settingsManager.GetModule<IVolumeModule>().MusicVolume;
            set
            {
                _settingsManager.GetModule<IVolumeModule>().MusicVolume = value;
                audioMixer.SetFloat("MusicVolume", value);
            }
        }
        
        private static AddressableAsset<AudioClip> musicAsset = null;

        public UniTask Initialize()
        {
            _settingsManager = ServiceLocator.Get<ISettingsManager>();
            return UniTask.CompletedTask;
        }

        public UniTask PostInitialize() => UniTask.CompletedTask;

        
        public async UniTask PlayMusic(string musicName)
        {
            if (string.IsNullOrEmpty(musicName))
            {
                musicName = musicNames[Random.Range(0, musicNames.Count)];
            }

            if (musicName == currentMusic)
                return;

            if (currentMusic != null)
            {
                await StopMusic();
            }

            currentMusic = musicName;
            
            musicAsset?.Release();
            musicAsset = await ServiceLocator.Get<IAssetManager>().GetAssetAsync<AudioClip>(musicName);
            GetAudioSource().clip = musicAsset.GetReference();
            GetAudioSource().Play();
            await DOTween.To(() => GetAudioSource().volume, (t) => GetAudioSource().volume = t, 1, 1f)
                .From(0)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }

        public async UniTask PlaySfx(string sfxName)
        {
            var asset = await ServiceLocator.Get<IAssetManager>().GetAssetAsync<AudioClip>(sfxName);
            sfxSource.PlayOneShot(asset.GetReference());
            asset.Release(); ;
        }
        
        public UniTask PlaySfx(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
            return UniTask.CompletedTask;
        }

        public async UniTask StopMusic()
        {
            await DOTween.To(() => GetAudioSource().volume, (t) =>
                {
                    GetAudioSource().volume = t;
                }, 0.0f, 1.0f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
            GetAudioSource().Stop();
            currentMusic = null;
        }

        public static void Release()
        {
            musicAsset?.Release();
        }

        private AudioSource GetAudioSource() => musicSource;

        public bool MusicEnabled
        {
            get => !musicSource.mute;
            set
            {
                MusicStateChanged?.Invoke(value);
                if (value)
                    musicSource.mute = false;
                DOTween.To(
                        () => musicSource.volume,
                        t => musicSource.volume = t,
                        value ? 1 : 0, 1.0f)
                    .SetEase(Ease.Linear)
                    .onComplete = () => { musicSource.mute = !value; };
            }
        }
        
        public bool SfxEnabled
        {
            get => !sfxSource.mute;
            set => sfxSource.mute = !value;
        }
    }
}