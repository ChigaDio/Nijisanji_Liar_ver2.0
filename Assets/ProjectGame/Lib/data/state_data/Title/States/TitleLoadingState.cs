using UnityEngine;

using GameCore.States.Branch;
using Cysharp.Threading.Tasks;
using GameCore.States.Managers;
using System.Threading;
using GameCore.Sound;
using GameCore.Tables;
namespace GameCore.States
{
    public class TitleLoadingState : BaseTitleLoadingState
    {
        public override void Enter(GameCore.States.Managers.TitleStateManagerData state_manager_data)
        {
            IsActiveOff();
        }

        public override async UniTask EnterAsync(TitleStateManagerData state_manager_data, CancellationToken ct)
        {
            //マップの読み込み
            await SceneLoader.LoadSceneAsync(GameCore.Enums.GameSceneID.MorningRoom,true);

            
    
            
            await GameCore.Enums.TableIdUtils.LoadAsyncCore();
    
            
        
    
            await GameCore.Sound.SoundCore.Instance.LoadGroupAsync(GameCore.Sound.SoundGroup.UI,AddressableSystem.GroupCategory.Menu,null);
    
            await GameCore.Sound.SoundCore.Instance.LoadGroupAsync(GameCore.Sound.SoundGroup.Title,AddressableSystem.GroupCategory.Title,null);
    
            await TitleCore.Instance.SetCharacter();
            //BGMを鳴らす
            SoundCore.Instance.PlayBGM(SoundGroup.Title,GameCore.Enums.SoundID.Title_Main_BGM);
    
            //使われていないキャラクターは消す
            GuestCharacterTable.UnloadWhere((id,row) => row.Use == false);

            IsActiveAsyncOff();

            await UniTask.CompletedTask;
        }
        public override void Update(GameCore.States.Managers.TitleStateManagerData state_manager_data) { }
        public override void Exit(GameCore.States.Managers.TitleStateManagerData state_manager_data) { }
        // __LIFECYCLE_OVERRIDES_START__
        public override bool UseEnterAsync => true;
        // __LIFECYCLE_OVERRIDES_END__
    }
}
