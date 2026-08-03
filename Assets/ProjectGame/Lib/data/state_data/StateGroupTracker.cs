using GameCore.States.ID;

namespace GameCore.States
{
    /// <summary>
    /// 現在アクティブな State グループ（StateGroupID）と、その1つ前のグループを
    /// 追跡する静的トラッカー。各 Base{name}StateControl が起動/再開する度に
    /// ChangeGroup を呼び出す。
    /// </summary>
    public static class StateGroupTracker
    {
        public static StateGroupID CurrentGroup { get; private set; } = StateGroupID.None;
        public static StateGroupID PreviousGroup { get; private set; } = StateGroupID.None;

        public static void ChangeGroup(StateGroupID new_group)
        {
            if (new_group == CurrentGroup) return;
            PreviousGroup = CurrentGroup;
            CurrentGroup = new_group;
        }
    }
}
