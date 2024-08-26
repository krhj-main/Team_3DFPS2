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
        charCamera.enabled = true;
    }

    public void Use() {
            dron.gameObject.transform.position = PlayerController.Instance.PlayerCamera.transform.position+ PlayerController.Instance.PlayerCamera.transform.forward;
            dron.gameObject.SetActive(true);
            dron.transform.SetParent(null);
           
            Rigidbody _rid = dron.gameObject.GetComponent<Rigidbody>();
        
            if (_rid)
            {
            _rid.velocity = Vector3.zero;
            _rid.AddForce(PlayerController.Instance.PlayerCamera.transform.forward * 1, ForceMode.Impulse);
            }
            isOut = !isOut;

        
           
    }

    public override void OnHandEnter()
    {
        phone.enabled = true;
        sphere.SetActive(false);
        arms.SetActive(true);
        PlayerController.Instance.PlayerCamera.transform.SetParent(CameraPos);
        PlayerController.Instance.PlayerCamera.transform.localPosition = Vector3.zero;
        PlayerController.Instance.PlayerCamera.transform.localRotation = Quaternion.Euler(0, 180, -0.15f);
    }
    public override void OnHandExit()
    {
        phone.enabled = false;
        sphere.SetActive(true);
        arms.SetActive(true);
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
                Use();
            }
        }
    }
    public override void Interaction(GameObject target)
    {
        base.Interaction(target);
        phone.enabled = true;

    }
}
