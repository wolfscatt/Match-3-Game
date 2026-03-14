using Match3Game.Domain.Board;
using Match3Game.Domain.Match;
using Match3Game.Domain.Tiles;
using NUnit.Framework;

namespace Match3Game.Tests.Unit
{
    public class MatchFinderTests
    {
        private StandardMatchStrategy _strategy;

        [SetUp]
        public void SetUp()
        {
            _strategy = new StandardMatchStrategy();
        }

        // ── Helpers ──────────────────────────────────────────────────

        /// <summary>
        /// Char array'den board üretir.
        /// R=Red, B=Blur, G=Green, Y=Yellow, .=None
        /// </summary>
        private BoardModel BuildBoard(string[] rows)
        {
            int r = rows.Length;
            int c = rows[0].Length;
            var board = new BoardModel(r, c);

            for(int row = 0; row < r; row++)
                for(int col = 0; col < c; col++)
                {
                    var color = rows[row][col] switch
                    {
                        'R' => TileColor.Red,
                        'B' => TileColor.Blue,
                        'G' => TileColor.Green,
                        'Y' => TileColor.Yellow,
                        'P' => TileColor.Purple,
                        _ => TileColor.None
                    };

                    if(color != TileColor.None)
                        board.SetTile(row, col, new TileModel(color, row, col));
                }

            return board;
        }

        // ── No Match ─────────────────────────────────────────────────

        [Test]
        public void FindMatches_NoMatch_ReturnsEmpty()
        {
            var board = BuildBoard(new[]
            {
                "RBR",
                "BRB",
                "RBR"
            });

            var result = _strategy.FindMatches(board);
            Assert.IsFalse(result.HasMatch);
        }

        // ── Horizontal ───────────────────────────────────────────────

        [Test]
        public void FindMatches_ThreeInRow_DetectsMatch()
        {
            var board = BuildBoard(new[]
            {
                "RRR",
                "BGY",
                "YGB"
            });

            var result = _strategy.FindMatches(board);
            Assert.IsTrue(result.HasMatch);
            Assert.AreEqual(1, result.Groups.Count);
            Assert.AreEqual(3, result.Groups[0].Tiles.Count);
            Assert.AreEqual(TileColor.Red, result.Groups[0].Color);
        }

        [Test]
        public void FindMatches_FourInRow_DetectsCorrectShape()
        {
            var board = BuildBoard(new[]
            {
                "RRRR",
                "BGYP",
                "YGBR"
            });

            var result = _strategy.FindMatches(board);
            Assert.IsTrue(result.HasMatch);
            Assert.AreEqual(MatchShape.FourInRow, result.Groups[0].Shape);
        }

        [Test]
        public void FindMatches_FiveInRow_DetectsColorBombShape()
        {
            var board = BuildBoard(new[]
            {
                "RRRRR",
                "BGYPR",
                "YGBRP"
            });

            var result = _strategy.FindMatches(board);
            Assert.IsTrue(result.HasMatch);
            Assert.AreEqual(MatchShape.FiveInRow, result.Groups[0].Shape);
        }

        // ── Vertical ─────────────────────────────────────────────────

        [Test]
        public void FindMatches_ThreeInColumn_DetectsMatch()
        {
            var board = BuildBoard(new[]
            {
                "RGB",
                "RYP",
                "RGB"
            });

            var result = _strategy.FindMatches(board);
            Assert.IsTrue(result.HasMatch);
            Assert.AreEqual(TileColor.Red, result.Groups[0].Color);
        }

        // ── L / T Shape ──────────────────────────────────────────────

        [Test]
        public void FindMatches_LShape_DetectedAsSingleGroup()
        {
            // R R R
            // R . .
            // R . .
            var board = BuildBoard(new[]
            {
                "RRR",
                "RBG",
                "RPY"
            });

            var result = _strategy.FindMatches(board);
            Assert.IsTrue(result.HasMatch);

            // L Shape -- tek group, 5 tile
            Assert.AreEqual(1, result.Groups.Count);
            Assert.AreEqual(5, result.Groups[0].Tiles.Count);
        }

        // ── Special Tile Spawn ────────────────────────────────────────

        [Test]
        public void MatchGroup_FourInRow_ShouldSpawnSpecial()
        {
            var board = BuildBoard(new[]
            {
                "RRRR",
                "BGYP",
                "YGBR"
            });

            var result = _strategy.FindMatches(board);
            Assert.IsTrue(result.Groups[0].ShouldSpawnSpecial);
            Assert.AreEqual(TileSpecialType.RocketH, result.Groups[0].SpawnedSpecialType);

        }

        [Test]
        public void FindMatches_ThereeInRow_ShouldNotSpawnSpecial()
        {
            var board = BuildBoard(new[]
            {
                "RRR",
                "BGY",
                "YGB"
            });

            var result = _strategy.FindMatches(board);
            Assert.IsFalse(result.Groups[0].ShouldSpawnSpecial);
        }

        // ── Multiple Groups ───────────────────────────────────────────

        [Test]
        public void FindMatches_TwoSeparateGroups_BothDetected()
        {
            var board = BuildBoard(new[]
            {
                "RRRBBB",
                "GYGYGY",
                "PYGPYG"
            });
            
            var result = _strategy.FindMatches(board);
            Assert.AreEqual(2, result.Groups.Count);
        
        }

    }

}
