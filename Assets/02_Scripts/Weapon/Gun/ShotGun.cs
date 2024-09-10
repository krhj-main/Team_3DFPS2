using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShotGun : MainWeapon
{
    private bool canReset = true;         // 처음에만 총알 넣어주기 위해
    private float nextFireTime;           // 다음 발사 주기
    public TextMeshProUGUI ammoTxt;       // 탄약 UI 표시
    private int shell = 10;               // 발사되는 셸
    private float spreadAngle = 30f;      // 퍼지는각도
    //private bool _isReloading = false;    // 장전중
    //private bool stopReloading = false;   // 한발씩 장전

    private float reloadEnter = 0.18f;
    private float reloadEnd = 2.12f;

    protected override void Awake()
    {
        base.Awake();
        initializeAmmo = 50;             // 총기 최대 탄약
        maxLoadedAmmo = 5;              // 장전될 수 있는 탄약
        damage = 6;                      // 데미지
        bulletRange = 5f;                // 총알 발사 거리
        fireRate = 1.26f;                 // 총알 발사 주기
        recoilX = 0.5f;                  // 좌우 반동
        recoilY = 10f;                    // 수직 반동
        recoilRecoverySpeed = 5f;        // 반동 회복 속도
        reloadTime = 1.3f;              // 장전 시간  //0.18f + 1.12f + 0.5f+2.12f
        adsSpeed = 5;                    // 정조준 속도
        adsFOV = 45;                     // 정조준시 CameraFOV
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
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            base.Shoot(_firePos);

            if (loadedAmmo <= 0)
            {
                Debug.Log("장전된 탄약 없음");
                Reload();
                canShoot = false;                 // 총알 없으면 슈팅 불가능
            }
        }

        // 장전 중이면 장전 끝
        if (isReloading && loadedAmmo > 0)
        {
            anim.SetBool("isReloading", false);
            //stopReloading = true;
            isReloading = false;
            return;
        }
    }

    // 장전 함수
    public override void Reload()
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
        //stopReloading = false;
        //StartCoroutine(ShotgunReloading());
        anim.SetBool("isReloading", true);
    }
    /*
    IEnumerator ShotgunReloading()
    {
        anim.SetBool("isReloading", true);
        yield return new WaitForSeconds(reloadEnter);

        while (loadedAmmo < maxLoadedAmmo && remainAmmo > 0 && !stopReloading)
        {
            if (loadedAmmo == maxLoadedAmmo - 1)
            {
                anim.SetBool("isReloading", false);
                break;
            }
            if (loadedAmmo < maxLoadedAmmo - 1)
            {
                anim.SetTrigger("doReload");
            }
            yield return new WaitForSeconds(reloadTime);
        }
    }
        
    IEnumerator ShotgunReloading()
    {
        _isReloading = true;
        anim.SetBool("isReloading", true);
        while (loadedAmmo < maxLoadedAmmo && remainAmmo > 0 && !stopReloading)
        {
            if (loadedAmmo < maxLoadedAmmo - 1)
            {
                anim.SetTrigger("doReload");
            }
            else if (loadedAmmo == maxLoadedAmmo - 1)
            {
                anim.SetBool("isReloading", false);
            }

            yield return new WaitForSeconds(reloadTime);

            if (stopReloading)
            {
                Debug.Log("장전 중단됨");
                anim.SetBool("isReloading", false);
                anim.SetTrigger("doAttack");
                break;
            }
            loadedAmmo++;
            remainAmmo--;
        }
    }
    */


    #region "적 FireBullet"
    // 발사 함수
    public override void FireBullet(Transform _firePos)
    {
        base.FireBullet(firePos);
        for (int i = 0; i < shell; i++)
        {
            Vector3 spreadDirection = CalculateSpreadDirection(spreadAngle, _firePos);
            Debug.DrawRay(_firePos.position, spreadDirection * bulletRange, Color.red, 1f);

            RaycastHit hit;

            if (canShoot)
            {
                if (Physics.Raycast(_firePos.position, spreadDirection, out hit, bulletRange))       // 카메라 포지션에서 정면으로 총알 사거리만큼 쏨
                {
                    if ((canAttackMask.value & (1 << hit.transform.gameObject.layer)) == 0)
                    {
                        continue;
                    }
                    IDamageAble target = hit.transform.GetComponent<IDamageAble>();
                    if (target != null)
                    {
                        target.Damaged(damage, hit.point);
                    }
                }
            }
        }
    }
    #endregion

    #region "플레이어 FireBullet"
    // 발사 함수
    public override void PlayerFireBullet()
    {
        base.PlayerFireBullet();
        for (int i = 0; i < shell; i++)
        {
            Vector3 spreadDirection = CalculateSpreadDirection(spreadAngle, cam.transform);
            

            RaycastHit hit;

            if (canShoot)
            {
                Debug.DrawRay(cam.transform.position, spreadDirection * bulletRange, Color.red, 1f);
                if (Physics.Raycast(cam.transform.position, spreadDirection, out hit, bulletRange))       // 카메라 포지션에서 정면으로 총알 사거리만큼 쏨
                {
                    
                    if ((canAttackMask.value & (1 << hit.transform.gameObject.layer)) == 0)
                    {
                        continue;
                    }
                    IDamageAble target = hit.transform.GetComponent<IDamageAble>();
                    if (target != null)
                    {
                        target.Damaged(damage, hit.point);
                    }
                }
            }
        }
    }
    #endregion

    private Vector3 CalculateSpreadDirection(float _speadAngle, Transform _firePos)
    {
        // 균일한 원 내의 랜덤한 점 생성
        float randomRadius = Random.Range(0f, 1f);                  // 원의 중심으로부터 거리
        float randomAngle = Random.Range(0f, 2f * Mathf.PI);        // 360도 사이의 무작위 각도

        // 원뿔모양의 표면의 값을 점으로 변환
        float x = Mathf.Cos(randomAngle) * randomRadius;
        float y = Mathf.Sin(randomAngle) * randomRadius;
        float z = Mathf.Sqrt(1f - randomRadius * randomRadius);     // z좌표는 점이 단위 구의 표면 위에 위치

        // 퍼짐 각도 적용
        Vector3 spreadVector = new Vector3(x, y, z);                // Vector3 값으로 변환
        spreadVector = Vector3.Slerp(Vector3.forward, spreadVector, _speadAngle / 180f);    // 각도를 정규화

        // 총알 나가는 곳을 기준으로 확산
        return _firePos.rotation * spreadVector;
    }
}
