using UnityEngine;

using GameCore.States.Branch;
using Cysharp.Threading.Tasks;
namespace GameCore.States
{
    public class TitleFadeOutState : BaseTitleFadeOutState
    {
        public override void Enter(GameCore.States.Managers.TitleStateManagerData state_manager_data) { }
        public override void Update(GameCore.States.Managers.TitleStateManagerData state_manager_data) { }
        public override void Exit(GameCore.States.Managers.TitleStateManagerData state_manager_data) { }

        public override async UniTask EnterAsync(GameCore.States.Managers.TitleStateManagerData state_manager_data, System.Threading.CancellationToken ct)
        {
            await FadeCanvasCore.Instance.FadeOut(1.0f);
            await UniTask.CompletedTask;
        }
        // __LIFECYCLE_OVERRIDES_START__
        public override bool UseEnterSync => false;
        public override bool UseEnterAsync => true;
        // __LIFECYCLE_OVERRIDES_END__
    }
}
