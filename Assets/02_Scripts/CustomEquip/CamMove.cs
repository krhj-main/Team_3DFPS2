using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class CamMove : MonoBehaviour
{
    public GameObject character;

    Quaternion originRotation;
    float rotationSpeed = 5f;
    private float mouseX;

    void Start()
    {
        originRotation = character.transform.rotation;
    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            mouseX = Input.GetAxis("Mouse X");

            character.transform.Rotate(Vector3.up, mouseX * -rotationSpeed);
        }
        if(Input.GetMouseButtonUp(0))
        {
            character.transform.rotation = originRotation;
        }
    }
}
