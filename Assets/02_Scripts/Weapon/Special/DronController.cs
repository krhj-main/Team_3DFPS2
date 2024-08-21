using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronController : SpecialWeapon
{
    public Dron dron;

    public bool isOut = false;
    public Camera charCamera;

    

    // Start is called before the first frame update
    void Start()
    {
        charCamera = Camera.main;
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

    public override void OnHand(Transform _tr, Vector3 _offset)
    {
        transform.position = _tr.position + _offset;  //오브젝트 위치 조정
        transform.rotation = _tr.rotation;
    }

    public override void InputKey()
    {
        if (Input.GetMouseButton(0)) {
            Use();
        }
    }
}
