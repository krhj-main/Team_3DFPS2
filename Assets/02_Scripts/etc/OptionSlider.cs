using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionSlider : MonoBehaviour
{
    [SerializeField] Slider contrast;
    [SerializeField] Slider volume;
    [SerializeField] Slider mouse;
    ColorGrading ppv;
    AudioSource audio;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        ppv = FindObjectOfType<PostProcessVolume>().GetComponent<ColorGrading>();//GetComponent<PostProcessVolume>();
        audio = GameObject.Find("BGM").GetComponentInChildren<AudioSource>();


        volume.onValueChanged.RemoveAllListeners();
        contrast.onValueChanged.RemoveAllListeners();
        mouse.onValueChanged.RemoveAllListeners();

        volume.onValueChanged.AddListener(VolumeChange);

        contrast.onValueChanged.AddListener(ExposureChagne);

        mouse.onValueChanged.AddListener(MouseSensChange);
    }

    private void FixedUpdate()
    {
        
    }
    void VolumeChange(float _volume)
    {
        audio.volume = volume.value;
    }
    void ExposureChagne(float _exposure)
    {
        ppv.postExposure.value = contrast.value;
    }
    void MouseSensChange(float _sensitivity)
    {
        CameraController.mouseSensitivity = mouse.value;
    }


}
