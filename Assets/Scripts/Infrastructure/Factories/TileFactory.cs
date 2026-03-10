using Match3Game.Domain.Tiles;
using Match3Game.Infrastructure.Pool;
using Match3Game.Presentation.Board;
using UnityEngine;

namespace Match3Game.Infrastructure.Factories
{
    /// <summary>
    /// TileView üretimini ve pool yönetimini üstlenir.
    /// Domain'den TileModel alır, Presentation'a TileView verir.
    /// </summary>
    public class TileFactory
    {
        private readonly ObjectPool<TileView> _pool;
        private readonly TileViewConfig _config;

        public TileFactory(ObjectPool<TileView> pool, TileViewConfig config)
        {
            _pool = pool;
            _config = config;
        }

        public TileView Create(TileModel model, Vector3 worldPosition)
        {
            var view = _pool.Get();
            view.transform.position = worldPosition;
            view.Initialize(model, _config.GetSpriteForColor(model.Color));
            return view;
        }

        public void Return(TileView view)
        {
            view.ResetView();
            _pool.Return(view);
        }

    }

}
