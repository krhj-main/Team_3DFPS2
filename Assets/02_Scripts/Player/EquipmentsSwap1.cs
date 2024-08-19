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
    
    float dropForce = 5;
    public Vector3 offsetPos;
    public Transform firePos;
    Inventory Inventory;
    EquipmentsSlot slot;
    IEquipMent equip;


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
    // Start is called before the first frame update
    void Start()
    {
        Inventory = GetComponent<Inventory>();
        firePos = Camera.main.transform;
        InputManger.Instance.keyAction += Inputkey;
        Swap(0);
    }

    // Update is called once per frame
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

    public void Inputkey() {
                                 //무기가 하나이상 있으면
            
            if (Input.GetKeyDown(dropKey))
            {
                DropWeapon(equip);
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
                if (index == 2)
                {
                slot.Next();
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
        
        int _slot;
        switch (type)
        {
            case EquipType.Weapon://타입별로 아이템 매니저에 정의된 크기로 슬롯을 구성
                
                _slot = 0;
                break;
            case EquipType.Throw:
                _slot = 1;
                break;
            case EquipType.Special:
                _slot = 2;
                break;
            default:
                _slot = 3;
                break;
        }
        if (_slot != null)
        {//입력받은 슬롯으로 무기 추가
            
            AddWeapon(weapon, _slot);
        }

    }
    
    void Swap(int _setIndex)
    {//1또는 -1으로 들고있는 무기를 전환하는 함수
        offsetPos = Vector3.zero;
        //IEquipMent _equp = Inventory.Get(_setIndex);
        if (equip != null)
        {//무기를 들고있으면 전환
            equip.gameObject.SetActive(false);
            equip.OutHand();
            InputManger.Instance.keyAction -= equip.InputKey;
        }
        Index = _setIndex;
        equip = Inventory.Get(Index);
        slot = Inventory.GetSlotToIndex(Index);
        if (equip != null)
        {
            equip.gameObject.SetActive(true);
            InputManger.Instance.keyAction += equip.InputKey;
        }
    }
    void SwapNext() { Swap(Index + 1); }
    void SwapPrev() { Swap(Index - 1); }
    //수류탄이랑 특수장비 예외처리해야함...
    void AddWeapon(IEquipMent _weapon, int _index)
    {
        if (_weapon.type == EquipType.Weapon){
            ((MainWeapon)_weapon).firePos = firePos;
        }
        else if (_weapon.type == EquipType.Throw)
        {
            ((ThrowingWeapon)_weapon).firePos = firePos;
        }
        slot = Inventory.GetSlot(_index);
        if (slot.isFull) {
            IEquipMent _equip = slot.Current();
            DropWeapon(_equip);
            _equip.gameObject.SetActive(true);
        }
        Inventory.Set(_index,_weapon);
        int _num = Inventory.SlotIndexToIndex(_index);
        Debug.Log(_num);
        Swap(_num);
        
        //주어진 슬롯에 남는 칸이 없으면
        
        _weapon.transform.SetParent(transform);
        
    }

    public void DropWeapon(IEquipMent _equip)
    {
        IEquipMent _go;
        _go = _equip;
        Inventory.Set(Index,null);

        Rigidbody _rid = _go.gameObject.GetComponent<Rigidbody>();
        if (_rid)
        {
           _rid.AddForce(Camera.main.transform.forward * dropForce+Vector3.up, ForceMode.Impulse);
        }
        InputManger.Instance.keyAction -= _equip.InputKey;
        _go.OutHand();
        Utill.DestroyOnLoad(_go.gameObject);
        SwapNext();

    }
}

