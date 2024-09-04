using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Header("플레이어 HUD")]
    public GameObject playerUI;
    public GameObject UI_HPPanel;
    public Image playerHUD;
    public Sprite playerStand;
    public Sprite playerCrouch;

    [Header("플레이어 HP")]
    [SerializeField] Slider playerHPBar;
    [SerializeField] TextMeshProUGUI playerHP_TXT;
    [SerializeField] Image playerPortrait;

    [Header("장착중인 총의 탄")]
    [SerializeField] TextMeshProUGUI playerAmmo_TXT;
    [SerializeField] TextMeshProUGUI playerMaxAmmo_TXT;
    int playerAmmo;
    int playerMaxAmmo;


    [Header("무기 이미지 / 이름")]
    public GameObject UI_WeaponPanel;
    [SerializeField] Image weaponMain1;
    [SerializeField] TextMeshProUGUI weaponMain1_TXT;
    string weaponMain1Name;
    [SerializeField] Image weaponMain2;
    [SerializeField] TextMeshProUGUI weaponMain2_TXT;
    string weaponMain2Name;
    [SerializeField] Image weaponThrow;
    [SerializeField] TextMeshProUGUI weaponThrow_TXT;
    int weaponThrowCount;
    [SerializeField] Image weaponTactical;
    [SerializeField] TextMeshProUGUI weaponTactical_TXT;
    int weaponTacticalCount;

    [Header("미션 정보")]
    [SerializeField] public GameObject missionViewer;
    [SerializeField] TextMeshProUGUI missionTime;
    [SerializeField] TextMeshProUGUI missionEnemy;
    int missionTimeLimit;
    int missionTimeCurrent;
    int missionEnemyCount;

    [Header("ESC 메뉴")]
    public GameObject escMenu;

    [Header("섬광탄 효과 (임시)")]
    public FlashEffectEnd FlashImage;

    [Header("줌 UI")]
    public Image snimperZoomUI;
    public Image crosshair;

    [Header("사망 관련")]
    public GameObject deadPanelObj;


    public enum SceneName
    {
        MainTitle,
        LoddingScene,
        Lobby,
        Mission1,
    }

    public SceneName sName;
    private void Start()
    {
        PlayerController.Instance.deadAction += playerDead;
    }

    private void Update()
    {
        StatUIUpdate();
    }


    // 플레이어 체력관련 UI 업데이트
    public void StatUIUpdate()
    {
        playerHPBar.value = (float)PlayerController.Instance.pHP / (float)PlayerController.Instance.maxHP;
        playerHP_TXT.text = string.Format("{0}", PlayerController.Instance.pHP);
    }

    // 총알 사용시 UI업데이트 임시변수
    public void AmmoUIUpdate(int _ammo)
    {
        playerAmmo_TXT.text = _ammo.ToString();
    }
    public void ReloadAmmoUIUpdate(int _ammo,int _maxAmmo)
    {
        playerAmmo_TXT.text = _ammo.ToString();
        playerMaxAmmo_TXT.text = _maxAmmo.ToString();
    }

    public void ThrowUIUpdate(Sprite _throw,int _count) {
        weaponThrow_TXT.text = _count.ToString();
        weaponThrow.sprite = _throw;
    }

    // 무기교체시 UI 업데이트 임시 변수
    public void ChangeWeaponUIUpdate(Sprite _weapon1, int _throwCount, int _tacticalCount)
    {
        weaponMain1.sprite = _weapon1;
        //weaponMain1 = ;
        //weaponMain1_TXT.text = weaponMain1Name;
        //weaponMain2.sprite = ;
        //weaponMain2 = ;
        //weaponMain2_TXT.text = weaponMain2Name;
        // playerMaxAmmo = ;

        //weaponSub.sprite = ;
        //weaponSub = ;
        //weaponThrow_TXT.text = string.Format("{0}", weaponThrow);

        //weaponTactical.sprite = ;
        //weaponTactical = ;
        //weaponTactical_TXT.text = string.Format("{0}", weaponTactical);
    }

    public void ChangeSpecialWeaponUIUpdate(Sprite _specialWeaponImage)
    {
        weaponTactical.sprite = _specialWeaponImage;
    }


    // 미션 정보에서의 적 수, 시간 등에 관한 UI업데이트 임시
    public void MissionInfoUIUpdate()
    {
        missionTime.text = string.Format("{0}", missionTimeCurrent);
        missionEnemy.text = string.Format("{0}", missionEnemyCount);
    }

    // "남은 적 수 / 최대 적 수" 를 표시해주는 메서드
    // 해당 메서드는 에너미가 죽었을 때 사용
    public void RemainEnemy()
    {
        // 에너미가 죽을 때 남은 수를 업데이트
        GameManager.Instance.remainEnemy = GameManager.Instance.enemies.Count;

        // 남은 적 : n
        missionEnemy.text = string.Format("남은 테러리스트 : {0} 명", GameManager.Instance.remainEnemy);
        // 남은 적 : n / n
        //missionEnemy.text = $"남은 적 : {GameManager.Instance.remainEnemy} / {GameManager.Instance.maxEnemy}";
    }

    public void SceneTransition(int _sceneName)
    {
        SceneManager.LoadScene($"{(SceneName)_sceneName}");
        GameManager.Instance.openUI = PlayerController.Instance.pState.isOnViewer = PlayerController.Instance.pState.isOnESCMenu = false;
        GameManager.Instance.selectSceneNum = (int)_sceneName;
    }
    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN
            Application.Quit();
#endif
    }
    // 크로스헤어의 OnOff를 담당하는 메서드
    public void CrossHair(bool _onoff)
    {
        crosshair.enabled = _onoff;
    }


    public void OpenUIMenu()
    {
        CrossHair(false);
        
    }
    public void CloseUIMenu()
    {
        CrossHair(true);
        
    }

    public void playerDead()
    {
        snimperZoomUI.enabled = false;
        PlayerController.Instance.pState.isOnESCMenu = false;
        escMenu.SetActive(false);
        Debug.Log(escMenu.activeSelf);
        CrossHair(false);
    }
}