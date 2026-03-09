using System;
using System.Collections.Generic;
using Match3Game.Domain.Board;
using Match3Game.Domain.Tiles;
using UnityEditor.PackageManager.Requests;

namespace Match3Game.Domain.Rules
{
    /// <summary>
    /// Özel taşların aktivasyon alanını hesaplar.
    /// Hangi tile'ların temizleneceğini döndürür -- yan etki yok.
    /// </summary>
    public class SpecialTileRule
    {
        public List<(int row, int col)> GetAffectedCells(
            BoardModel board,
            int row,
            int col,
            TileSpecialType specialType
        )
        {
            return specialType switch
            {
                TileSpecialType.Bomb => GetBombCells(board, row, col),
                TileSpecialType.RocketH => GetRocketHCells(board, row),
                TileSpecialType.RocketV => GetRocketVCells(board, col),
                TileSpecialType.ColorBomb => GetColorBombCells(board, row, col),
                _ => new List<(int, int)>()
            };
        }

        // ── Bomb: 3x3 ───────────────────────────────────────────────
        private List<(int, int)> GetBombCells(BoardModel board, int row, int col)
        {
            var cells = new List<(int,int)>();
            for(int r = row - 1; r <= row + 1; r++)
                for(int c = col - 1; c <= col + 1; c++)
                    if(board.IsPlayable(r, c))
                        cells.Add((r, c));
            return cells;
        }

        // ── Rocket Horizontal: Tüm satır ────────────────────────────
        private List<(int, int)> GetRocketHCells(BoardModel board, int row)
        {
            var cells = new List<(int,int)>();
            for(int c = 0; c < board.Cols; c++)
                if(board.IsPlayable(row, c))
                    cells.Add((row, c));
            return cells;
        }

        // ── Rocket Vertical: Tüm sütun ──────────────────────────────
        private List<(int, int)> GetRocketVCells(BoardModel board, int col)
        {
            var cells = new List<(int, int)>();
            for(int r = 0; r < board.Rows; r++)
                if(board.IsPlayable(r, col))
                    cells.Add((r, col));
            return cells;
        }

        // ── Color Bomb: Aynı renkteki tüm tile'lar ──────────────────
        private List<(int, int)> GetColorBombCells(BoardModel board, int row, int col)
        {
            var cells = new List<(int,int)>();
            var sourceTile = board.GetTile(row, col);
            if(sourceTile == null) return cells;

            // Color bomb aktive edildiğinde swap yapılan tile'ın rengi hedef olur.
            // Bu metod sadece alanı hesaplar, renk UseCase'den geçer
            var targetColor = sourceTile.Color;

            for(int r = 0; r < board.Rows; r++)
                for(int c = 0; c < board.Cols; c++)
                {
                    var tile = board.GetTile(r, c);
                    if(tile != null && tile.Color == targetColor)
                        cells.Add((r, c));
                }
            return cells;
        }

    }

}
