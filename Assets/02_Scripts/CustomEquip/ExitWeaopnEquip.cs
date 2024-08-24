using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitWeaopnEquip : MonoBehaviour
{
    public GameObject weaponEquip;
    public GameObject fadeOutCanvas;
    float canvasAlpha = 0;
    private Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(FadeOut());
        Debug.Log(canvasAlpha);
    }

    IEnumerator FadeOut()
    {
        canvasAlpha = 0;
        while (canvasAlpha <= 1f)
        {
            canvasAlpha += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, canvasAlpha);
            yield return null;
        }
        weaponEquip.SetActive(!weaponEquip.activeSelf);
        fadeOutCanvas.gameObject.SetActive(false);
    }
}
