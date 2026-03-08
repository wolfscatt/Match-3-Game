using System;
using System.Collections;
using System.Collections.Generic;
using Match3Game.Domain.Tiles;
using UnityEngine.Tilemaps;

namespace Match3Game.Domain.Board
{
    /// <summary>
    /// Board'un tüm veri katmanı.
    /// Tile erişimi, cell tip sorguları ve bounds kontrolü burada.
    /// </summary>
    public class BoardModel
    {
        // ── Properties ──────────────────────────────────────────────
        public int Rows {get;}
        public int Cols {get;}

        // ── Private State ──────────────────────────────────────────────
        private readonly TileModel[,] _tiles;
        private readonly CellType[,] _cellTypes;

        // ── Constructor ──────────────────────────────────────────────
        public BoardModel(int rows, int cols, CellType[,] cellTypes = null)
        {
            if( rows <= 0) throw new ArgumentException("Rows pozitif olmalı.", nameof(rows));
            if( cols <= 0) throw new ArgumentException("Cols pozitif olmalı.", nameof(cols));

            Rows = rows;
            Cols = cols;
            _tiles = new TileModel[Rows, Cols];
            _cellTypes = cellTypes ?? BuildDefaultCellTypes(rows, cols);
        }

        // ── Tile CRUD ───────────────────────────────────────────────
        public TileModel GetTile(int row, int col)
        {
            AssertInBounds(row, col);
            return _tiles[row, col];
        }
        public void SetTile(int row, int col, TileModel tile)
        {
            AssertInBounds(row, col);
            _tiles[row, col] = tile;

            if(tile != null)
            {
                tile.Row = row;
                tile.Col = col;
            }
        }

        public void ClearTile(int row, int col)
        {
            AssertInBounds(row, col);
            _tiles[row, col] = null;
        }

        public void SwapTiles(int r1, int c1, int r2, int c2)
        {
            AssertInBounds(r1, c1);
            AssertInBounds(r2, c2);

            (_tiles[r1, c1], _tiles[r2, c2]) = (_tiles[r2, c2], _tiles[r1, c1]);

            // Position'ları güncelle
            if(_tiles[r1, c1] != null){ _tiles[r1, c1].Row = r1; _tiles[r1, c1].Col = c1; }
            if(_tiles[r2, c2] != null){ _tiles[r2, c2].Row = r2; _tiles[r2, c2].Col = c2;}
        }

        // ── Cell Queries ────────────────────────────────────────────
        public CellType GetCellType(int row, int col) => _cellTypes[row, col];

        public bool IsPlayable(int row, int col) => 
            IsInBounds(row, col) && _cellTypes[row, col] == CellType.Normal;

        public bool IsOccupied(int row, int col) =>
            IsPlayable(row, col) && _tiles[row, col] != null;

        public bool IsEmpty(int row, int col) =>
            IsPlayable(row, col) && _tiles[row, col] == null;

        public bool IsInBounds(int row, int col) =>
            row >= 0 && row < Rows && col >= 0 && col < Cols;

        // ── Iteration ───────────────────────────────────────────────
        public IEnumerable<TileModel> AllTiles()
        {
            for(int r = 0; r < Rows; r++)
                for(int c = 0; c < Cols; c++)
                    if(_tiles[r, c] != null)
                        yield return _tiles[r, c];
        }

        public IEnumerable<(int row, int col)> AllPlayableCells()
        {
            for(int r = 0; r < Rows; r++)
                for( int c = 0; c < Cols; c++)
                    if(_cellTypes[r, c] == CellType.Normal)
                        yield return (r,c);
        }

        /// <summary> Sadece sistemlerin (GravitySystem vb.) doğrudan erişmesi için. </summary>
        public TileModel[,] GetRawGrid() => _tiles;

        // ── Private Helpers ─────────────────────────────────────────
        private void AssertInBounds(int row, int col)
        {
            if(!IsInBounds(row, col))
                throw new IndexOutOfRangeException($"({row},{col}) sınır dışı. Board boyutu: {Rows}x{Cols}");
        }

        private static CellType[,] BuildDefaultCellTypes(int rows, int cols)
        {
            var types = new CellType[rows, cols];
            for(int r = 0; r < rows; r++)
                for(int c = 0; c < cols; c++)
                    types[r, c] = CellType.Normal;
            return types;
        }
    }

}
