using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronController : SpecialWeapon
{
    public Dron dron;

    public bool isOut = false;
    public Camera charCamera;
    public Vector3 offset=Vector3.zero;
    public Material phoneMat;
    public MeshRenderer phone;
    public GameObject sphere;
    [SerializeField] Animator anim;
    [SerializeField] GameObject arms;
    [SerializeField] public Transform CameraPos;
    [SerializeField] Transform dronpos;
    // Start is called before the first frame update
    void Start()
    {
        charCamera = Camera.main;
    }

    public void DronReturn() {
        isOut = false;
        //gameObject.SetActive(false);
        dron.transform.SetParent(dronpos);
        anim.SetBool("isThrow", isOut);
    }

    public void Use() {
             dron.rig.isKinematic = false;
            dron.col.enabled = true;
            dron.gameObject.SetActive(true);
            dron.transform.SetParent(null);

       
        
            if (dron.rig)
            {
            dron.rig.velocity = Vector3.zero;
            dron.rig.AddForce(PlayerController.Instance.PlayerCamera.transform.forward * 10, ForceMode.Impulse);
            }
            isOut = true;
        anim.SetBool("isThrow", isOut);

    }

    public override void OnHandEnter()
    {
        dron.cam.gameObject.SetActive(true);
        anim.enabled = true;
        anim.SetBool("isThrow", isOut);
        phone.enabled = true;
        arms.SetActive(true);
        PlayerController.Instance.PlayerCamera.transform.SetParent(CameraPos);
        PlayerController.Instance.PlayerCamera.transform.localPosition = Vector3.zero;
        PlayerController.Instance.PlayerCamera.transform.localRotation = Quaternion.Euler(0, 180, -0.15f);
    }
    public override void OnHandExit()
    {
        dron.cam.gameObject.SetActive(false);
        anim.enabled = false;
        if (dron.isActive == false) {
            dron.rig.isKinematic = true;
        }
        
        if (!isOut) {
            dron.col.enabled = false;
        }
        
        phone.enabled = false;
        arms.SetActive(false);
        PlayerController.Instance.PlayerCamera.transform.SetParent(null);
    }
    public override void OnHand(Transform _tr, Vector3 _offset)
    {
        transform.position = _tr.position;
        transform.rotation = _tr.rotation;
        if (!isOut) {
           dron.transform.position = dronpos.position;
           dron.transform.rotation = dronpos.rotation;
        }


    }

    public override void InputKey()
    {
        if (Input.GetMouseButtonDown(0)) {
            if (isOut)
            {
                dron.DronAwake();
            }
            else
            {
                anim.SetTrigger("doThrow");
                
            }
        }
    }
    public override void Interaction(GameObject target)
    {
        base.Interaction(target);
        phone.enabled = true;
    }

}
