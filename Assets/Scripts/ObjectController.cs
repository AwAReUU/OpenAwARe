using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    //* Sloppy test code, should be relocated refactored etc etc
    private void RotateToCamera()
    {
        Vector3 position = transform.position;
        Vector3 cameraPosition = Camera.main.transform.position;
        //* Flipped direction formula because text_tmp was flipped in scene and im lazy
        //Vector3 direction = cameraPosition - position;
        Vector3 direction = position - cameraPosition;
        Vector3 targetRotationEuler = Quaternion.LookRotation(direction).eulerAngles;
        Vector3 scaledEuler = Vector3.Scale(targetRotationEuler, transform.up.normalized);
        Quaternion targetRotation = Quaternion.Euler(scaledEuler);
        transform.rotation = targetRotation;
    }

    void FixedUpdate()
    {
        RotateToCamera();
    }
}
