using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed;
    public float zoomSpeed;

    float hAxis;
    float vAxis;
    float zAxis;

    Vector3 moveVec;
    Vector3 zoomVec;

    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal1");
        vAxis = Input.GetAxisRaw("Vertical1");
        zAxis = Input.GetAxisRaw("Mouse ScrollWheel");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        zoomVec = new Vector3(0, zAxis, 0).normalized;

        transform.position += moveVec * moveSpeed * Time.deltaTime;
        transform.position += zoomVec * zoomSpeed * Time.deltaTime;
    }
}
