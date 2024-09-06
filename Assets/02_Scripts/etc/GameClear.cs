using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameClear : MonoBehaviour
{
    [SerializeField] TMP_Text clearGuideText;
    public GameObject missionManager;
    TimeManager timeManager;


    private void Awake()
    {
        timeManager = missionManager.GetComponent<TimeManager>();
    }

    void Start()
    {
        
        PlayerController.Instance.pState.gameClear = false;
        if (clearGuideText != null)
        {
            clearGuideText.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.enemies.Count < 32)
        {
            PlayerController.Instance.pState.gameClear = true;
            GameManager.Instance.clearGoals[0][0] = true;               //  퀘스트 모든 적 섬멸 클리어
        }

        if (PlayerController.Instance.pState.gameClear)
        {
            if (clearGuideText != null)
                clearGuideText.gameObject.SetActive(true);
        }
    }


    void ClearCondition()
    {
        // 메서드 실행 조건 GameManager.Instance.enemies.Count <= 0
        // 문 트리거에 도착했을 때
        timeManager.stopTime = true;                   // 게임 클리어 되면 시간 멈춤
        GameManager.Instance.clearpanel.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider _col)
    {
        if (_col.CompareTag("Player"))
        {
            /* 원본
            if (GameManager.Instance.enemies.Count <= 0)
            {
                ClearCondition();
            }
            */

            // 테스트용
            if (GameManager.Instance.enemies.Count < 32)
            {
                ClearCondition();
            }
        }
    }
}
