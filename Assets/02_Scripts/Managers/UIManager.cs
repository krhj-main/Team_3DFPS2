using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{

    [SerializeField] Slider playerHPBar;
    [SerializeField] TextMeshProUGUI playerHP_TXT;
    [SerializeField] Image playerPortrait;

    [SerializeField] TextMeshProUGUI playerAmmo_TXT;
    [SerializeField] TextMeshProUGUI playerMaxAmmo_TXT;
    int playerAmmo;
    int playerMaxAmmo;

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

    [SerializeField] TextMeshProUGUI missionTime;
    [SerializeField] TextMeshProUGUI missionEnemy;
    int missionTimeLimit;
    int missionTimeCurrent;
    int missionEnemyCount;

    public Image FlashImage;

    [SerializeField] public GameObject missionViewer;
    public Image snimperZoomUI;

    public GameObject deadPanel;
    float deadPanelAlpha;
    public GameObject[] deadPanelUI;

    private void Start()
    {

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

    // 미션 정보에서의 적 수, 시간 등에 관한 UI업데이트 임시
    public void MissionInfoUIUpdate()
    {
        missionTime.text = string.Format("{0}", missionTimeCurrent);
        missionEnemy.text = string.Format("{0}", missionEnemyCount);
    }

    public void OnDeadPanel()
    {
        deadPanel.SetActive(true);
        StartCoroutine(DeadPanelFadeOut());
    }

    IEnumerator DeadPanelFadeOut()
    {
        // 화면알파값 올려서 검정 화면 만들어줌
        deadPanelAlpha = 0;
        while (deadPanelAlpha <= 1f)
        {
            deadPanelAlpha += Time.deltaTime;
            deadPanel.GetComponent<Image>().color = new Color(255, 255, 255, deadPanelAlpha);
            yield return null;
        }

        // 검정화면 끝나면 데드패널에 있는 모든 ui 켜줌
        foreach(GameObject ui in deadPanelUI)
        {
            ui.SetActive(true);
        }
    }
}