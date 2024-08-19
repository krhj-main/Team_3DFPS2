using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum HostageState
{
    Idle,
    Move,
    Damaged,
    Dead,
}

public class Hostage : MonoBehaviour//, IDamageAble
{
    /*
    // 인질의 상태
    HostageState hostageState;

    // 플레이어와의 거리
    public float distanceToPlayer = 3f;

    // 인질의 체력
    public float hp;

    NavMeshAgent hostage;
    CharacterController cc;

    private void Awake()
    {
        hostage = GetComponent<NavMeshAgent>();
        cc = GetComponent<CharacterController>();

        hostageState = HostageState.Idle;
    }

    private void Update()
    {
        switch (hostageState)
        {
            case HostageState.Idle:
                
                break;
            case HostageState.Move:

                break;
        }
    }

    void Idle()
    {

    }

    void Move()
    {
        hostage.isStopped = true;

        hostage.ResetPath();

        hostage.stoppingDistance = distanceToPlayer;

        hostage.destination = PlayerController.Instance.transform.position;
    }

    #region "피격 행동"
    void Damaged()
    {
        StartCoroutine(DamageProcess());
    }

    IEnumerator DamageProcess()
    {
        // 피격 모션 만큼 대기
        yield return new WaitForSeconds(1f);

        hostageState = HostageState.Move;
    }
    #endregion

    #region "사망"
    void Die()
    {
        // 진행 중인 피격 코루틴을 중지
        StopAllCoroutines();

        // 죽음 상태를 처리하기 위한 코루틴 실행
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        // 캐릭터 컨트롤러 컴포넌트 비활성화
        cc.enabled = false;

        yield return new WaitForSeconds(0.01f);
        Destroy(gameObject);
    }
    #endregion

    public void Damaged(int damage)
    {
        // 죽어있을 경우 데미지를 적용하지 않는 예외 처리
        if (hostageState == HostageState.Dead)
        {
            return;
        }

        hp -= damage;

        hostage.isStopped = true;
        hostage.ResetPath();

        if (hp > 0)
        {
            hostageState = HostageState.Damaged;

            // 플레이어를 바라봄
            Vector3 _dirP = (PlayerController.Instance.transform.position - agent.transform.position).normalized;
            _dirP.y = 0;    // 이걸 뺼 경우 몸통이 같이 위를 향함, 추후 모델 넣고 수정
            //transform.forward = _dirP;

            // 현재 방향에서 목표 방향으로 부드럽게 회전
            Quaternion targetRotation = Quaternion.LookRotation(_dirP);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            chasePos = PlayerController.Instance.transform.position;

            //anim.SetTrigger("Damaged");
            Damaged();
        }
        else
        {
            enemyState = EnemyState.Dead;

            //anim.SetTrigger("Die");
            Die();
        }
    }
    */
}
