using Match3Game.Domain.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using DG.Tweening;
using Match3Game.Application.Services;

namespace Match3Game.Presentation.Popups
{
    /// <summary>
    /// Level kazanıldığında gösterilir.
    /// EventBus'tan LevelWonEvent dinler.
    /// </summary>
    public class WinPopup : BasePopup
    {
        [Header("Win UI")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Transform _starsContainer;
        [SerializeField] private GameObject[] _stars;

        private IEventBus _eventBus;
        private SceneService _sceneService;

        [Inject]
        public void Construct(IEventBus eventBus, SceneService sceneService)
        {
            _eventBus = eventBus;
            _sceneService = sceneService;
        }

        private void Awake()
        {
            gameObject.SetActive(false);
            _nextLevelButton?.onClick.AddListener(OnNextLevel);
            _menuButton?.onClick.AddListener(OnMenu);
        }

        private void Onable() =>
            _eventBus.Subscribe<LevelWonEvent>(OnLevelWon);

        private void OnDisable() =>
            _eventBus.Unsubscribe<LevelWonEvent>(OnLevelWon);

        // ── Event Handler ────────────────────────────────────────────

        private void OnLevelWon(LevelWonEvent _)
        {
            Show(() => AnimateStars());
        }

        protected override void OnPopupShown()
        {
            if (_titleText != null)
            {
                _titleText.text = "Level Complete!";
                _titleText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
            }
        }

        // ── Private ──────────────────────────────────────────────────

        private void AnimateStars()
        {
            if (_stars == null) return;

            for (int i = 0; i < _stars.Length; i++)
            {
                int idx = i;
                DOVirtual.DelayedCall(0.2f * idx, () =>
                {
                    _stars[idx].SetActive(true);
                    _stars[idx].transform
                        .DOPunchScale(Vector3.one * 0.5f, 0.3f)
                        .SetEase(Ease.OutBack);
                });
            }
        }

        public void SetScore(int score)
        {
            if (_scoreText != null)
                _scoreText.text = score.ToString("N0");
        }

        private void OnNextLevel() => Hide(() => _sceneService.LoadNextLevel());
        private void OnMenu() => Hide(() => _sceneService.LoadMainMenu());
    }

}
