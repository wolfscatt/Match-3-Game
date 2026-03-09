using Match3Game.Domain.Events;

namespace Match3Game.Application.Services
{
    /// <summary>
    /// Hamle sayısını yönetir.
    /// </summary>
    public class MoveService
    {
        private readonly IEventBus _eventBus;

        public int RemainingMoves {get; private set;}
        public bool HasMoves => RemainingMoves > 0;

        public MoveService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Initialize(int moveLimit)
        {
            RemainingMoves = moveLimit;
        }

        public void ConsumeMove()
        {
            if(!HasMoves) return;

            RemainingMoves--;
            _eventBus.Publish(new MoveConsumedEvent(RemainingMoves));

            if(!HasMoves)
                _eventBus.Publish(new LevelLostEvent());
        }
    }

}
