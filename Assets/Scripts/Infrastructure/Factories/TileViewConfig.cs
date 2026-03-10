using System;
using Match3Game.Domain.Tiles;
using UnityEngine;

namespace Match3Game.Infrastructure.Factories
{
    [CreateAssetMenu(fileName = "TileViewConfig", menuName = "Match3/Tile View Config")]
    public class TileViewConfig : ScriptableObject
    {
        [SerializeField] private TileColorSprite[] _colorSprites;
        [SerializeField] private Sprite[] _specialSprites;      // Index = TileSpecialType

        public Sprite GetSpriteForColor(TileColor color)
        {
            foreach(var entry in _colorSprites)
                if(entry.color == color) return entry.sprite;

            Debug.LogWarning($"[TileViewConfig] {color} için sprite bulunamadı.");
            return null;
        }

        public Sprite GetSpriteForSpecial(TileSpecialType special)
        {
            int idx = (int)special;
            return idx > 0 && idx < _specialSprites.Length ? _specialSprites[idx] : null;
        }
    }

    [Serializable]
    public struct TileColorSprite
    {
        public TileColor color;
        public Sprite sprite;
    }

}
