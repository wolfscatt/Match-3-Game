using Match3Game.Domain.Events;
using Match3Game.Domain.Tiles;
using UnityEngine;
using VContainer;

namespace Match3Game.Presentation.VFX
{
    /// <summary>
    /// Match, special tile ve combo efektlerini tetikler.
    /// EventBus'ı dinler -- tamamen decoupled.
    /// </summary>
    public class VFXService : MonoBehaviour
    {
        [Header("Particle Prefabs")]
        [SerializeField] private ParticleSystem _matchParticlePrefab;
        [SerializeField] private ParticleSystem _bombParticlePrefab;
        [SerializeField] private ParticleSystem _rocketParticlePrefab;
        [SerializeField] private ParticleSystem _colorBombParticlePrefab;
        [SerializeField] private ParticleSystem _comboParticlePrefab;

        [Header("Color Map")]
        [SerializeField] private TileColorParticle[] _colorParticles;

        private IEventBus _eventBus;

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _eventBus?.Subscribe<MatchFoundEvent>(OnMatchFound);
            _eventBus?.Subscribe<SpecialTileActivatedEvent>(OnSpecialActivated);
        }

        private void OnDisable()
        {
            _eventBus?.Unsubscribe<MatchFoundEvent>(OnMatchFound);
            _eventBus?.Unsubscribe<SpecialTileActivatedEvent>(OnSpecialActivated);
        }

        // ── Event Handlers ───────────────────────────────────────────

        private void OnMatchFound(MatchFoundEvent e)
        {
            foreach (var group in e.Result.Groups)
            {
                foreach (var tile in group.Tiles)
                {
                    var pos = new Vector3(tile.Col, -tile.Row, 0f);
                    PlayMatchParticle(pos, tile.Color);
                }
            }
        }

        private void OnSpecialActivated(SpecialTileActivatedEvent e)
        {
            var pos = new Vector3(e.Col, -e.Row, 0f);

            var prefab = e.SpecialType switch
            {
                TileSpecialType.Bomb      => _bombParticlePrefab,
                TileSpecialType.RocketH   => _rocketParticlePrefab,
                TileSpecialType.RocketV   => _rocketParticlePrefab,
                TileSpecialType.ColorBomb => _colorBombParticlePrefab,
                _                         => null
            };

            if (prefab != null)
                PlayParticle(prefab, pos);
        }

        // ── Private ──────────────────────────────────────────────────

        private void PlayMatchParticle(Vector3 position, TileColor color)
        {
            if (_matchParticlePrefab == null) return;

            var ps = PlayParticle(_matchParticlePrefab, position);

            // Renk override
            foreach (var entry in _colorParticles)
            {
                if (entry.color != color) continue;
                var main  = ps.main;
                main.startColor = entry.particleColor;
                break;
            }
        }

        private ParticleSystem PlayParticle(ParticleSystem prefab, Vector3 position)
        {
            var ps = Instantiate(prefab, position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
            return ps;
        }
    }

    [System.Serializable]
    public struct TileColorParticle
    {
        public TileColor color;
        public Color     particleColor;
    }
}

