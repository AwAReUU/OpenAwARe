// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AwARe.UI.Objects
{
    /// <summary>
    /// An invisible UI element to catch any clicks or other pointer events that hits no other UI element. <br/>
    /// This UI element is always placed on the back canvas behind every other UI element (except the background itself)
    /// The class also provides access to the last PointerEventData, including the last screen position pressed.
    /// </summary>
    public class InputMissCatcher : Graphic,
                                    IPointerClickHandler, IPointerEnterHandler, IPointerEventData
        //IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        // Unity events.
        [SerializeField] private UnityEvent onClick = new();
        [SerializeField] private UnityEvent onEnter = new();
    
        /// <summary>
        /// Gets or sets the Unity event invoked on a pointer click.
        /// </summary>
        /// <value>
        /// The Unity Event invoked on a pointer click.
        /// </value>
        public UnityEvent OnClick
        {
            get => onClick;
            set => onClick = value;
        }
    
        /// <summary>
        /// Gets or sets the Unity event invoked on a pointer press.
        /// </summary>
        /// <value>
        /// The Unity Event invoked on a pointer press.
        /// </value>
        public UnityEvent OnEnter
        {
            get => onEnter;
            set => onEnter = value;
        }

        /// <inheritdoc/>
        public PointerEventData PointerEventData { get; private set; }

        /// <summary>
        /// Handle new pointer event data.
        /// </summary>
        /// <param name="eventData">The new event data.</param>
        private void HandleNewEventData(PointerEventData eventData) =>
            PointerEventData = eventData;

        /// <summary>
        /// Decides when any event should be invoked.
        /// </summary>
        /// <param name="eventData">The current pointer event.</param>
        /// <returns>True if an event should be thrown.</returns>
        private bool DoReact(PointerEventData eventData) =>
            eventData.button == PointerEventData.InputButton.Left && IsActive();

        /// <inheritdoc/>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!DoReact(eventData))
                return;

            HandleNewEventData(eventData);
            onClick.Invoke();
        }
    
        /// <inheritdoc/>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!DoReact(eventData))
                return;
        
            HandleNewEventData(eventData);
            onEnter.Invoke();
        }
    }
}