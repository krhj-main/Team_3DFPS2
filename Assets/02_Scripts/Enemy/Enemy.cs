using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using static UnityEditorInternal.VersionControl.ListControl;
using Unity.VisualScripting;

public enum EnemyState
{
    Idle,
    Patrol,
    Hide,
    Move,
    Attack,
    Damaged,
    Dead,
    Blind
}

public class Enemy : MonoBehaviour, IDamageAble
{
    // 상태 enum 변수
    [Header("초기 에너미 상태 설정")]
    public EnemyState firstState;
    [Header("추적 실패 후 상태 설정")]
    public EnemyState missingState;
    [HideInInspector]
    public EnemyState enemyState;

    [HideInInspector]
    public FieldOfView fov;

    // 체력 변수
    [Header("체력")]
    public int hp;
    public int maxHp = 15;
    Slider hpSlider;

    // 속도 변수
    [Header("추적 시 속도")]
    public float trackingSpd = 4.5f;
    [Header("순찰 시 속도")]
    public float patrolSpd = 3.5f;
    float rotateSpeed = 30f;

    // 플레이어 발견 범위
    [Header("감지 범위 설정")]
    public float findDis;

    // 플레이어 공격 가능 범위 ( 무기의 레이캐스트 distance 값 )
    [HideInInspector]
    public float atkDis = 5f;

    // 기존에 가지고 있는 시야
    [HideInInspector]
    public float originFindDis;
    [HideInInspector]
    public float originAtkDis;

    [Header("시야각")]
    [Range(0, 360)]
    public float viewAngle = 120f;

    // 마스크 2종
    [Header("적으로 인식할 레이어")]
    public LayerMask targetMask;
    [Header("장애물로 인식할 레이어")]
    public LayerMask obstacleMask;


    // 누적 시간
    [HideInInspector]
    public float currentTime = 0;
    // 공격 딜레이
    [HideInInspector]
    public float atkDelay = 2f;
    [HideInInspector]
    public float reloadTime = 2f;

    // 추적중 시간
    float curTrackTime;
    // 추적 리턴 시간
    float trackTime = 5f;

    // Patrol 관련 변수
    float patrolDis;    // Enemy와 웨이포인트와의 거리
    [Header("웨이 포인트 리스트")]
    public List<Transform> wayPoints;
    int index;

    // 상태이상 관련 변수
    public float blindTime;

    // 컴포넌트
    CharacterController cc;
    Animator anim;
    [HideInInspector]
    public Vector3 chasePos;  // 시야각 내에 있을 때 플레이어를 담는 변수
    [HideInInspector]
    public NavMeshAgent agent;

    void Awake()
    {
        index = 0;
        //index = Random.Range(0, wayPoints.Count);

        cc = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
        hpSlider = GetComponentInChildren<Slider>();
        //anim = transform.GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        // 외부 변수 관련 초기화
        GetComponentInChildren<MainWeapon>().loadedAmmo = 99999;
        atkDis = GetComponentInChildren<MainWeapon>().bulletRange;
        GetComponentInChildren<MainWeapon>().fireRate = 0.5f;
        atkDelay = GetComponentInChildren<MainWeapon>().fireRate;

        hp = maxHp;
        originFindDis = findDis;
        originAtkDis = atkDis;
        enemyState = firstState;
    }

    void Update()
    {
        // HP 실시간 파악
        hpSlider.value = (float)hp / (float)maxHp;
        //Debug.Log("에너미 " + hp);
        switch (enemyState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Hide:
                Hide();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Damaged:
                //Damaged();
                break;
            case EnemyState.Dead:
                //Die();
                break;
            case EnemyState.Blind:
                Blind();
                break;
        }
    }

    #region "대기"
    void Idle()
    {
        // Enemy 시야에 플레이어가 들어왔으면
        if (fov.visibleTargets.Count > 0)
        {
            // 상태를 Move로 변경
            enemyState = EnemyState.Move;

            //anim.SetTrigger("IdleToMove");
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }
    #endregion

    #region "순찰"
    void Patrol()
    {
        // 플레이어가 enemy의 시야 범위 내에 들어와 있다면
        if (fov.visibleTargets.Count > 0)
        {
            // 플레이어 발견 범위보다 거리가 낮으면
            if (Vector3.Distance(transform.position, fov.visibleTargets[0].position) < findDis)
            {
                // 상태를 Move로 변경
                enemyState = EnemyState.Move;
                //anim.SetTrigger("IdleToMove");
            }
        }
        // 캐릭터가 시야 내에 없을 때
        else
        {
            // 지정된 위치를 왕복 이동
            agent.stoppingDistance = 0;

            patrolDis = Vector3.Distance(transform.position, wayPoints[index].position);

            if (patrolDis < 0.1f)
            {
                //index++;
                //if (index == wayPoints.Count) index = 0;
                index = Random.Range(0, wayPoints.Count);
                agent.speed = patrolSpd;
            }
            agent.SetDestination(wayPoints[index].position);
        }
    }
    #endregion

    #region "존버"
    void Hide()
    {
        // 만약 시야범위가 아닌 공격범위로 할 경우 아래 코드나 Hide 실행부분을 주석처리하면 됨 //

        if (fov.targetsInViewRadius.Length > 0)
        {
            // Enemy 범위에 플레이어가 들어왔다면 
            if (Vector3.Distance(transform.position, fov.targetsInViewRadius[0].transform.position) < atkDis)
            {
                chasePos = PlayerController.Instance.transform.position;

                // 상태를 Move로 변경
                enemyState = EnemyState.Move;

                //anim.SetTrigger("IdleToMove");
            }
        }
    }
    #endregion

    #region "Blind"
    public void Blind()
    {
        // 시야가 좁아지고 움직임을 멈추고 타겟을 놓친다
        findDis = 0.1f;
        atkDis = 0f;
        fov.visibleTargets.Clear();
        // 이동을 멈추고 경로 초기화
        agent.isStopped = true;
        agent.ResetPath();

        if (GameManager.Instance.BlindTimer(blindTime))
        {
            // 시야를 복구하고, 플레이어를 놓친 상태로 설정
            findDis = originFindDis;
            atkDis = originAtkDis;
            agent.isStopped = false;
            enemyState = missingState;
        }
    }
    #endregion

    #region "이동"
    void Move()
    {
        // 플레이어가 Enemy 시야안에 들어왔을 경우
        if (fov.visibleTargets.Count > 0)
        {
            //추적중 일정시간 동안 시야에서 벗어나면 담겨있는 리스트를 초기화하고 타이머 초기화
            if (Vector3.Distance(transform.position, fov.visibleTargets[0].position) > findDis)
            {
                curTrackTime += Time.deltaTime;
                if (curTrackTime >= trackTime)
                {
                    fov.visibleTargets.Clear();
                    curTrackTime = 0;
                    enemyState = missingState;
                }
            }
            else
            {
                curTrackTime = 0;
            }

            // 거리가 atkDis보다 크다면 플레이어를 추적
            if (Vector3.Distance(transform.position, fov.visibleTargets[0].position) > atkDis)
            {
                // 내비게이션 에이전트의 이동을 멈추고 경로를 초기화
                agent.isStopped = true;
                agent.ResetPath();
                agent.speed = trackingSpd;

                // 내비게이션으로 접근하는 최소 거리를 공격 가능 범위로 지정
                agent.stoppingDistance = atkDis;

                // 내이게이션의 목적지를 플레이어의 위치로 지정
                agent.destination = fov.visibleTargets[0].position;
            }
        }
        // 플레이어가 Enemy 시야에 들어온 적이 없었을 경우
        else if (fov.visibleTargets.Count <= 0)
        {
            // 플레이어의 소리를 들어서 플레이어의 위치값을 전달받았을 경우
            if (chasePos != null)
            {
                // 내비게이션 에이전트의 이동을 멈추고 경로를 초기화
                agent.isStopped = true;
                agent.ResetPath();

                // 내비게이션으로 접근하는 최소 거리를 해당 자리까지
                agent.stoppingDistance = 0;

                // 내이게이션의 목적지를 소리난 위치로 지정
                agent.destination = chasePos;
            }
        }
    }
    #endregion

    #region "공격"
    void Attack()
    {
        if (Vector3.Distance(transform.position, fov.visibleTargets[0].position) < atkDis)
        {
            agent.velocity = Vector3.zero;
            Vector3 _dirP = (fov.visibleTargets[0].position - agent.transform.position).normalized;
            _dirP.y = 0;    // 이걸 뺼 경우 몸통이 같이 위를 향함, 추후 모델 넣고 수정
            //transform.forward = _dirP;

            // 현재 방향에서 목표 방향으로 부드럽게 회전
            Quaternion targetRotation = Quaternion.LookRotation(_dirP);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            if (currentTime > atkDelay)
            {
                GetComponentInChildren<MainWeapon>().Shoot(transform);
                currentTime = 0;

                // 단순 if문이 아니라 좀 더 범용적으로 수정? -> 상속으로 Enemy 종류 자체를 나눈다?, 
                if (gameObject.name.Contains("Shotgun"))
                {
                    atkDelay = Random.Range(1.5f, 2f);
                    GetComponentInChildren<MainWeapon>().fireRate = atkDelay;
                }
                else
                {
                    atkDelay = Random.Range(0.25f, 1.5f);
                    GetComponentInChildren<MainWeapon>().fireRate = atkDelay;
                }
                
                //anim.SetTrigger("StartAttack");
            }
            else
            {
                currentTime += Time.deltaTime;
            }
        }
        else
        {
            enemyState = EnemyState.Move;
            currentTime = 0;

            //anim.SetTrigger("AttackToMove");
        }
    }
    #endregion

    #region "피격 행동"
    void Damaged()
    {
        StartCoroutine(DamageProcess());
    }

    IEnumerator DamageProcess()
    {
        // 피격 모션 만큼 대기
        yield return new WaitForSeconds(1f);

        enemyState = EnemyState.Move;
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
        // enemy의 리스트에서 죽은 자신을 제거
        GameManager.Instance.enemies.Remove(this);

        yield return new WaitForSeconds(0.01f);
        Destroy(gameObject);
    }
    #endregion

    // 데미지 인터페이스 구현
    public void Damaged(int damage)
    {
        // 죽어있을 경우 데미지를 적용하지 않는 예외 처리
        if (enemyState == EnemyState.Dead)
        {
            return;
        }

        hp -= damage;

        agent.isStopped = true;
        agent.ResetPath();

        if (hp > 0)
        {
            enemyState = EnemyState.Damaged;

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

    // 연막탄에 닿았을 시 Enemy에게 끼치는 영향
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("SmokeGrenade"))
        {
            findDis = 2f;
            atkDis = 1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SmokeGrenade"))
        {
            findDis = originFindDis;
            atkDis = originAtkDis;
        }
    }
}
