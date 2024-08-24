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
    private bool _isReloading = false;    // 장전중
    private bool stopReloading = false;   // 한발씩 장전

    protected override void Awake()
    {
        base.Awake();
        initializeAmmo = 50;             // 총기 최대 탄약
        maxLoadedAmmo = 10;              // 장전될 수 있는 탄약
        damage = 5;                      // 데미지
        bulletRange = 5f;                // 총알 발사 거리
        fireRate = 0.5f;                 // 총알 발사 주기
        recoilX = 0.2f;                  // 좌우 반동
        recoilY = 1f;                    // 수직 반동
        recoilRecoverySpeed = 5f;        // 반동 회복 속도
        reloadTime = 1f;                 // 장전 시간
        adsSpeed = 5;                    // 정조준 속도
        adsFOV = 45;                     // 정조준시 CameraFOV
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
            base.Shoot(_firePos);                   // 이거 아이디어 물어보자.. 베이스로 들어가면 리로드 포함되어있는데 리로드는 이미 여기서 다 정의해놓음 어떻게?
            FireBullet(_firePos);
        }

        // 장전 중이면 장전 끝
        if (_isReloading)
        {
            stopReloading = true;
            _isReloading = false;
            return;
        }
    }

    // 장전 함수
    public override void Reload()
    {
        if (_isReloading)
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
        stopReloading = false;
        StartCoroutine(Reloading());
    }

    IEnumerator Reloading()
    {
        _isReloading = true;
        while (loadedAmmo < maxLoadedAmmo && remainAmmo > 0 && !stopReloading)
        {
            yield return new WaitForSeconds(reloadTime);

            if (stopReloading)
            {
                Debug.Log("장전 중단됨");
                break;
            }
            loadedAmmo++;
            remainAmmo--;
        }
    }

    // 발사 함수
    public override void FireBullet(Transform _firePos)
    {
        base.FireBullet(firePos);
        for (int i = 0; i< shell; i++)
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
                    /*
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
                            hit.transform.GetComponent<IDamageAble>().Damaged(damage * 2);
                        }
                        else
                        {
                            hit.transform.GetComponent<IDamageAble>().Damaged(damage);
                        }
                    }*/
                }
            }
        }
        /*
        List<RaycastHit> allHits = new List<RaycastHit>();
        
        for (int i = 0; i < shell; i++)
        {
            Vector3 spreadDirection = CalculateSpreadDirection(spreadAngle, _firePos);
            Debug.DrawRay(_firePos.position, spreadDirection * bulletRange, Color.red, 1f);

            RaycastHit[] hits = Physics.RaycastAll(_firePos.position, spreadDirection, bulletRange, canAttackMask);
            allHits.AddRange(hits);
        }
        
        foreach (RaycastHit hit in allHits)
        {
            CharacterController _cc = hit.collider.GetComponent<CharacterController>();
            if (_cc != null)
            {
                // CharacterController의 실제 높이 계산, lossyScale은 오브젝트의 절대적인 크기
                float _controllerHeight = _cc.height * hit.transform.lossyScale.y;

                // CharacterController의 하단 y 좌표 계산 ( 지면 y좌표 계산 )
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
                    Debug.Log("바디샷");
                    hit.transform.GetComponent<IDamageAble>().Damaged(damage);
                }
            }
        }
        */
    }

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
