using System;
using System.Collections.Generic;
using Match3Game.Domain.Board;
using Match3Game.Domain.Match;
using Match3Game.Domain.Tiles;

namespace Match3Game.Domain.Rules
{
    /// <summary>
    /// Deadlock tespiti ve board shuffle.
    /// Deadlock = hiçbir spaw match üretmiyor.
    /// </summary>
    public class ShuffleService
    {
        private readonly IMatchStrategy _matchStrategy;
        private readonly Random _random;
        private const int MaxShuffleAttempts = 10;

        public ShuffleService(IMatchStrategy matchStrategy, Random random = null)
        {
            _matchStrategy = matchStrategy;
            _random = random ?? new Random();
        }

        /// <summary>
        /// Board'da geçerli bir hamle var mı?
        /// </summary>
        public bool HasValidMove(BoardModel board)
        {
            // Her tile için 4 yönde swap dene
            int[] dr = {-1, 1, 0, 0};
            int[] dc = {0, 0, -1, 1};

            for(int r = 0; r < board.Rows; r++)
            {
                for(int c = 0; c < board.Cols; c++)
                {
                    if(!board.IsPlayable(r, c)) continue;

                    for(int d = 0; d < 4; d++)
                    {
                        int nr = r + dr[d];
                        int nc = c + dc[d];
                        if(!board.IsPlayable(nr, nc)) continue;

                        // Geçici swap
                        board.SwapTiles(r, c, nr, nc);
                        bool hasMatch = _matchStrategy.FindMatches(board).HasMatch;
                        board.SwapTiles(r, c, nr, nc);   // Geri al

                        if(hasMatch) return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Board'u karıştır -- geçerli hamle çıkana kadar dene.
        /// </summary>
        public void Shuffle(BoardModel board)
        {
            for(int attempt = 0; attempt < MaxShuffleAttempts; attempt++)
            {
                ShuffleInPlace(board);

                if(HasValidMove(board)) return;
            }

            // MaxAttempt'e ulaşıldıysa son shuffle'ı kabul et
            // (edge case: çok küçük board + az renk)
        }

        // ── Private ─────────────────────────────────────────────────

        private void ShuffleInPlace(BoardModel board)
        {
            // Tüm oynanabilir tile'ları topla
            var tiles = new List<TileModel>();
            var cells = new List<(int r, int c)>();

            for(int r = 0; r < board.Rows; r++)
            {
                for(int c = 0; c < board.Cols; c++)
                {
                    if(!board.IsPlayable(r, c)) continue;

                    var tile = board.GetTile(r, c);
                    if(tile == null) continue;

                    tiles.Add(tile);
                    cells.Add((r, c));
                    board.ClearTile(r, c);
                }
            }

            // Fisher-Yates shuffle
            for(int i = tiles.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (tiles[i], tiles[j]) = (tiles[j], tiles[i]);
            }

            // Geri yerleştir
            for(int i = 0; i < cells.Count; i++)
                board.SetTile(cells[i].r, cells[i].c, tiles[i]);
        }
    }

}
