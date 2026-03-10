using Match3Game.Domain.Board;
using Match3Game.Domain.Common;
using Match3Game.Domain.Tiles;
using UnityEngine;

namespace Match3Game.Infrastructure.Config
{
    [CreateAssetMenu(fileName = "Level_01", menuName = "Match3/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Board Layout")]
        [Min(3)] public int rows = 8;
        [Min(3)] public int cols = 8;

        [Header("Cell Types")]
        [Tooltip("rows * cols boyutunda. Boş bırakılırsa tümü normal.")]
        public CellType[] cellTypeOverrides;

        [Header("Allowed Colors")]
        public TileColor[] allowedColors =
        {
            TileColor.Red,
            TileColor.Green,
            TileColor.Blue,
            TileColor.Yellow,
            TileColor.Purple,
        };

        [Header("Goals")]
        public GoalConfigEntry[] goals;

        [Header("Limits")]
        [Min(1)] public int moveLimit = 30;

        [Header("Special Tiles")]
        [Range(0f, 0.3f)]
        public float specialTileSpawnChance = 0.05f;

        // ── Builder Methods ──────────────────────────────────────────

        public CellType[,] BuildCellTypeGrid()
        {
            var grid = new CellType[rows, cols];
            for(int r = 0; r < rows; r++)
                for(int c = 0; c < cols; c++)
                {
                    int idx = r * cols + c;
                    grid[r, c] = (cellTypeOverrides != null && idx < cellTypeOverrides.Length)
                        ? cellTypeOverrides[idx]
                        : CellType.Normal;
                }
            return grid;
        }

        public GoalData[] BuildGoalData()
        {
            var result = new GoalData[goals.Length];
            for(int i = 0; i < goals.Length; i++)
                result[i] = new GoalData(goals[i].targetColor, goals[i].requiredCount);
            return result;
        }
    } 

    /// <summary>
    /// Inspector'da serialize edilebilir GoalData wrapper'ı.
    /// GoalData pure domain class olduğu için SerializeField desteklemiyor,
    /// bu yüzden ayrı bir Inspector-friendly struct kullanıyoruz.
    /// </summary>
    [System.Serializable]
    public struct GoalConfigEntry
    {
        public TileColor targetColor;
        [Min(1)] public int requiredCount;
    }  

}
