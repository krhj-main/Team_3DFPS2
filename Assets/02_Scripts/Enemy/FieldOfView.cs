using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    // 시야 영역의 반지름과 시야 각도
    [HideInInspector]
    public float viewRadius;

    //[Header("시야각")]
    //[Range(0, 360)]
    [HideInInspector]
    public float viewAngle;

    // 마스크 2종
    //[Header("적으로 인식할 레이어")]
    [HideInInspector]
    public LayerMask targetMask;
    //[Header("장애물로 인식할 레이어")]
    [HideInInspector]
    public LayerMask obstacleMask;

    // Target mask에 ray hit된 transform을 보관하는 리스트
    [HideInInspector]   // 플레이어 판단용
    public List<Transform> visibleTargets = new List<Transform>();

    //[HideInInspector]
    //public List<Transform> visibleObjects = new List<Transform>();

    [HideInInspector]
    public Collider[] targetsInViewRadius;  // Hide 상태에서 사용

    Enemy enemy;

    [HideInInspector] public float weight = 1f;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        targetMask = enemy.targetMask;
        obstacleMask = enemy.obstacleMask;

        // 0.2초 간격으로 코루틴 호출
        StartCoroutine(FindTargetsWithDelay(0.2f));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            viewRadius = enemy.findDis * weight;
            weight = 1f;
            viewAngle = enemy.viewAngle;
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        //visibleObjects.Clear();
        // viewRadius를 반지름으로 한 원 영역 내 targetMask 레이어(플레이어)인 콜라이더를 모두 가져옴
        targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // 플레이어와 forward와 target이 이루는 각이 설정한 각도 내라면
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                // 타겟으로 가는 레이캐스트에 obstacleMask가 걸리지 않으면 visibleTargets에 Add
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    //visibleObjects.Add(target);
                    /*
                    // 공격 범위 안에 플레이어가 들어오면
                    if (Vector3.Distance(transform.position, visibleTargets[0].position) <= enemy.atkDis)
                    {
                        if (enemy.enemyState != EnemyState.Attack && enemy.enemyState != EnemyState.Dead) 
                        {
                            // 공격 상태로 전환
                            enemy.enemyState = EnemyState.Attack;
                            enemy.currentTime = enemy.atkDelay;
                        }
                    }
                    */
                }
            }
        }
    }

    // y축 오일러 각을 3차원 방향 벡터로 변환한다.
    // 원본과 구현이 살짝 다름에 주의. 결과는 같다.
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }
}
