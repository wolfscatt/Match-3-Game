using UnityEngine;

namespace Match3Game.Domain.Board
{
    public enum CellType
    {
        Normal = 0,         // Tile spawn olur, oynanabilir
        Blocked = 1,        // Tile spawn olmaz, gravity geçmez
        Empty = 2           // Görsel yok ama gravity geçer
    }

}
