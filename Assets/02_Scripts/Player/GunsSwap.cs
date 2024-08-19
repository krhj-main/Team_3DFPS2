using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunsSwap : MonoBehaviour
{

    public Transform GunPosition;                 //총이 있어야할 위치(빈게임오브젝트)  
    [SerializeField] KeyCode dropKey;                       //들고있는 총 버리기 키
    public GameObject[] Guns;                           //담아야하는 요소를 모아둔 리스트 
    int index = 0;                                          //선택된 항목의 인덱스
    IEqupMent equp;
    int size = 0;
    float dropForce = 5;
    public Vector3 offsetPos;

    public enum WeaponType { 
        Weapon,
        Throw,
        Special
    }

    public int Index                                        //인덱스를 순환시키기 위한 프로퍼티 
    {
        get { return index; }
        set
        {
            if (size > 0)                             //리스트에 아무것도 없으면 0
            {
                if (value < 0)                              //음수 할당시 그만큼 순환(재귀) 
                {
                    Index = Guns.Length + value;
                }
                else if (value > Guns.Length - 1)            //최대 크기 이상 할당시 그만큼 순환(재귀) 
                {
                    Index = value- Guns.Length;
                }
                else { index = value;}                      //그런게 아니면 값 할당
            }
            else {
                index = 0;
            }
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Guns = GameManager.ItemManager.Guns;                //아이템 메니저의 수치를 변경함
    }

    // Update is called once per frame
    void Update()
    {
        if (size != 0) {                                      //무기가 하나이상 있으면
            float _wheelInput = Input.GetAxis("Mouse ScrollWheel"); //휠 입력을 받고
            if (_wheelInput > 0)                                    //휠 입력에 따라 후치 연산자를 통해 현재 선택된 오브젝트를 끄고
            {                                                       //Index를 증감함
                Swap(1);
            }
            else if (_wheelInput < 0)
            {
                Swap(-1);
            }
            
            Guns[Index].SetActive(true);                            //증감해서 변경된 오브젝트를 켜주고
            Guns[Index].transform.position = GunPosition.position + offsetPos;  //오브젝트 위치 조정
            Guns[Index].transform.rotation = GunPosition.rotation;

            // 변경시 변경한 총의 탄약수 반영
            // 추후에 웨폰 매니저등에서 MainWeapon 값을 가지고 있다가 반영하게끔 수정요함
            if (Guns[index].GetComponent<Weapon>().type == WeaponType.Weapon)
            {
                UIManager.Instance.ReloadAmmoUIUpdate(Guns[Index].GetComponent<MainWeapon>().loadedAmmo, Guns[Index].GetComponent<MainWeapon>().remainAmmo);
                UIManager.Instance.ChangeWeaponUIUpdate(Guns[Index].GetComponent<MainWeapon>().myImage, 0, 0);
            }
            

            if (Input.GetKeyDown(dropKey))
            {
                
                DropWeapon();
            }
        }

    }
    public void WeaponChange2(GameObject weapon, WeaponType type)//타입별 무기 스왑
    {
        Vector2 slot;
        switch (type) {
            case WeaponType.Weapon://타입별로 아이템 매니저에 정의된 크기로 슬롯을 구성
                slot = GameManager.ItemManager.weaponSlot;
                break;
            case WeaponType.Throw:
                slot = GameManager.ItemManager.throwSlot;
                break;
            case WeaponType.Special:
                slot = GameManager.ItemManager.specialSlot;
                break;
            default:
                slot = Vector2.zero;
                break;
        }
        if (slot != Vector2.zero) {//입력받은 슬롯으로 무기 추가
            AddWeapon(weapon, (int)slot.x, (int)slot.y);
        }
        
    }
    void Swap(int dir) {//1또는 -1으로 들고있는 무기를 전환하는 함수
        
        offsetPos = Vector3.zero;
        if (Guns[Index] != null) {//무기를 들고있으면 전환
            Guns[Index].gameObject.SetActive(false);
        }
        for (int i = 0; i < Guns.Length; i++) {//빈칸이있으면 무시함
            Index += dir;
            if (Guns[Index] != null) {
                return;
            }
        }
    }

    void AddWeapon(GameObject Weapon, int slotStart,int slotSize) {
        //게임오브젝트와, 슬롯시작,슬롯 크기값을 입력받고 그 슬롯에 무기를 추가하는함수
        for (int i = slotStart; i < (slotStart+slotSize); i++) {//남는칸이 있을때
            if (Guns[i] == null) {
                Guns[i] = Weapon;
                Guns[i].transform.SetParent(transform);
                if (Guns[Index] != null)
                {
                    Guns[Index].SetActive(false);
                }
                size++;
                Index = i;
                return;
            }
        }
        //주어진 슬롯에 남는 칸이 없으면
        GameObject _gun;
        if (Index >= slotStart && slotStart + slotSize > Index)//손에 들고있는거랑 같으면
        {
            _gun = Guns[Index];
            Guns[Index] = Weapon;
        }
        else {                                                  //손에 들고있는게 아니면 첫번째랑 교체
            _gun = Guns[slotStart];
            Guns[slotStart] = Weapon;
            Weapon.SetActive(false);
        }
        _gun.transform.position = Weapon.transform.position;
        Weapon.transform.SetParent(transform);
        Utill.DestroyOnLoad(_gun);
    }

    public void DropWeapon() 
    {
        GameObject _go;
        _go = Guns[Index];
        Guns[Index] = null;
        size--;
        GunShootTest gunShootTest = GetComponent<GunShootTest>();
        ThrowingWeapon throwingWeapon = _go.GetComponent<ThrowingWeapon>();
        if (throwingWeapon != null)
        {
            // ThrowingWeapon 스크립트가 있는 경우, Throw 메서드를 사용
            throwingWeapon.Throw(Camera.main.transform);
         
        }
        else
        {
            // ThrowingWeapon이 아닌 경우, 기존의 드롭 로직 사용
            Rigidbody _rid = _go.GetComponent<Rigidbody>();
            if (_rid)
            {
                _rid.AddForce(Camera.main.transform.forward * dropForce, ForceMode.Impulse);
            }
            
        }
        Utill.DestroyOnLoad(_go);
        Swap(1);
    
    }
    
}
