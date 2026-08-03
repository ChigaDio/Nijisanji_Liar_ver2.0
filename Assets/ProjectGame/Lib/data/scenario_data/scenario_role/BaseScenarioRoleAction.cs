


using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
namespace GameCore.Scenario
{
    public class BaseScenarioRoleAction<T> : BaseGeneralScenarioRoleAction<T> where T : BaseScenarioRoleData
    {

        public BaseScenarioRoleAction(T roleData) : base(roleData)
        {

        }

        public override  void OnInitialize(ScenarioExecuteData executeData, CancellationTokenSource ct)
        {
            base.OnInitialize(executeData,ct);
        }
        
        public override async UniTask OnInitializeAsync(ScenarioExecuteData executeData, CancellationTokenSource ct)
        {
            await base.OnInitializeAsync(executeData, ct);
        }



    }
}


