using UnityEngine;

using GameCore.States.Branch;
namespace GameCore.States
{
    public class MainLoopGameState : BaseMainLoopGameState
    {
        public override void Enter(GameCore.States.Managers.MainLoopStateManagerData state_manager_data) { }
        public override void Update(GameCore.States.Managers.MainLoopStateManagerData state_manager_data) { }
        public override void Exit(GameCore.States.Managers.MainLoopStateManagerData state_manager_data) { }
        // __LIFECYCLE_OVERRIDES_START__
        // __LIFECYCLE_OVERRIDES_END__
        public override GameCore.States.ID.MainLoopStateID BranchNextState(GameCore.States.Managers.MainLoopStateManagerData state_manager_data)
        {
            var branch = new MainLoopGameStateBranch();
            var next_id = branch.ConditionsBranch(state_manager_data, this);
            return next_id;
        }
    }
}
