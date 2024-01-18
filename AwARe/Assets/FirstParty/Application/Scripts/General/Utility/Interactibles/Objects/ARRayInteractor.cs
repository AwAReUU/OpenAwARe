using System;

using AwARe.InterScenes.Objects;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

[RequireComponent(typeof(XRRayInteractor), typeof(ActionBasedController))]
public class ARRayInteractor : MonoBehaviour
{
    private const float EPS = 0.000001f;

    [SerializeField] private InputActionProperty positionAction;
    [SerializeField] private Camera camera;

    private XRRayInteractor rayInteractor;

    private Vector2 screenPosition;
    private Vector3 position;

    private void OnEnable()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        positionAction.action.performed += MoveRayInteractor;
    }
    private void OnDisable()
    {
        positionAction.action.performed -= MoveRayInteractor;
    }

    private void MoveRayInteractor(InputAction.CallbackContext context) =>
        MoveRayInteractor(context.ReadValue<Vector2>());

    private void MoveRayInteractor(GameObject _, PointerEventData pointerEventData) =>
        MoveRayInteractor(pointerEventData.position);

    private void MoveRayInteractor(Vector2 screenPosition)
    {
        float minDist = camera.nearClipPlane + EPS;
        this.screenPosition = screenPosition;
        Ray ray = camera.ScreenPointToRay((Vector3)this.screenPosition + minDist * Vector3.forward);
        rayInteractor.transform.SetPositionAndRotation(ray.origin, Quaternion.LookRotation(ray.direction));
    }
}