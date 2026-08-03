// 自動生成ファイルです。手動編集しても generate 実行時に上書きされます。
using System;
using GameCore.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameCore.Sound
{
    public partial class SoundCore
    {
        public static readonly SoundID[] _Sound_UI_TitleToSoundID = new SoundID[]
        {
            SoundID.None, // Sound_UI_Title.None
            SoundID.UI_SelectMove, // Sound_UI_Title.SelectMove
            SoundID.UI_PushEnter, // Sound_UI_Title.PushEnter
        };

        public void LoadSingle(Sound_UI_TitleID id, AddressableSystem.GroupCategory groupCategory, Action onCompleted = null)
            => LoadSingle(SoundGroup.UI, _Sound_UI_TitleToSoundID[(int)id], groupCategory, onCompleted);

        public async UniTask LoadSingleAsync(Sound_UI_TitleID id, AddressableSystem.GroupCategory groupCategory, Action onCompleted = null)
            => await LoadSingleAsync(SoundGroup.UI, _Sound_UI_TitleToSoundID[(int)id], groupCategory, onCompleted);

        public void UnloadSingle(Sound_UI_TitleID id, Action onCompleted = null)
            => UnloadSingle(SoundGroup.UI, _Sound_UI_TitleToSoundID[(int)id], onCompleted);

       public async UniTask UnloadSingleAsync(Sound_UI_TitleID id, Action onCompleted = null)
            => await UnloadSingleAsync(SoundGroup.UI, _Sound_UI_TitleToSoundID[(int)id], onCompleted);

       public void PlayBGM(Sound_UI_TitleID id, float volume = 1f, float fadeTime = 0f)
            => PlayBGM(SoundGroup.UI,_Sound_UI_TitleToSoundID[(int)id],volume,fadeTime);

       public void CrossFadeBGM(Sound_UI_TitleID id, float volume = 1f, float fadeTime = 1f)
            => CrossFadeBGM(SoundGroup.UI,_Sound_UI_TitleToSoundID[(int)id],volume,fadeTime);

        public static readonly SoundID[] _Sound_Title_MainToSoundID = new SoundID[]
        {
            SoundID.None, // Sound_Title_Main.None
            SoundID.Title_Main_BGM, // Sound_Title_Main.Main_BGM
        };

        public void LoadSingle(Sound_Title_MainID id, AddressableSystem.GroupCategory groupCategory, Action onCompleted = null)
            => LoadSingle(SoundGroup.Title, _Sound_Title_MainToSoundID[(int)id], groupCategory, onCompleted);

        public async UniTask LoadSingleAsync(Sound_Title_MainID id, AddressableSystem.GroupCategory groupCategory, Action onCompleted = null)
            => await LoadSingleAsync(SoundGroup.Title, _Sound_Title_MainToSoundID[(int)id], groupCategory, onCompleted);

        public void UnloadSingle(Sound_Title_MainID id, Action onCompleted = null)
            => UnloadSingle(SoundGroup.Title, _Sound_Title_MainToSoundID[(int)id], onCompleted);

       public async UniTask UnloadSingleAsync(Sound_Title_MainID id, Action onCompleted = null)
            => await UnloadSingleAsync(SoundGroup.Title, _Sound_Title_MainToSoundID[(int)id], onCompleted);

       public void PlayBGM(Sound_Title_MainID id, float volume = 1f, float fadeTime = 0f)
            => PlayBGM(SoundGroup.Title,_Sound_Title_MainToSoundID[(int)id],volume,fadeTime);

       public void CrossFadeBGM(Sound_Title_MainID id, float volume = 1f, float fadeTime = 1f)
            => CrossFadeBGM(SoundGroup.Title,_Sound_Title_MainToSoundID[(int)id],volume,fadeTime);

    }

    public sealed partial class SoundObjectPool
    {
        public static UniTask<SoundHandle> PlaySE(Sound_UI_TitleID id, Vector3 position, float volume = 1f, float pitch = 1f, float forceDuration = -1f, float distance = 0f, Action<SoundHandle> onCompleted = null)
            => PlaySE(SoundGroup.UI, SoundCore._Sound_UI_TitleToSoundID[(int)id], position, volume, pitch, forceDuration, distance, onCompleted);

        public static UniTask PlayBGM(int channel, Sound_UI_TitleID id, float fadeIn = 1f, float volume = 1f)
            => PlayBGM(channel, SoundGroup.UI, SoundCore._Sound_UI_TitleToSoundID[(int)id], fadeIn, volume);

        public void StopSE(Sound_UI_TitleID id)
            => StopSE(SoundGroup.UI, SoundCore._Sound_UI_TitleToSoundID[(int)id]);
    }

    public sealed partial class SoundObjectPool
    {
        public static UniTask<SoundHandle> PlaySE(Sound_Title_MainID id, Vector3 position, float volume = 1f, float pitch = 1f, float forceDuration = -1f, float distance = 0f, Action<SoundHandle> onCompleted = null)
            => PlaySE(SoundGroup.Title, SoundCore._Sound_Title_MainToSoundID[(int)id], position, volume, pitch, forceDuration, distance, onCompleted);

        public static UniTask PlayBGM(int channel, Sound_Title_MainID id, float fadeIn = 1f, float volume = 1f)
            => PlayBGM(channel, SoundGroup.Title, SoundCore._Sound_Title_MainToSoundID[(int)id], fadeIn, volume);

        public void StopSE(Sound_Title_MainID id)
            => StopSE(SoundGroup.Title, SoundCore._Sound_Title_MainToSoundID[(int)id]);
    }

}