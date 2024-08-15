using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;  // 씬 매니저

public class GameManager : Singleton<GameManager>
{
    ItemManager itemManager = new ItemManager();                  
    public static ItemManager ItemManager
    {
        get {return Instance.itemManager; }
    }

    public List<Enemy> enemies = new List<Enemy>();
    float calduration;
    float baseTime = 1.5f;
    //public List<IDamageAble> damageAbles = new List<IDamageAble>();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 씬이 로드될 때마다 실행되는 이벤트에 넣을 함수들이나 기능 구현
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AddEnemyOnNowScene();
    }

    // 씬이 로드될 때 존재하는 모든 Enemy를 List에 담는 함수 ( ScnenManager 등에서 호출 )
    public void AddEnemyOnNowScene()
    {
        // 에너미 리스트 초기화
        enemies.Clear();
        // 씬에 존재하는 모든 enemy를 담는다
        enemies.AddRange(FindObjectsOfType<Enemy>());
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Enemy의 어그로를 끄는 메서드
    public void AggroEnemy(Vector3 _soundPos, float _radius)
    {
        foreach(Enemy enemy in enemies)
        {
            // 에너미와 소리난 곳의 거리 계산
            float _distance = Vector3.Distance(_soundPos, enemy.gameObject.transform.position);

            // 거리가 범위 이내라면
            if (_distance <= _radius)
            {
                // 소리의 위치를 chasePos 변수에 담고
                enemy.chasePos = _soundPos;

                // 존버 상태가 아닐때만
                if (enemy.enemyState != EnemyState.Hide)
                {
                    // enemy의 상태를 Move로 변경해 소리가 난 곳으로 이동
                    enemy.enemyState = EnemyState.Move;
                }
            }
        }
    }
    #region "AggroEnemy 원본"
    /*
    // 플레이어쪽에서 호출해 일정 범위 안에 있는 enemy에게 플레이어의 위치값을 전달해주는 함수
    public void AggroEnemy(Vector3 soundPos, float radius)
    {
        // radius 범위안에 Enemy 레이어를 가진 콜라이더를 다 담는다
        LayerMask _targetMask = LayerMask.GetMask("Enemy");
        Collider[] _targetsInRadius = Physics.OverlapSphere(transform.position, radius, _targetMask);

        // null 체크 _targetsInRadius가 존재하면
        if (_targetsInRadius != null)
        {
            // _targetsInRadius 속의 _target들에게 아래 명령을 실행
            foreach (Collider _target in _targetsInRadius)
            {
                if (_target.GetComponent<Enemy>().enemyState != EnemyState.Blind)
                {
                    // 소리의 위치를 player 변수에 담고
                    _target.GetComponent<Enemy>().player = soundPos;

                    if (_target.GetComponent<Enemy>().enemyState != EnemyState.Hide)
                    {
                        // enemy의 상태를 Move로 변경해 소리가 난 곳으로 이동
                        _target.GetComponent<Enemy>().enemyState = EnemyState.Move;
                    }
                }
            }
        }
    }
    */
    #endregion

    // 섬광탄 효과 ( 눈뽕, 에너미 멈춤 등 )
    public IEnumerator FlashGrenadeExplode(Transform _explode, float _radius, float _effectDuration)
    {
        // 플레이어와 폭발한 곳의 거리 계산
        float _distanceToPlayer = Vector3.Distance(_explode.position, PlayerController.Instance.transform.position);
        if ( _distanceToPlayer <= _radius )
        {
            if (IsLookingAtFlash(_explode, PlayerController.Instance.transform))
            {
                // 거리별 값 판별 ( 멀어질수록 작은 값 )
                //float _rangePersentToPlayer = 1 - (_distanceToPlayer / _radius);
                //calduration = Mathf.RoundToInt(_effectDuration * _rangePersentToPlayer);
                // 눈뽕
                UIManager.Instance.FlashImage.gameObject.SetActive(true);
            }
        }

        // 에너미
        foreach (Enemy enemy in enemies)
        {
            // 에너미와 폭발한 곳의 거리 계산
            float _distance = Vector3.Distance(_explode.position, enemy.transform.position);

            // 거리별 값 판별 ( 멀어질수록 작은 값 )
            //float _rangePersent = 1 - (_distance / _radius);
            //calduration = Mathf.RoundToInt(_effectDuration * _rangePersent);

            // 거리가 범위 이내라면
            if (_distance < _radius)
            {
                // Enemy가 섬광탄을 보고있다면
                if (IsLookingAtFlash(_explode, enemy.transform))
                {
                    // 시야가 좁아지고 움직임을 멈추고 타겟을 놓친다
                    enemy.findDis = 0.1f;
                    enemy.atkDis = 0f;
                    enemy.fov.visibleTargets.Clear();
                    enemy.enemyState = EnemyState.Blind;
                }
            }
        }

        // 1.5초 + 거리별 시간 이후 섬광 끝 // 현재 각각 다르게 적용되어야 할 시간이 하나로만 적용중
        yield return new WaitForSeconds(_effectDuration);

        UIManager.Instance.FlashImage.gameObject.SetActive(false);

        foreach (Enemy enemy in enemies)
        {
            // 시야를 복구하고, 플레이어를 놓친 상태로 설정
            enemy.findDis = enemy.originFindDis;
            enemy.atkDis = enemy.originAtkDis;
            enemy.agent.isStopped = false;
            enemy.enemyState = enemy.missingState;
        }
    }

    // 캐릭터가 섬광탄을 보고있는지 판단하는 메서드
    bool IsLookingAtFlash(Transform _flash, Transform _character)
    {
        // 플레이어 위치에서 섬광탄 위치로의 방향 벡터를 계산
        Vector3 _dirToFlash = _flash.position - _character.position;

        // 카메라가 바라보는 방향과, 플레이어에서 섬광탄으로의 방향 사이의 각도를 계산
        float angle = Vector3.Angle(_character.forward, _dirToFlash);
        // 시야각 확인 // 60 = 좌우로 60
        if (angle < 60f)
        {
            // 레이캐스트로 장애물 체크
            RaycastHit hit;
            if (Physics.Raycast(_character.position, _dirToFlash, out hit))
            {
                // 레이캐스트가 섬광탄에 먼저 닿았는지 확인
                if (hit.collider.gameObject == _flash.gameObject)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 수류탄 효과 ( 데미지 )
    public void FlagGrenadeExplode(Transform _explode, float _radius, float _damage)
    {
        // 플레이어와 폭발한 곳의 거리 계산
        float _distanceToPlayer = Vector3.Distance(_explode.position, PlayerController.Instance.transform.position);
        if (_distanceToPlayer < _radius)
        {
            // 거리별 값 판별 ( 멀어질수록 작은 값 )
            float _damagePersentToPlayer = 1 - (_distanceToPlayer / _radius);
            int _calDamage = Mathf.RoundToInt(_damage * _damagePersentToPlayer);
            PlayerController.Instance.Damaged(_calDamage);
        }

        // 에너미
        foreach (Enemy enemy in enemies)
        {
            RaycastHit hit;
            // 에너미와 폭발물의 방향 계산
            Vector3 _hitDir = (enemy.transform.position - _explode.position).normalized;
            // 에너미와 폭발한 곳의 거리 계산
            float _distance = Vector3.Distance(_explode.position, enemy.transform.position);

            // 폭발물에서 에너미 방향으로 레이 발사
            if (Physics.Raycast(_explode.position, _hitDir, out hit, _distance))
            {
                // 맞은 콜라이더가 에너미가 맞다면 데미지
                if (hit.collider.CompareTag("Enemy"))
                {
                    // 거리별 값 판별 ( 멀어질수록 작은 값 )
                    float _damagePersent = 1 - (_distance / _radius);
                    int _calDamage = Mathf.RoundToInt(_damage * _damagePersent);
                    enemy.Damaged(_calDamage);
                }
            }
        }
    }
}
