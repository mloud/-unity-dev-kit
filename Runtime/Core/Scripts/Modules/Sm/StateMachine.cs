using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core.Modules.Sm
{
    public class StateMachine
    {
        public class StateMachineEvents
        {
            public Action<IState> StateExited;
            public Action<IState> StateEntered;
        }

        public StateMachineEvents Events { get; } = new();
        public IState CurrentState => currentState;
        
        private IState currentState;

        private Dictionary<Type, IState> states = new();

        /// <summary>
        /// Set a new state and handle the transition asynchronously.
        /// </summary>
        public async UniTask SetStateAsync<T>(StateData stateData = null, bool waitForCurrentStateExit = true) where T: IState
        {
            if (!states.TryGetValue(typeof(T), out var newState))
            {
                Debug.LogError($"No such state {typeof(T)} exists");
                return;
            }
            
            if (currentState != null)
            {
                if (waitForCurrentStateExit)
                {
                    await currentState.ExitAsync();
                }
                else
                {
                    currentState.ExitAsync().Forget();
                }
                Events.StateExited?.Invoke(currentState);
            }

            currentState = newState;

            if (currentState != null)
            {
                await currentState.EnterAsync(stateData);
                Events.StateEntered?.Invoke(currentState);
                await currentState.ExecuteAsync();
            }
        }

        public async UniTask RegisterState<T> () where T: IState, new()
        {
            var state = new T
            {
                StateMachine = this
            };
            states.Add(typeof(T), state);
            await state.Initialize();
        }
    }
}