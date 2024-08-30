using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class Lodding : MonoBehaviour
{
    public Slider loadingBar;
    public TextMeshProUGUI loadingTxt;
    public TextMeshProUGUI skipInfoTxt;

    float fakeVal = 0;
    float randTime;

    public VideoPlayer[] videoPlayers;
    bool videoFinished = false;


    /// <summary>
    /// 로딩스크립트 위한 구현부분
    /// </summary>
    /// 

    LoadingImage lo;

    void Start()
    {
        lo = GameObject.Find("@LoadingImage").GetComponent<LoadingImage>();
        /*if (SceneManager.GetActiveScene().buildIndex < 2)
        {
            GameManager.Instance.selectSceneNum = 2;
        }*/
        if (GameManager.Instance.selectSceneNum < 2) {
            GameManager.Instance.selectSceneNum = 2;
        }

        loadingTxt.text = fakeVal + "%";

        StartCoroutine(TransitionNextScene(GameManager.Instance.selectSceneNum));
        //ActiveVideo();
    }

    // 비동기 신
    IEnumerator TransitionNextScene(int num)
    {

        yield return null;

        // 지정된 씬을 비동기 형식으로 로드한다
        AsyncOperation _ao = SceneManager.LoadSceneAsync(num);

        // 준비가 완료되어도 다음 씬으로 넘어가지 않게
        // 단, 이걸 사용하면 progree는 0.9까지밖에 안됨 -> 유니티 내부 구조의 문제

        _ao.allowSceneActivation = false;
        lo.LoadingInit();
        // 로딩이 완료될 때까지 반복해서 요소들을 로드하고 진행 과정을 하면에 표시한다
        while (!_ao.isDone)
        {
            
            // 로딩 진행률을 슬라이더 바와 텍스트로 표시한다
            float _progress = Mathf.Clamp01(_ao.progress / 0.9f);
            loadingBar.value = _progress;
            loadingTxt.text = (_progress * 100f) + "%";

            // 만일 씬 로드 진행률이 90%를 넘어가면
            if (_ao.progress >= 0.9f)
            {
                skipInfoTxt.enabled = true;
                // 비디오가 종료되거나 엔터키를 누르면 다음씬 활성화
                if (videoFinished || Input.GetKeyDown(KeyCode.Escape))
                {
                    _ao.allowSceneActivation = true;
                    MouseCursorMove.HideCursor();
                }
            }

            yield return null;
        }
    }


    // 동영상 재생
    void ActiveVideo()
    {
        VideoPlayer _selectedVideo = videoPlayers[GameManager.Instance.selectSceneNum];
        _selectedVideo.gameObject.SetActive(true);
        _selectedVideo.Play();
        _selectedVideo.loopPointReached += OnVideoFinished;
    }

    // 동영상 재생이 끝나면 실행할 함수
    void OnVideoFinished(VideoPlayer vp)
    {
        videoFinished = true;
    }
}
