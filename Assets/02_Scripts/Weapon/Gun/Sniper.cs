using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Sniper : MainWeapon
{
    private bool canReset = true;         // 처음에만 총알 넣어주기 위해
    private float nextFireTime;           // 다음 발사 주기
    public TextMeshProUGUI ammoTxt;       // 탄약 UI 표시

    private int canPenetrateEnemy = 2;       // 관통 가능한 숫자
    public GameObject scope;

    protected override void Awake()
    {
        bulletSpread = 10f;
        spentBullet = 1;
        maxSpread = 10f;

        base.Awake();
        initializeAmmo = 50;              // 총기 최대 탄약
        maxLoadedAmmo = 5;                // 장전될 수 있는 탄약
        damage = 60;                      // 데미지
        bulletRange = 200f;               // 총알 발사 거리
        fireRate = 2.5f;                  // 총알 발사 주기
        recoilX = 1f;                   // 좌우 반동
        recoilY = 10f;                     // 수직 반동
        recoilRecoverySpeed = 5f;         // 반동 회복 속도
        reloadTime = 4.5f;                  // 장전 시간
        adsSpeed = 4;                     // 정조준 속도
        adsFOV = 10;                      // 정조준시 CameraFOV
        ResetAmmo(initializeAmmo);        // 탄약 세팅
        adsPos = new Vector3(0, -0.17f, 0.5f); // 스나만 다른 위치로 정조준 ( 조준경 때문에 )
        // 실험
        bulletSpread = 2;
    }

    private void Update()
    {
        
    }


    // 시작할 때, 탄약 세팅 함수 //
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
    // 발사 함수 ( 수정 필요 )
    public override void FireBullet(Transform _firePos)
    {
        base.FireBullet(firePos);
        int _penetrateEnemy = 0;
        RaycastHit hit;
        Vector3 _bulletDir = GetShootDir(_firePos);
        Vector3 currentPosition = _firePos.position;
        Vector3 direction = _firePos.forward + _bulletDir;
        float distanceTraveled = 0f;

        Debug.DrawRay(_firePos.position, direction * bulletRange, Color.red, 5f);

        if (canShoot)
        {
            while (_penetrateEnemy <= canPenetrateEnemy && distanceTraveled < bulletRange)
            {
                if (Physics.Raycast(currentPosition, direction, out hit, bulletRange - distanceTraveled))
                {
                    // canAttackMask에 해당하지 않는 오브젝트에 닿으면 즉시 중단
                    if ((canAttackMask.value & (1 << hit.transform.gameObject.layer)) == 0)
                    {
                        Debug.Log($"벽에 닿음: {hit.transform.name}");
                        break; 
                    }
                    IDamageAble target = hit.transform.GetComponent<IDamageAble>();
                    if (target != null)
                    {
                        target.Damaged(damage, hit.point);
                        _penetrateEnemy++;
                    }
                    // 거리 계산하고 레이캐스트 다시 앞으로 나가기
                    distanceTraveled += hit.distance;
                    currentPosition = hit.point + direction * 0.001f;
                }
                else
                {
                    break;  // 아무것도 맞지 않았다면 루프 종료
                }
            }
        }
    }
    #endregion

    #region "플레이어 FireBullet"
    public override void PlayerFireBullet()
    {
        base.PlayerFireBullet();

        int _penetrateEnemy = 0;
        RaycastHit hit;
        Vector3 _bulletDir = GetShootDir(cam.transform);
        Vector3 currentPosition = cam.transform.position;
        Vector3 direction = cam.transform.forward + _bulletDir;
        float distanceTraveled = 0f;

        Debug.DrawRay(cam.transform.position, direction * bulletRange, Color.red, 5f);

        if (canShoot)
        {
            while (_penetrateEnemy <= canPenetrateEnemy && distanceTraveled < bulletRange)
            {
                if (Physics.Raycast(currentPosition, direction, out hit, bulletRange - distanceTraveled))
                {
                    // canAttackMask에 해당하지 않는 오브젝트에 닿으면 즉시 중단
                    if ((canAttackMask.value & (1 << hit.transform.gameObject.layer)) == 0)
                    {
                        Debug.Log($"벽에 닿음: {hit.transform.name}");
                        break;
                    }
                    IDamageAble target = hit.transform.GetComponent<IDamageAble>();
                    if (target != null)
                    {
                        target.Damaged(damage, hit.point);
                        _penetrateEnemy++;
                    }
                    // 거리 계산하고 레이캐스트 다시 앞으로 나가기
                    distanceTraveled += hit.distance;
                    currentPosition = hit.point + direction * 0.001f;
                }
                else
                {
                    break;  // 아무것도 맞지 않았다면 루프 종료
                }
            }
        }
    }
    #endregion


    public override void OnHand(Transform _tr, Vector3 _offSet)
    {
        base.OnHand(_tr, _offSet);

        UIManager.Instance.snimperZoomUI.enabled = isADS;
        scope.SetActive(!isADS);
    }

    public override void OnHandExit()
    {
        base.OnHandExit();

        UIManager.Instance.snimperZoomUI.enabled = false;
        scope.SetActive(false);
    }

    public override void Aming(bool _whatAim)
    {
        base.Aming(_whatAim);
        UIManager.Instance.CrossHair(!_whatAim);
        UIManager.Instance.snimperZoomUI.enabled = _whatAim;
        scope.SetActive(!_whatAim);
    }
}
