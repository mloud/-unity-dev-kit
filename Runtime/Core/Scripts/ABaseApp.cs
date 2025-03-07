using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core
{
    [DefaultExecutionOrder(-100)]
    public abstract class ABaseApp : MonoBehaviour
    {
        [SerializeField] private Environment environment;
        private void Awake()
        {
            Boot().Forget();
        }

        private async UniTask Boot()
        {
            await UnityGS.Initialize(environment.ToString());
            await RegisterServices();
            await OnBoot();
            await SetApplicationStateMachine();
        }

        protected abstract UniTask RegisterServices();
        protected abstract UniTask SetApplicationStateMachine();
        protected abstract UniTask OnBoot();
    }
}