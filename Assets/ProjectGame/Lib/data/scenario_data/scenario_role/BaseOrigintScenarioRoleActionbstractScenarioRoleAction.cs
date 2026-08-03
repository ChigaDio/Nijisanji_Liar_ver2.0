



using System;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
namespace GameCore.Scenario
{
    public  class BaseOrigintScenarioRoleAction
    {
        public bool IsCompleted { get; protected set; } = false;
        public bool IsOneExecute { get; protected set; } = false;
        public bool IsStartUp { get; protected set; } = false;
        public bool IsRelease { get; protected set; } = false;
        public virtual void ReadBinary(BinaryReader reader)
        {
            
        }
        public virtual void OnInitialize(ScenarioExecuteData executeData, CancellationTokenSource ct)
        {
            IsCompleted = false;
        }
        public virtual void OnOneExecute(ScenarioExecuteData executeData, CancellationTokenSource ct)
        {
            // Implement action logic here
        }
        public virtual void OnExecute(ScenarioExecuteData executeData, CancellationTokenSource ct)
        {
            // Implement action logic here
        }
        public virtual void OnFinalize(ScenarioExecuteData executeData, CancellationTokenSource ct)
        {
            // Implement cleanup logic here
        }
        
        public virtual async UniTask OnInitializeAsync(ScenarioExecuteData executeData, CancellationTokenSource ct)
        {
            IsCompleted = false;
            await UniTask.CompletedTask;
        }
        public virtual async UniTask OnOneExecuteAsync(ScenarioExecuteData executeData, CancellationTokenSource ct)
        {
            // Implement action logic here
            await UniTask.CompletedTask;
        }
        public virtual async UniTask OnExecuteAsync(ScenarioExecuteData executeData, CancellationTokenSource ct)
        {
            // Implement action logic here
            await UniTask.CompletedTask;
        }
        public virtual async UniTask OnFinalizeAsync(ScenarioExecuteData executeData, CancellationTokenSource ct)
        {
            await UniTask.CompletedTask;
        }
    }
}



