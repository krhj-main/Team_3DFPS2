using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Customize : MonoBehaviour
{
    public GameObject[] character;          // 캐릭터
    public GameObject[] unifromPanel;       // 상의 선택 패널
    public Material[] uniformMat;           // 캐릭터 유니폼
    public Button[] uniformSelectButton;
    int selectedUniformIndex = 2;
    SelectEquip selectEquip;

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
}
