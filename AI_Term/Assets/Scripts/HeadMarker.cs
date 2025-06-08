using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMarker : MonoBehaviour
{
   
    [SerializeField] 
    public GameObject targetAgent;
    public float xPos = 0.1f;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;

    }


    void LateUpdate()
    {
        transform.forward = cam.forward;
        this.gameObject.transform.localPosition = targetAgent.transform.position + new Vector3(xPos, 2f, 0); // ¸Ó¸® À§
    }
}
