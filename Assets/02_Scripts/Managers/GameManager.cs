using System.Collections;
using System.Collections.Generic;
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

    public void AggroEnemy(Vector3 soundPos, float radius)
    {
        foreach(Enemy enemy in enemies)
        {
            // 에너미와 소리난 곳의 거리 계산
            float distance = Vector3.Distance(soundPos, enemy.gameObject.transform.position);

            // 거리가 범위 이내라면
            if (distance <= radius)
            {
                // 소리의 위치를 chasePos 변수에 담고
                enemy.chasePos = soundPos;

                // 존버 상태가 아닐때만
                if (enemy.enemyState != EnemyState.Hide)
                {
                    // enemy의 상태를 Move로 변경해 소리가 난 곳으로 이동
                    enemy.enemyState = EnemyState.Move;
                }
            }
        }
    }

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
}
