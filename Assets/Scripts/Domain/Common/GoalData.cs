using Match3Game.Domain.Tiles;
using UnityEngine;

namespace Match3Game.Domain.Common
{
    /// <summary>
    /// Bir level hedefini tanımlar.
    /// Hangi renkten kaç adet kırılması gerektiğini tutar.
    /// </summary>
    [System.Serializable] // LevelConfig ScriptableObject'te serialize edilebilsin
    public class GoalData
    {
        public TileColor TargetColor    { get; }
        public int       RequiredCount  { get; }

        public GoalData(TileColor targetColor, int requiredCount)
        {
            TargetColor   = targetColor;
            RequiredCount = requiredCount;
        }
    }

}
