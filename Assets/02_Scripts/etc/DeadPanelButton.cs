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
        foreach(GameObject go in deadPanelUI)
        {
            go.SetActive(false);
        }
        PlayerController.Instance.pHP = 100;
        pState.isDead = false;
        Time.timeScale = 1;
        deathCam.enabled = false;
    }

}
