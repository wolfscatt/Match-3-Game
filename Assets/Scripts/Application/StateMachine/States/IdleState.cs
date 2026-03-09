using Match3Game.Domain.Events;
using UnityEngine;

namespace Match3Game.Application.StateMachine.States
{
    /// <summary>
    /// Oyuncu input bekliyor.
    /// </summary>
    public class IdleState : IGameState
    {
        private readonly IEventBus _eventBus;

        public IdleState(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        public void Enter()
        {
            // Input'u aktif et
        }

        public void Exit()
        {
            // Input'u pasif et
        }

        public void Tick()
        {
           
        }

    }

}
