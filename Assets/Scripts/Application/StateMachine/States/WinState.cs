namespace Match3Game.Application.StateMachine.States
{
    public class WinState : IGameState
    {
        public void Enter() { /* Win popup tetikle */ }
        public void Exit()  { }
        public void Tick()  { }
    }

    public class LoseState : IGameState
    {
        public void Enter() { /* Lose popup tetikle */ }
        public void Exit()  { }
        public void Tick()  { }
    }
 
}
