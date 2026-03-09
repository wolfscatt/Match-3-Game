using System;
using System.Collections.Generic;
using Match3Game.Domain.Common;
using Match3Game.Domain.Events;
using Match3Game.Domain.Tiles;
using UnityEngine;

namespace Match3Game.Application.Services
{
    /// <summary>
    /// Level hedeflerini takip eder.
    /// EventBus üzerinden match eventlerini dinler.
    /// </summary>
    public class GoalService : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly Dictionary<TileColor, GoalEntry> _goals = new();

        public bool AllGoalsCompleted
        {
            get
            {
                foreach(var g in _goals.Values)
                    if(!g.IsCompleted) return false;
                return true;
            }
        }

        public GoalService(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<MatchFoundEvent>(OnMatchFound);
            _eventBus.Subscribe<SpecialTileActivatedEvent>(OnSpecialActivated);        
        }

        public void Initialize(IEnumerable<GoalData> goals)
        {
            _goals.Clear();
            foreach(var g in goals)
                _goals[g.TargetColor] = new GoalEntry(g.TargetColor, g.RequiredCount);
        }

        public IEnumerable<GoalEntry> GetAllGoals() => _goals.Values;

        public int GetRemaining(TileColor color) =>
            _goals.TryGetValue(color, out var entry) ? entry.Remaining : 0;

        // ── Event Handlers ───────────────────────────────────────────

        private void OnMatchFound(MatchFoundEvent e)
        {
            foreach(var group in e.Result.Groups)
            {
                if(!_goals.TryGetValue(group.Color, out var entry)) continue;

                int cleared = group.Tiles.Count;
                entry.AddProgress(cleared);

                _eventBus.Publish(new GoalProgressEvent(group.Color, cleared));

                if(AllGoalsCompleted)
                    _eventBus.Publish(new LevelWonEvent());
            }
        }

        private void OnSpecialActivated(SpecialTileActivatedEvent e)
        {
            // Special tile aktivasyonları da goal'a sayılır
            // Detaylı implementasyon UseCase'de
        }
        public void Dispose()
        {
            _eventBus.Unsubscribe<MatchFoundEvent>(OnMatchFound);
            _eventBus.Unsubscribe<SpecialTileActivatedEvent>(OnSpecialActivated);
        }

    }

    public class GoalEntry
    {
        public TileColor Color {get;}
        public int Required {get;}
        public int Current {get; private set;}
        public int Remaining => Math.Max(0, Required - Current);
        public bool IsCompleted => Current >= Required;

        public GoalEntry(TileColor color, int required)
        {
            Color = color;
            Required = required;
        }

        public void AddProgress(int amount) => Current = Math.Min(Required, Current + amount);
    }

}
