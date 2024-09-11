using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;


public class PlayerInTracking : MonoBehaviour
{

    CinemachineVirtualCameraBase cv;


    // Start is called before the first frame update
    void Start()
    {
        cv.Follow = PlayerController.Instance.transform;
        cv.LookAt = PlayerController.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cv.Follow = PlayerController.Instance.transform;
        cv.LookAt = PlayerController.Instance.transform;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
