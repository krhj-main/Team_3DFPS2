using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Rifle : MainWeapon
{
    private bool canReset = true;         // 처음에만 총알 넣어주기 위해
    private float nextFireTime;           // 다음 발사 주기
    public TextMeshProUGUI ammoTxt;       // 탄약 UI 표시

    protected override void Awake()
    {
        // 실험
        bulletSpread = 0.3f;
        spentBullet = 1;
        maxSpread = 1f;

        base.Awake();
        maxLoadedAmmo = 30;               // 장전될 수 있는 탄약
        initializeAmmo = 180;             // 총기 최대 탄약
        damage = 5;                       // 데미지
        bulletRange = 10f;                // 총알 발사 거리
        fireRate = 0.06f;                 // 총알 발사 주기
        recoilX = 0.5f;                  // 좌우 반동
        recoilY = 10f;                   // 수직 반동
        recoilRecoverySpeed = 10f;        // 반동 회복 속도
        reloadTime = 2.5f;                  // 장전 시간
        adsSpeed = 6;                     // 정조준 속도
        adsFOV = 45;                      // 정조준시 CameraFOV
        ResetAmmo(initializeAmmo);        // 탄약 세팅
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
        RaycastHit hit;
        Vector3 _bulletDir = GetShootDir(_firePos);
        Vector3 direction = _firePos.forward + _bulletDir;

        Debug.DrawRay(_firePos.position, direction * bulletRange, Color.red, 1f);

        if (canShoot)
        {
            base.FireBullet(firePos);
            if (Physics.Raycast(_firePos.position, direction, out hit, bulletRange))       // 카메라 포지션에서 정면으로 총알 사거리만큼 쏨
            {
                if ((canAttackMask.value & (1 << hit.transform.gameObject.layer)) == 0)
                {
                    //Debug.Log($"벽에 닿음: {hit.transform.name}");
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

        Debug.DrawRay(cam.transform.position, direction * bulletRange, Color.red, 1f);

        if (canShoot)
        {
            base.FireBullet(firePos);
            if (Physics.Raycast(cam.transform.position, direction, out hit, bulletRange))       // 카메라 포지션에서 정면으로 총알 사거리만큼 쏨
            {
                if ((canAttackMask.value & (1 << hit.transform.gameObject.layer)) == 0)
                {
                    //Debug.Log($"벽에 닿음: {hit.transform.name}");
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
}
