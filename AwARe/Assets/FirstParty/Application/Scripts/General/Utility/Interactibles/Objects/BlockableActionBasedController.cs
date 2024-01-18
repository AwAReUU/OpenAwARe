// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

namespace AwARe.Objects
{
    [AddComponentMenu("AR Controller (Action based)")]
    public class BlockableActionBasedController : ActionBasedController
    {
        public bool IsBlocked() =>
            EventSystem.current.IsPointerOverGameObject();

        protected virtual bool IsPressed(InputAction action)
        {
            if(IsBlocked())
                return false;
            return base.IsPressed(action);
        }
    }
}