using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    public int enemyDeathCount = 0;
    public int playerDeathCount = 0;
    public EGameState gameState = EGameState.setupState;
    public GameObject playerContainer;
    public GameObject EndScreenPrefab;
    public PlayerSpawn psScript;

    private bool transitioningToMM = false;

    // Start is called before the first frame update
    void Start()
    {
        psScript = playerContainer.GetComponent<PlayerSpawn>();
        initialiseEndScreen();
    }

    // Update is called once per frame
    void Update()
    {
                
    }

    public void PlayerDeathCheck()
    {
        if(playerDeathCount == psScript.numberOfOperators)
        {
            gameState = EGameState.failureScreen;
        }
    }

    public void EnemyDeathCheck()
    {
        if (enemyDeathCount == 7) // 7 is the number of enemies in current level, may need to put it into a variable soon.
        {
            gameState = EGameState.victoryScreen;
        }
    }

    public void initialiseEndScreen()
    {
        gameState = EGameState.victoryScreen;
        GameObject canvas = GameObject.Find("Canvas");
        GameObject eomScreen = Instantiate(EndScreenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        eomScreen.transform.SetParent(canvas.transform, false);
    }

    public bool isPlayerTurn()
    {
        return gameState == EGameState.playerTurn;
    }

    public bool isEnemyTurn()
    {
        return gameState == EGameState.enemyTurn;
    }

    public void loadMainMenu()
    {
        if (!transitioningToMM)
        {
            SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            transitioningToMM = true;
        }
    }
}
