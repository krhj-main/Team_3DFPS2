using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#region 무기 설명 텍스트 변경관련 클래스
[System.Serializable]
public class InfomationText
{
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI infoText;

    public void ChangeText(string type, string name, string info)
    {
        typeText.text = type;
        nameText.text = name;
        infoText.text = info;
    }
}
#endregion

public class LoadOut : MonoBehaviour
{
    #region 패널 선택 및 설명 텍스트
    [Header("패널 선택 및 설명 텍스트")]

    [Tooltip("무기 선택 패널")]
    public GameObject[] weaponSelectPanel;

    [Tooltip("주무기 설명 텍스트")]
    public InfomationText mainWeaponInfoText;

    [Tooltip("투척무기 설명 텍스트")]
    public InfomationText throwingWeaponInfoText;

    public InfomationText specialWeaponInfoText;
    #endregion

    #region 주무기 관련 변수
    [Space(5)]
    [Header("주무기 세팅 부분")]

    [Tooltip("장착한 무기")]
    public Image[] equipWeaponImage;  

    [Tooltip("무기 이미지")]
    public Sprite[] mainWeaponImage; 

    [Tooltip("주무기 선택 버튼")]
    public Button[] selectMainWeaponBtn;   

    [Tooltip("주무기 장착칸")]
    public int equipMainWeaponIndex; 
    public List <int> equipMainWeaponList = new List<int>();

    public GameObject[] mainWeaponObject;
    public int mainWeaponObjectIndex;

    #endregion

    #region 투척무기 관련 변수
    [Space(5)]
    [Header("투척무기 세팅 부분")]

    [Tooltip("투척무기 갯수 텍스트")]
    public TextMeshProUGUI[] throwingPcs;

    [Tooltip("플러스 버튼")]
    public Button[] throwingPlusBtn;

    [Tooltip("마이너스 버튼")]
    public Button[] throwingMiusBtn;

    [Tooltip("투척무기 갯수")]
    public int[] countThrwing;

    [Tooltip("로드아웃 패널 투척무기 텍스트")]
    public TextMeshProUGUI[] loadoutPanelThrowCount;

    int getThrowing;
    int maxGetThrowing = 5;

    public static int[] originThrowingCount = new int[3];    // 저장되는 투척무기 갯수
    #endregion

    #region 특수무기 관련 변수
    [Space(5)]
    [Header("특수무기 세팅 부분")]
    public GameObject drone;
    public GameObject specialWeaponPanel;
    #endregion


    #region Awake
    SelectEquip selectEquip;
    public GameObject playerCharacter;
    AnimIKPlayer animIkPlayer;

    private void Awake()
    {
        selectEquip = GetComponent<SelectEquip>();
        animIkPlayer = playerCharacter.GetComponent<AnimIKPlayer>();
        ApplyThrowingCount();
    }
    #endregion

    #region 무기 선택 패널 오픈
    public void OpenSelectPanel(int _index)
    {
        selectEquip.ClosePanel();                           // 켜져 있는 패널 있으면 스택에서 빼고 꺼주기
        selectEquip.PushPanel(weaponSelectPanel[_index]);   // 패널 스택에 넣고 켜주기
        equipMainWeaponIndex = _index;                      // 주무기 패널 열면 슬롯 확인
        UpdateSelectWeaponButtonColor();                    // 주무기 패널 열면 색깔 업데이트
    }
    #endregion

    #region 주무기 세팅
    public void GetMainWeapon(int _index)
    {
        // 이미 장착된 무기를 선택하면 무시
        if (equipMainWeaponList.Contains(_index))
        {
            return;
        }

        equipMainWeaponList[equipMainWeaponIndex] = _index;                             // 선택한 무기를 해당 슬롯에 장착
        equipWeaponImage[equipMainWeaponIndex].sprite = mainWeaponImage[_index];        // 무기 이미지 업데이트
        selectEquip.ClosePanel();                                                       // 패널 꺼주기
    }

    // 색깔 업데이트
    void UpdateSelectWeaponButtonColor()
    {
        for (int i = 0; i < selectMainWeaponBtn.Length; i++)
        {
            ColorBlock _colorBlock = selectMainWeaponBtn[i].colors;

            _colorBlock.highlightedColor = equipMainWeaponList.Contains(i) ? Color.red : Color.green;

            selectMainWeaponBtn[i].colors = _colorBlock;
        }
    }

    // ==== 이벤트 트리거 ====

    public void MainWeaponSelectChangeText1()
    {
        mainWeaponInfoText.ChangeText("돌격소총", "M4A1", "카빈형 돌격소총\n모든 개선점이 총합된\n완전자동발사형");
        mainWeaponObjectIndex = 0;
        SetAcitiveWeapon();
    }

    public void MainWeaponSelectChangeText2()
    {
        mainWeaponInfoText.ChangeText("산탄총", "M870", "펌프액션 산탄총\n산탄총 앞에서는\n모두가 평등하다");
        mainWeaponObjectIndex = 1;
        SetAcitiveWeapon();
    }

    public void MainWeaponSelectChangeText3()
    {
        mainWeaponInfoText.ChangeText("저격소총", "M24", "볼트액션 저격소총\n강력한 파괴력을 자랑하지만\n조준을 하지 않으면 명중률이 낮다");
        mainWeaponObjectIndex = 2;
        SetAcitiveWeapon();
    }

    public void MainWeaponSelectChangeText4()
    {
        mainWeaponInfoText.ChangeText("권총", "HK45", "자동권총\n살상력은 낮지만 은밀하고\n라이트를 사용할 수 있다");
        mainWeaponObjectIndex = 3;
        SetAcitiveWeapon();
    }

    public void mainWeaponButtonChangeIK1()
    {
        mainWeaponObjectIndex = equipMainWeaponList[0];
        SetAcitiveWeapon();
    }

    public void mainWeaponButtonChangeIK2()
    {
        mainWeaponObjectIndex = equipMainWeaponList[1];
        SetAcitiveWeapon();
    }

    void SetAcitiveWeapon()
    {
        for (int i = 0; i < mainWeaponObject.Length; i++)
        {
            if(mainWeaponObjectIndex == i)
            {
                mainWeaponObject[i].SetActive(true);
                animIkPlayer.currentIkIndex = i;
            }
            else
            {
                mainWeaponObject[i].SetActive(false);
            }
        }
    }
    #endregion

    #region 투척무기 세팅

    // 투척무기 업데이트
    void UpdateThrowingWeapon(int _index, int _change)
    {
        countThrwing[_index] += _change;
        throwingPcs[_index].text = countThrwing[_index].ToString();
        loadoutPanelThrowCount[_index].text = throwingPcs[_index].text;
        CalculateGetThrowing();
        UpdateButtonActive();
        ApplyThrowingCount();
    }

    // 투척무기 최대갯수 계산
    void CalculateGetThrowing()
    {
        getThrowing = 0;
        foreach(int _count in countThrwing)
        {
            getThrowing += _count;
        }
    }

    // 투척무기 최대 갯수 버튼 제한
    void UpdateButtonActive()
    {
        bool _canPlus = getThrowing < maxGetThrowing;

        foreach (Button btn in throwingPlusBtn)
        {
            btn.interactable = _canPlus;
        }
    }

    // 투척무기 저장
    void ApplyThrowingCount()
    {
        originThrowingCount[0] = countThrwing[0];
        originThrowingCount[1] = countThrwing[1];
        originThrowingCount[2] = countThrwing[2];
        Debug.Log(originThrowingCount[0]);
        Debug.Log(originThrowingCount[1]);
        Debug.Log(originThrowingCount[2]);
    }

    // 투척무기 갯수 플러스 ( 버튼 연결 )
    public void PlusThrowingWeapon(int _index)
    {
        UpdateThrowingWeapon(_index, 1);
        throwingMiusBtn[_index].interactable = true;
    }

    // 투척무기 갯수 마이너스 ( 버튼 연결 )
    public void MiusThrowingWeapon(int _index)
    {
        UpdateThrowingWeapon(_index, -1);
        if(countThrwing[_index] <= 0)
        {
            throwingMiusBtn[_index].interactable = false;
        }
    }

    // ==== 이벤트 트리거 ====

    public void ThrowChangeText1()
    {
        throwingWeaponInfoText.ChangeText("수류탄", "MK.2", "폭발 시 외피가 잘게 쪼개져\n더욱 큰 살상력을 기대할 수 있다");
    }

    public void ThrowChangeText2()
    {
        throwingWeaponInfoText.ChangeText("연막탄", "M18", "적의 시야를 차단하여\n아군의 회피율을 증가시킨다");
    }

    public void ThrowChangeText3()
    {
        throwingWeaponInfoText.ChangeText("섬광탄", "M84", "직접적인 살상력 없이\n적을 무력화할 수 있다");
    }
    #endregion

    #region 특수무기 세팅
    public void SpecialChangeText1()
    {
        if(selectEquip.selectPanelStack.Count <= 0)
        {
            specialWeaponInfoText.ChangeText("드론", "Dororone", "적진에 침투하기 전\n거미 드론으로 적을 은밀하게\n탐지할 수 있다");
            drone.SetActive(true);
            specialWeaponPanel.SetActive(true);
        }
    }

    public void FalseDrone()
    {
        specialWeaponInfoText.ChangeText("","","");
        drone.SetActive(false);
        specialWeaponPanel.SetActive(false);
    }
    #endregion

}