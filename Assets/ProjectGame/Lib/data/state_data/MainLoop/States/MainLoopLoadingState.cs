using UnityEngine;

using GameCore.States.Branch;
namespace GameCore.States
{
    public class MainLoopLoadingState : BaseMainLoopLoadingState
    {
        public override void Enter(GameCore.States.Managers.MainLoopStateManagerData state_manager_data) { }
        public override void Update(GameCore.States.Managers.MainLoopStateManagerData state_manager_data) { }
        public override void Exit(GameCore.States.Managers.MainLoopStateManagerData state_manager_data) { }
        // __LIFECYCLE_OVERRIDES_START__
        public override bool UseEnterAsync => true;
        // __LIFECYCLE_OVERRIDES_END__
    }
}
