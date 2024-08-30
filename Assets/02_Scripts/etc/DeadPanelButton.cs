using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadPanelButton : MonoBehaviour
{
    public GameObject[] deadPanelUI;
    public GameObject charcter;
    PlayerStateList pState;
    public Camera cam;
    Animator deathCam;

    private void Awake()
    {
        charcter = PlayerController.Instance.gameObject;
        pState = charcter.GetComponent<PlayerStateList>();
        deathCam = cam.GetComponent<Animator>();

    }

    public void RestartMission()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        offUI();
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene("Lobby");
        offUI(); 
    }

    void offUI()
    {
        // 데드 패널 UI 꺼줌
        foreach(GameObject go in deadPanelUI)
        {
            go.SetActive(false);
        }


        PlayerController.Instance.pHP = 100;    // 플레이어 HP 복구
        pState.isDead = false;                  // 플레이어 상태 복구
        Time.timeScale = 1;                     // 시간 복구
        deathCam.Play("Nothing");               // 카메라 복구

    }
}
