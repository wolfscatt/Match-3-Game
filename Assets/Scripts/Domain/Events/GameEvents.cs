using Match3Game.Domain.Match;
using Match3Game.Domain.Tiles;
using UnityEngine.Tilemaps;

namespace Match3Game.Domain.Events
{
    // ── Board Events ─────────────────────────────────────────────────
    public readonly struct BoardInitializedEvent
    {
        public readonly int Rows;
        public readonly int Cols;
        public BoardInitializedEvent(int rows, int cols) { Rows = rows; Cols = cols; }
    }

    public readonly struct TilesSwappedEvent
    {
        public readonly int R1, C1, R2, C2;
        public TilesSwappedEvent(int r1, int c1, int r2, int c2)
        { R1 = r1; C1 = c1; R2 = r2; C2 = c2; }
    }

    public readonly struct SwapRejectedEvent
    {
        public readonly int R1, C1, R2, C2;
        public SwapRejectedEvent(int r1, int c1, int r2, int c2) 
        { R1 = r1; C1 = c1; R2 = r2; C2 = c2; }
    }

    // ── Match Events ─────────────────────────────────────────────────
    public readonly struct MatchFoundEvent
    {
        public readonly MatchResult Result;
        public MatchFoundEvent(MatchResult result) { Result = result;}
    }

    public readonly struct SpecialTileCreatedEvent
    {
        public readonly int Row, Col;
        public readonly TileSpecialType SpecialType;
        public SpecialTileCreatedEvent(int row, int col, TileSpecialType type)
        { Row = row; Col = col; SpecialType = type;}
    }

    public readonly struct SpecialTileActivatedEvent
    {
        public readonly int Row, Col;
        public readonly TileSpecialType SpecialType;
        public SpecialTileActivatedEvent(int row, int col, TileSpecialType type)
        { Row = row; Col = col; SpecialType = type; }
    }

    // ── Gravity Events ────────────────────────────────────────────────
    public readonly struct TilesFellEvent{}
    public readonly struct BoardRefillledEvent{}

    // ── Score & Goal Events ───────────────────────────────────────────
    public readonly struct ScoreChangedEvent
    {
        public readonly int NewScore;
        public ScoreChangedEvent(int score) { NewScore = score; }
    }

    public readonly struct GoalProgressEvent
    {
        public readonly TileColor Color;
        public readonly int Amount;
        public GoalProgressEvent(TileColor color, int amount)
        { Color = color; Amount = amount;}
    }

    // ── Game Flow Events ──────────────────────────────────────────────
    public readonly struct MoveConsumedEvent
    {
        public readonly int RemainingMoves;
        public MoveConsumedEvent(int remaining){RemainingMoves = remaining;}
    }

    public readonly struct LevelWonEvent{}
    public readonly struct LevelLostEvent {}
    public readonly struct BoardDeadlockedEvent {}

}
