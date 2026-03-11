using Match3Game.Application.Services;
using Match3Game.Infrastructure.Audio;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Match3Game.Presentation.Popups
{
    public class PausePopup : BasePopup
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Slider _bgmSlider;

        private AudioService _audioService;
        private SceneService _sceneService;

        [Inject]
        public void Construct(AudioService audioService, SceneService sceneService)
        {
            _audioService = audioService;
            _sceneService = sceneService;
        }

        private void Awake()
        {
            gameObject.SetActive(false);
            _resumeButton?.onClick.AddListener(OnResume);
            _menuButton?.onClick.AddListener(OnMenu);
            _sfxSlider?.onValueChanged.AddListener(_audioService.SetSFXVolume);
            _bgmSlider?.onValueChanged.AddListener(_audioService.SetBGMVolume);
        }

        private void OnResume() => Hide();
        private void OnMenu() => Hide(() => _sceneService.LoadMainMenu());
    }

}
