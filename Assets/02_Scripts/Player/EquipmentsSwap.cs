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
    IEqupMent[] equp;
    int size = 0;
    float dropForce = 5;
    public Vector3 offsetPos;
    public Transform firePos;



    public int Index                                        //인덱스를 순환시키기 위한 프로퍼티 
    {
        get { return index; }
        set
        {
            if (size > 0)                             //리스트에 아무것도 없으면 0
            {
                if (value < 0)                              //음수 할당시 그만큼 순환(재귀) 
                {
                    Index = equp.Length + value;
                }
                else if (value > equp.Length - 1)            //최대 크기 이상 할당시 그만큼 순환(재귀) 
                {
                    Index = value - equp.Length;
                }
                else { index = value; }                      //그런게 아니면 값 할당
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
        firePos = Camera.main.transform;
        equp = GameManager.ItemManager.EqupMents;
        InputManger.Instance.keyAction += Inputkey;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (equp[index] != null)
        {
            equp[index].OnHand(GunPosition, offsetPos);
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
        if (size != 0)
        {                                      //무기가 하나이상 있으면
            
            if (Input.GetKeyDown(dropKey))
            {
                DropWeapon();
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

    }
    public void WeaponChange2(IEqupMent weapon, EquipType type)//타입별 무기 스왑
    {
        Vector2 slot;
        switch (type)
        {
            case EquipType.Weapon://타입별로 아이템 매니저에 정의된 크기로 슬롯을 구성
                slot = GameManager.ItemManager.weaponSlot;
                break;
            case EquipType.Throw:
                slot = GameManager.ItemManager.throwSlot;
                break;
            case EquipType.Special:
                slot = GameManager.ItemManager.specialSlot;
                break;
            default:
                slot = Vector2.zero;
                break;
        }
        if (slot != Vector2.zero)
        {//입력받은 슬롯으로 무기 추가
            AddWeapon(weapon, (int)slot.x, (int)slot.y);
        }

    }
    
    void Swap(int _setIndex, int _dir = 1)
    {//1또는 -1으로 들고있는 무기를 전환하는 함수
        offsetPos = Vector3.zero;
        if (equp[index] != null)
        {//무기를 들고있으면 전환
            equp[index].gameObject.SetActive(false);
            equp[index].OutHand();
            InputManger.Instance.keyAction -= equp[index].InputKey;
        }
        Index = _setIndex;
        if (equp[Index] != null)
        {
            equp[Index].gameObject.SetActive(true);
            InputManger.Instance.keyAction += equp[Index].InputKey;
        }
    }
    void SwapNext() { Swap(Index + 1); }
    void SwapPrev() { Swap(Index - 1, -1); }
    void AddWeapon(IEqupMent Weapon, int slotStart, int slotSize)
    {
        if (Weapon.type == EquipType.Weapon){
            ((MainWeapon)Weapon).firePos = firePos;
        }
        //게임오브젝트와, 슬롯시작,슬롯 크기값을 입력받고 그 슬롯에 무기를 추가하는함수
        for (int i = slotStart; i < (slotStart + slotSize); i++)
        {//남는칸이 있을때
            if (equp[i] == null)
            {
                equp[i] = Weapon;
                equp[i].transform.SetParent(transform);
                if (equp[Index] != null)
                {
                    equp[Index].gameObject.SetActive(false);
                }

                size++;
                Swap(i);
                return;
            }
        }
        //주어진 슬롯에 남는 칸이 없으면
        IEqupMent _gun;
        if (Index >= slotStart && slotStart + slotSize > Index)//손에 들고있는거랑 같으면
        {
            InputManger.Instance.keyAction -= equp[index].InputKey;
            _gun = equp[Index];
            equp[Index] = Weapon;
            InputManger.Instance.keyAction += equp[index].InputKey;
        }
        else
        {                                                  //손에 들고있는게 아니면 첫번째랑 교체
            _gun = equp[slotStart];
            equp[slotStart] = Weapon;
            Weapon.gameObject.SetActive(false);
        }
        _gun.transform.position = Weapon.transform.position;
        Weapon.transform.SetParent(transform);
        Utill.DestroyOnLoad(_gun.gameObject);
    }

    public void DropWeapon()
    {
        IEqupMent _go;
        _go = equp[Index];
        equp[Index] = null;
        size--;
        /*
        if (throwingWeapon != null)
        {
            // ThrowingWeapon 스크립트가 있는 경우, Throw 메서드를 사용
            throwingWeapon.Throw(Camera.main.transform);

        }*/
            // ThrowingWeapon이 아닌 경우, 기존의 드롭 로직 사용
        Rigidbody _rid = _go.gameObject.GetComponent<Rigidbody>();
        if (_rid)
        {
           _rid.AddForce(Camera.main.transform.forward * dropForce, ForceMode.Impulse);
        }
        _go.OutHand();
        Utill.DestroyOnLoad(_go.gameObject);
        SwapNext();

    }

}
