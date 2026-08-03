using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameCore;
using GameCore.Sound;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TitleCanvasCore : BaseSingleton<TitleCanvasCore>
{
    public enum TitleButtonType
    {
        Start,
        Load,
        Exit,
        Option,
    }

    [SerializeField]
    private List<ButtonUI<TitleButtonType>> buttonList = new();

    [SerializeField]
    private RectTransform selected_rectTransform;

    private DG.Tweening.Sequence sequence;

    public readonly float W_SELECT_MOVE_X_POS = -420.0f;




    public override void AwakeSingleton()
    {
        base.AwakeSingleton();
    }

    public void MoveSelectedRectTransform(TitleButtonType buttonType)
    {
        var find = buttonList.Find(x => EqualityComparer<TitleButtonType>.Default.Equals(x.buttonType, buttonType));

        if (find == null || selected_rectTransform == null)
            return;

        if (sequence != null && sequence.IsActive())
            sequence.Kill();

        sequence = DOTween.Sequence();

        //音を鳴らす
        SoundCore.Instance.PlaySE(SoundGroup.UI,GameCore.Enums.SoundID.UI_SelectMove);


        var targetPosition = find.AddOffsetPositionX(new Vector2(W_SELECT_MOVE_X_POS, 0f));

        sequence.Join(selected_rectTransform.DOLocalMove(targetPosition, 0.5f));
        sequence.Play();
    }


    public void AddButtonEvent(UnityAction<TitleButtonType> action)
    {
        foreach (var button in buttonList)
            button.AddEventButtonAction(action);
    }


    /// <summary>
    /// ボタンにフォーカスイベントを追加する
    /// </summary>
    /// <param name="enterAction"> フォーカス入力時のアクション </param>
    /// <param name="exitAction"> フォーカス終了時のアクション </param>
    public void AddForcusEvent(
        UnityAction<TitleButtonType> enterAction,
        UnityAction<TitleButtonType> exitAction)
    {
        foreach (var button in buttonList)
            button.AddForcusEvent(enterAction, exitAction);
    }

    public void RemoveButtonEvent()
    {
        foreach (var button in buttonList)
            button.RemoveButtonEvent();
    }


    private async void Start()
    {


        
        sequence = DOTween.Sequence();

        //テスト
        AddForcusEvent(
            enterAction: (buttonType) => MoveSelectedRectTransform(buttonType),
            exitAction: (buttonType) => { });

        await UniTask.Yield();
    }

}