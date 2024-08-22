using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainWeapon : MonoBehaviour, Interactable, IEquipMent
{
    // 실험
    protected float originBulletSpread;
    protected float bulletSpread = 1f;           // 탄 퍼짐
    protected float maxSpread = 1f;

    // 탄약 관련 변수
    public virtual int initializeAmmo { get; set; }  // 최대 탄약
    public virtual int maxLoadedAmmo { get; set; }   // 최대 장탄수
    public virtual int loadedAmmo { get; set; }      // 장전된 탄약
    public virtual int remainAmmo { get; set; }      // 남은 탄약

    // 데미지 관련 변수
    public virtual int damage { get; set; }          // 데미지
    public virtual float bulletRange { get; set; }   // 총알 사거리
    public virtual float fireRate { get; set; }      // 발사 주기 
    public virtual int spentBullet { get; set; }     // 총알 발사한 숫자 ( 탄 퍼짐 관련 변수 실험중)
    protected bool canShoot = true;                  // 발사 가능 변수
    public LayerMask canAttackMask;                  // 데미지 입힐 수 있는 유닛

    // 반동 관련 변수
    public virtual float recoilX { get; set; }               // 좌우 반동 크기
    public virtual float recoilY { get; set; }               // 수직 반동 크기
    public virtual float recoilRecoverySpeed { get; set; }   // 반동 회복 속도
    public Vector3 currentRotation;                          // 현재 카메라 값
    protected Vector3 targetRotation;                        // 반동될 카메라 값

    // 재장전 관련 변수
    public virtual float reloadTime { get; set; }   // 재장전 시간
    private bool isReloading = false;               // 장전중

    // 정조준 관련 변수
    public virtual float adsSpeed { get; set; }     // 정조준 속도
    public virtual float adsFOV { get; set; }       // 줌 정도
    private float shoulderFOV;
    private float targetFOV;
    Vector3 shoulderPos;                            // 견착 위치
    protected Vector3 adsPos;                       // 정조준 위치
    Vector3 targetPos;                              // 이동할 위치
    private EquipmentsSwap gunSwap;                       // 건스왑에서 총기 위치 옮겨줌
    bool isAming = false;
    public Transform firePos;                       // 총기가 발사될 위치
    [SerializeField] protected Camera cam;          // 메인 카메라
    public virtual float headRatio { get; set; }    // 머리 비율
    Transform IEquipMent.transform { get => transform; set { } }
    GameObject IEquipMent.gameObject { get => gameObject; set { } }
    bool isADS = false;
    [field: SerializeField]
    public EquipType type { get; set; }
    [SerializeField] public Sprite myImage;         // 무기 이미지
    [Range(-100,100)]
    [SerializeField] float speedDownForce;
    [SerializeField] GameObject arms;
    [SerializeField] public Transform CameraPos;
    protected virtual void Awake()
    {
        originBulletSpread = bulletSpread;
        headRatio = 0.3f; // 더 작게 하려면 0.125 / 더 크게하려면 0.143 / 현재는 임의로 지정
        //camController = GetComponentInParent<CharacterController>().GetComponentInChildren<CameraController>();
        adsPos = new Vector3(0, -0.25f, 0f);
    }

    // 부모가 생기면 초기화 해줌
    private void OnTransformParentChanged()
    {
        if (transform.parent != null)
        {
            gunSwap = GetComponentInParent<EquipmentsSwap>();
            if (gunSwap) {
                shoulderPos = gunSwap.GunPosition.localPosition;
            }

            shoulderFOV = cam.fieldOfView;
        }
        else
        {
            return;
        }
    }

    #region 슈팅 함수
    public virtual void Shoot(Transform _firePos)
    {
        
        if (loadedAmmo > 0)                  // 장전된 탄약이 0보다 크면 탄약 빼주기
        {
            loadedAmmo--;                     // 탄약 마이너스
            if (!_firePos.gameObject.CompareTag("Enemy"))
            {
                CameraController camController = GetComponentInParent<CharacterController>().GetComponentInChildren<CameraController>();
                camController.ApplyRecoil(recoilX, recoilY);    // 반동
            }

            canShoot = true;                  // 슈팅 가능
        }
    }
    #endregion

    #region 장전 함수
    public virtual void Reload()
    {
        if (isReloading)
        {
            Debug.Log("재장전중");
            return;
        }

        if (loadedAmmo == maxLoadedAmmo)
        {
            Debug.Log("탄창 꽉참");
            return;
        }

        if (remainAmmo <= 0)
        {
            Debug.Log("남은 탄약 없음");
            return;
        }

        StartCoroutine(Reloading());
    }

    // 장전 코루틴
    IEnumerator Reloading()
    {
        isReloading = true;
        Debug.Log("장전시작");
        // 이 위치에 장전 애니메이션 추가

        yield return new WaitForSeconds(reloadTime);  // 장전 걸리는 시간

        int _ammoToMagazine = Mathf.Min(maxLoadedAmmo - loadedAmmo, remainAmmo);    // 둘 중 작은 값 비교하기
        loadedAmmo += _ammoToMagazine;
        remainAmmo -= _ammoToMagazine;
        isReloading = false;
        Debug.Log("장전 끝");

        // 장전 끝나고 총알 수 UI에 반영
        UIManager.Instance.ReloadAmmoUIUpdate(loadedAmmo, remainAmmo);
    }
    #endregion

    #region 발사 함수
    public virtual void FireBullet(Transform _firePos) { }    // 무조건 자식 클래스에서 재정의
    #endregion

    #region 정조준 함수

    public void Aming(bool _whatAim)
    {
        isAming = true;

        if (_whatAim)
        {
            targetPos = adsPos;
            targetFOV = adsFOV;
            bulletSpread = 0;
        }

        if (!_whatAim)
        {
            targetPos = shoulderPos;
            targetFOV = shoulderFOV;
            bulletSpread = originBulletSpread;
        }
    }

    void UpdateAiming()
    {
        if (gunSwap) 
        { 
            gunSwap.GunPosition.localPosition = Vector3.Lerp(gunSwap.GunPosition.localPosition, targetPos, Time.deltaTime * adsSpeed); 
        }
        // 무기 위치 업데이트
        

        // 카메라 FOV 업데이트
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * adsSpeed);

        if (cam.fieldOfView == targetFOV)
        {
            isAming = false;
        }
    }
    #endregion

    #region 탄 퍼짐 함수
    protected Vector3 GetShootDir(Transform _firePos)
    {
        Vector3 _direction = _firePos.forward;  // 전방 기준
        float _spread = UnityEngine.Random.Range(-bulletSpread, bulletSpread) * spentBullet;

        // 카메라의 위치 동기화 + 탄퍼짐
        if (_spread < maxSpread)
        {
            _direction += _firePos.up * _spread;
            _direction += _firePos.right * _spread;
        }
        else
        {
            _direction = _firePos.up * maxSpread;
            _direction = _firePos.right * maxSpread;
        }

        // 결과값 리턴
        return _direction.normalized;
    }
    #endregion

    
    //상호작용
    public virtual void Interaction(GameObject target)
    {
        EquipmentsSwap swap = target.GetComponent<EquipmentsSwap>();
        if (swap != null)
        {
            swap.WeaponChange(this, EquipType.Weapon);
        }
    }
    public void OnHandEnter()
    {

        PlayerController.Instance.moveSpeedScale = speedDownForce/100;
        arms.SetActive(true);
        firePos.SetParent(CameraPos);
    }
    //손에있을때 할 행동
    public virtual void OnHand(Transform _tr,Vector3 _offSet)
    {
        transform.position = _tr.position + _offSet;  //오브젝트 위치 조정
        transform.rotation = _tr.rotation;
        UIManager.Instance.ReloadAmmoUIUpdate(loadedAmmo, remainAmmo);
        UIManager.Instance.ChangeWeaponUIUpdate(myImage, 0, 0);
        
        if (isAming)
        {
            UpdateAiming();
        }
    }
    public virtual void OnHandExit()
    {
        PlayerController.Instance.moveSpeedScale = 0;
        StopCoroutine(Reloading());
        isReloading = false;
        isAming = false;
        if (gunSwap)
        {
            gunSwap.GunPosition.localPosition = shoulderPos;
        }
        // 무기 위치 업데이트


        // 카메라 FOV 업데이트
        cam.fieldOfView = shoulderFOV;

        if (cam.fieldOfView == targetFOV)
        {
            isAming = false;
        }
        arms.SetActive(false);
        firePos.SetParent(null);
    }
    //키입력
    public virtual void InputKey()
    {
        if (Input.GetMouseButton(0))
        {
            Shoot(firePos);
            GameManager.Instance.AggroEnemy(firePos.position, 30f);

        }

        if (Input.GetMouseButtonDown(1))
        {
            Aming(isADS);
            isADS = !isADS;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }   
    //무기 바꿀때 할 행동
    

    
}

    
