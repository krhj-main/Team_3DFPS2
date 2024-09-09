using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DronController : SpecialWeapon
{
    public Dron dron;

    public bool isOut = false;
    public bool isThrowing = false;
    public Camera charCamera;
    public Vector3 offset=Vector3.zero;
    public GameObject sphere;
    [SerializeField] Animator anim;
    [SerializeField] GameObject arms;
    [SerializeField] public Transform CameraPos;
    [SerializeField] Transform dronpos;
    public GameObject guide;
    // Start is called before the first frame update
    void Start()
    {
        charCamera = Camera.main;
        PlayerController.Instance.deadAction += PlayerDead;
        GameManager.Instance.sconeLoaded += Init;
    }

    public void DronReturn() {
        dron.cam.enabled = false;
        dron.isActive = false;
        isOut = false;
        //gameObject.SetActive(false);
        dron.transform.SetParent(transform);
        dron.gameObject.SetActive(false);
        sphere.SetActive(true);
        anim.SetBool("isThrow", isOut);
        guide.SetActive(false);
    }

    public void Use() {
        dron.col.enabled = true;   
        dron.transform.SetParent(null);
        dron.transform.position= sphere.transform.position;
        dron.transform.rotation= sphere.transform.rotation;
        dron.gameObject.SetActive(true);
        sphere.SetActive(false);
        if (dron.rig)
        {
            dron.rig.velocity = Vector3.zero;
            dron.rig.AddForce(PlayerController.Instance.PlayerCamera.transform.forward * 10, ForceMode.Impulse);
        }
        isOut = true;
        isThrowing = false;
        anim.SetBool("isThrow", isOut);

    }

    public override void OnHandEnter()
    {
        sphere.SetActive(!isOut);
        UIManager.Instance.ChangeSpecialWeaponUIUpdate(myImage);
        guide.SetActive(isOut);
        dron.cam.gameObject.SetActive(true);
        anim.enabled = true;
        anim.SetBool("isThrow", isOut);
        arms.SetActive(true);
        PlayerController.Instance.PlayerCamera.transform.SetParent(CameraPos);
        PlayerController.Instance.PlayerCamera.transform.localPosition = new Vector3(0, 0, -0.1f);
        PlayerController.Instance.PlayerCamera.transform.localRotation = Quaternion.Euler(0, 180, -0.15f);
    }
    public override void OnHandExit()
    {
        sphere.SetActive(true);
        dron.cam.gameObject.SetActive(false); 
        guide.SetActive(false);
        anim.enabled = false;
        if (!isOut) {
            dron.col.enabled = false;
            dron.gameObject.SetActive(false);
        }
        arms.SetActive(false);
        PlayerController.Instance.PlayerCamera.transform.SetParent(null);
    }
    public override void OnHand(Transform _tr, Vector3 _offset)
    {
        base.OnHand(_tr, _offset);
        transform.position = _tr.position;
        transform.rotation = _tr.rotation;
    }

    public override void InputKey()
    {
        if (Input.GetMouseButtonDown(0)&&!PlayerController.Instance.UIState()) {
            if (isOut&&dron.isActive&&!dron.dronCam.enabled)
            {
                dron.DronAwake();
                guide.SetActive(false);
            }
            else if(!isThrowing)
            {
                isThrowing = true;
                anim.SetTrigger("doThrow"); 
            }
        }
    }
    public override void Interaction(GameObject target)
    {
        base.Interaction(target);
    }
    public void PlayerDead()
    {
        Init();
    }
    public void Init() 
    {
        dron.DronDisable();
        DronReturn();
    }

    private void OnDestroy()
    {
        Destroy(dron);
    }
}
