using DG.Tweening;
using Match3Game.Domain.Tiles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Game.Presentation.HUD
{
    /// <summary>
    /// Tek bir goal'ün UI temsili.
    /// Renk ikonu + kalan sayısı + tamamlanma efekti.
    /// </summary>
    public class GoalEntryUI : MonoBehaviour
    {
        [SerializeField] private Image _colorIcon;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private GameObject _completedOverlay;
        [SerializeField] private Sprite[] _colorSprites;    // Index = TileColor

        public TileColor Color {get; private set;}
        public int Remaining {get; private set;}

        public void Setup(TileColor color, int required)
        {
            Color = color;
            Remaining = required;

            int spriteIdx = (int)color;
            if(_colorIcon != null && spriteIdx < _colorSprites.Length)
                _colorIcon.sprite = _colorSprites[spriteIdx];

            UpdateCountText();

            if(_completedOverlay != null)
                _completedOverlay.SetActive(false);
        }

        public void UpdateProgress(int amount)
        {
            Remaining = Mathf.Max(0, Remaining - amount);
            UpdateCountText();

            // Bounce animasyonu
            transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);

            if(Remaining <= 0)
                ShowCompleted();
        }

        private void UpdateCountText()
        {
            if(_countText != null)
                _countText.text = Remaining > 0 ? Remaining.ToString() : "✓";
        }

        private void ShowCompleted()
        {
            if(_completedOverlay != null)
                _completedOverlay.SetActive(true);

            // Tamamlanma efekti
            transform.DOPunchScale(Vector3.one * 0.4f, 0.4f).SetEase(Ease.OutBack);
            _colorIcon?.DOColor(UnityEngine.Color.green, 0.3f);
        }
    }

}
