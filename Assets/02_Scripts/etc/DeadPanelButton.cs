using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadPanelButton : MonoBehaviour
{
    public GameObject[] deadPanelUI;
    public GameObject character;
    PlayerStateList pState;
    public Camera cam;
    Animator deathCam;
    public GameObject inventory;
    EquipmentsInit equipmentsInit;

    private void Awake()
    {
        character = PlayerController.Instance.gameObject;
        pState = character.GetComponent<PlayerStateList>();
        deathCam = cam.GetComponent<Animator>();
        equipmentsInit = inventory.GetComponent<EquipmentsInit>();
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
        deathCam.enabled = false;
        //equipmentsInit.Init();
    }
}
