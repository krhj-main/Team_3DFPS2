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

    public VideoPlayer[] videoPlayers;
    bool videoFinished = false;

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex < 2)
        {
            GameManager.Instance.selectSceneNum = 2;
        }
        StartCoroutine(TransitionNextScene(GameManager.Instance.selectSceneNum));
        //ActiveVideo();
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
            float _progress = Mathf.Clamp01(_ao.progress / 0.9f);
            loadingBar.value = _progress;
            loadingTxt.text = (_progress * 100f).ToString("F0") + "%";

            // 만일 씬 로드 진행률이 90%를 넘어가면
            if (_ao.progress >= 0.9f)
            {
                skipInfoTxt.enabled = true;
                // 비디오가 종료되거나 엔터키를 누르면 다음씬 활성화
                if (videoFinished || Input.GetKeyDown(KeyCode.Escape))
                {
                    _ao.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
