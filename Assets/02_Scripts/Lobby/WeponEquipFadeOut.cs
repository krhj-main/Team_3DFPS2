using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeponEquipFadeOut : MonoBehaviour
{
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
        SceneManager.LoadScene("WeaponEquip");
    }
}
