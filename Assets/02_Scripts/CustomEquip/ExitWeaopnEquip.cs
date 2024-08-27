using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitWeaopnEquip : MonoBehaviour
{
    public GameObject[] whenEnterWeaponEquip;   // 웨폰이큅 진입시 켜고 꺼질 UI
    public GameObject fadeOutCanvas;            // 페이드 아웃 캔버스
    private float canvasAlpha = 0;              // 캔버스 알파 값
    private Image fadeImage;                    // 페이드 아웃 될 이미지

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        canvasAlpha = 0;
        while (canvasAlpha <= 1f)                                   // 알파값이 1이 될때까지 알파값을 더해줌
        {
            canvasAlpha += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, canvasAlpha);
            yield return null;
        }

        foreach (GameObject _uiPanel in whenEnterWeaponEquip)       // 알파값이 1이 되면 패널들 켜고 꺼줌
        {
            _uiPanel.SetActive(!_uiPanel.activeSelf);
        }

        fadeOutCanvas.gameObject.SetActive(false);                  // 페이드아웃 패널 꺼줌
    }
}
