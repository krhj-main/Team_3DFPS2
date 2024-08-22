using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadOut : MonoBehaviour
{
    public GameObject[] weaponSelectPanel;

    [Space(5)]
    public Image[] mainWeaponImage;



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

    #region 주무기 세팅
    public void GetMainWeapon()
    {

    }


    // ==== 이벤트 트리거 ====
    void ChangeMainText(string type, string name, string info)
    {
        throwType.text = type;
        throwName.text = name;
        throwInfo.text = info;
    }

    public void MainChangeText1()
    {
        ChangeMainText("돌격소총", "M4A1", "나치 놈들과 쪽바리들에게\n강력한 폭발력을 자랑한다");
    }

    public void MainChangeText2()
    {
        ChangeMainText("샷건", "M18", "적의 시야를 차단하여\n아군의 회피율을 증가시킨다");
    }

    public void MainChangeText3()
    {
        ChangeMainText("저격총", "M84", "직접적인 살상력 없이\n적을 무력화할 수 있다");
    }

    public void MainChangeText4()
    {
        ChangeMainText("권총", "M84", "직접적인 살상력 없이\n적을 무력화할 수 있다");
    }
    #endregion

    #region 투척무기 세팅
    void UpdateThrowingWeapon(int _index, int _change)
    {
        countThrwing[_index] += _change;
        throwingPcs[_index].text = countThrwing[_index].ToString();
        loadoutPanelThrowCount[_index].text = throwingPcs[_index].text;
        CalculateGetThrowing();
        UpdateButtonActive();
    }

    void CalculateGetThrowing()
    {
        getThrowing = 0;
        foreach(int _count in countThrwing)
        {
            getThrowing += _count;
        }
    }

    void UpdateButtonActive()
    {
        bool _canPlus = getThrowing < maxGetThrowing;

        foreach (Button btn in throwingPlusBtn)
        {
            btn.interactable = _canPlus;
        }
    }

    public void PlusThrowingWeapon(int _index)
    {
        UpdateThrowingWeapon(_index, 1);
        throwingMiusBtn[_index].interactable = true;
    }

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
