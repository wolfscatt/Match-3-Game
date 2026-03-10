using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3Game.Infrastructure.Pool
{
    /// <summary>
    /// Generic Object Pool -- MonoBehavior için.
    /// TileView, VFX particle'ları için kullanılır.
    /// Instantiate/Destroy maliyetini ortadan kaldırır.
    /// </summary>
    public class ObjectPool<T> where T : MonoBehaviour
    {
       private readonly T  _prefab;
       private readonly Transform _parent;
       private readonly Stack<T> _pool = new();
       private readonly HashSet<T> _active = new();
       private readonly int _maxSize;

       public int ActiveCount => _active.Count;
       public int InactiveCount => _pool.Count;

       public ObjectPool(T prefab, Transform parent, int initialSize, int maxSize = 100)
        {
            _prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
            _parent = parent;
            _maxSize = maxSize;

            Prewarm(initialSize);
        }

        // ── Public API ───────────────────────────────────────────────

        public T Get()
        {
            T item = _pool.Count > 0 ? _pool.Pop() : CreateNew();

            item.gameObject.SetActive(true);
            _active.Add(item);
            return item;
        }

        public void Return(T item)
        {
            if (!_active.Contains(item))
            {
                Debug.LogWarning($"[ObjectPool] {item.name} zaten pool'da veya bu pool'a ait değil.");
                return;
            }

            _active.Remove(item);
            item.gameObject.SetActive(false);
            item.transform.SetParent(_parent);

            if(_pool.Count < _maxSize)
                _pool.Push(item);
            else
                UnityEngine.Object.Destroy(item.gameObject);
        }

        public void ReturnAll()
        {
            var snapshot =  new List<T>(_active);
            foreach (var item in snapshot)
                Return(item);
        }

        // ── Private ─────────────────────────────────────────────────

        private void Prewarm(int count)
        {
            for(int i = 0; i < count; i++)
            {
                var item = CreateNew();
                item.gameObject.SetActive(false);
                _pool.Push(item);
            }
        }

        private T CreateNew()
        {
            var obj = UnityEngine.Object.Instantiate(_prefab, _parent);
            obj.name = $"{_prefab.name}_{_active.Count + _pool.Count}";
            return obj;
        }
    }

}
