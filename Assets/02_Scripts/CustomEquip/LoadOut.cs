using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadOut : MonoBehaviour
{
    public GameObject[] weaponSelectPanel;

    [Space(5)]
    public Image[] equipWeaponImage; // 장착한 무기
    public Sprite[] mainWeaponImage;  // 무기 이미지
    public Button[] selectMainWeaponBtn;
    int openMainWeaponIndex;

    // 주무기 이벤트 트리거
    public TextMeshProUGUI mainType;
    public TextMeshProUGUI mainName;
    public TextMeshProUGUI mainInfo;

    #region 투척무기 변수
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

    // 투척무기 이벤트 트리거
    public TextMeshProUGUI throwType;
    public TextMeshProUGUI throwName;
    public TextMeshProUGUI throwInfo;
    #endregion

    // 무기 선택 패널 오픈 함수 ( 직접 연결 )
    public void OpenSelectPanel(int _index)
    {
        weaponSelectPanel[_index].SetActive(true);
        openMainWeaponIndex = _index;
    }

    #region 주무기 세팅
    public void GetMainWeapon(int _index)
    {
        equipWeaponImage[openMainWeaponIndex].sprite = mainWeaponImage[_index];
        weaponSelectPanel[openMainWeaponIndex].SetActive(false);

        ColorBlock _colorBlock = selectMainWeaponBtn[_index].colors;
        _colorBlock.highlightedColor = Color.red;
        selectMainWeaponBtn[_index].colors = _colorBlock;
    }


    // ==== 이벤트 트리거 ====
    void ChangeMainText(string type, string name, string info)
    {
        mainType.text = type;
        mainName.text = name;
        mainInfo.text = info;
    }

    public void MainChangeText1()
    {
        ChangeMainText("돌격소총", "M4A1", "카빈형 돌격소총\n모든 개선점이 총합된\n완전자동발사형");
    }

    public void MainChangeText2()
    {
        ChangeMainText("산탄총", "M870", "펌프액션 산탄총\n산탄총 앞에서는\n모두가 평등하다");
    }

    public void MainChangeText3()
    {
        ChangeMainText("저격소총", "M24", "볼트액션 저격소총\n강력한 파괴력을 자랑하지만\n조준을 하지 않으면 명중률이 낮다");
    }

    public void MainChangeText4()
    {
        ChangeMainText("권총", "HK45", "자동권총\n살상력은 낮지만 은밀하고\n라이트를 사용할 수 있다");
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

    // 투척무기 갯수 플러스 버튼
    public void PlusThrowingWeapon(int _index)
    {
        UpdateThrowingWeapon(_index, 1);
        throwingMiusBtn[_index].interactable = true;
    }

    // 투척무기 갯수 마이너스 버튼
    public void MiusThrowingWeapon(int _index)
    {
        UpdateThrowingWeapon(_index, -1);
        if(countThrwing[_index] <= 0)
        {
            throwingMiusBtn[_index].interactable = false;
        }
    }

    // ==== 이벤트 트리거 ====

    void ChangeThrowText(string type, string name, string info)
    {
        throwType.text = type;
        throwName.text = name;
        throwInfo.text = info;
    }

    public void ThrowChangeText1()
    {
        ChangeThrowText("수류탄", "MK.2", "나치 놈들과 쪽바리들에게\n강력한 폭발력을 자랑한다");
    }

    public void ThrowChangeText2()
    {
        ChangeThrowText("연막탄", "M18", "적의 시야를 차단하여\n아군의 회피율을 증가시킨다");
    }

    public void ThrowChangeText3()
    {
        ChangeThrowText("섬광탄", "M84", "직접적인 살상력 없이\n적을 무력화할 수 있다");
    }
    #endregion
}
