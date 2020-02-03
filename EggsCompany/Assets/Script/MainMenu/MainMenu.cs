using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Canvas titleScreenCanvas;
    public Canvas levelSelectionCanvas;
    public GameObject mainMenu;

    // Start is called before the first frame update
    void Start()
    {
        titleScreenCanvas.gameObject.SetActive(true);
        levelSelectionCanvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LevelSelectToggle()
    {
        titleScreenCanvas.gameObject.SetActive(!titleScreenCanvas.gameObject.activeInHierarchy);
        levelSelectionCanvas.gameObject.SetActive(!levelSelectionCanvas.gameObject.activeInHierarchy);
    }

    public void LoadLevel()
    {
        //Change state to the level state
        mainMenu.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
