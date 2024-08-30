using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    [SerializeField] GameObject creditPanel;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnPlayClick()
    {
        SceneManager.LoadScene(1);
    }
    public void OnCreditClick()
    {
        creditPanel.SetActive(true);
    }
    public void OnExitClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE_WIN
            Application.Quit();
        #endif
    }
}
