using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class FlashGrenade : ThrowingWeapon
{
    float baseTime;
    int calduration;

    protected override void Awake()
    {
        base.Awake();
        explosiondelay = 2f;
        explosionRadius = 10f;
        effectDuration = 5f;
        baseTime = 2f;
    }

    protected override IEnumerator Explode()
    {
        yield return new WaitForSeconds(explosiondelay);

        StartCoroutine(GameManager.Instance.FlashGrenadeExplode(transform, explosionRadius, effectDuration));
        
        /*
        yield return new WaitForSeconds(explosiondelay);

        Collider[] _targetsInRadius = Physics.OverlapSphere(transform.position, explosionRadius, attackableMask);

        // null 체크 _targetsInRadius가 존재하면
        if (_targetsInRadius != null)
        {
            // _targetsInRadius 속의 _target들에게 아래 명령을 실행
            foreach (Collider _target in _targetsInRadius)
            {
                // 거리별 값 판별 ( 멀어질수록 작은 값 )
                float _distance = Vector3.Distance(transform.position, _target.transform.position);
                float _rangePersent = 1 - (_distance / explosionRadius);
                calduration = Mathf.RoundToInt(effectDuration * _rangePersent);

                // 에너미는
                if (_target.CompareTag("Enemy"))
                {
                    Debug.Log(_target.name);
                    Enemy enemy = _target.GetComponent<Enemy>();
                    // 섬광탄을 보고있다면
                    if (IsPlayerLookingAtFlashbang(_target.transform))
                    {
                        // 시야가 좁아지고 움직임을 멈추고 타겟을 놓친다
                        enemy.findDis = 0.1f;
                        enemy.atkDis = 0f;
                        enemy.fov.visibleTargets.Clear();
                        //enemy.fov.visibleObjects.Clear();
                        enemy.enemyState = EnemyState.Blind;
                    }
                }
                // 플레이어는
                else if (_target.CompareTag("Player"))
                {
                    // 플레이어가 섬광탄을 보고 있는지 확인
                    if (IsPlayerLookingAtFlashbang(Camera.main.transform))
                    {
                        // 눈뽕
                        UIManager.Instance.FlashImage.gameObject.SetActive(true);
                    }
                }
            }
            // 섬광탄 지속 시간 이후
            yield return new WaitForSeconds(calduration + baseTime);

            // _targetsInRadius 속의 _target들에게 아래 명령을 실행
            foreach (Collider _target in _targetsInRadius)
            {
                // 에너미는
                if (_target.CompareTag("Enemy"))
                {
                    Enemy enemy = _target.GetComponent<Enemy>();

                    // 시야를 복구하고, 플레이어를 놓친 상태로 설정
                    enemy.findDis = enemy.originFindDis;
                    enemy.atkDis = enemy.originAtkDis;
                    enemy.agent.isStopped = false;
                    enemy.enemyState = enemy.missingState;
                }
                // 플레이어는
                else if (_target.CompareTag("Player"))
                {
                    UIManager.Instance.FlashImage.gameObject.SetActive(false);
                }
            }
        }
        //Destroy(gameObject);
        */
    }

    // 플레이어가 섬광탄을 보고있는지 판단하는 메서드
    bool IsPlayerLookingAtFlashbang(Transform _character)
    {
        // 플레이어 카메라 참조
        //Camera playerCamera = Camera.main;

        // 플레이어 위치에서 섬광탄 위치로의 방향 벡터를 계산
        Vector3 directionToFlashbang = transform.position - _character.position;

        // 카메라가 바라보는 방향과, 플레이어에서 섬광탄으로의 방향 사이의 각도를 계산
        float angle = Vector3.Angle(_character.forward, directionToFlashbang);
        Debug.Log(angle);
        // 시야각 확인 // 60 = 좌우로 60
        if (angle < 90f)
        {
            // 레이캐스트로 장애물 체크
            RaycastHit hit;
            if (Physics.Raycast(_character.position, directionToFlashbang, out hit))
            {
                // 레이캐스트가 섬광탄에 먼저 닿았는지 확인
                if (hit.collider.gameObject == gameObject)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
