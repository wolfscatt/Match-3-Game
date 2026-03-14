using Match3Game.Application.Services;
using Match3Game.Application.UseCases;
using Match3Game.Domain.Board;
using Match3Game.Domain.Events;
using Match3Game.Domain.Match;
using Match3Game.Domain.Tiles;
using NUnit.Framework;
using UnityEngine;

namespace Match3Game.Tests.Integration
{
    /// <summary>
    /// SwapTileUseCase -- EventBus + BoardModel + MoveService entegrasyonu.
    /// </summary>
    public class SwapTilesUseCaseTests
    {
        private BoardModel _board;
        private EventBus _eventBus;
        private MoveService _moveService;
        private SwapTilesUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _board = new BoardModel(5, 5);
            _eventBus = new EventBus();
            _moveService = new MoveService(_eventBus);
            _moveService.Initialize(30);

            _useCase = new SwapTilesUseCase(_board, new StandardMatchStrategy(), _moveService, _eventBus);
        }

        // ── Valid Swap ────────────────────────────────────────────────

        [Test]
        public void Execute_ValidSwapWithMatch_ReturnsValid()
        {
            // R R . R R -- (0,2)'ye R getirince 5'li match
            _board.SetTile(0, 0, new TileModel(TileColor.Red, 0, 0));
            _board.SetTile(0, 1, new TileModel(TileColor.Red, 0, 1));
            _board.SetTile(0, 2, new TileModel(TileColor.Blue, 0, 2)); // swap edilecek
            _board.SetTile(0, 3, new TileModel(TileColor.Red, 0, 3));
            _board.SetTile(0, 4, new TileModel(TileColor.Red, 0, 4));
            _board.SetTile(1, 2, new TileModel(TileColor.Red, 1, 2)); // swap kaynağı

            var result = _useCase.Execute(1, 2, 0, 2);

            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(result.Match.HasMatch);
        }

        [Test]
        public void Execute_ValidSwap_ConsumesMove()
        {
            SetupSimpleMatchableBoard();
            int before = _moveService.RemainingMoves;

            _useCase.Execute(0, 0, 0, 1);

            Assert.AreEqual(before - 1, _moveService.RemainingMoves);
        }

        // ── Invalid Swap ──────────────────────────────────────────────

        [Test]
        public void Execute_SwapWithNoMatch_ReturnsInvalid()
        {
            _board.SetTile(0, 0, new TileModel(TileColor.Red, 0, 0));
            _board.SetTile(0, 1, new TileModel(TileColor.Blue, 0, 1));

            var result = _useCase.Execute(0, 0, 0, 1);

            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void Execute_SwapWithNoMatch_BoardReverted()
        {
            var red = new TileModel(TileColor.Red, 0, 0);
            var blue = new TileModel(TileColor.Blue, 0, 1);
            _board.SetTile(0, 0, red);
            _board.SetTile(0, 1, blue);

            _useCase.Execute(0, 0, 0, 1);

            // Board geri alınmış olmalı
            Assert.AreEqual(red, _board.GetTile(0, 0));
            Assert.AreEqual(blue, _board.GetTile(0, 1));
        }

        [Test]
        public void Execute_NonAdjacentSwap_ReturnsInvalid()
        {
            _board.SetTile(0, 0, new TileModel(TileColor.Red, 0, 0));
            _board.SetTile(0, 3, new TileModel(TileColor.Blue, 0, 3));

            var result = _useCase.Execute(0, 0, 0, 3) ;

            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void Execute_InvalidSwap_DoesNotConsumeMove()
        {
            _board.SetTile(0, 0, new TileModel(TileColor.Red,  0, 0));
            _board.SetTile(0, 1, new TileModel(TileColor.Blue, 0, 1));
            int before = _moveService.RemainingMoves; 

            _useCase.Execute(0, 0, 0, 1);

            Assert.AreEqual(before, _moveService.RemainingMoves);
        }

        // ── Events ────────────────────────────────────────────────────

        [Test]
        public void Execute_ValidSwap_PublishesMatchFoundEvent()
        {
            bool eventReceived = false;
            _eventBus.Subscribe<MatchFoundEvent>( _ => eventReceived = true);

            SetupSimpleMatchableBoard();
            _useCase.Execute(0, 0, 0, 1);

            Assert.IsTrue(eventReceived);
        }

        [Test]
        public void Execute_InvalidSwap_PublishesSwapRejectedEvent()
        {
            bool eventReceived = false;
            _eventBus.Subscribe<SwapRejectedEvent>( _ => eventReceived = true);

            _board.SetTile(0, 0, new TileModel(TileColor.Red, 0, 0));
            _board.SetTile(0, 1, new TileModel(TileColor.Blue, 0, 1));

            _useCase.Execute(0, 0, 0, 1);

            Assert.IsTrue(eventReceived);
        }

        // ── Helpers ───────────────────────────────────────────────────

        private void SetupSimpleMatchableBoard()
        {
            // R R B -> swap (0,1)↔(0,2) -> R R R match yok
            // Daha net: 3 kırmızı yan yana olacak şekilde
            // R R
            // B R
            // R B
            _board.SetTile(0, 0, new TileModel(TileColor.Red,  0, 0));
            _board.SetTile(0, 1, new TileModel(TileColor.Red,  0, 1));
            _board.SetTile(1, 0, new TileModel(TileColor.Blue, 1, 0));
            _board.SetTile(1, 1, new TileModel(TileColor.Red,  1, 1));
            _board.SetTile(2, 0, new TileModel(TileColor.Red,  2, 0));
            _board.SetTile(2, 1, new TileModel(TileColor.Blue, 2, 1));

            // (1,0) Blue ↔ (1,1) Red swap → col 0'da R,B,R yok
            // Basit senaryo: dikey 3 Red
            // . . R
            // . . R
            // . . B
            // . . R
            _board.SetTile(0, 2, new TileModel(TileColor.Red, 0, 2));
            _board.SetTile(1, 2, new TileModel(TileColor.Red, 1, 2));
            _board.SetTile(2, 2, new TileModel(TileColor.Blue, 2, 2));
            _board.SetTile(3, 2, new TileModel(TileColor.Red, 3, 2));
        }
    }

}
