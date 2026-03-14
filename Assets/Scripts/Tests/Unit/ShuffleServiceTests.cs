using Match3Game.Domain.Board;
using Match3Game.Domain.Match;
using Match3Game.Domain.Rules;
using Match3Game.Domain.Tiles;
using NUnit.Framework;

namespace Match3Game.Tests.Unit
{
    public class ShuffleServiceTests
    {
        private ShuffleService _shuffleService;
        private StandardMatchStrategy _strategy;

        [SetUp]
        public void SetUp()
        {
            _strategy = new StandardMatchStrategy();
            _shuffleService = new ShuffleService(_strategy, new System.Random(42));
        }

        // ── Deadlock Detection ────────────────────────────────────────

        [Test]
        public void HasValidMove_BoardWithObviousMove_ReturnsTrue()
        {
            // İki aynı tenkli tile yan yana -- swap -> match
            // R R B
            // B R B
            // G G G
            var board = new BoardModel(3, 3);
            board.SetTile(0, 0, new TileModel(TileColor.Red, 0, 0));
            board.SetTile(0, 1, new TileModel(TileColor.Red, 0, 1));
            board.SetTile(0, 2, new TileModel(TileColor.Blue, 0, 2));
            board.SetTile(1, 0, new TileModel(TileColor.Blue, 1, 0));
            board.SetTile(1, 1, new TileModel(TileColor.Red, 1, 1)); // swap (0,2)↔(1,2) → match
            board.SetTile(1, 2, new TileModel(TileColor.Blue, 1, 2));
            board.SetTile(2, 0, new TileModel(TileColor.Green, 2, 0));
            board.SetTile(2, 1, new TileModel(TileColor.Green, 2, 1));
            board.SetTile(2, 2, new TileModel(TileColor.Green, 2, 2));

            Assert.IsTrue(_shuffleService.HasValidMove(board));
        }

        [Test]
        public void HasValidMove_DeadlockedBoard_ReturnsFalse()
        {
            // Checkerboard -- hiçbir swap match yapmaz
            var board = new BoardModel(4, 4);
            var colors = new[] { TileColor.Red, TileColor.Blue};

            for(int r = 0; r < 4; r++)
                for(int c = 0; c < 4; c++)
                {
                    var color = (r + c) % 2 == 0 ? TileColor.Red : TileColor.Blue;
                    board.SetTile(r, c, new TileModel(color, r, c));
                }
            
            Assert.IsFalse(_shuffleService.HasValidMove(board));
        }

        // ── Shuffle ───────────────────────────────────────────────────

        [Test]
        public void Shuffle_AfterDeadlock_BoardHasValidMove()
        {
            // Deadlocked Checkerboard
            var board = new BoardModel(4, 4);
            for(int r = 0; r < 4; r++)
                for(int c = 0; c < 4; c++)
                {
                    var color = (r + c) % 2 == 0 ? TileColor.Red : TileColor.Blue;
                    board.SetTile(r, c, new TileModel(color, r, c));
                }

            _shuffleService.Shuffle(board);

            Assert.IsTrue(_shuffleService.HasValidMove(board));
        }

        [Test]
        public void Shuffle_TileCountPreserved()
        {
            var board = new BoardModel(4, 4);
            for(int r = 0; r < 4; r++)
                for(int c = 0; c < 4; c++)
                    board.SetTile(r, c, new TileModel(TileColor.Red, r, c));

            _shuffleService.Shuffle(board);

            int count = 0;
            foreach(var _ in board.AllTiles()) count ++;
            Assert.AreEqual(16, count);
        }
    }

}
