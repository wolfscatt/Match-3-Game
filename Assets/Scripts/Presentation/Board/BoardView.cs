using System;
using System.Collections.Generic;
using Match3Game.Domain.Board;
using Match3Game.Domain.Events;
using Match3Game.Domain.Match;
using Match3Game.Domain.Rules;
using Match3Game.Infrastructure.Factories;
using UnityEngine;
using VContainer;

namespace Match3Game.Presentation.Board
{
    /// <summary>
    /// BoardModel'in görsel temsili.
    /// TileView'ları yönetir, animasyonları koordine eder.
    /// EventBus'ı dinler -- domain'e bağımlılık yok.
    /// </summary>
    public class BoardView : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private float _tileSize = 1.1f;
        [SerializeField] private float _tileOffset = 0.05f;

        // ── Injected ─────────────────────────────────────────────────
        private TileFactory _tileFactory;
        private IEventBus _eventBus;

        // ── State ────────────────────────────────────────────────────
        private TileView[,] _views;
        private int _rows, _cols;

        // Animasyon tamamlanınca dışarıya bildir
        public event Action OnAnimationComplete;

        // ── VContainer Inject ────────────────────────────────────────

        [Inject]
        public void Construct(TileFactory tileFactory, IEventBus eventBus)
        {
            _tileFactory = tileFactory;
            _eventBus = eventBus;
        }

        // ── Public API ───────────────────────────────────────────────

        public void Initialize(int rows, int cols)
        {
            _rows = rows;
            _cols = cols;
            _views = new TileView[rows, cols];
        }

        /// <summary>
        /// BoardInitializedEvent'ten sonra tüm view'ları oluşturur.
        /// </summary>
        public void SpawnAllTiles(BoardModel board, TileViewConfig config)
        {
            for(int r = 0; r < _rows; r++)
            {
                for(int c = 0; c < _cols; c++)
                {
                    var model = board.GetTile(r, c);
                    if(model == null) continue;

                    var worldPos = GetWorldPosition(r, c);
                    var spawnPos = worldPos + Vector3.up * (_rows * _tileSize);

                    var view = _tileFactory.Create(model, worldPos);

                    _views[r, c] = view;
                    view.AnimateSpawn(spawnPos, worldPos);
                }
            }
        }

        /// <summary>
        /// Gravity sonrası tile'ları yeni konumlarına taşır.
        /// </summary>
        public void AnimateFalls(List<TileMoveData> moves, Action onComplete)
        {
            if(moves.Count == 0) { onComplete?.Invoke(); return; }

            int pending = moves.Count;

            foreach(var move in moves)
            {
                var view = _views[move.FromRow, move.FromCol];
                if(view == null) { Decrement(); continue; }

                // View matrisini güncelle
                _views[move.ToRow, move.ToCol] = view;
                _views[move.FromRow, move.FromCol] = null;

                var targetPos = GetWorldPosition(move.ToRow, move.ToCol);
                view.AnimateFall(targetPos, Decrement);
            }

            void Decrement()
            {
                if(--pending <= 0) onComplete?.Invoke();
            }
        }

        /// <summary>
        /// Match olan tile'ları yok eder.
        /// </summary>
        public void AnimateMatches(IEnumerable<MatchGroup> groups, Action onComplete)
        {
            var toDestroy = new HashSet<TileView>();

            foreach(var group in groups)
                foreach(var tile in group.Tiles)
                {
                    var view = _views[tile.Row, tile.Col];
                    if(view != null) toDestroy.Add(view);
                }

            if(toDestroy.Count == 0) { onComplete?.Invoke(); return; }

            int pending = toDestroy.Count;

            foreach(var view in toDestroy)
            {
                int r = view.Model.Row;
                int c = view.Model.Col;
                _views[r, c] = null;

                view.AnimateMatch(() =>
                {
                    _tileFactory.Return(view);
                    if(--pending <= 0) onComplete?.Invoke();
                });
            }
        }

        /// <summary>
        /// Yeni spawn olan tile'ları gösterir.
        /// </summary>
        public void AnimateSpawns(List<TileSpawnData> spawns, TileViewConfig config)
        {
            foreach(var spawn in spawns)
            {
                var model = spawn.Tile;
                var worldPos = GetWorldPosition(model.Row, model.Col);
                var spawnPos = GetWorldPosition(0, spawn.SpawnCol) + Vector3.up * _tileSize;

                var view = _tileFactory.Create(model, worldPos);

                _views[model.Row, model.Col] = view;
                view.AnimateSpawn(spawnPos, worldPos);
            }
        }

        public void AnimateSwap(int r1, int c1, int r2, int c2, Action onComplete)
        {
            var view1 = _views[r1, c1];
            var view2 = _views[r2, c2];
            if(view1 == null || view2 == null) { onComplete?.Invoke(); return; }

            int pending = 2;

            (_views[r1, c1], _views[r2, c2]) = (_views[r2, c2], _views[r1, c1]);

            view1.AnimateSwap(GetWorldPosition(r2, c2), Decrement);
            view2.AnimateSwap(GetWorldPosition(r1, c1), Decrement);

            void Decrement()
            {
                if(--pending <= 0) onComplete?.Invoke();
            }
        }

        public void AnimateInvalidSwap(int r1, int c1, int r2, int c2, Action onComplete)
        {
            var view1 = _views[r1, c1];
            int pending = 2;

            view1?.AnimateInvalidSwap(GetWorldPosition(r1, c1), Decrement);
            _views[r2, c2]?.AnimateInvalidSwap(GetWorldPosition(r2, c2), Decrement);

            void Decrement() { if(--pending <= 0) onComplete?.Invoke(); }
        }

        public TileView GetView(int row, int col) => _views[row, col];

        // ── Layout ───────────────────────────────────────────────────

        public Vector3 GetWorldPosition(int row, int col)
        {
            float step = _tileSize + _tileOffset;

            // Board'u ekran ortasına hizala
            float offsetX = (_cols - 1) * step * 0.5f;
            float offsetY = (_rows - 1) * step * 0.5f;

            return new Vector3(
                col * step - offsetX,
                -row * step + offsetY,
                0f);
        }

        /// <summary>
        /// World position'dan grid koordinatına çevirir.
        /// </summary>
        public bool TryGetGridPosition(Vector3 worldPos, out int row, out int col)
        {
            float step = _tileSize + _tileOffset;
            float offsetX = (_cols - 1) * step * 0.5f;
            float offsetY = (_rows - 1) * step * 0.5f;

            col = Mathf.RoundToInt((worldPos.x + offsetX) / step);
            row = Mathf.RoundToInt((-worldPos.y + offsetY) / step);

            return row >= 0 && row < _rows && col >= 0 && col < _cols;
        }
    }
}

