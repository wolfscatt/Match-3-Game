using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Game.Presentation.Popups
{
    /// <summary>
    /// Tüm popup'ların base class'ı.
    /// Aç/kapat animasyonları, backdrop blur.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class BasePopup : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] protected float _showDuration = 0.3f;
        [SerializeField] protected float _hideDuration = 0.2f;

        [Header("References")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _backdrop;

        public bool IsVisible {get; private set;}

        public event Action OnShown;
        public event Action OnHidden;

        // ── Public API ───────────────────────────────────────────────

        public void Show(Action onComplete = null)
        {
            gameObject.SetActive(true);
            IsVisible = true;

            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            transform.localScale = Vector3.one * 0.8f;

            DOTween.Sequence()
                .Append(_canvasGroup.DOFade(1f, _showDuration))
                .Join(transform.DOScale(Vector3.one, _showDuration).SetEase(Ease.OutBack))
                .OnComplete(() =>
                {
                    _canvasGroup.interactable = true;
                    _canvasGroup.blocksRaycasts = true;
                    OnShown?.Invoke();
                    onComplete?.Invoke();
                    OnPopupShown();
                });
        }

        public void Hide(Action onComplete = null)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            IsVisible = false;

            DOTween.Sequence()
                .Append(_canvasGroup.DOFade(0f, _hideDuration))
                .Join(transform.DOScale(Vector3.one * 0.8f, _hideDuration))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    OnHidden?.Invoke();
                    onComplete?.Invoke();
                    OnPopupHidden();
                });
        }

        // ── Overridable ──────────────────────────────────────────────

        protected virtual void OnPopupShown()  { }
        protected virtual void OnPopupHidden() { }
    }

}
