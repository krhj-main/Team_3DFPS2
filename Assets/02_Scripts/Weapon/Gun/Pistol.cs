using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pistol : MainWeapon
{
    private bool canReset = true;         // 처음에만 총알 넣어주기 위해
    private float nextFireTime;           // 다음 발사 주기
    public TextMeshProUGUI ammoTxt;       // 탄약 UI 표시
    [SerializeField] Light flashLight;
    [SerializeField] KeyCode lightToggle;
    protected override void Awake()
    {
        bulletSpread = 0.01f;
        spentBullet = 1;
        maxSpread = 0.2f;

        base.Awake();
        initializeAmmo = 80;             // 총기 최대 탄약
        maxLoadedAmmo = 20;              // 장전될 수 있는 탄약
        damage = 10;                     // 데미지
        bulletRange = 10f;               // 총알 발사 거리
        fireRate = 0.6f;                 // 총알 발사 주기
        recoilX = 0.25f;                    // 좌우 반동
        recoilY = 5f;                   // 수직 반동
        recoilRecoverySpeed = 5f;        // 반동 회복 속도
        reloadTime = 1.5f;               // 장전 시간
        adsSpeed = 8;                    // 정조준 속도
        adsFOV = 50;                     // 정조준시 CameraFOV
        bulletSpread = 0;
        ResetAmmo(initializeAmmo);       // 탄약 세팅
    }

    // 시작할 때, 탄약 세팅 함수
    public void ResetAmmo(int _totalAmmo)
    {
        if (canReset)
        {
            loadedAmmo = maxLoadedAmmo;
            remainAmmo = _totalAmmo - loadedAmmo;
            canReset = false;
        }
    }

    // 슈팅 함수
    public override void Shoot(Transform _firePos)
    {
        if (Time.time >= nextFireTime && !isReloading && canShoot)
        {
            nextFireTime = Time.time + fireRate;
            base.Shoot(_firePos);

            if (loadedAmmo <= 0)
            {
                Reload();
            }
        }
    }

    #region "적 FireBullet"
    // 발사 함수
    public override void FireBullet(Transform _firePos)
    {
        base.FireBullet(firePos);
        RaycastHit hit;
        Vector3 _bulletDir = GetShootDir(_firePos);
        Vector3 direction = _firePos.forward + _bulletDir;


        Debug.DrawRay(_firePos.position, direction * bulletRange, Color.red, 5f);

        if (canShoot)
        {
            if (Physics.Raycast(_firePos.position, direction, out hit, bulletRange))       // 카메라 포지션에서 정면으로 총알 사거리만큼 쏨
            {
                if ((canAttackMask.value & (1 << hit.transform.gameObject.layer)) == 0)
                {
                    Debug.Log($"벽에 닿음: {hit.transform.name}");
                    return;
                }
                IDamageAble target = hit.transform.GetComponent<IDamageAble>();
                if (target != null)
                {
                    target.Damaged(damage, hit.point);
                }
            }
        }
    }
    #endregion

    #region "플레이어 FireBullet"
    public override void PlayerFireBullet()
    {
        base.PlayerFireBullet();

        RaycastHit hit;
        Vector3 _bulletDir = GetShootDir(cam.transform);
        Vector3 direction = cam.transform.forward + _bulletDir;

        Debug.DrawRay(cam.transform.position, direction * bulletRange, Color.red, 5f);

        if (canShoot)
        {
            if (Physics.Raycast(cam.transform.position, direction, out hit, bulletRange))       // 카메라 포지션에서 정면으로 총알 사거리만큼 쏨
            {
                if ((canAttackMask.value & (1 << hit.transform.gameObject.layer)) == 0)
                {
                    Debug.Log($"벽에 닿음: {hit.transform.name}");
                    return;
                }
                IDamageAble target = hit.transform.GetComponent<IDamageAble>();
                if (target != null)
                {
                    target.Damaged(damage, hit.point);
                }
            }
        }
    }
    #endregion

    public override void OnHand(Transform _tr, Vector3 _offSet)
    {
        base.OnHand(_tr, _offSet);
        flashLight.transform.position = PlayerController.Instance.PlayerCamera.transform.position;
        flashLight.transform.rotation = PlayerController.Instance.PlayerCamera.transform.rotation;
    }
    public override void InputKey()
    {
        base.InputKey();
        if (Input.GetKeyDown(lightToggle)) {
            flashLight.enabled = !flashLight.enabled;
        }
    }
}
