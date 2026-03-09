using System;
using Match3Game.Domain.Events;

namespace Match3Game.Application.Services
{

    /// <summary>
    /// Skor hesaplama ve combo multiplier.
    /// </summary>
    public class ScoreService
    {
        private readonly IEventBus _eventBus;

        public int Score {get; private set;}
        public int ComboMultiplier {get; private set;} = 1;

        private const int BaseScorePerTile = 50;
        private const int ComboScoreBonus = 100;
        private const int MaxComboMultiplier = 8;

        public ScoreService(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<MatchFoundEvent>(OnMatchFound);
            _eventBus.Subscribe<TilesFellEvent>(OnTilesFell);
        }

        public void ResetCombo() => ComboMultiplier = 1;

        // ── Private ─────────────────────────────────────────────────
        
        private void OnMatchFound(MatchFoundEvent e)
        {
            int earned = 0;

            foreach(var group in e.Result.Groups)
            {
                int baseScore = group.Tiles.Count * BaseScorePerTile;
                earned += baseScore * ComboMultiplier;

                // 4+ Match bonus
                if(group.Tiles.Count >= 4)
                    earned += ComboScoreBonus;
            }

            Score += earned;

            // Combo artır (cascade için - gravity sonrası yeni match)
            ComboMultiplier = Math.Min(ComboMultiplier + 1, MaxComboMultiplier);

            _eventBus.Publish(new ScoreChangedEvent(Score));
        }

        private void OnTilesFell(TilesFellEvent _)
        {
            // Her gravity döngüsü combo'yu sıfırlar
            // Cascade match'lerde artmaya devam eder.
        }
    }

}
