using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;  // 씬 매니저

public class GameManager : Singleton<GameManager>
{
    ItemManager itemManager = new ItemManager();
    public static ItemManager ItemManager
    {
        get { return Instance.itemManager; }
    }

    /*
    FragGrenade fragGrenade = new FragGrenade();
    public static FragGrenade FragGrenade
    {
        get { return Instance.fragGrenade; }
    }

    FlashGrenade flashGrenade = new FlashGrenade();
    public static FlashGrenade FlashGrenade
    {
        get { return Instance.flashGrenade; }
    }

    SmokeGrenade smokeGrenade = new SmokeGrenade();
    public static SmokeGrenade SmokeGrenade
    {
        get { return Instance.smokeGrenade; }
    }
    */
    [HideInInspector]
    public List<Enemy> enemies = new List<Enemy>();
    [HideInInspector]
    public List<IDamageAble> attackables = new List<IDamageAble>();
    [HideInInspector]
    public float maxEnemy;
    [HideInInspector]
    public float remainEnemy;

    float defaultGravity;

    public Canvas playerUI;

    public GameObject inventory;

    public GameObject[] applyCustomCharacter;
     
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 씬이 로드될 때마다 실행되는 이벤트에 넣을 함수들이나 기능 구현
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AddEnemyOnNowScene();
        AddAttackableOnNowScene();

        SceneToInit();

        PlayerInit();

        playerUI.gameObject.SetActive(scene.buildIndex >=2);
    }

    // 씬이 로드될 때 존재하는 모든 Enemy를 List에 담는 함수 ( ScnenManager 등에서 호출 )
    public void AddEnemyOnNowScene()
    {
        // 에너미 리스트 초기화
        enemies.Clear();
        // 씬에 존재하는 모든 enemy를 담는다
        enemies.AddRange(FindObjectsOfType<Enemy>());
        // 최대 에너미 수를 현재 씬에 있는 에너미 수만큼 저장
        maxEnemy = enemies.Count;

        UIManager.Instance.RemainEnemy();
    }

    public void AddAttackableOnNowScene()
    {
        attackables.Clear();

        attackables.AddRange(FindObjectsOfType<MonoBehaviour>().OfType<IDamageAble>());
    }

    // 씬 로드시 플레이어 관련 설정 초기화
    public void PlayerInit()
    {
        PlayerController _pc = PlayerController.Instance;

        _pc.cc.enabled = false;

        if (FindObjectOfType<SetPlayerPosition>() != null)
        {
            Vector3 _newPosition = FindObjectOfType<SetPlayerPosition>().transform.position;
            PlayerController.Instance.transform.position = _newPosition;
            PlayerController.Instance.gravityAcc = defaultGravity;
        }
        else
        {
            PlayerController.Instance.gravityAcc = 0;
        }

        _pc.enabled = true;
        _pc.cc.enabled = true;
        _pc.pHP = _pc.maxHP;
    }

    public void SceneToInit()
    {
        string _nowScene = SceneManager.GetActiveScene().name;

        if (_nowScene.Contains("Title"))
        {
            UIManager.Instance.CrossHair(false);
        }
        else if (_nowScene.Contains("Lodding"))
        {
            UIManager.Instance.CrossHair(false);
        }
        else
        {
            UIManager.Instance.CrossHair(true);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        defaultGravity = PlayerController.Instance.gravityAcc;
    }

    void Start()
    {
        
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    #region "AggroEnemy"
    // Enemy의 어그로를 끄는 메서드
    public void AggroEnemy(Vector3 _soundPos, float _radius)
    {
        foreach (Enemy enemy in enemies)
        {
            /*
            if (Mathf.Abs( _soundPos.y - enemy.gameObject.transform.position.y) >= 3)
            {
                return;
            }
            */

            // 에너미와 소리난 곳의 거리 계산
            float _distance = Vector3.Distance(_soundPos, enemy.gameObject.transform.position);
            // 거리가 범위 이내라면
            if (_distance <= _radius)
            {
                // 존버 상태가 아닐때만
                if (enemy.enemyState != EnemyState.Hide && enemy.enemyState != EnemyState.Blind && enemy.enemyState != EnemyState.Attack)
                {
                    enemy.curTrackTime = 0;
                    enemy.curTrackingTime = 0;
                    // 소리의 위치를 chasePos 변수에 담고
                    enemy.chasePos = _soundPos;
                    enemy.agent.stoppingDistance = 0;
                    enemy.agent.speed = enemy.trackingSpd;
                    enemy.agent.SetDestination(enemy.chasePos);

                    if (enemy.agent.remainingDistance >= enemy.remainDis)
                    {
                        return;
                    }
                    // enemy의 상태를 Move로 변경해 소리가 난 곳으로 이동
                    enemy.enemyState = EnemyState.Move;
                }
            }
        }
    }
    #endregion

    #region "AggroEnemy 원본"

    public void AggroEnemy2(Vector3 _soundPos, float _radius)
    {
        foreach (Enemy enemy in enemies)
        {
            // 에너미와 소리난 곳의 거리 계산
            float _distance = Vector3.Distance(_soundPos, enemy.gameObject.transform.position);

            // 거리가 범위 이내라면
            if (_distance <= _radius)
            {
                // 소리의 위치를 chasePos 변수에 담고
                enemy.chasePos = _soundPos;

                // 존버 상태가 아닐때만
                if (enemy.enemyState != EnemyState.Hide && enemy.enemyState != EnemyState.Blind)
                {
                    // enemy의 상태를 Move로 변경해 소리가 난 곳으로 이동
                    enemy.enemyState = EnemyState.Move;
                }
            }
        }
    }

    #endregion


    #region 로비 미션 UI 관련 ( 미션선택, 움직임 제한 )

    public int canMissionChoice = 0;        // 게임 클리어 시 미션 목록에서 다음 미션 선택 가능하게 해줌
    public bool openUI = false;             // UI 열리면 움직임 제한
    public int selectSceneNum = 0;

    #endregion

    #region "타이머"
    public float curTime;
    public bool Timer(float _targetTime)
    {
        curTime += Time.deltaTime;
        if (curTime > _targetTime)
        {
            curTime = 0;
            return true;
        }

        return false;
    }
    #endregion

    #region 게임 클리어
    public string[] missionNames = new string[] {"테러리스트 소탕", "인질 구출"};
    public string[][] goals = new string[][]
    {
        new string[] {"모든 적 소탕", "5분 내 클리어"},
        new string[] {"모든 적 소탕", "인질 구출","7분 내 클리어"}
    };
    public bool[][] clearGoals = new bool[][]
    {
        new bool[] {false,false},
        new bool[] {false,false,false}
    };

    public float clearTime;
    public float bestClearTime;
    public int bestScore;
    public string bestGrade;
    public string bestColor;

    public int enemyScore;

    // 베스트 스코어 , 타임 텍스트를 생성하는 속성
    public string bestScoreText => $"<color={bestColor}>{bestScore} ({bestGrade})</color>";
    public string bestClearTimeText;

    public string ClearTimeText()
    {
        if(clearTime != 0)
        {
            int _min = (int)(clearTime / 60);   // 분
            int _sec = (int)(clearTime % 60);   // 초
            return string.Format("{0:D2} : {1:D2}", _min, _sec); // 분 : 초 텍스트 만들어 반환
        }
        return "00 : 00";
    }

    #endregion
}
