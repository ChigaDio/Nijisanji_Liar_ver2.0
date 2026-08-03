
using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
        
namespace GameCore.Enums
{
    public static class TableIdUtils
    {
        public static async UniTask LoadAsyncCore(Action action = null)
        {
            await ClassDataIDCore.Instance.LoadClassDataAsync(async (reader, header) =>
            {
                header.GetData<GameCore.Tables.GuestCharacterTable>(GameCore.Enums.TableID.GuestCharacter, reader);
                await UniTask.Yield();
                header.GetData<GameCore.Tables.RoleTypeTable>(GameCore.Enums.TableID.RoleType, reader);
                await UniTask.Yield();
                header.GetData<GameCore.Tables.PlaceMapTable>(GameCore.Enums.TableID.PlaceMap, reader);
                await UniTask.Yield();
                action?.Invoke();
                await UniTask.CompletedTask;
            });
        }

        public static void LoadCore(Action action = null)
        {
            UniTask.Action(async () =>
            {
                await ClassDataIDCore.Instance.LoadClassDataAsync(async (reader, header) =>
                {
                    header.GetData<GameCore.Tables.GuestCharacterTable>(GameCore.Enums.TableID.GuestCharacter, reader);
                    header.GetData<GameCore.Tables.RoleTypeTable>(GameCore.Enums.TableID.RoleType, reader);
                    header.GetData<GameCore.Tables.PlaceMapTable>(GameCore.Enums.TableID.PlaceMap, reader);
                    action?.Invoke();
                    await UniTask.CompletedTask;
                });
            }).Invoke();
        }

    }
}
