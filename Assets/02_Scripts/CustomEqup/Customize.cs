using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customize : MonoBehaviour
{
    public GameObject[] character;            // 캐릭터
    public GameObject[] unifromPanel;       // 상의 선택 패널
    public Material[] characterUniform;     // 캐릭터 유니폼


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void UnifromPanel(int _num)
    {
        unifromPanel[_num].SetActive(true);
    }

    public void SelectUnifrom(int _num)
    {
        for(int i = 0; i < character.Length; i++)
        {
            character[i].GetComponent<SkinnedMeshRenderer>().material = characterUniform[_num];
        }
    }


}
