using System.Collections.Generic;
using Match3Game.Domain.Board;
using Match3Game.Domain.Tiles;

namespace Match3Game.Domain.Rules
{
    /// <summary>
    /// Match sonrası tile'ların aşağı düşmesini ve
    /// boş hücrelere yeni tile spawn edilmesini yönetir.
    /// </summary>
    public class GravitySystem
    {
        /// <summary>
        /// Tüm sütunlarda tile'ları aşağı kaydırır.
        /// </summary>
        /// <returns> Hareket eden tile'ların listesi (view animasyonu için)</returns>
        public List<TileMoveData> ApplyGravity(BoardModel board)
        {
            var moves = new List<TileMoveData>();

            for(int c = 0; c < board.Cols; c++)
                moves.AddRange(ApplyGravityToColumn(board, c));

            return moves;
        }

        /// <summary>
        /// Boş hücrelere yeni tile'lar spawn eder.
        /// </summary>
        /// <returns>Spawn edilen tile'ların listesi</returns>
        public List<TileSpawnData> FillEmpty(
            BoardModel board,
            TileColor[] allowedColors,
            System.Random random
        )
        {
            var spawns = new List<TileSpawnData>();

            for(int c = 0; c < board.Cols; c++)
            {
                // Yukarıdan aşağıya tara -- en üstteki bos hücreleri doldur
                for(int r = 0; r < board.Rows; r++)
                {
                    if(!board.IsEmpty(r, c)) continue;

                    var color = allowedColors[random.Next(allowedColors.Length)];
                    var tile = new TileModel(color, r, c);
                    board.SetTile(r, c, tile);

                    spawns.Add(new TileSpawnData(tile, c));   // c = hangi sütundan düştü

                }
            }
            return spawns;
        }

        // ── Private ─────────────────────────────────────────────────

        private List<TileMoveData> ApplyGravityToColumn(BoardModel board, int col)
        {
            var moves = new List<TileMoveData>();
            int writeIndex = board.Rows - 1;     // En alttan başla

            for(int r = board.Rows - 1; r >= 0; r--)
            {
                if(!board.IsPlayable(r, col))
                {
                    // Blocked hücre -- gravity sınırı sıfırla
                    writeIndex = r - 1;
                    continue;
                }

                var tile = board.GetTile(r, col);
                if(tile == null) continue;

                if(r != writeIndex)
                {
                    // Tile düşüyor
                    moves.Add(new TileMoveData(tile, r, col, writeIndex, col));
                    board.SetTile(writeIndex, col, tile);
                    board.ClearTile(r, col);
                }

                writeIndex--;
            }

            return moves;
        }
    }

    // ── Data Transfer Objects ────────────────────────────────────────

    public readonly struct TileMoveData
    {
        public readonly TileModel Tile;
        public readonly int FromRow, FromCol;
        public readonly int ToRow, ToCol;

        public TileMoveData(TileModel tile, int fromRow, int fromCol, int toRow, int toCol)
        {
            Tile = tile;
            FromRow = fromRow; FromCol = fromCol;
            ToRow = toRow; ToCol = toCol;
        }
    }
    
    public readonly struct TileSpawnData
    {
        public readonly TileModel Tile;
        public readonly int SpawnCol;    // View'da hangi sütunun üstünden düşeceği

        public TileSpawnData(TileModel tile, int spawnCol)
        {
            Tile = tile;
            SpawnCol = spawnCol;
        }
    }

}
