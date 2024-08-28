using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class Mission : MonoBehaviour
{
    // 미션 목록
    public Button[] missionBtn;
    public TextMeshProUGUI[] missionTxt;
    public GameObject[] missionImage;

    // 미션UI 캔버스
    public GameObject missionCanvas;

    // 마우스 커서 조작
    public MouseCursorMove mouseCursor;

    // UI 화면 켜지면 클리어 조건에 따라 버튼 클릭 가능
    private void OnEnable()
    {
        mouseCursor.ShowCursor();
        for (int i = 0; i < missionBtn.Length; i++)
        {
            if (i <= GameManager.Instance.canMissionChoice)
            {
                missionBtn[i].interactable = true;
                missionTxt[i].color = Color.white;
            }
        }
        GameManager.Instance.openUI = true;
    }

    // UI 화면 꺼지면 이미지 다 끄기
    private void OnDisable()
    {
        mouseCursor.HideCursor();
        for (int i = 0; i < missionImage.Length; i++)
        {
            missionImage[i].SetActive(false);
        }
        GameManager.Instance.openUI = false;
    }

    // ESC 클릭시 UI 창 나가기
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            missionCanvas.SetActive(false);
        }
    }


    // 미션 버튼 클릭 시 할 행동 ( 직접 연결 해줌 )
    public void MissionBtn(int num)
    {
        GameManager.Instance.selectSceneNum = num+2;
        MissionImage(num);
    }

    // 게임 시작 버튼 ( 직접 연결 해줌 )
    public void StartBtn()
    {
        SceneManager.LoadScene(1);
    }

    // 미션 이미지 켜주기
    void MissionImage(int num)
    {
        for(int i = 0; i < missionImage.Length; i++)
        {
            if(i == num)
            {
                missionImage[i].SetActive(true);
            }
            else
            {
                missionImage[i].SetActive(false);
            }
        }
    }

}
