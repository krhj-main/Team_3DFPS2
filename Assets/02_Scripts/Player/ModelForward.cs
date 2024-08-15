using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelForward : MonoBehaviour
{
    [SerializeField] Transform arm;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(arm.position.x,transform.position.y,arm.position.z);
        transform.forward = new Vector3(arm.forward.x, 0, arm.forward.z);
    }
}