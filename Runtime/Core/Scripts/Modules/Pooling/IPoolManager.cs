using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core.Modules.Pooling
{
    public interface IPoolManager
    {
        /// <summary>
        /// Get or create a pooled GameObject from an addressable key
        /// </summary>
        UniTask<GameObject> GetAsync(string addressableKey, Transform parent = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a pooled GameObject from an addressable key in sync way
        /// </summary>
        GameObject GetSync(string addressableKey, Transform parent = null);
        
        /// <summary>
        /// Return a GameObject to its pool
        /// </summary>
        void Return(GameObject instance);

        /// <summary>
        /// Preload a specific number of instances for an addressable key
        /// </summary>
        UniTask PreloadAsync(string addressableKey, int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// Clear all pools
        /// </summary>
        void ClearAllPools();

        /// <summary>
        /// Clear a specific pool by addressable key
        /// </summary>
        void ClearPool(string addressableKey);
    }
}