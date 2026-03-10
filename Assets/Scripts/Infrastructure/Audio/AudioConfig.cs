using UnityEngine;

namespace Match3Game.Infrastructure.Audio
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Match3/Audio Config")]
    public class AudioConfig : ScriptableObject
    {
        [Header("BGM")]
        public AudioClip gameplayBGM;

        [Header("SFX")]
        public AudioClip swapSFX;
        public AudioClip matchSFX;
        public AudioClip specialTileSFX;
        public AudioClip fallSFX;
        public AudioClip winSFX;
        public AudioClip loseSFX;
        public AudioClip invalidSwapSFX;



    }

}
