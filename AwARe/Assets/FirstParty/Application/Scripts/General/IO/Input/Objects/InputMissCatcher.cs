using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UI.CoroutineTween;

public class InputMissCatcher : Graphic,
                                IPointerClickHandler, IPointerEnterHandler, IPointerEventData
    //ILayoutElement, ICanvasElement
    //IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField]
    private UnityEvent onClick = new();
    
    [SerializeField]
    private UnityEvent onEnter = new();
    
    public UnityEvent OnClick
    {
        get => onClick;
        set => onClick = value;
    }

    public UnityEvent OnEnter
    {
        get => onEnter;
        set => onEnter = value;
    }

    public PointerEventData PointerEventData { get; private set; }

    public event OnPointerEventDataChangeHandler OnPointerEventDataChanged;

    private void HandleNewEventData(PointerEventData eventData)
    {
        PointerEventData = eventData;
        OnPointerEventDataChanged?.Invoke(this, eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || !IsActive())
            return;

        //UISystemProfilerApi.AddMarker("InputMissCatcher.onClick", this); // TODO Include? Nah?
        HandleNewEventData(eventData);
        onClick.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || !IsActive())
            return;
        
        //UISystemProfilerApi.AddMarker("InputMissCatcher.onEnter", this); // TODO Include? Nah?
        HandleNewEventData(eventData);
        onEnter.Invoke();
    }
}