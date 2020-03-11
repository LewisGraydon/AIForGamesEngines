using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Canvas titleScreenCanvas;
    public Canvas levelSelectionCanvas;
    public Canvas loadingScreenCanvas;
    public Text loadingText;
    public GameObject mainMenu;

    private float timeToWait;
    private int dotCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        titleScreenCanvas.gameObject.SetActive(true);
        levelSelectionCanvas.gameObject.SetActive(false);
        loadingScreenCanvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    { 
        if (loadingScreenCanvas.gameObject.activeSelf)
        {
            timeToWait += Time.deltaTime;

            if (timeToWait > 0.5f)
            {
                loadingText.text += ".";
                dotCount++;

                if (dotCount > 3)
                {
                    loadingText.text = "Loading";
                    dotCount = 0;
                }

                timeToWait = 0;
            }
        }
    }

    public void LevelSelectToggle()
    {
        titleScreenCanvas.gameObject.SetActive(!titleScreenCanvas.gameObject.activeInHierarchy);
        levelSelectionCanvas.gameObject.SetActive(!levelSelectionCanvas.gameObject.activeInHierarchy);
    }

    public void LoadLevel(int index)
    {
        //Change state to the level state
        //mainMenu.SetActive(false);
        levelSelectionCanvas.gameObject.SetActive(false);
        loadingScreenCanvas.gameObject.SetActive(true);
        SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
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
