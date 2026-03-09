namespace Match3Game.Application.StateMachine.States
{
    /// <summary>
    /// Tile'lar düşüyor, yenileri spawn oldu.
    /// </summary>
    public class FallState : IGameState
    {
        private readonly GameStateMachine _stateMachine;

        public FallState(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            // Gravity + refill uygulanır
            // Cascade match varsa → MatchState
            // Yoksa → IdleState
        }

        public void Exit()
        {
            
        }

        public void Tick()
        {
            
        }
    }

 
}
