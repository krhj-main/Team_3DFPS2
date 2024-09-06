using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ClearPanel : MonoBehaviour
{
    public TextMeshProUGUI missionName;         // 미션 이름

    public GameObject[] goal;                   // 목표 ( 씬의 목표 갯수에 맞게 활성화 )
    public TextMeshProUGUI goalCount;           // 목표 갯수
    public TextMeshProUGUI[] goalName;          // 목표 이름
    public GameObject[] clearGoal;              // 클리어 여부 ( 클리어되면 빨간 밑줄 쳐짐 )

    public Image fillScore;                     // 가운데 점수에 따라 채워질 이미지 
    public TextMeshProUGUI scoreResultText;     // 점수 결과 (문자)
    public TextMeshProUGUI scoreResultNum;      // 점수 결과 (숫자)
    public TextMeshProUGUI bestScoreText;       // 최고 점수 (문자)
    private int score;                          // 점수

    public TextMeshProUGUI timeText;            // 클리어 시간 텍스트
    public TextMeshProUGUI bestClearTimeText;   // 최고 클리어 시간 텍스트

    private string hexaCode;                    // 글자색 헥사코드
    private float emphasizeDuration = 1f;       // 글자 강조 시간
    private float emphasizeScale = 3f;          // 글자 커지는 크기

    int goalScore = 0;                          // 미션 점수

    // 화면이 켜지면 실행
    private void OnEnable()
    {
        GameManager.Instance.stopTime = true;
        SettingClearPanel();
        if(GameManager.Instance.bestClearTimeText != null)
        {
            bestClearTimeText.text = GameManager.Instance.bestClearTimeText;
        }
    }

    void SettingClearPanel()
    {
        int _sceneGoal = GameManager.Instance.sceneGoal;
        int _currentMissionNum = GameManager.Instance.selectSceneNum - 3;              // 진행한 미션 ( 이 숫자에 맞춰 미리 작성해놓은 값들이 변경 됨 )
        int _clearCount = 0;                                                           // 미션 클리어시 카운트 증가 ( 목표갯수 숫자 설정 )
        missionName.text = GameManager.Instance.missionNames[_sceneGoal];              // 미션 이름
        
        for (int i = 0; i < GameManager.Instance.goals[_sceneGoal].Length; i++)
        {
            goal[i].SetActive(true);                                                   // 게임매니저에 적어놓은 만큼 ui 활성화
            goalName[i].text = GameManager.Instance.goals[_sceneGoal][i];              // 게임매니저에 적어놓은 글자 활성화
            clearGoal[i].SetActive(GameManager.Instance.clearGoals[_sceneGoal][i]);    // 미션 클리어시 빨간 밑줄

            if (GameManager.Instance.clearGoals[_sceneGoal][i])                        // 게임 클리어가 되면
            {
                _clearCount++;      // 목표 갯수 증가
                goalScore += 500; // 목표 점수 추가
            }
        }

        goalCount.text = $"{_clearCount} / {GameManager.Instance.goals[_currentMissionNum].Length}";        // 목표 갯수 출력
        Debug.Log(GameManager.Instance.goals[_currentMissionNum].Length);
        scoreResultText.text = "";                                          // 초기화
        scoreResultNum.text = "0";                                          // 초기화
        scoreResultText.transform.localScale = new Vector3(1,1,1);          // 글자 크기 초기화

        StartCoroutine(CountScore(CalculaterScore(), 0));
    }

    #region 점수 올라가는 코루틴
    IEnumerator CountScore(float _target, float _current)       // _target : 최종점수 _current : 현재 점수
    {
        Debug.Log("퀘스트 점수"+ goalScore);
        Debug.Log("킬 점수" + GameManager.Instance.enemyScore);
        Debug.Log("시간 점수" + TimeManager.timeScore);
 
        yield return new WaitForSeconds(1f);                    // 1초 후에 시작

        timeText.text = GameManager.Instance.ClearTimeText();   // 게임매니저에서 시간 텍스트 가져옴

        // 카운팅에 걸리는 시간
        float _duration = 2f;
        float _offset = (_target - _current) / _duration;

        while (_current < _target)
        {
            _current += _offset * Time.deltaTime;

            scoreResultNum.text = string.Format("{0:n0}", (int)_current);
            fillScore.fillAmount = _current * 0.0005f;
            UpdateScoreResult();
            yield return null;
        }
        _current = _target;
        scoreResultNum.text = string.Format("{0:n0}", (int)_current);

        if(_current == _target)
        {
            score = CalculaterScore();
            UpdateBestScore();
            UpdateBestClearTime();
        }

        // 다음 스테이지 선택 가능
        int _canMission = GameManager.Instance.canMissionChoice;
        if (GameManager.Instance.selectSceneNum - 3 >= _canMission)
        {
            GameManager.Instance.canMissionChoice = GameManager.Instance.selectSceneNum - 3;
        }

        yield return new WaitForSeconds(5f);
        UIManager.Instance.SceneTransition(1);
        GameManager.Instance.missionTime = 0;
        ResetScore();
        this.gameObject.SetActive(false);
    }
    #endregion

    #region 점수 계산
    int CalculaterScore()
    {
        int _finalScore = goalScore + GameManager.Instance.enemyScore + TimeManager.timeScore;

        return _finalScore;
    }
    #endregion

    #region 점수 초기화
    void ResetScore()
    {
        goalScore = 0;
        GameManager.Instance.enemyScore = 0;
        TimeManager.timeScore = 500;
    }

    #endregion

    #region 점수 글자 변경 및 효과
    void UpdateScoreResult()
    {
        // 등급과 색상을 정의한 배열
        (float _threshold, string _grade, string _color)[] gradeInfo =
        {
            (1f, "S", "#A19228"),
            (0.8f, "A", "#BCC071"),
            (0.6f, "B", "#9F9D97"),
            (0.4f, "C", "#666666"),
            (0.25f, "D", "#505050"),
            (0.1f, "E", "#383838"),
        };

        // 적절한 등급 찾기
        string _grade = "F";
        string _color = "#272727";

        for (int i = 0; i < gradeInfo.Length; i++)
        {
            if (fillScore.fillAmount >= gradeInfo[i]._threshold)
            {
                _grade = gradeInfo[i]._grade;
                _color = gradeInfo[i]._color;
                break;
            }
        }

        // 결과 적용
        scoreResultText.text = _grade;
        hexaCode = _color;
        EmphasizeString();
        UpdateColor(hexaCode);
    }

    // 색변경
    void UpdateColor(string _hexa)
    {
        Color _color;
        ColorUtility.TryParseHtmlString(_hexa, out _color);
        scoreResultText.color = _color;
    }

    // 텍스트 작아지는 효과
    void EmphasizeString()
    {
        // 원래 값 저장
        Vector3 _originalScale = scoreResultText.transform.localScale;

        // 처음에 텍스트를 큰 상태로 설정
        scoreResultText.transform.localScale = _originalScale * emphasizeScale;

        // 작아지는 효과 실행
        scoreResultText.transform.DOScale(_originalScale, emphasizeDuration);
    }
    #endregion

    #region 최고 점수 UI 변경
    void UpdateBestScore()
    {

        if (score <= GameManager.Instance.bestScore)
        {
            return;
        }
        else
        {
            GameManager.Instance.bestScore = score;

            (int _threshold, string _grade, string _color)[] _gradeInfo =
            {
            (2000, "S", "#A19228"),
            (1600, "A", "#BCC071"),
            (1200, "B", "#9F9D97"),
            (800, "C", "#666666"),
            (500, "D", "#505050"),
            (200, "E", "#383838"),
            };

            int _bs = GameManager.Instance.bestScore;
            Debug.Log(_bs);
            for (int i = 0; i < _gradeInfo.Length; i++)
            {
                if (_bs >= _gradeInfo[i]._threshold)
                {
                    GameManager.Instance.bestGrade = _gradeInfo[i]._grade;
                    GameManager.Instance.bestColor = _gradeInfo[i]._color;
                    break;
                }
            }
            bestScoreText.text = GameManager.Instance.bestScoreText;
        }
    }

    void UpdateBestClearTime()
    {
        if(GameManager.Instance.clearTime >= GameManager.Instance.bestClearTime)
        {
            return;
        }
        else
        {
            GameManager.Instance.bestClearTime = GameManager.Instance.clearTime;
            float _bsT = GameManager.Instance.bestClearTime;
            int _min = Mathf.FloorToInt(_bsT / 60);                 
            int _sec = Mathf.FloorToInt(_bsT % 60);                  
            bestClearTimeText.text = string.Format("{0:D2} : {1:D2}", _min, _sec);
            GameManager.Instance.bestClearTimeText = bestClearTimeText.text;
        }
    }
    #endregion
}
