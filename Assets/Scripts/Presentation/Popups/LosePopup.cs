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
    /// Level kaybedildiğinde gösterilir.
    /// </summary>
    public class LosePopup : BasePopup
    {
        [Header("Lose UI")]
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _menuButton;

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
            _retryButton?.onClick.AddListener(OnRetry);
            _menuButton?.onClick.AddListener(OnMenu);
        }

        private void OnEnable() =>
            _eventBus.Subscribe<LevelLostEvent>(OnLevelLost);

        private void OnDisable() =>
            _eventBus.Unsubscribe<LevelLostEvent>(OnLevelLost);

        private void OnLevelLost(LevelLostEvent _) => Show();

        protected override void OnPopupShown()
        {
            if (_messageText != null)
            {
                _messageText.text = "Out of moves!";
                _messageText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
            }
        }

        private void OnRetry() => Hide(() => _sceneService.ReloadCurrentLevel());
        private void OnMenu() => Hide(() => _sceneService.LoadMainMenu());

    }

}
