using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashEffectEnd : MonoBehaviour
{
    [SerializeField] AudioSource flashSound;

    public float duration;
    public float Duration {
        set {
            duration = Mathf.Clamp(value,0, Mathf.Infinity);
            if (duration <= 0) { flash.enabled = false; }
            else if(!flash.enabled) { 
                flash.enabled = true;
                flashSound.Play();
            }
        }
        get => duration;
    }
    Image flash;

    private void Awake()
    {
        flash=GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (duration > 0) {

        Duration -= Time.deltaTime;
        }
    }
}
