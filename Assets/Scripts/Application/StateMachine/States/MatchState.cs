namespace Match3Game.Application.StateMachine.States
{
    /// <summary>
    /// Match'ler temizleniyor, special tile'lar aktive ediliyor.
    /// </summary>
    public class MatchState : IGameState
    {
        private readonly GameStateMachine _stateMachine;

        public MatchState(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            // Animasyon tamamlanınca FallState'e geçilir
            // Presentation katmanı bu event'i tetikler
        }

        public void Exit()
        {
           
        }

        public void Tick()
        {
            
        }
    }

}
