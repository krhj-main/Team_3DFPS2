using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

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

public enum PatrolState
{
    Random,
    Constant
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

    // 공격 관련 변수
    [Header("발사 위치")]
    public Transform enemyFirePos;
    [HideInInspector]
    public float atkDelay = 2f;
    [HideInInspector]
    public float reloadTime = 2f;

    // 추적중 시간
    float curTrackTime;
    // 추적 리턴 시간
    float trackTime = 5f;

    // Patrol 관련 변수
    [Header("순찰 방식(일정/랜덤)")]
    public PatrolState patrolState;
    float patrolDis;    // Enemy와 웨이포인트와의 거리
    [Header("웨이 포인트 리스트")]
    public List<Transform> wayPoints;
    int index;

    // 상태이상 관련 변수
    [Header("실명 시간")]
    public float blindTime = 5f;

    // 애니메이션 관련 변수
    [Header("애니메이션 변수")]
    public AudioClip walkSound;

    // 컴포넌트
    CharacterController cc;
    Animator anim;
    [HideInInspector]
    public Vector3 chasePos;  // 시야각 내에 있을 때 플레이어를 담는 변수
    [HideInInspector]
    public NavMeshAgent agent;
    MainWeapon weapon;
    

    [Header("머리 비율 설정")]
    public float headRatio =0.3f;


    void Awake()
    {
        switch (patrolState)
        {
            case PatrolState.Constant:
                index = 0;
                break;
            case PatrolState.Random:
                index = Random.Range(0, wayPoints.Count);
                break;
        }

        cc = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
        hpSlider = GetComponentInChildren<Slider>();
        anim = transform.GetComponentInChildren<Animator>();
        weapon = GetComponentInChildren<MainWeapon>();
    }

    private void Start()
    {
        // 외부 변수 관련 초기화
        weapon.loadedAmmo = 99999;
        atkDis = weapon.bulletRange;
        weapon.fireRate = 1.5f;
        atkDelay = weapon.fireRate;

        hp = maxHp;
        originFindDis = findDis;
        originAtkDis = atkDis;
        enemyState = firstState;
    }

    void Update()
    {
        // HP 실시간 파악
        hpSlider.value = (float)hp / (float)maxHp;

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
            // Idle 애니메이션 종료
            anim.SetBool("isIdle", false);

            // 상태를 Move로 변경
            enemyState = EnemyState.Move;
        }
        else
        {
            // Idle 애니메이션 재생
            anim.SetBool("isIdle", true);

            agent.isStopped = true;
            agent.ResetPath();
        }
    }
    #endregion

    #region "순찰"
    void Patrol()
    {
        if (fov.visibleTargets.Count > 0)
        {
            // Patrol 애니메이션 종료
            anim.SetBool("isPatrol", false);

            // 상태를 Move로 변경
            enemyState = EnemyState.Move;
        }
        // 캐릭터가 시야 내에 없을 때
        else
        {
            // Patrol 애니메이션 재생
            anim.SetBool("isPatrol", true);

            // 지정된 위치를 왕복 이동
            agent.stoppingDistance = 0;

            // CharacterController의 실제 높이 계산
            float _controllerHeight = cc.height * transform.lossyScale.y;
            // CharacterController의 하단 y 좌표 계산 ( 지면 )
            float _bottomY = transform.position.y + cc.center.y * transform.lossyScale.y - _controllerHeight / 2;
            // 지면을 기준으로 거리 판단
            Vector3 _enemyPos = new Vector3(transform.position.x, _bottomY, transform.position.z);

            patrolDis = Vector3.Distance(_enemyPos, wayPoints[index].position);

            if (patrolDis < 0.5f)
            {
                switch (patrolState)
                {
                    case PatrolState.Constant:
                        index++;
                        if (index == wayPoints.Count) index = 0;
                        break;
                    case PatrolState.Random:
                        index = Random.Range(0, wayPoints.Count);
                        break;
                }
                agent.speed = patrolSpd;
            }
            agent.SetDestination(wayPoints[index].position);
        }
    }
    #endregion

    #region "존버"
    void Hide()
    {
        // Hide(Idle) 애니메이션 재생
        anim.SetBool("isIdle", true);
        // 만약 시야범위가 아닌 공격범위로 할 경우 아래 코드나 Hide 실행부분을 주석처리하면 됨 //

        if (fov.targetsInViewRadius.Length > 0)
        {
            // Enemy 범위에 플레이어가 들어왔다면 
            if (Vector3.Distance(transform.position, fov.targetsInViewRadius[0].transform.position) < atkDis)
            {
                // Hide(Idle) 애니메이션 종료
                anim.SetBool("isIdle", false);

                // 쫓아갈 위치 대입
                chasePos = PlayerController.Instance.transform.position;

                // 상태를 Move로 변경
                enemyState = EnemyState.Move;
            }
        }
    }
    #endregion

    #region "Blind"
    public void Blind()
    {
        // Hide(Idle) 애니메이션 재생
        anim.SetTrigger("doFlashbang");

        // 시야가 좁아지고 움직임을 멈추고 타겟을 놓친다
        findDis = 0.1f;
        atkDis = 0f;
        fov.visibleTargets.Clear();
        // 이동을 멈추고 경로 초기화
        agent.isStopped = true;
        agent.ResetPath();

        if (GameManager.Instance.Timer(blindTime))
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
        // Move 애니메이션 재생
        anim.SetBool("isMove", true);

        // 플레이어가 Enemy 시야안에 들어왔을 경우
        if (fov.visibleTargets.Count > 0)
        {
            // 쫓아갈 위치 대입
            chasePos = PlayerController.Instance.transform.position;

            // 내비게이션 에이전트의 이동을 멈추고 경로를 초기화
            agent.isStopped = true;
            agent.ResetPath();
            agent.speed = trackingSpd;

            // 내비게이션으로 접근하는 최소 거리를 공격 가능 범위로 지정
            agent.stoppingDistance = 0;

            // 내이게이션의 목적지를 플레이어의 위치로 지정
            agent.destination = chasePos;

            // 거리가 atkDis보다 크다면 플레이어를 추적
            if (Vector3.Distance(transform.position, fov.visibleTargets[0].position) <= atkDis)
            {
                anim.SetBool("isMove", false);
                currentTime = atkDelay;
                enemyState = EnemyState.Attack;
            }
        }
        // 플레이어가 Enemy 시야에 없을 경우
        else
        {
            // 내비게이션 에이전트의 이동을 멈추고 경로를 초기화
            agent.isStopped = false;
            agent.ResetPath();

            // 내비게이션으로 접근하는 최소 거리를 해당 자리까지
            agent.stoppingDistance = 0;

            // 내이게이션의 목적지를 소리난 위치로 지정
            agent.destination = chasePos;

            // 소리난 곳까지 오고 다음 행동 지정
            if (Vector3.Distance(transform.position, chasePos) < 1f)
            {
                // Move 애니메이션 종료
                anim.SetBool("isMove", false);
                anim.SetBool("isIdle", true);

                // 일정 시간 후 상태 전환
                if (Timer(trackTime))
                {
                    anim.SetBool("isIdle", false);
                    enemyState = missingState;
                }
            }
            else
            {
                currentTime = 0;
            }
        }
    }
    #endregion

    #region "공격"
    void Attack()
    {
        // Move 애니메이션 종료
        anim.SetBool("isMove", false);

        if (fov.visibleTargets.Count > 0)
        {
            // 내비게이션 에이전트의 이동을 멈추고 경로를 초기화
            agent.isStopped = true;
            agent.ResetPath();

            // 내비게이션으로 접근하는 최소 거리를 해당 자리까지
            agent.stoppingDistance = atkDis;

            // 내비게이션 위치를 확인한 플레이어 위치까지 지정
            agent.destination = fov.visibleTargets[0].position;

            // 공격범위 이내라면
            if (Vector3.Distance(transform.position, fov.visibleTargets[0].position) <= atkDis)
            {
                agent.velocity = Vector3.zero;
                Vector3 _dirP = (fov.visibleTargets[0].position - agent.transform.position).normalized;
                _dirP.y = 0;    // 이걸 뺼 경우 몸통이 같이 위를 향함, 추후 모델 넣고 수정

                // 현재 방향에서 목표 방향으로 부드럽게 회전
                Quaternion targetRotation = Quaternion.LookRotation(_dirP);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);


                if (Timer(atkDelay))
                {
                    // Attack 애니메이션 재생
                    anim.SetTrigger("doEnemyAttack");
                    weapon.Shoot(enemyFirePos);

                    // 무기 종류에 따라 공격 속도 설정
                    if (gameObject.name.Contains("Shotgun"))
                    {
                        atkDelay = Random.Range(1.5f, 2f);
                        Debug.Log(weapon.fireRate);
                        weapon.fireRate = atkDelay;
                    }
                    else
                    {
                        atkDelay = Random.Range(0.25f, 0.5f);
                        weapon.fireRate = atkDelay;
                    }
                }
            }
            // 공격범위 밖이라면
            else
            {
                chasePos = PlayerController.Instance.transform.position;
                enemyState = EnemyState.Move;
                currentTime = 0;
            }
        }
        // 플레이어가 시야에 없다면
        else
        {
            chasePos = PlayerController.Instance.transform.position;
            enemyState = EnemyState.Move;
        }
    }
    #endregion

    #region "피격 행동"
    void DamagedAction()
    {
        if (enemyState != EnemyState.Attack)
        {
            enemyState = EnemyState.Move;
        }
    }
    #endregion

    #region "사망"
    void Die()
    {
        // 진행 중인 피격 코루틴을 중지
        StopAllCoroutines();

        // 사망 애니메이션 재생
        anim.SetTrigger("doDead");
        // 캐릭터 컨트롤러 컴포넌트 비활성화
        cc.enabled = false;
        // enemy의 리스트에서 죽은 자신을 제거
        GameManager.Instance.enemies.Remove(this);

        //UI 업데이트
        UIManager.Instance.RemainEnemy();

        GameManager.Instance.enemyScore += 10;
    }
    #endregion

    // 데미지 인터페이스 구현
    public void Damaged(int damage , Vector3 hitpoint)
    {
        // 죽어있을 경우 데미지를 적용하지 않는 예외 처리
        if (enemyState == EnemyState.Dead)
        {
            return;
        }

        if (IsHeadShot(hitpoint))
        {
            hp -= damage*2;
            GameManager.Instance.enemyScore += 5;
        }
        else 
        {
            hp -= damage;
        }
        

        agent.isStopped = true;
        agent.ResetPath();

        if (hp > 0)
        {
            if (enemyState != EnemyState.Attack)
            {
                enemyState = EnemyState.Damaged;
            }

            // 플레이어를 바라봄
            Vector3 _dirP = (PlayerController.Instance.transform.position - agent.transform.position).normalized;
            _dirP.y = 0;    // 이걸 뺼 경우 몸통이 같이 위를 향함, 추후 모델 넣고 수정
            //transform.forward = _dirP;

            // 현재 방향에서 목표 방향으로 부드럽게 회전
            Quaternion targetRotation = Quaternion.LookRotation(_dirP);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            // 목표지점 (플레이어) 지정
            chasePos = PlayerController.Instance.transform.position;

            DamagedAction();
        }
        else
        {
            enemyState = EnemyState.Dead;

            Die();
        }
    }

    // 연막탄에 닿았을 시 Enemy에게 끼치는 영향
    void OnTriggerEnter(Collider other)
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
    private bool IsHeadShot(Vector3 _hitpoint)
    {
        // CharacterController의 실제 높이 계산
        float _controllerHeight = cc.height * transform.lossyScale.y;

        // CharacterController의 하단 y 좌표 계산 ( 지면 )
        float _bottomY = transform.position.y + cc.center.y * transform.lossyScale.y - _controllerHeight / 2;

        // hit.point의 상대적 높이 비율 계산
        float _relativeHeight = (_hitpoint.y - _bottomY) / _controllerHeight;
        // 히트한 높이가 헤드샷 지정 높이 이상이면 헤드샷 / 아니면 바디샷
        return (_relativeHeight >= (1 - headRatio));
    }

    public bool Timer(float _targetTime)
    {
        currentTime += Time.deltaTime;
        if (currentTime > _targetTime)
        {
            currentTime = 0;
            return true;
        }

        return false;
    }
}
