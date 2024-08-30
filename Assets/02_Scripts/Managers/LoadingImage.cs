using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;



/// <summary>
/// 로딩 시 UI 이미지 띄울 것 받아서 배열로 저장 후, 씬 별로 혹은 랜덤으로 출력시킬 것
/// UI 이미지와 내용으로 적힐 시나리오 텍스트도 포함
/// </summary>
public class LoadingImage : MonoBehaviour
{
    [SerializeField] Sprite[] loadingImage;
    [TextArea]
    [SerializeField] string[] loadingSceanario;

    TMP_Text text;
    public Image backgroundImage;

    private void Start()
    {
        
        
        backgroundImage.sprite = loadingImage[0];
    }

    private void Update()
    {
        
    }


}
