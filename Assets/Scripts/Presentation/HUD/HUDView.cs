using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Match3Game.Application.Services;
using Match3Game.Domain.Events;
using TMPro;
using UnityEngine;
using VContainer;

namespace Match3Game.Presentation.HUD
{
    /// <summary>
    /// Oyun içi HUD -- hamle sayısı, skor, hedefler.
    /// EventBus'ı dinler, hiçbir sisteme doğrudan referans yok.
    /// </summary>
    public class HUDView : MonoBehaviour
    {
        [Header("Move Counter")]
        [SerializeField] private TextMeshProUGUI _moveCountText;
        [SerializeField] private Animator _moveLowWarningAnimator;
        [SerializeField] private int _lowMoveThreshold = 5;

        [Header("Score")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _comboText;
        [SerializeField] private float _scoreAnimDuration = 0.4f;

        [Header("Goals")]
        [SerializeField] private GoalEntryUI[] _goalEntries;

        // ── Injected ─────────────────────────────────────────────────
        private IEventBus _eventBus;

        // ── State ─────────────────────────────────────────────────────
        private int _displayedScore;

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<MoveConsumedEvent>(OnMoveConsumed);
            _eventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
            _eventBus.Subscribe<GoalProgressEvent>(OnGoalProgress);
            _eventBus.Subscribe<BoardInitializedEvent>(OnBoardInitialized);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<MoveConsumedEvent>(OnMoveConsumed);
            _eventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
            _eventBus.Unsubscribe<GoalProgressEvent>(OnGoalProgress);
            _eventBus.Unsubscribe<BoardInitializedEvent>(OnBoardInitialized);
        }

        // ── Init ─────────────────────────────────────────────────────

        public void Initialize(int moveLimit, IEnumerable<GoalEntry> goals)
        {
            SetMoveCount(moveLimit);
            SetScore(0);
            SetupGoals(goals);
            HideCombo();
        }

        // ── Event Handlers ───────────────────────────────────────────

        private void OnBoardInitialized(BoardInitializedEvent _) {}

        private void OnMoveConsumed(MoveConsumedEvent e)
        {
            SetMoveCount(e.RemainingMoves);

            if(e.RemainingMoves <= _lowMoveThreshold)
                TriggerLowMoveWarning();
        }

        private void OnScoreChanged(ScoreChangedEvent e)
        {
            AnimateScoreTo(e.NewScore);
        }

        private void OnGoalProgress(GoalProgressEvent e)
        {
            foreach(var entry in _goalEntries)
            {
                if(entry.Color != e.Color) continue;
                entry.UpdateProgress(e.Amount);
                break;
            }
        }

        // ── Private ──────────────────────────────────────────────────

        private void SetMoveCount(int count)
        {
            if(_moveCountText == null) return;
            _moveCountText.text = count.ToString();

            // Renk uyarısı
            _moveCountText.color = count <= _lowMoveThreshold ? Color.red : Color.white;
        }

        private void SetScore(int score)
        {
            _displayedScore = score;
            if(_scoreText != null)
                _scoreText.text = score.ToString("N0");
        }

        private void AnimateScoreTo(int targetScore)
        {
            DOTween.To(
                () => _displayedScore,
                x =>
                {
                    _displayedScore = x;
                    if(_scoreText != null)
                        _scoreText.text = x.ToString("N0");
                },
                targetScore,
                _scoreAnimDuration
            ).SetEase(Ease.OutQuad);
        }

        private void TriggerLowMoveWarning()
        {
            _moveLowWarningAnimator?.SetTrigger("Pulse");

            // Kamera sarsıntısı -- opsiyonel
            Camera.main?.DOShakePosition(0.2f, strength: 0.1f);
        }

        private void SetupGoals(IEnumerable<GoalEntry> goals)
        {
            // Tüm entry'leri gizle
            foreach(var entry in _goalEntries)
                entry.gameObject.SetActive(false);

            int i = 0;
            foreach(var goal in goals)
            {
                if(i >= _goalEntries.Length) break;
                _goalEntries[i].Setup(goal.Color, goal.Required);
                _goalEntries[i].gameObject.SetActive(true);
                i++;
            }
        }
        
        private void HideCombo()
        {
            if(_comboText != null)
                _comboText.gameObject.SetActive(false);
        }

        public void ShowCombo(int multiplier)
        {
            if(_comboText == null) return;

            _comboText.gameObject.SetActive(true);
            _comboText.text = $"x{multiplier} COMBO!";

            _comboText.transform
                .DOPunchScale(Vector3.one * 0.3f, 0.3f)
                .OnComplete(HideCombo);
        }
    }

}
