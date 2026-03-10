using Match3Game.Domain.Tiles;
using UnityEngine;
using DG.Tweening;
using System;

namespace Match3Game.Presentation.Board
{
    /// <summary>
    /// TileModel'in görsel temsili.
    /// Domain'i bilmez -- sadece görsel state'i yönetir.
    /// DOTween ile animasyonlar.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class TileView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteRenderer _specialIconRenderer;

        [Header("Animation Settings")]
        [SerializeField] private float _swapDuration = 0.2f;
        [SerializeField] private float _fallDuration = 0.3f;
        [SerializeField] private float _matchDuration = 0.15f;
        [SerializeField] private float _spawnDuration = 0.25f;
        [SerializeField] private float _invalidSwapShake = 0.1f;

        // ── State ────────────────────────────────────────────────────
        public TileModel Model { get; private set; }
        public bool IsMoving { get; private set; }

        private Sequence _currentSequence;

        // ── Init ─────────────────────────────────────────────────────

        public void Initialize(TileModel model, Sprite colorSprite, Sprite specialSprite = null)
        {
            Model = model;
            _spriteRenderer.sprite = colorSprite;

            if(_specialIconRenderer != null)
            {
                _specialIconRenderer.sprite = specialSprite;
                _specialIconRenderer.enabled = specialSprite != null;
            }
        }

        public void ResetView()
        {
            KillCurrentSequence();
            Model = null;
            _spriteRenderer.sprite = null;
            _spriteRenderer.color = Color.white;
            transform.localScale = Vector3.one;
            IsMoving = false;

            if(_specialIconRenderer != null)
                _specialIconRenderer.enabled = false;
        }

        // ── Animations ───────────────────────────────────────────────

        /// <summary>
        /// Swap animasyonu -- iki tile eş zamanlı hareket eder.
        /// </summary>
        public void AnimateSwap(Vector3 targetPosition, Action onComplete = null)
        {
            KillCurrentSequence();
            IsMoving = true;

            _currentSequence = DOTween.Sequence()
                .Append(transform.DOMove(targetPosition, _swapDuration).SetEase(Ease.OutQuad))
                .OnComplete(() =>
                {
                    IsMoving = false;
                    onComplete?.Invoke();
                });
        }

        /// <summary>
        /// Geçersiz swap -- sağa sola sallama.
        /// </summary>
        public void AnimateInvalidSwap(Vector3 originalPosition, Action onComplete = null)
        {
            KillCurrentSequence();

            _currentSequence = DOTween.Sequence()
                .Append(transform.DOShakePosition(_invalidSwapShake, strength: 0.15f, vibrato: 10))
                .Append(transform.DOMove(originalPosition, 0.1f))
                .OnComplete(() => onComplete?.Invoke());
        }

        /// <summary>
        /// Match -- küçül ve yok ol.
        /// </summary>
        public void AnimateMatch(Action onComplete = null)
        {
            KillCurrentSequence();

            _currentSequence = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.zero, _matchDuration).SetEase(Ease.InBack))
                .Join(_spriteRenderer.DOFade(0f, _matchDuration))
                .OnComplete(() => onComplete?.Invoke());
        }

        /// <summary>
        /// Gravity -- yukarıdan aşağıya düşme.
        /// </summary>
        public void AnimateFall(Vector3 targetPosition, Action onComplete = null)
        {
            KillCurrentSequence();
            IsMoving = true;

            float distance = Vector3.Distance(transform.position, targetPosition);
            float duration = Mathf.Clamp(_fallDuration * (distance / 2f), 0.1f, 0.5f);

            _currentSequence = DOTween.Sequence()
                .Append(transform.DOMove(targetPosition, duration).SetEase(Ease.InQuad))
                .Append(transform.DOPunchScale(Vector3.one * 0.15f, 0.1f))   // Yere çarpma
                .OnComplete(() =>
                {
                    IsMoving = false;
                    onComplete?.Invoke();
                });
        }

        /// <summary>
        /// Spawn -- yukarıdan düşerek belirir.
        /// </summary>
        public void AnimateSpawn(Vector3 fromPosition, Vector3 toPosition, Action onComplete = null)
        {
            transform.position = fromPosition;
            transform.localScale = Vector3.zero;
            KillCurrentSequence();
            IsMoving = true;

            _currentSequence = DOTween.Sequence()
                .Append(transform.DOMove(toPosition, _spawnDuration).SetEase(Ease.InQuad))
                .Join(transform.DOScale(Vector3.one, _spawnDuration * 0.5f))
                .OnComplete(() =>
                {
                    IsMoving = false;
                    onComplete?.Invoke();
                });
        }

        /// <summary>
        /// Special tile upgrade efekti -- parlama.
        /// </summary>
        public void AnimateUpgradeToSpecial(Sprite specialSprite, Action onComplete = null)
        {
            KillCurrentSequence();

            _currentSequence = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * 1.3f, 0.1f))
                .AppendCallback(() =>
                {
                    if(_specialIconRenderer != null)
                    {
                        _specialIconRenderer.sprite = specialSprite;
                        _specialIconRenderer.enabled = true;
                    }
                })
                .Append(transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutQuad))
                .OnComplete(() => onComplete?.Invoke());
        }

        /// <summary>
        /// Seçili tile pulse efekti.
        /// </summary>
        public void AnimateSelect()
        {
            KillCurrentSequence();
            _currentSequence = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * 1.1f, 0.1f))
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void AnimateDeselect()
        {
            KillCurrentSequence();
            transform.DOScale(Vector3.one, 0.1f);
        }

        // ── Private ──────────────────────────────────────────────────

        private void KillCurrentSequence()
        {
            _currentSequence?.Kill();
            _currentSequence = null;
        }

        private void OnDestroy() => KillCurrentSequence();

    }
}

