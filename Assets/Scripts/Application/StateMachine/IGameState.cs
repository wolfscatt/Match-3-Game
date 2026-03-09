using UnityEngine;

namespace Match3Game.Application.StateMachine
{
    public interface IGameState
    {
        void Enter();
        void Exit();
        void Tick();
    }

}
