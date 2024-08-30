using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearImage : MonoBehaviour
{
    Image image;

    private void Awake()
    {
        image = this.GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(FillImage());
    }

    IEnumerator FillImage()
    {
        while(image.fillAmount <= 1)
        {
            image.fillAmount += Time.deltaTime;
            yield return null;
        }
        image.fillAmount = 1;
    }
}
