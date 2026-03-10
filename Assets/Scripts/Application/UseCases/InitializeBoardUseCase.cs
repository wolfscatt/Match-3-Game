using Match3Game.Application.Services;
using Match3Game.Domain.Board;
using Match3Game.Domain.Events;
using Match3Game.Domain.Rules;
using Match3Game.Infrastructure.Config;
using UnityEngine;

namespace Match3Game.Application.UseCases
{
    /// <summary>
    /// Level başında board'u hazırlar:
    /// generate -> goal init -> move init -> event yayar.
    /// </summary>
    public class InitializeBoardUseCase
    {
        private readonly BoardModel _board;
        private readonly BoardGenerator _generator;
        private readonly GoalService _goalService;
        private readonly MoveService _moveService;
        private readonly IEventBus _eventBus;
        private readonly LevelConfig _levelConfig;

        public InitializeBoardUseCase(BoardModel board, BoardGenerator generator, GoalService goalService, MoveService moveService, IEventBus eventBus, LevelConfig levelConfig)
        {
            _board = board;
            _generator = generator;
            _goalService = goalService;
            _moveService = moveService;
            _eventBus = eventBus;
            _levelConfig = levelConfig;
        }

        public void Execute()
        {
            // 1. Board'u doldur
            _generator.Generate(_board, _levelConfig.allowedColors);

            // 2. Servisleri başlat
            _goalService.Initialize(_levelConfig.BuildGoalData());
            _moveService.Initialize(_levelConfig.moveLimit);

            // 3. View'ı haberdar et
            _eventBus.Publish(new BoardInitializedEvent(_board.Rows, _board.Cols));
        }
    }

}
