using Match3Game.Application.UseCases;
using Match3Game.Domain.Events;

namespace Match3Game.Application.StateMachine.States
{
    /// <summary>
    /// İki tile swap ediliyor, match kontrol ediliyor.
    /// </summary>
    public class SwapState : IGameState
    {
        private readonly SwapTilesUseCase _swapUseCase;
        private readonly GameStateMachine _stateMachine;
        private readonly IEventBus _eventBus;

        private int _r1, _c1, _r2, _c2;

        public SwapState(
            SwapTilesUseCase swapUseCase,
            GameStateMachine stateMachine,
            IEventBus eventBus
        )
        {
            _swapUseCase = swapUseCase;
            _stateMachine = stateMachine;
            _eventBus = eventBus;
        }

        public void SetSwap(int r1, int c1, int r2, int c2)
        {
            _r1 = r1; _c1 = c1;
            _r2 = r2; _c2 = c2;
        }
        public void Enter()
        {
            var result = _swapUseCase.Execute(_r1, _c1, _r2, _c2);

            if(result.IsValid)
                _stateMachine.ChangeState<MatchState>();
            else
                _stateMachine.ChangeState<IdleState>();
        }

        public void Exit()
        {
            
        }

        public void Tick()
        {
            
        }
    }

}
