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
        fireRate = 1.5f;                  // 총알 발사 주기
        recoilX = 0.5f;                   // 좌우 반동
        recoilY = 1f;                     // 수직 반동
        recoilRecoverySpeed = 5f;         // 반동 회복 속도
        reloadTime = 5f;                  // 장전 시간
        adsSpeed = 4;                     // 정조준 속도
        adsFOV = 15;                      // 정조준시 CameraFOV
        ResetAmmo(initializeAmmo);        // 탄약 세팅
        adsPos = new Vector3(0, -0.19f, 0.5f); // 스나만 다른 위치로 정조준 ( 조준경 때문에 )
        // 실험
        spentBullet = 1;
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
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            base.Shoot(_firePos);
            FireBullet(_firePos);
        }
    }
    //
    // 장전 함수
    public override void Reload()
    {
        base.Reload();
    }

    // 발사 함수 ( 수정 필요 )
    public override void FireBullet(Transform _firePos)
    {
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
                    /*
                    // 데미지 입히기
                    CharacterController _cc = hit.collider.GetComponent<CharacterController>();
                    if (_cc != null)
                    {
                        // CharacterController의 실제 높이 계산
                        float _controllerHeight = _cc.height * hit.transform.lossyScale.y;

                        // CharacterController의 하단 y 좌표 계산 ( 지면 )
                        float _bottomY = hit.transform.position.y + _cc.center.y * hit.transform.lossyScale.y - _controllerHeight / 2;

                        // hit.point의 상대적 높이 비율 계산
                        float _relativeHeight = (hit.point.y - _bottomY) / _controllerHeight;

                        // 히트한 높이가 헤드샷 지정 높이 이상이면 헤드샷 / 아니면 바디샷
                        if (_relativeHeight >= (1 - headRatio))
                        {
                            Debug.Log("헤드샷");
                            hit.transform.GetComponent<IDamageAble>().Damaged(damage * 2);
                        }
                        else
                        {
                            hit.transform.GetComponent<IDamageAble>().Damaged(damage);
                        }
                        
                    }*/
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
}
