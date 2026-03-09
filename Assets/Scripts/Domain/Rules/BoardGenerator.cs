using Match3Game.Domain.Board;
using Match3Game.Domain.Match;
using Match3Game.Domain.Tiles;
using System;
using UnityEngine.Rendering;

namespace Match3Game.Domain.Rules
{
    /// <summary>
    /// Oyun başında match içermeyen, çözülebilir bir board üretir.
    /// Pure C# - test edilebilir.
    /// </summary>
    public class BoardGenerator
    {
        private readonly IMatchStrategy _matchStrategy;
        private readonly Random _random;

        public BoardGenerator(IMatchStrategy matchStrategy, Random random = null)
        {
            _matchStrategy = matchStrategy;
            _random = random ?? new Random();
        }

        /// <summary>
        /// LevelConfig'e göre board'u tile'larla doldurur.
        /// Hiçbir başlangıç match'i olmayacak şekilde renk seçer.
        /// </summary>
        public void Generate(BoardModel board, TileColor[] allowedColors)
        {
            if(allowedColors == null || allowedColors.Length < 2)
                throw new ArgumentException("En az 2 renk gerekli.", nameof(allowedColors));

            for(int r = 0; r < board.Rows; r++)
            {
                for(int c = 0; c < board.Cols; c++)
                {
                    if(!board.IsPlayable(r, c)) continue;

                    var color = PickNonMatchingColor(board, r, c, allowedColors);
                    board.SetTile(r, c, new TileModel(color, r, c));
                }
            }
        }

        // ── Private ─────────────────────────────────────────────────

        private TileColor PickNonMatchingColor(
            BoardModel board,
            int row,
            int col,
            TileColor[] allowedColors)
        {
            // Shuffle -- deterministik değil, her seferinde farklı board
            var shuffled = Shuffle(allowedColors);

            foreach(var color in shuffled)
            {
                // Geçici tile yerleştir, match kontrol et
                board.SetTile(row, col, new TileModel(color, row, col));
                var result = _matchStrategy.FindMatches(board);

                if(!result.HasMatch)
                    return color;
            }

            // Tüm renkler match yapıyorsa (çok küçük board) ilkini kullan
            board.ClearTile(row, col);
            return shuffled[0];
        }

        private TileColor[] Shuffle(TileColor[] colors)
        {
            var copy = (TileColor[])colors.Clone();
            for(int i = copy.Length - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (copy[i], copy[j]) = (copy[j], copy[i]);
            }
            return copy;
        }
    }

}
