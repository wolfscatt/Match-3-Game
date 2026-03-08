using System;

namespace Match3Game.Domain.Tiles
{
    /// <summary>
    /// Board üzerindeki tek bir tile'ın tüm state'ini tutar.
    /// Pure domain object - Unity, MonoBehavior dependency yok.
    /// </summary>
    public class TileModel
    {
        // ── Identity ──────────────────────────────────────────────────────────────── 
        public Guid Id { get;}
        public TileColor Color { get; private set;}
        public TileSpecialType SpecialType { get; private set;}

        // ── Position ────────────────────────────────────────────────────────────────
        public int Row {get; set;}
        public int Col {get; set;}

        // ── State Flags ────────────────────────────────────────────────────────────────
        public bool IsMatched { get; set; }
        public bool IsLocked { get; set; }

        // ── Computed ────────────────────────────────────────────────────────────────
        public bool IsNormal => SpecialType == TileSpecialType.None;
        public bool IsSpecial => SpecialType != TileSpecialType.None;

        // ── Constructor ────────────────────────────────────────────────────────────────
        public TileModel(TileColor color, int row, int col, 
                        TileSpecialType specialType = TileSpecialType.None)
        {
            Id = Guid.NewGuid();
            Color = color;
            SpecialType = specialType;
            Row = row;
            Col = col;
        }

        // ── Mutations ───────────────────────────────────────────────
        public void UpgradeToSpecial(TileSpecialType specialType)
        {
            if(SpecialType == TileSpecialType.None)
                throw new ArgumentException("Upgrade için NONE kullanılamaz.");
            
            SpecialType = specialType;
        }

        public void Reset()
        {
            IsMatched = false;
            IsLocked = false;
        }

        public override string ToString() => $"Tile[{Row},{Col}] Color={Color} Special={SpecialType}";
        
    }
}
