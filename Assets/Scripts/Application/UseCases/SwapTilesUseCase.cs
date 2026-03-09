using System;
using Match3Game.Application.DTOs;
using Match3Game.Application.Services;
using Match3Game.Domain.Board;
using Match3Game.Domain.Events;
using Match3Game.Domain.Match;

namespace Match3Game.Application.UseCases
{
    /// <summary>
    /// Swap isteğini alır -> validate -> execute -> event yayar.
    /// Single Responsibility: Sadece swap mantığı.
    /// </summary>
    public class SwapTilesUseCase
    {
        private readonly BoardModel _board;
        private readonly IMatchStrategy _matchStrategy;
        private readonly MoveService _moveService;
        private readonly IEventBus _eventBus;

        public SwapTilesUseCase(
            BoardModel board, 
            IMatchStrategy matchStrategy, 
            MoveService moveService, 
            IEventBus eventBus)
        {
            _board = board;
            _matchStrategy = matchStrategy;
            _moveService = moveService;
            _eventBus = eventBus;
        }

        public SwapResult Execute(int r1, int c1, int r2, int c2)
        {
            // Komşu kontrolü
            if(!AreAdjacent(r1, c1, r2, c2))
                return SwapResult.Invalid("Komşu Değil.");

            // Swap
            _board.SwapTiles(r1, c1, r2, c2);

            var matchResult = _matchStrategy.FindMatches(_board);

            if (!matchResult.HasMatch)
            {
                // Geri Al
                _board.SwapTiles(r1, r1, c1, c2);
                _eventBus.Publish(new SwapRejectedEvent(r1, c1, r2, c2));
                return SwapResult.Invalid("Match yok.");
            }

            // Geçerli hamle
            _moveService.ConsumeMove();
            _eventBus.Publish(new TilesSwappedEvent(r1, c1, r2, c2));
            _eventBus.Publish(new MatchFoundEvent(matchResult));

            return SwapResult.Valid(matchResult);
        }

        private bool AreAdjacent(int r1, int c1, int r2, int c2) =>
            (Math.Abs(r1 - r2) + Math.Abs(c1 - c2)) == 1;
        

    }

}
