using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using OneDay.Core.Debugging;
using OneDay.Core.Modules.Assets;
using UnityEngine;

namespace OneDay.Core.Modules.Pooling
{
    [LogSection("Pooling")]
    public class PoolManager : MonoBehaviour, IPoolManager, IService
    {
        private readonly Dictionary<string, ObjectPool> pools = new();
        private IAssetManager assetManager;
        private Transform poolRoot;

        public UniTask Initialize()
        {
            assetManager = ServiceLocator.Get<IAssetManager>();
            poolRoot = new GameObject("PooledObjects").transform;
            DontDestroyOnLoad(poolRoot);

            return UniTask.CompletedTask;
        }

        public UniTask PostInitialize() => UniTask.CompletedTask;

        public GameObject GetSync(string addressableKey, Transform parent = null)
        {
            // Check if the pool exists for this addressable key
            if (!pools.TryGetValue(addressableKey, out var pool))
            {
                D.LogError($"No preloaded pool found for addressable key: {addressableKey}. " +
                           $"Call PreloadAsync first before using GetSync.", this);
                return null;
            }

            return pool.Get(parent);
        }
        
        public async UniTask<GameObject> GetAsync(string addressableKey, Transform parent = null,
            CancellationToken cancellationToken = default)
        {
            // Get or create the pool for this addressable key
            if (!pools.TryGetValue(addressableKey, out var pool))
            {
                // Load the prefab asset first
                var asset = await assetManager.GetAssetAsync<GameObject>(addressableKey);
                if (asset == null)
                {
                    D.LogError($"Failed to load addressable asset: {addressableKey}", this);
                    return null;
                }

                // Create a new pool with this prefab
                pool = new ObjectPool(addressableKey, asset, poolRoot);
                pools[addressableKey] = pool;
            }

            // Get an instance from the pool
            return pool.Get(parent);
        }

        public void Return(GameObject instance)
        {
            if (instance == null) return;

            // Find which pool this instance belongs to
            var poolable = instance.GetComponent<IPoolable>();
            if (poolable == null)
            {
                D.LogError($"Attempted to return a non-pooled object: {instance.name}", this);
                Destroy(instance);
                return;
            }

            if (pools.TryGetValue(poolable.Key, out var pool))
            {
                pool.Return(instance);
            }
            else
            {
                D.LogError($"Pool for key {poolable.Key} no longer exists. Destroying object.", this);
                Destroy(instance);
            }
        }

        public async UniTask PreloadAsync(string addressableKey, int count,
            CancellationToken cancellationToken = default)
        {
            if (!pools.TryGetValue(addressableKey, out var pool))
            {
                var asset = await assetManager.GetAssetAsync<GameObject>(addressableKey);
                if (asset == null)
                {
                    Debug.LogError($"Failed to load addressable asset: {addressableKey}");
                    return;
                }

                pool = new ObjectPool(addressableKey, asset, poolRoot);
                pools[addressableKey] = pool;
            }

            await pool.PreloadAsync(count, cancellationToken);
        }

        public void ClearAllPools()
        {
            foreach (var pool in pools.Values)
            {
                pool.Clear();
            }

            pools.Clear();
        }

        public void ClearPool(string addressableKey)
        {
            if (pools.TryGetValue(addressableKey, out var pool))
            {
                pool.Clear();
                pools.Remove(addressableKey);
            }
            else
            {
                D.LogError($"Failed to clear pool - no such pool exists {addressableKey}", this);
            }
        }

        private void OnDestroy()
        {
            ClearAllPools();
        }
    }
}
