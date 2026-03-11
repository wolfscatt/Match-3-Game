using Match3Game.Application.Services;
using Match3Game.Domain.Events;
using Match3Game.Infrastructure.Config;
using Match3Game.Presentation.Popups;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Match3Game.Presentation.HUD
{
    /// <summary>
    /// Tüm UI elemanlarını koordine eder.
    /// HUD init, popup yönetimi, pause butonu.
    /// IStartable -- VContainer otomatik çağırır.
    /// </summary>
    public class UIManager : MonoBehaviour, IStartable
    {
        [Header("References")]
        [SerializeField] private HUDView _hudView;
        [SerializeField] private WinPopup _winPopup;
        [SerializeField] private LosePopup _losePopup;
        [SerializeField] private PausePopup _pausePopup;
        [SerializeField] private Button _pauseButton;

        // ── Injected ─────────────────────────────────────────────────
        private GoalService _goalService;
        private ScoreService _scoreService;
        private LevelConfig _levelConfig;
        private IEventBus _eventBus;

        [Inject]
        public void Construct(
            GoalService goalService,
            ScoreService scoreService,
            LevelConfig levelConfig,
            IEventBus eventBus
        )
        {
            _goalService = goalService;
            _scoreService = scoreService;
            _levelConfig = levelConfig;
            _eventBus = eventBus;
        }

        // ── IStartable ───────────────────────────────────────────────

        public void Start()
        {
            _pauseButton?.onClick.AddListener(OnPausePressed);
            _eventBus.Subscribe<LevelWonEvent>(OnLevelWon);
        }

        private void OnDestroy()
        {
            _eventBus.Unsubscribe<LevelWonEvent>(OnLevelWon);
        }

        // ── Init — BoardInitializedEvent sonrası çağrılır ─────────────

        public void Initialize()
        {
            _hudView.Initialize(_levelConfig.moveLimit, _goalService.GetAllGoals());
        }

        // ── Event Handlers ───────────────────────────────────────────

        private void OnLevelWon(LevelWonEvent _) =>
            _winPopup.SetScore(_scoreService.Score);
        
        // ── Button Handlers ──────────────────────────────────────────

        private void OnPausePressed() => _pausePopup.Show();
    }

}
