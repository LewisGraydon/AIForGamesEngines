using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    public int enemyDeathCount = 0;
    public int playerDeathCount = 0;
    public EGameState gameState = EGameState.setupState;
    public GameObject playerContainer;
    public GameObject enemyContainer;
    public GameObject EndScreenPrefab;
    public PlayerSpawn psScript;
    private EnemySpawn esScript;
    public PathfindingAgent pathfindingAgent;

    public CharacterBase activeCharacter;
    private int playerPipsRemaining = -1;
    private int enemyPipsRemaining = -1;

    public Text turnText;
    public GameObject badEggPrefab;
    public GameObject badEggsSpottedUI;
    public EnemyCharacter badEggTargetted = null;
    public GameObject attackPromptUI;
    public Text attackPromptText;
    public Text playerAmmoCountText;

    // End Of Mission
    private bool transitioningToMM = false;

    // Start is called before the first frame update
    void Start()
    {
        psScript = playerContainer.GetComponent<PlayerSpawn>();
        esScript = enemyContainer.GetComponent<EnemySpawn>();
        turnText.gameObject.SetActive(true);
        ProcessGameState();
    }
    private void Awake()
    {
        psScript = playerContainer.GetComponent<PlayerSpawn>();
        esScript = enemyContainer.GetComponent<EnemySpawn>();
        turnText.gameObject.SetActive(true);
        ProcessGameState();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState != EGameState.victoryScreen || gameState != EGameState.failureScreen)
        {
            if (Input.GetKeyUp(KeyCode.LeftBracket)) // For Debug purposes.
            {
                KillAllPlayers();
            }
            else if (Input.GetKeyUp(KeyCode.RightBracket)) // For Debug purposes.
            {
                KillAllEnemies();
            }

            if(gameState == EGameState.playerTurn)
            {
                if(Input.GetKeyUp(KeyCode.LeftAlt))
                {
                    for (int i = 0; i < playerContainer.transform.childCount; i++)
                    {
                        GameObject obj = playerContainer.transform.GetChild(i).gameObject;
                        PlayerCharacter pc = obj.GetComponent<PlayerCharacter>();
                        pc.actionPips = 0;
                        ProcessGameState();
                    }
                }
            }

            if (gameState == EGameState.enemyTurn)
            {
                if (Input.GetKeyUp(KeyCode.RightAlt))
                {
                    for (int i = 0; i < enemyContainer.transform.childCount; i++)
                    {
                        GameObject obj = enemyContainer.transform.GetChild(i).gameObject;
                        EnemyCharacter ec = obj.GetComponent<EnemyCharacter>();
                        ec.actionPips = 0;
                        ProcessGameState();
                    }
                }
            }

            PlayerDeathCheck();
            EnemyDeathCheck();
        }
        if (gameState == EGameState.movement)
        {
            EnemyManager enemyManager = GameObject.FindObjectOfType<EnemyManager>();
            PlayerManager playerManager = GameObject.FindObjectOfType<PlayerManager>();
            activeCharacter = enemyManager.activeCharacter != null ? (CharacterBase)enemyManager.activeCharacter : playerManager.selectedPlayer.GetComponent<CharacterBase>();
            activeCharacter.MoveCharacterAlongTilePath();
            playerManager.SetupCameraPosition();
        }
    }

    // Self explanatory, but checks if all players are dead and if so, it will initialise the end of mission screen in the failure state.
    public void PlayerDeathCheck()
    {
        if(playerDeathCount == psScript.numberOfOperators)
        {
            gameState = EGameState.failureScreen;
            initialiseEndScreen();
        }
    }

    // Self explanatory, but checks if all enemies are dead and if so, it will initialise the end of mission screen in the victory state.
    public void EnemyDeathCheck()
    {
        if (enemyDeathCount == esScript.numberOfEnemies) // 7 is the number of enemies in current level, may need to put it into a variable soon.
        {
            gameState = EGameState.victoryScreen;
            initialiseEndScreen();
        }
    }

    // Initialises the end of screen prefab and sets it to be the child of the canvas in the scene.
    public void initialiseEndScreen()
    {
        turnText.gameObject.SetActive(false);
        
        GameObject canvas = GameObject.Find("Canvas");
        GameObject eomScreen = Instantiate(EndScreenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        eomScreen.transform.SetParent(canvas.transform, false);
    }

    // Boolean check for whether it is the player turn.
    public bool isPlayerTurn()
    {
        return gameState == EGameState.playerTurn;
    }

    // Boolean check for whether it is the enemy turn.
    public bool isEnemyTurn()
    {
        return gameState == EGameState.enemyTurn;
    }

    // Function to load the main menu asynchronously.
    public void loadMainMenu()
    {
        if (!transitioningToMM)
        {
            SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            transitioningToMM = true;
            Debug.Log("Loading Main Menu");
        }
    }

    #region DebugFunctions

    // Debug function to kill all players.
    void KillAllPlayers()
    {
        playerDeathCount = psScript.numberOfOperators;

        for (int i = 0; i < playerContainer.transform.childCount; i++)
        {
            GameObject obj = playerContainer.transform.GetChild(i).gameObject;
            Destroy(obj);
        }
    }

    // Debug function to kill all enemies.
    void KillAllEnemies()
    {
        enemyDeathCount = esScript.numberOfEnemies;

        for(int i = 0; i < enemyContainer.transform.childCount; i++)
        {
            GameObject obj = enemyContainer.transform.GetChild(i).gameObject;
            Destroy(obj);
        }
    }

    #endregion

    // This should ideally get called after each action.
    public void ProcessGameState()
    {
        int pipCount = 0;

        switch(gameState)
        {
            case EGameState.setupState:
                enemyContainer.GetComponent<EnemyManager>().activeCharacter = null;
                InitialisePlayerPips();
                
                gameState = EGameState.playerTurn;
                turnText.text = "Player Turn";

                PlayerManager playerManager = playerContainer.GetComponent<PlayerManager>();
                PlayerCharacter playerCharacter = GameObject.FindObjectOfType<PlayerCharacter>();

                if (playerManager != null && playerCharacter != null)
                {
                    badEggsSpottedUI.SetActive(true);
                    addToBadEggsSpottedUI(playerManager.selectedPlayer.GetComponent<CharacterBase>().enemiesInSight);

                    if (!isAnyBadEggSpotted())
                    {
                        badEggsSpottedUI.SetActive(false);
                    }
                }
                break;

            case EGameState.playerTurn:
                enemyContainer.GetComponent<EnemyManager>().activeCharacter = null;
                for (int i = 0; i < playerContainer.transform.childCount; i++)
                {
                    GameObject obj = playerContainer.transform.GetChild(i).gameObject;
                    pipCount += obj.GetComponent<PlayerCharacter>().actionPips;
                }

                playerPipsRemaining = pipCount;

                if (playerPipsRemaining <= 0)
                {
                    clearBadEggsSpottedContainer();
                    badEggsSpottedUI.gameObject.SetActive(false);

                    InitialiseEnemyPips();
                    gameState = EGameState.enemySetup;
                    ProcessGameState();
                }

                break;

            case EGameState.enemyTurn:

                turnText.text = "Enemy Turn";
                for (int i = 0; i < enemyContainer.transform.childCount; i++)
                {
                    GameObject obj = enemyContainer.transform.GetChild(i).gameObject;
                    pipCount += obj.GetComponent<EnemyCharacter>().actionPips;
                }
                enemyPipsRemaining = pipCount;
                if (enemyPipsRemaining <= 0)
                {
                    gameState = EGameState.setupState;
                    ProcessGameState();
                }

                break;

            case EGameState.enemySetup:

                EnemyManager enemyManager = enemyContainer.GetComponent<EnemyManager>();
                for (int i = 0; i < enemyContainer.transform.childCount; i++)
                {
                    EnemyCharacter enemyCharacter = enemyContainer.transform.GetChild(i).gameObject.GetComponent<EnemyCharacter>();
                    enemyCharacter.actionPips = enemyCharacter.MaximumActionPips;
                    enemyCharacter.isOverwatching = false;
                    enemyManager.overwatchingEnemies.Remove(enemyCharacter);
                }
                enemyManager.SetUpEnemyTurn();
                gameState = EGameState.enemyTurn;
                ProcessGameState();
                break;

            default:
                break;
        }
    }

    // Called at the start of player turn / end of enemy turn (haven't decided which yet) to set the pips to be the number of operators.
    // Also, to note the number of operators should 
    void InitialisePlayerPips()
    {
        playerPipsRemaining = (psScript.numberOfOperators - playerDeathCount) * 2;
        PlayerCharacter[] playerCharacters = GameObject.FindObjectsOfType<PlayerCharacter>();
        PlayerManager playerManager= FindObjectOfType<PlayerManager>();
        foreach (PlayerCharacter player in playerCharacters)
        {
            player.actionPips = player.MaximumActionPips;
            player.isOverwatching = false;
            playerManager.overwatchingPlayers.Remove(player);
        }
    }

    void InitialiseEnemyPips()
    {
        enemyPipsRemaining = (esScript.numberOfEnemies - enemyDeathCount) * 2;
        foreach (CharacterBase enemy in enemyContainer.GetComponentsInChildren<CharacterBase>())
        {
            enemy.actionPips = enemy.MaximumActionPips;
        }
    }

    public void updateCanvasRotations()
    {
        foreach(CharacterBase player in playerContainer.GetComponentsInChildren<CharacterBase>())
        {
            player.faceCanvasToCamera();
        }
        foreach(CharacterBase enemy in enemyContainer.GetComponentsInChildren<CharacterBase>())
        {
            enemy.faceCanvasToCamera();
        }
    }

    public void updateOtherTeamSightLines(CharacterBase caller)
    {
        if (caller is PlayerCharacter)
        {
            EnemyCharacter[] enemyCharacters = GameObject.FindObjectsOfType<EnemyCharacter>();
            foreach (EnemyCharacter enemyCharacter in enemyCharacters)
            {
                enemyCharacter.FindSightline();
            }


            //Trigger Overwatch
            var watchers = enemyContainer.GetComponent<EnemyManager>().overwatchingEnemies;
            List<EnemyCharacter> triggeredEnemyWatchers = new List<EnemyCharacter>();
            foreach (EnemyCharacter enemy in watchers)
            {
                foreach (var keyValuePair in enemy.enemiesInSight)
                {
                    if (keyValuePair.Key == caller)
                    {
                        enemy.OverwatchAttackCharacter(caller);
                        enemy.isOverwatching = false;
                        triggeredEnemyWatchers.Add(enemy);
                        break;
                    }
                }

            }
            
            if(triggeredEnemyWatchers.Count > 0)
            {
                foreach(var chara in triggeredEnemyWatchers)
                {
                    watchers.Remove(chara);
                }   
            
            }

        }
        else
        {
            PlayerCharacter[] playerCharacters = GameObject.FindObjectsOfType<PlayerCharacter>();
            foreach (PlayerCharacter playerCharacter in playerCharacters)
            {
                playerCharacter.FindSightline();
            }

            var watchers = playerContainer.GetComponent<PlayerManager>().overwatchingPlayers;
            List<PlayerCharacter> triggeredPlayerWatchers = new List<PlayerCharacter>();
            foreach (PlayerCharacter player in watchers)
            {
                foreach(var keyValuePair in player.enemiesInSight)
                {
                    if(keyValuePair.Key == caller)
                    {
                        player.OverwatchAttackCharacter(caller);
                        player.isOverwatching = false;
                        triggeredPlayerWatchers.Add(player);
                        break;
                    }
                }
            }

            if(triggeredPlayerWatchers.Count > 0)
            {
                foreach(var chara in triggeredPlayerWatchers)
                {
                    watchers.Remove(chara);
                }
            }


        }
    }

    public void clearBadEggsSpottedContainer()
    {
        for (int i = 0; i < badEggsSpottedUI.transform.childCount; i++)
        {
            Destroy(badEggsSpottedUI.transform.GetChild(i).gameObject);
        }
    }

    public void addToBadEggsSpottedUI(List<KeyValuePair<CharacterBase, int>> badEggsSpotted)
    {
        foreach(KeyValuePair<CharacterBase,int> pair in badEggsSpotted)
        {
            GameObject go = Instantiate(badEggPrefab, new Vector3(badEggsSpottedUI.transform.position.x, badEggsSpottedUI.transform.position.y, badEggsSpottedUI.transform.position.z), Quaternion.identity);
            go.transform.SetParent(badEggsSpottedUI.transform, false);
            go.GetComponent<BadEggInfo>().badEgg = pair.Key.gameObject;
            go.GetComponent<BadEggInfo>().hitChanceText.text = pair.Value + "%";
        }
    }

    public bool isAnyBadEggSpotted()
    {
        if(badEggsSpottedUI.transform.childCount > 0)
        {
            return true;
        }
        return false;
    }
}
