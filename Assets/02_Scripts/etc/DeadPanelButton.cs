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
    EquipmentsInit equipmentsInit;
    MainWeapon[] mainWeapon;
    EquipmentsSwap swap;
    private void Awake()
    {
        character = PlayerController.Instance.gameObject;
        pState = character.GetComponent<PlayerStateList>();
        deathCam = cam.GetComponent<Animator>();
        equipmentsInit = GameManager.Instance.inventory.GetComponent<EquipmentsInit>();
        
        swap = character.GetComponent<EquipmentsSwap>();
        mainWeapon = new MainWeapon[2];
        mainWeapon[0] = (MainWeapon)swap.Inventory.Get(0);
        mainWeapon[1] = (MainWeapon)swap.Inventory.Get(1);
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
        deathCam.enabled = false;
        WeaponAmmuReset();

    }

    void WeaponAmmuReset()
    {
        // 투척무기 초기화
        if(swap.GrenadeFactory != null)
        {
            swap.GrenadeFactory.SetGrenadeCount(LoadOut.originThrowingCount[0], LoadOut.originThrowingCount[2], LoadOut.originThrowingCount[1]);
            equipmentsInit.frag = LoadOut.originThrowingCount[0];
            equipmentsInit.smoke = LoadOut.originThrowingCount[1];
            equipmentsInit.flash = LoadOut.originThrowingCount[2];
        }
        for (int i = 0; i < mainWeapon.Length; i++) {
            if (mainWeapon[i] != null)
            {
                mainWeapon[i].loadedAmmo = mainWeapon[i].maxLoadedAmmo;
                mainWeapon[i].remainAmmo = mainWeapon[i].initializeAmmo - mainWeapon[i].maxLoadedAmmo;
            }
        }
        // 메인웨폰 초기화
        
    }
}
