using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerController.Instance.transform.position = transform.position;
        PlayerController.Instance.transform.rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
