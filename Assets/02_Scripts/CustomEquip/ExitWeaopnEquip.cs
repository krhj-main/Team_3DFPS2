using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitWeaopnEquip : MonoBehaviour
{
    public GameObject weaponEquip;
    public GameObject fadeOutCanvas;
    float canvasAlpha = 0;
    private void OnEnable()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (canvasAlpha <= 1f)
        {
            canvasAlpha = this.GetComponent<Image>().color.a;
            canvasAlpha += Time.deltaTime;
            this.GetComponent<Image>().color = new Color(0, 0, 0, canvasAlpha);
            yield return null;
        }
        weaponEquip.SetActive(true);
        fadeOutCanvas.gameObject.SetActive(false);
    }
}
