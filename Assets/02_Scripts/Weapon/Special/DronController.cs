using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronController : MonoBehaviour, IEquipMent, Interectable
{
    public Dron dron;

    public bool isOut = false;
    public Camera charCamera;

    Transform IEquipMent.transform { get => transform; set { } }
    GameObject IEquipMent.gameObject { get => gameObject; set { } }
    public EquipType type { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        type = EquipType.Special;
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

    public void OnHand(Transform _tr, Vector3 _offset)
    {
        transform.position = _tr.position + _offset;  //오브젝트 위치 조정
        transform.rotation = _tr.rotation;
    }

    public void InputKey()
    {
        if (Input.GetMouseButton(0)) {
            Use();
        }
    }

    public void OutHand()
    {
        
    }

    public virtual void Interection(GameObject target)
    {
        EquipmentsSwap swap = target.GetComponent<EquipmentsSwap>();
        if (swap != null)
        {
            swap.WeaponChange(this, type);
        }
    }
}
