using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CreateObjectHandler : MonoBehaviour
{
    public GameObject[] prefabs;
    public int prefabIndex { get; private set; }

    public void SetPrefabIndex(int index)
    {
        prefabIndex = index;
    }

    public void CreateObject(ARRaycastHit hit, bool rotateToUser = true)
    {
        Pose pose = hit.pose;
        GameObject newObject = Instantiate(prefabs[prefabIndex], pose.position, pose.rotation);

        if (rotateToUser)
            Rotate(newObject);
    }

    private void Rotate(GameObject target)
    {
        Vector3 position = target.transform.position;
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 direction = cameraPosition - position;
        Vector3 targetRotationEuler = Quaternion.LookRotation(direction).eulerAngles;
        Vector3 scaledEuler = Vector3.Scale(targetRotationEuler, target.transform.up.normalized);
        Quaternion targetRotation = Quaternion.Euler(scaledEuler);
        target.transform.rotation = targetRotation;
    }
}
