using Match3Game.Domain.Board;
using Match3Game.Domain.Rules;
using Match3Game.Domain.Tiles;
using NUnit.Framework;
using Unity.Android.Gradle.Manifest;

namespace Match3Game.Tests.Unit
{
    public class GravitySystemTests
    {
        private GravitySystem _gravity;
        private BoardModel _board;

        [SetUp]
        public void SetUp()
        {
            _gravity = new GravitySystem();
            _board = new BoardModel(4, 4);
        }

        // ── Fall ─────────────────────────────────────────────────────

        [Test]
        public void ApplyGravity_TileAboveEmpty_FallsDown()
        {
            // Row 0'da tile var, row 1-3 boş
            var tile = new TileModel(TileColor.Red, 0, 0);
            _board.SetTile(0, 0, tile);

            _gravity.ApplyGravity(_board);

            // Tile en alta düşmeli (row 3)
            Assert.IsNull(_board.GetTile(0, 0));
            Assert.AreEqual(tile, _board.GetTile(3, 0));
        }

        [Test]
        public void ApplyGravity_NoEmptyBelow_TileStays()
        {
            var top = new TileModel(TileColor.Red, 0, 0);
            var bottom = new TileModel(TileColor.Blue, 3, 0);
            _board.SetTile(0, 0, top);
            _board.SetTile(3, 0, bottom);

            _gravity.ApplyGravity(_board);

            // Top tile row'2 ye düşmeli
            Assert.AreEqual(top, _board.GetTile(2, 0));
            Assert.AreEqual(bottom, _board.GetTile(3, 0));
        }

        [Test]
        public void ApplyGravity_ReturnsMoveData_ForMovedTiles()
        {
            var tile = new TileModel(TileColor.Red, 0, 0);
            _board.SetTile(0, 0, tile);

            var moves = _gravity.ApplyGravity(_board);

            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(0, moves[0].FromRow);
            Assert.AreEqual(3, moves[0].ToRow);
        }

        [Test]
        public void ApplyGravity_AlreadyAtBottom_NoMoves()
        {
            _board.SetTile(3, 0, new TileModel(TileColor.Red, 3, 0));
            _board.SetTile(3, 1, new TileModel(TileColor.Blue, 3, 1));

            var moves = _gravity.ApplyGravity(_board);

            Assert.AreEqual(0, moves.Count);
        }

        // ── Column Order Preserved ────────────────────────────────────

        [Test]
        public void ApplyGravity_MultiTileColumn_OrderPreserved()
        {
            // Row 0: Red, Row 2: Blue -- arası boş
            var red = new TileModel(TileColor.Red, 0, 0);
            var blue = new TileModel(TileColor.Blue, 2, 0);
            _board.SetTile(0, 0, red);
            _board.SetTile(2, 0, blue);

            _gravity.ApplyGravity(_board);

            // Blue altta, Red üstte kalmalı
            Assert.AreEqual(blue, _board.GetTile(3, 0));
            Assert.AreEqual(red, _board.GetTile(2, 0));
        }

        // ── Fill ──────────────────────────────────────────────────────

        [Test]
        public void FillEmpty_EmptyCells_AllFilled()
        {
            var colors = new [] {TileColor.Red, TileColor.Blue};
            var random = new System.Random(42);

            _gravity.FillEmpty(_board, colors, random);

            // Tüm hücreler dolu olmalı
            foreach(var tile in _board.AllTiles())
                Assert.IsNotNull(tile);

            int count = 0;
            foreach(var _ in _board.AllTiles()) count++;
            Assert.AreEqual(16, count);  // 4x4
        }

        [Test]
        public void FillEmpty_OnlyFillsEmptyCells_ExistingTilesUntouched()
        {
            var existing = new TileModel(TileColor.Purple, 3, 3);
            _board.SetTile(3, 3, existing);

            var colors = new[] { TileColor.Red, TileColor.Blue };
            _gravity.FillEmpty(_board, colors, new System.Random(0));

            Assert.AreEqual(existing, _board.GetTile(3, 3));
        }
    }

}
