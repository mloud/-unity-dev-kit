using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Assets;
using UnityEngine;

namespace OneDay.Core.Modules.Pooling
{
   public class ObjectPool
    {
        private readonly string addressableKey;
        private readonly AddressableAsset<GameObject> prefabAsset;
        private readonly Transform poolRoot;
        private readonly Stack<GameObject> inactiveObjects = new();
        private readonly HashSet<GameObject> activeObjects = new();
        private readonly Transform poolFolder;
        
        public ObjectPool(string addressableKey, AddressableAsset<GameObject> prefabAsset, Transform poolRoot)
        {
            this.addressableKey = addressableKey;
            this.prefabAsset = prefabAsset;
            this.poolRoot = poolRoot;
            
            // deactivate prefab
            this.prefabAsset.GetReference().gameObject.SetActive(false);
            
            var folderName = $"Pool-{addressableKey.Replace('/', '_')}";
            poolFolder = new GameObject(folderName).transform;
            poolFolder.SetParent(poolRoot);
        }
        
        public GameObject Get(Transform parent = null)
        {
            GameObject instance;
            IPoolable poolable;
            if (inactiveObjects.Count > 0)
            {
                instance = inactiveObjects.Pop();
                poolable = instance.GetComponent<IPoolable>();
            }
            else
            {
                var prefab = prefabAsset.GetReference();
                instance = Object.Instantiate(prefab);

                poolable = instance.GetComponent<IPoolable>() ?? instance.AddComponent<Poolable>();
                poolable.Key = addressableKey;
            }

            instance.SetActive(true);
            instance.transform.SetParent(parent, false);
            
            poolable.OnGetFromPool();
            activeObjects.Add(instance);
            return instance;
        }

        public void ReturnAll()
        {
            var activeObjectCopy = activeObjects.ToList();
            foreach (var gameObject in activeObjectCopy)
            {
                Return(gameObject);
            }
        }
        
        public void Return(GameObject instance)
        {
            Debug.Assert(instance != null);
            
            activeObjects.Remove(instance);
            
            instance.SetActive(false);
            instance.transform.SetParent(poolFolder);
        
            var poolable = instance.GetComponent<IPoolable>();
            Debug.Assert(poolable != null);
            poolable.OnReturnToPool();
            inactiveObjects.Push(instance);
        }
        
        public async UniTask PreloadAsync(int count, CancellationToken cancellationToken = default)
        {
            // Create the requested number of instances
            for (int i = 0; i < count - inactiveObjects.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested) break;
                
                var prefab = prefabAsset.GetReference();
                var instance = Object.Instantiate(prefab, poolFolder);
                var poolable = instance.GetComponent<IPoolable>() ?? instance.AddComponent<Poolable>();
                poolable.Key = addressableKey;
                instance.SetActive(false);
                inactiveObjects.Push(instance);
            }
        }
        
        public void Clear()
        {
            // Destroy all pooled objects
            foreach (var obj in inactiveObjects)
            {
                if (obj != null)
                {
                    Object.Destroy(obj);
                }
            }
            inactiveObjects.Clear();
            
            // Destroy all active objects
            foreach (var obj in activeObjects)
            {
                if (obj != null)
                {
                    Object.Destroy(obj);
                }
            }
            activeObjects.Clear();
            
            // Release the prefab asset
            prefabAsset.Release();
            
            // Destroy the pool folder
            if (poolFolder != null)
            {
                Object.Destroy(poolFolder.gameObject);
            }
        }
    }
}