using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMissionScreen : MonoBehaviour
{
    public Text enemyDeathCount;
    public Text playerDeathCount;
    public Text missionOutcome;
    public Button button;

    private GameObject gameStateManager;
    private GameState stateScript;

    // Start is called before the first frame update
    void Start()
    {
        gameStateManager = GameObject.Find("GameStateManager");
        stateScript = gameStateManager.GetComponent<GameState>();
        enemyDeathCount.text = stateScript.enemyDeathCount + " / " + "7";
        playerDeathCount.text = stateScript.playerDeathCount + " / " + stateScript.psScript.numberOfOperators;
        button.onClick.AddListener(stateScript.loadMainMenu);

        if(stateScript.gameState == EGameState.victoryScreen)
        {
            missionOutcome.text = "EGGCELLENT VICTORY";  
        }

        else if(stateScript.gameState == EGameState.failureScreen)
        {
            missionOutcome.text = "CRACKED UNDER PRESSURE";
        }
    }
}
