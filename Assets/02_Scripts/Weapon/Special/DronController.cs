using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronController : SpecialWeapon
{
    public Dron dron;

    public bool isOut = false;
    public Camera charCamera;
    public Vector3 offset=Vector3.zero;
    Vector3 defaultPos;
    public Material phoneMat;
    public MeshRenderer phone;
    public GameObject sphere;
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
            dron.transform.SetParent(null);
           
            Rigidbody _rid = dron.gameObject.GetComponent<Rigidbody>();
            if (_rid)
            {
                _rid.AddForce(Camera.main.transform.forward * 1, ForceMode.Impulse);
            }
            isOut = !isOut;

        }
           
    }

    public override void OnHandEnter()
    {
        defaultPos = Swap.GunPosition.localPosition;
        Swap.GunPosition.localPosition = offset;
        phone.enabled = true;
        sphere.SetActive(false);
    }
    public override void OnHandExit()
    {
        Swap.GunPosition.localPosition = defaultPos;
        phone.enabled = false;
        sphere.SetActive(true);
    }
    public override void OnHand(Transform _tr, Vector3 _offset)
    {
        transform.position = Swap.GunPosition.position;
        transform.rotation = Swap.GunPosition.rotation;
        
    }

    public override void InputKey()
    {
        if (Input.GetMouseButton(0)) {
            Use();
        }
    }
    public override void Interaction(GameObject target)
    {
        base.Interaction(target);
        phone.enabled = true;

    }
}
