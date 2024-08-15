using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronController : MonoBehaviour, SpecialWeapon
{
    public Dron dron;

    public bool isOut = false;
    Camera charCamera;
    // Start is called before the first frame update
    void Start()
    {
        charCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void DronReturn() {
        charCamera.enabled = true;
    }

    public void Use() {
        if (isOut)
        {
            dron.DronAwake();
        }
        else {
            dron.gameObject.transform.position = Camera.main.transform.position+ Camera.main.transform.forward;
            dron.gameObject.SetActive(true);
            Utill.DestroyOnLoad(dron.gameObject);
           
            Rigidbody _rid = dron.gameObject.GetComponent<Rigidbody>();
            if (_rid)
            {
               
                _rid.AddForce(Camera.main.transform.forward * 1, ForceMode.Impulse);
                
            }
            isOut = !isOut;

        }
           
    }
}
