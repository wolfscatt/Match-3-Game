using Unity.VisualScripting;
using UnityEngine;

namespace Match3Game.Infrastructure.Audio
{
    /// <summary>
    /// Ses yönetimi. EventBus'ı dinler, clip çalar.
    /// SFX için pool, BGM için tek AudioSource.
    /// </summary>
    public class AudioService : IInitializable
    {
        private readonly AudioSource _bgmSource;
        private readonly AudioSource _sfxSource;
        private readonly AudioConfig _config;

        public AudioService(AudioSource bgmSource, AudioSource sfxSource, AudioConfig config)
        {
            _bgmSource = bgmSource;
            _sfxSource = sfxSource;
            _config = config;
        }

        public void Initialize()
        {
            PlayBGM(_config.gameplayBGM);
        }

        public void PlaySFX(AudioClip clip)
        {
            if(clip == null) return;
            _sfxSource.PlayOneShot(clip);
        }

        public void PlayBGM(AudioClip clip)
        {
            if(clip == null || _bgmSource.clip == clip) return;
            _bgmSource.clip = clip;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }

        public void SetMasterVolume(float volume) => 
            AudioListener.volume = Mathf.Clamp01(volume);

        public void SetSFXVolume(float volume) =>
            _sfxSource.volume = Mathf.Clamp01(volume);

        public void SetBGMVolume(float volume) =>
            _bgmSource.volume = Mathf.Clamp01(volume);

    }

}
