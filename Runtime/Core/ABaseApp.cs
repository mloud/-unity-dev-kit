using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core
{
    [DefaultExecutionOrder(-100)]
    public abstract class ABaseApp : MonoBehaviour
    {
        private void Awake()
        {
            Boot().Forget();
        }

        private async UniTask Boot()
        {
            await OnBoot();
        }

        protected abstract UniTask OnBoot();
    }
}