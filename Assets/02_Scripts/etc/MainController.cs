using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    [SerializeField] GameObject creditPanel;
    [SerializeField] GameObject optionPanel;

    public void OnPlayClick()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void OnOptionClick()
    {
        optionPanel.SetActive(true);
    }
    public void OnTitleClick()
    {
        SceneManager.LoadScene(0);
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
