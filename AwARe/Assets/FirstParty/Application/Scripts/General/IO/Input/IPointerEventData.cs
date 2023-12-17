using UnityEngine.EventSystems;
using static InputMissCatcher;

public interface IPointerEventData
{
    public PointerEventData PointerEventData { get; }

    public event OnPointerEventDataChangeHandler OnPointerEventDataChanged;
}

public delegate void OnPointerEventDataChangeHandler(object sender, PointerEventData data);