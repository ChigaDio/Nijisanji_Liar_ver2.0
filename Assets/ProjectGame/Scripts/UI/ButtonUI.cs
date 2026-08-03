using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class ButtonUI<T> where T : struct, System.Enum
{
    public Button button;
    public T buttonType;

    public RectTransform rect_transform;



    public void Initialize()
    {
        if (button == null)
            return;

       
    }

    public Vector2 AddOffsetPositionX(Vector2 offset)
    {
        if (rect_transform == null)
            return Vector2.zero;
        var originalPosition = rect_transform.localPosition;
        return new Vector2(originalPosition.x + offset.x, originalPosition.y + offset.y);
    }

    public void AddEventButtonAction(UnityAction<T> action)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => action(buttonType));
    }

    public void AddForcusEvent(UnityAction<T> enterAction, UnityAction<T> exitAction)
    {
        if (button == null)
            return;

        var eventTrigger = button.GetComponent<EventTrigger>();

        if (eventTrigger == null)
            eventTrigger = button.gameObject.AddComponent<EventTrigger>();

        eventTrigger.triggers.Clear();

        var enter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        enter.callback.AddListener(_ => enterAction(buttonType));
        eventTrigger.triggers.Add(enter);

        var exit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        exit.callback.AddListener(_ => exitAction(buttonType));
        eventTrigger.triggers.Add(exit);
    }

    public void RemoveButtonEvent()
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();

        var eventTrigger = button.GetComponent<EventTrigger>();

        if (eventTrigger != null)
            eventTrigger.triggers.Clear();
    }
}