using System.Collections.Generic;
using Match3Game.Domain.Board;
using Match3Game.Domain.Tiles;

namespace Match3Game.Domain.Match
{
    /// <summary>
    /// Klasik Match-3 kuralları: yatay ve dikey 3+ eşleşme.
    /// L/T shape tespiti de dahil.
    /// </summary>
    public class StandardMatchStrategy : IMatchStrategy
    {
        private const int MinMatch = 3;
        public MatchResult FindMatches(BoardModel board)
        {
            // Her tile bir kez işlenmeli - visited seti
            var visited = new HashSet<(int, int)>();
            var groups = new List<MatchGroup>();

            // Önce yatay sonra dikey tara, intersec'leri birleştir
            var horizontalRuns = FindRuns(board, horizontal: true);
            var verticalRuns = FindRuns(board, horizontal: false);

            // Intersect tespiti -> L/T shape birleştirme
            groups.AddRange(MergeIntersectingRuns(horizontalRuns, verticalRuns, visited));

            // Kalan tekil run'ları ekle
            foreach (var run in horizontalRuns)
                if(!IsFullyCovered(run, visited))
                    groups.Add(BuildGroup(run, visited, horizontal: true));

            
            foreach (var run in verticalRuns)
                if(!IsFullyCovered(run, visited))
                    groups.Add(BuildGroup(run, visited, horizontal: false));

            return new MatchResult(groups);
        }

        // ── Run Tespiti ─────────────────────────────────────────────

        private List<List<TileModel>> FindRuns(BoardModel board, bool horizontal)
        {
            var runs = new List<List<TileModel>>();
            int outerMax = horizontal ? board.Rows : board.Cols;
            int innerMax = horizontal ? board.Cols : board.Rows;

            for(int outer = 0; outer < outerMax; outer++)
            {
                var current = new List<TileModel>();

                for(int inner = 0; inner < innerMax; inner++)
                {
                    int row = horizontal ? outer : inner;
                    int col = horizontal ? inner : outer;

                    var tile = board.GetTile(row, col);

                    if(tile != null && !tile.IsMatched && CanExtendRun(current, tile))
                    {
                        current.Add(tile);
                    }
                    else
                    {
                        if(current.Count >= MinMatch) runs.Add(new List<TileModel>(current));
                        current.Clear();
                        if(tile != null) current.Add(tile);
                    }
                }

                if(current.Count >= MinMatch) runs.Add(new List<TileModel>(current));
            }
            return runs;
        }

        private bool CanExtendRun(List<TileModel> run, TileModel tile)
        {
            if(run.Count == 0) return true;
            return run[0].Color == tile.Color && tile.Color != TileColor.None;
        }

        // ── L/T Shape Birleştirme ───────────────────────────────────

        private List<MatchGroup> MergeIntersectingRuns(
            List<List<TileModel>> hRuns,
            List<List<TileModel>> vRuns,
            HashSet<(int, int)> visited)
        {
            var merged = new List<MatchGroup>();

            foreach(var hRun in hRuns)
            {
                foreach(var vRun in vRuns)
                {
                    // Aynı renk mi ve kesişiyor mu?
                    if(hRun[0].Color != vRun[0].Color) continue;

                    var intersection = FindIntersection(hRun, vRun);
                    if(intersection == null) continue;

                    // Birleşik tile seti oluştur (duplicate yok)
                    var tileSet = new HashSet<TileModel>(hRun);
                    foreach(var t in vRun) tileSet.Add(t);

                    var tiles = new List<TileModel>(tileSet);
                    var shape = ResolveShape(hRun.Count, vRun.Count);
                    merged.Add(new MatchGroup(tiles, shape));

                    foreach(var t in tiles) visited.Add((t.Row, t.Col));
                }
            }
            return merged;
        }

        private TileModel FindIntersection(List<TileModel> hRun, List<TileModel> vRun)
        {
            foreach(var h in hRun)
                foreach(var v in vRun)
                    if(h.Row == v.Row && h.Col == v.Col) return h;
            return null;
        }

        private MatchShape ResolveShape(int hCount, int vCount)
        {
            if(hCount >= 5 || vCount >= 5) return MatchShape.FiveInRow;
            return MatchShape.TShape;
        }

        // ── Helpers ─────────────────────────────────────────────

        private bool IsFullyCovered(List<TileModel> run, HashSet<(int, int)> visited)
        {
            foreach(var t in run)
                if(!visited.Contains((t.Row, t.Col))) return false;
            return true;
        }

        private MatchGroup BuildGroup(
            List<TileModel> run,
            HashSet<(int, int)> visited,
            bool horizontal)
        {
            MatchShape shape;
            if(run.Count >= 5)
                shape = MatchShape.FiveInRow;
            else if (run.Count == 4)
                shape = horizontal ? MatchShape.FourInRow : MatchShape.FourInCol;
            else
                shape = horizontal ?  MatchShape.ThreeInRow : MatchShape.ThreeInCol;

            foreach(var t in run) visited.Add((t.Row, t.Col));
            return new MatchGroup(run, shape);
        }
    }

}
