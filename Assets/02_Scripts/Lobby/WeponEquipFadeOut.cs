using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeponEquipFadeOut : MonoBehaviour
{
    public Image fadeOutImage;

    private void OnEnable()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (true)
        {
            float _colorA = 0;
            _colorA += Time.deltaTime;
            fadeOutImage.color = new Color(0, 0, 0, _colorA);
            if(fadeOutImage.color.a >= 1)
            {
                break;
            }
        }
        yield return null;
        SceneManager.LoadScene("WeaponEquip");
    }
}
