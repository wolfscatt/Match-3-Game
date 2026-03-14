using System;
using Match3Game.Domain.Board;
using Match3Game.Domain.Tiles;
using NUnit.Framework;
using UnityEngine.Tilemaps;

namespace Match3Game.Tests.Unit
{
    public class BoardModelTests
    {
        private BoardModel _board;

        [SetUp]
        public void SetUp()
        {
            _board = new BoardModel(8, 8);
        }

        // ── Bounds ───────────────────────────────────────────────────

        [Test]
        public void IsInBounds_ValidCell_ReturnsTrue()
        {
            Assert.IsTrue(_board.IsInBounds(0, 0));
            Assert.IsTrue(_board.IsInBounds(7, 7));
            Assert.IsTrue(_board.IsInBounds(4, 4));
        }

        [Test]
        public void IsInBounds_InvalidCell_ReturnsFalse()
        {
            Assert.IsFalse(_board.IsInBounds(-1, 0));
            Assert.IsFalse(_board.IsInBounds(0, -1));
            Assert.IsFalse(_board.IsInBounds(8, 0));
            Assert.IsFalse(_board.IsInBounds(0, 8));
        }

        [Test]
        public void GetTile_OutOfBounds_ThrowsException()
        {
            Assert.Throws<IndexOutOfRangeException>(() => _board.GetTile(-1, 0));
        }

        // ── Tile CRUD ────────────────────────────────────────────────

        [Test]
        public void SetTile_UpdatesTilePosition()
        {
            var tile = new TileModel(TileColor.Red, 0, 0);
            _board.SetTile(3, 5, tile);

            Assert.AreEqual(3, tile.Row);
            Assert.AreEqual(5, tile.Col);
        }

        [Test]
        public void SetTile_ThenGetTile_ReturnsSameTile()
        {
            var tile = new TileModel(TileColor.Blue, 2, 2);
            _board.SetTile(2, 2, tile);

            Assert.AreEqual(tile, _board.GetTile(2, 2));
        }

        [Test]
        public void ClearTile_MakesSlotEmpty()
        {
            var tile = new TileModel(TileColor.Red, 0, 0);
            _board.SetTile(0, 0, tile);
            _board.ClearTile(0, 0);

            Assert.IsNull(_board.GetTile(0, 0));
            Assert.IsTrue(_board.IsEmpty(0, 0));
        }

        // ── Swap ─────────────────────────────────────────────────────

        [Test]
        public void SwapTiles_ExchangesTilePositions()
        {
            var red = new TileModel(TileColor.Red, 0, 0);
            var blue = new TileModel(TileColor.Blue, 0, 1);
            _board.SetTile(0, 0, red);
            _board.SetTile(0, 1, blue);

            _board.SwapTiles(0, 0, 0, 1);

            Assert.AreEqual(blue, _board.GetTile(0, 0));
            Assert.AreEqual(red, _board.GetTile(0, 1));
        }

        [Test]
        public void SwapTiles_UpdatesTileRowColProperties()
        {
            var red = new TileModel(TileColor.Red, 0, 0);
            var blue = new TileModel(TileColor.Blue, 0, 1);
            _board.SetTile(0, 0, red);
            _board.SetTile(0, 1, blue);

            _board.SwapTiles(0, 0, 0, 1);

            Assert.AreEqual(0, blue.Row); Assert.AreEqual(0, blue.Col);
            Assert.AreEqual(0, red.Row); Assert.AreEqual(1, red.Col);
        }

        // ── Cell Types ───────────────────────────────────────────────

        [Test]
        public void BlockedCell_IsNotPlayable()
        {
            var cellTypes = new CellType[3, 3];
            cellTypes[1, 1] = CellType.Blocked;
            var board = new BoardModel(3, 3, cellTypes);

            Assert.IsFalse(board.IsPlayable(1, 1));
        }

        [Test]
        public void AllTiles_ReturnsOnlyNonNullTiles()
        {
            _board.SetTile(0, 0, new TileModel(TileColor.Red, 0, 0));
            _board.SetTile(1, 1, new TileModel(TileColor.Blue, 1, 1));

            int count = 0;
            foreach(var _ in _board.AllTiles()) count ++;

            Assert.AreEqual(2, count);
        }
    }

}
