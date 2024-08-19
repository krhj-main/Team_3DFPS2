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
    public Image[] missionImage;

    // 비동기씬
    public Slider loadingBar;               // 로딩 슬라이더 바
    public TextMeshProUGUI loadingTxt;      // 로딩 진행 텍스트


    void Start()
    {
        for(int i = 0; i < missionBtn.Length; i++)
        {
            if(i <= GameManager.Instance.gameClearNum)
            {
                missionBtn[i].interactable = true;
                missionTxt[i].color = Color.white;
            }
        }
    }

    // 각 버튼마다 연결
    public void MissionBtn(int num)
    {
        StartCoroutine(TransitionNextScene(num));
        MissionImage(num);
    }

    // 비동기 신
    IEnumerator TransitionNextScene(int num)
    {
        // 지정된 씬을 비동기 형식으로 로드한다
        AsyncOperation _ao = SceneManager.LoadSceneAsync(num);

        // 로드되는 씬의 모습이 화면에 보이지 않게 한다
        _ao.allowSceneActivation = false;

        // 로딩이 완료될 때까지 반복해서 요소들을 로드하고 진행 과정을 하면에 표시한다
        while (!_ao.isDone)
        {
            // 로딩 진행률을 슬라이더 바와 텍스트로 표시한다
            loadingBar.value = _ao.progress;
            loadingTxt.text = (_ao.progress * 100f).ToString() + "%";

            // 만일 씬 로드 진행률이 90%를 넘어가면
            if (_ao.progress >= 0.9f)
            {
                // 로드된 씬을 화면에 보이게 한다
                _ao.allowSceneActivation = true;
            }
            // 다음 프레임이 될 때까지 기다린다
            yield return null;
        }
    }

    // 미션 이미지 or 동영상 켜주기
    void MissionImage(int num)
    {
        for(int i = 0; i < missionImage.Length; i++)
        {
            if(i == num)
            {
                missionImage[i].enabled = true;
            }
            else
            {
                missionImage[i].enabled = false;
            }
        }
    }

}
