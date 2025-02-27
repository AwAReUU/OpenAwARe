// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

public class ObjectDataWindow : MonoBehaviour
{
    private void RotateYToCamera()
    {
        Vector3 position = transform.position;
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 direction = position - cameraPosition;
        Vector3 targetRotationEuler = Quaternion.LookRotation(direction).eulerAngles;

        // Scale with transform.up to only rotate for y-axis, this is the axis when you look straight ahead
        Vector3 scaledEuler = Vector3.Scale(targetRotationEuler, transform.up.normalized);
        Quaternion targetRotation = Quaternion.Euler(scaledEuler);
        transform.rotation = targetRotation;
    }

    void FixedUpdate()
    {
        RotateYToCamera();
    }
}
