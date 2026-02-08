using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UI_Base : MonoBehaviour
{
    public static void BindEvent(GameObject go, Action<PointerEventData> action)
    {
        UI_EventHandler evt = go.GetComponent<UI_EventHandler>();
        if (evt == null) evt = go.AddComponent<UI_EventHandler>();

        evt.OnClickHandler -= action;
        evt.OnClickHandler += action;
    }
}