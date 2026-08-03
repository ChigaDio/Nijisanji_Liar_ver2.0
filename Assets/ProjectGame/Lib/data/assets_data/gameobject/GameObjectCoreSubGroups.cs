// 自動生成ファイルです。手動編集しても generate 実行時に上書きされます。
using System;
using GameCore.Enums;

namespace GameCore.Gameobject
{
    public partial class GameObjectCore
    {
        public void LoadSubGroup(GameObject_CharacterID subGroupId, AddressableSystem.GroupCategory groupCategory, Action onCompleted = null)
            => LoadSubGroupInternal(GameObjectGroup.Character, (int)subGroupId, groupCategory, onCompleted);

        public void UnloadSubGroup(GameObject_CharacterID subGroupId, Action onCompleted = null)
            => UnloadSubGroupInternal(GameObjectGroup.Character, (int)subGroupId, onCompleted);

    }
}