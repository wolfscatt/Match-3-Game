using System;
using System.Collections.Generic;

namespace Match3Game.Application.StateMachine
{

    public class GameStateMachine
    {
        private IGameState _current;    
        private readonly Dictionary<Type, IGameState> _states = new();

        public void RegisterState<T>(T state) where T : IGameState => 
            _states[typeof(T)] = state;
        
        public void ChangeState<T>() where T : IGameState
        {
            if(!_states.TryGetValue(typeof(T), out var next))
                throw new InvalidOperationException($"{typeof(T).Name} Kayıtlı değil.");

            _current?.Exit();
            _current = next;
            _current.Enter();
        }

        public void Tick() => _current?.Tick();

        public bool IsIn<T>() where T : IGameState =>
            _current is T;
    }

}
