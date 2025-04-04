using UnityEngine;

namespace OneDay.Core.Modules.Pooling
{
    public class Poolable : MonoBehaviour, IPoolable
    {
        public string Key { get; set; }

        public virtual void OnGetFromPool()
        { }
        public virtual void OnReturnToPool()
        { }
    }
}