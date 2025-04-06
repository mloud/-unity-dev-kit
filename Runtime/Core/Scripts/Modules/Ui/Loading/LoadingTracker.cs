using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Core.Modules.Ui.Loading
{
    public class LoadingOperation
    {
        public string Message { get; }
        private Func<UniTask> Operation { get; }
        
        public LoadingOperation(Func<UniTask> operation, string message)
        {
            Operation = operation;
            Message = message;
        }

        public async UniTask Execute() => await Operation();
    }
    
    public class LoadingTracker
    {
        private ILoading LoadingLayer { get; }
        private List<LoadingOperation> Operations { get; }

        public LoadingTracker(ILoading loadingLayer)
        {
            Operations = new List<LoadingOperation>();
            LoadingLayer = loadingLayer;
        }

        public LoadingTracker RegisterOperation(Func<UniTask> operation, string message)
        {
            RegisterOperation(new LoadingOperation(operation, message));
            return this;
        }
       
        public LoadingTracker RegisterOperation(LoadingOperation operation)
        {
            Operations.Add(operation);
            return this;
        }

        public async UniTask Execute()
        {
            for (int i = 0; i < Operations.Count; i++)
            {
                LoadingLayer.SetProgress(i,Operations.Count, Operations[i].Message);
                await Operations[i].Execute();
                LoadingLayer.SetProgress(i+1, Operations.Count, null);
            }
        }
    }
}