using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CustomInfomationText
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI infoText;

    public void ChangeText(string name, string info)
    {
        nameText.text = name;
        infoText.text = info;
    }
}

public class Customize : MonoBehaviour
{
    public GameObject[] character;          // 캐릭터
    public GameObject[] unifromPanel;       // 상의 선택 패널
    public Material[] uniformMat;           // 캐릭터 유니폼
    public Button[] uniformSelectButton;
    int selectedUniformIndex = 2;
    SelectEquip selectEquip;
    public CustomInfomationText customInfomationText;

    private void Awake()
    {
        selectEquip = GetComponent<SelectEquip>();
    }

    public void UnifromPanel(int _num)
    {
        selectEquip.PushPanel(unifromPanel[_num]);
        UpdateCustomButtonColor();
    }

    public void SelectUnifrom(int _num)
    {
        for(int i = 0; i < character.Length; i++)
        {
            selectedUniformIndex = _num;
            character[i].GetComponent<SkinnedMeshRenderer>().material = uniformMat[_num];
            UpdateCustomButtonColor();
        }
    }

    void UpdateCustomButtonColor()
    {
        for (int i = 0; i < uniformSelectButton.Length; i++)
        {
            ColorBlock _colorBlock;
            _colorBlock = uniformSelectButton[i].colors;

            if (selectedUniformIndex == i)
            {
                _colorBlock.highlightedColor = Color.red;
                uniformSelectButton[i].colors = _colorBlock;
            }
            else
            {
                _colorBlock.highlightedColor = Color.green;
                uniformSelectButton[i].colors = _colorBlock;
            }
        }
    }

    public void ApplyCustomize()
    {
        foreach(GameObject go in GameManager.Instance.applyCustomCharacter)
        {
            go.GetComponent<SkinnedMeshRenderer>().material = uniformMat[selectedUniformIndex];
        }
    }

    public void ChangeCustomInfoText1()
    {
        customInfomationText.ChangeText("MTP", "환경과 계절에 상관없이\n위장력을 높였다\n모든 지형에 대응이 가능하다");
    }

    public void ChangeCustomInfoText2()
    {
        customInfomationText.ChangeText("NWU I형", "밝은곳에서는 위력을\n발휘하기 어렵지만 어두운 곳에서\n탁월한 위장 성능을 발휘한다");
    }

    public void ChangeCustomInfoText3()
    {
        customInfomationText.ChangeText("ACU", "디지털 픽셀을 채용했으며\n4계절용 원단을 사용하였다\n시가전에서 높은 위장력을 자랑한다.");
    }

    public void ChangeCustomInfoText4()
    {
        customInfomationText.ChangeText("CWU", "항공대를 위해 보급된 군복이다\n조종사들이 질리도록 입고\n나오는 옷이라 꽤 친숙하다.");
    } 
    
    public void ChangeCustomInfoText5()
    {
        customInfomationText.ChangeText("SWAT", "위장무늬 특유의 군대를\n연상시키는 느낌도 없다.\n알 수 없는 상대라는 느낌을 주기 좋다.");
    }
}
