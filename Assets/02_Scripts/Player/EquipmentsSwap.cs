using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class EquipmentsSwap : MonoBehaviour
{

    public Transform GunPosition;                 //총이 있어야할 위치(빈게임오브젝트)  
    [SerializeField] KeyCode dropKey;                       //들고있는 총 버리기 키
    int index = 0;                                          //선택된 항목의 인덱스
    
    float dropForce = 3;
    public Vector3 offsetPos;
    public Transform firePos;
    public Inventory Inventory;
    
    EquipmentsSlot slot;
    public IEquipMent equip;


    [SerializeField] Knife playerArms;
    [SerializeField] GrenadeFactory grenadeFactory;
    public GrenadeFactory GrenadeFactory { get { return grenadeFactory; } }
    [SerializeField] Transform playerSight;


    public int Index                                        //인덱스를 순환시키기 위한 프로퍼티 
    {
        get { return index; }
        set
        {
            if (Inventory.size > 0)                             //리스트에 아무것도 없으면 0
            {
                if (value < 0)                              //음수 할당시 그만큼 순환(재귀) 
                {
                    Index = Inventory.size + value;
                }
                else if (value > Inventory.size - 1)            //최대 크기 이상 할당시 그만큼 순환(재귀) 
                {
                    Index = value - Inventory.size;
                }
                else { index = value; }
            }
            else
            {
                index = 0;
            }

        }
    }

    void Start()
    {
        Inventory = GetComponent<Inventory>();
        firePos = Camera.main.transform;
        InputManger.Instance.keyAction += Inputkey;
        AddWeapon(grenadeFactory, 1);
        Swap(0);
    }

    void Update()
    {
        if (equip != null)
        {
            equip.OnHand(GunPosition, offsetPos);
        }

        float _wheelInput = Input.GetAxis("Mouse ScrollWheel"); //휠 입력을 받고

        if (_wheelInput > 0)                                    //휠 입력에 따라 후치 연산자를 통해 현재 선택된 오브젝트를 끄고
        {                                                       //Index를 증감함
            SwapNext();
        }

        else if (_wheelInput < 0)
        {
            SwapPrev();
        }
    }

    public void Inputkey() 
    {
        if (GameManager.Instance.openUI) { return; }

        //무기가 하나이상 있으면
        if (Input.GetKeyDown(dropKey))
        {
            if (index != 2)
            {
                DropWeapon(equip, Index);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Swap(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Swap(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (index == 2&& equip!=null)
            {
            ((GrenadeFactory)equip).Changetype();
            }
            else 
            {
                Swap(2); ;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Swap(3);
        }
    }

    public void WeaponChange(IEquipMent weapon, EquipType type)//타입별 무기 스왑
    {
        
        int _slot=-1;
        switch (type)
        {
            case EquipType.Weapon://타입별로 아이템 매니저에 정의된 크기로 슬롯을 구성
                
                _slot = 0;
                break;
            case EquipType.Special:
                _slot = 2;
                break;
            default:
                _slot = 3;
                break;
        }
        if (_slot != -1)
        {//입력받은 슬롯으로 무기 추가
            
            AddWeapon(weapon, _slot);
        }

    }
    
    void Swap(int _setIndex)
    {
        
        offsetPos = Vector3.zero;

        //IEquipMent _equp = Inventory.Get(_setIndex);
        if (equip != null)
        {
            //무기를 들고있으면 전환
            equip.gameObject.SetActive(false);
            equip.OnHandExit();
            InputManger.Instance.keyAction -= equip.InputKey;
        }
        if (_setIndex == -1)
        {
            equip = playerArms.gameObject.GetComponent<IEquipMent>();

        }
        else
        {
            Index = _setIndex;
            equip = Inventory.Get(Index);
            slot = Inventory.GetSlotToIndex(Index);
        }
        

        if (equip != null)
        {

            playerArms.gameObject.SetActive(false);
            playerArms.OnHandExit();
            equip.gameObject.SetActive(true);

            equip.OnHandEnter();

            // 무기가 전환되는 부분
            InputManger.Instance.keyAction += equip.InputKey;
        }
        else 
        {
            firePos.SetParent(playerSight);
            firePos.localPosition = Vector3.zero;
            firePos.localRotation = Quaternion.Euler(0,180,-0.15f);
            playerArms.OnHandEnter();
            playerArms.gameObject.SetActive(true);
        }
    }

    void SwapNext() { Swap(Index + 1); }
    void SwapPrev() { Swap(Index - 1); }

    //수류탄이랑 특수장비 예외처리해야함...
    void AddWeapon(IEquipMent _weapon, int _index)
    {
        if (_weapon.type == EquipType.Weapon)
        {
            ((MainWeapon)_weapon).firePos = firePos;
        }
        else if (_weapon.type == EquipType.Throw)
        {
            ((ThrowingWeapon)_weapon).firePos = firePos;
        }
        else if (_weapon.type == EquipType.Special) {
            ((SpecialWeapon)_weapon).Swap = this;
        }

        slot = Inventory.GetSlot(_index);

        if (slot.isFull)
        {
            IEquipMent _equip = slot.Current();
            DropWeapon(_equip, Inventory.SlotIndexToIndex(slot.Index));
        }
        
        _weapon.gameObject.SetActive(false);
        int _num = Inventory.SlotIndexToIndex(_index);
        Inventory.Set(_num, _weapon);
        _num = Inventory.SlotIndexToIndex(_index);
        Swap(_num);
        _weapon.transform.SetParent(GunPosition);
        _weapon.transform.position = Vector3.zero;
    }

    public void DropWeapon(IEquipMent _equip,int _index)
    {
        if (_index != 2) 
        {
            IEquipMent _go;
            _go = _equip;
            Inventory.Set(_index, null);
            equip = null;
            if (_go!= null) {
                Rigidbody _rid = _go.gameObject.GetComponent<Rigidbody>();

                if (_rid)
                {
                    _rid.AddForce((PlayerController.Instance.PlayerCamera.transform.forward + Vector3.up) * dropForce, ForceMode.Impulse);
                    Debug.DrawRay(PlayerController.Instance.PlayerCamera.transform.position, PlayerController.Instance.PlayerCamera.transform.forward);
                }
                InputManger.Instance.keyAction -= _equip.InputKey;
                _go.OnHandExit();
                Utill.DestroyOnLoad(_go.gameObject);

                SwapNext();
                _equip.gameObject.SetActive(true);
            }
            
        }
    }
}

