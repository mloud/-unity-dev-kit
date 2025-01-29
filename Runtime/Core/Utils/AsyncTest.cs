using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Meditation.Core.Utils
{
    public class AsyncTest : MonoBehaviour
    {
        private CancellationTokenSource ctx;
        private void OnEnable()
        {
            Run().Forget();            
        }

        private async UniTask Run()
        {
            ctx?.Dispose();
            ctx = new CancellationTokenSource();
            try
            {

                Debug.Log("xx Waiting 1 started");
                await UniTask.WaitForSeconds(5, cancellationToken: ctx.Token);
                Debug.Log("xx Waiting 1 finished");
                Debug.Log("xx Waiting 2 started");
                await UniTask.WaitForSeconds(5, cancellationToken: ctx.Token);
                Debug.Log("xx Waiting 2 finished");
                Debug.Log("xx end");
            }
            catch (OperationCanceledException ex)
            {
                Debug.LogError(ex.ToString());
            }
        }



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("xx Cancel called");
                ctx.Cancel();
            }
        }
    }
}