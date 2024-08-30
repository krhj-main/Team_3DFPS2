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

    public TextMeshProUGUI scenario;
    public Image backgroundImage;

    private void Start()
    {
        //LoadingInit();

    }

    private void Update()
    {
        
    }

    public void LoadingInit()
    {
        int _sceneNum = Mathf.Clamp(GameManager.Instance.selectSceneNum - 2, 0, 100);
        backgroundImage.sprite = loadingImage[_sceneNum];
        scenario.text = loadingSceanario[_sceneNum];
        scenario.rectTransform.anchoredPosition = new Vector2(0, -scenario.preferredHeight);
        StartCoroutine(ScrollScript());
    }

    public IEnumerator ScrollScript()
    {
        float _preferHeight = scenario.preferredHeight;        

        while (scenario.rectTransform.rect.top > -_preferHeight)
        {
            
            scenario.rectTransform.anchoredPosition = new Vector2(scenario.rectTransform.anchoredPosition.x, 
                                                        scenario.rectTransform.anchoredPosition.y + 1f);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }        
    }


}
