// 自動生成ファイルです。手動編集しても generate 実行時に上書きされます。
using System;
using GameCore.Enums;

namespace GameCore.Sound
{
    public partial class SoundCore
    {
        public void LoadSubGroup(Sound_UIID subGroupId, AddressableSystem.GroupCategory category, Action onCompleted = null)
            => LoadSubGroupInternal(SoundGroup.UI, (int)subGroupId, category, onCompleted);

        public void UnloadSubGroup(Sound_UIID subGroupId, Action onCompleted = null)
            => UnloadSubGroupInternal(SoundGroup.UI, (int)subGroupId, onCompleted);

        public void LoadSubGroup(Sound_TitleID subGroupId, AddressableSystem.GroupCategory category, Action onCompleted = null)
            => LoadSubGroupInternal(SoundGroup.Title, (int)subGroupId, category, onCompleted);

        public void UnloadSubGroup(Sound_TitleID subGroupId, Action onCompleted = null)
            => UnloadSubGroupInternal(SoundGroup.Title, (int)subGroupId, onCompleted);

    }
}