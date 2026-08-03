


using System;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
namespace GameCore.Scenario
{
    public  class BaseGeneralScenarioRoleAction<T> : BaseOrigintScenarioRoleAction where T : BaseScenarioRoleData
    {
        public T RoleData { get; private set; }

        public BaseGeneralScenarioRoleAction(T roleData) : base()
        {
            RoleData = roleData;
        }


        public override void OnInitialize(ScenarioExecuteData executeData,CancellationTokenSource ct)
        {
            base.OnInitialize(executeData,ct);
        }
        
        public override async UniTask OnInitializeAsync(ScenarioExecuteData executeData, CancellationTokenSource ct)
        {
            await base.OnInitializeAsync(executeData, ct);
        }
    }
}


