using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    #region Variables

    #region Game State Variables
    private EGameState _gameState;
    public EGameState gameState
    {
        get => _gameState;
        set
        {
            previousState = _gameState;
            _gameState = value;
        }
    }
    private EGameState previousState;
    #endregion

    #region Character containers
    public List<PlayerCharacter> playerCharacters = null;
    public List<EnemyCharacter> rottenEggs = null;
    #endregion

    #region External UI References
    public GameObject endingScreenPrefab = null;
    public Text turnText;
    public GameObject badEggPrefab;
    public GameObject badEggSeenUI;
    #endregion

    #region External References (non-UI)
    PathfindingAgent _pathfindingAgent;
    public PathfindingAgent PathfindingAgent { get => _pathfindingAgent; }

    private int cameraPositionIndex = 0;
    Vector3[] cameraPositionArray = new Vector3[4];

    public List<CharacterBase> overwatchingEnemies;
    public List<CharacterBase> overwatchingPlayers;
    #endregion

    #region Active Character Variables
    private CharacterBase _activeCharacter;
    public CharacterBase activeCharacter
    {
        get => _activeCharacter;
        set
        {
            _activeCharacter = value;
            activeCharacterMovementRange = PathfindingAgent.FindMovementRange(activeCharacter.occupiedTile, activeCharacter.MovementRange);
            Debug.LogError("Update Camera");
        }
    }
    private List<INodeSearchable> _activeCharacterMovementRange = null;
    public List<INodeSearchable> activeCharacterMovementRange 
    { 
        get => _activeCharacterMovementRange;
        set
        {
            PathfindingAgent.NodeReset(_activeCharacterMovementRange);
            _activeCharacterMovementRange = value != null ? value : new List<INodeSearchable>();
        }
    }
    Stack<INodeSearchable> activeCharacterPath = null;
    public Tile destinationTile = null;
    #endregion

    #endregion

    #region Functions
    void Awake()
    {
        gameState = EGameState.setupState;
        _pathfindingAgent = FindObjectOfType<PathfindingAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            //setUpState: state to gather all the information and populate the Character List, Enemy List etc. and ensure the game hasn't ended;
            case EGameState.setupState:
                {
                    #region Set Variables;
                    if(PathfindingAgent == null)
                    {
                        _pathfindingAgent = FindObjectOfType<PathfindingAgent>();
                        if(PathfindingAgent == null)
                        {
                            Debug.LogError("PATHFINDING AGENT IS STILL NULL");
                        }
                    }
                    playerCharacters = FindObjectsOfType<PlayerCharacter>().ToList();
                    rottenEggs = FindObjectsOfType<EnemyCharacter>().ToList();
                    destinationTile = null;
                    #endregion

                    #region Game End Checks;
                    if (playerCharacters.Count == 0)
                        gameState = EGameState.failureScreen;
                    else if (rottenEggs.Count == 0)
                        gameState = EGameState.victoryScreen;
                    #endregion

                    #region Turn End Check?
                    if (playerCharacters.Find((PlayerCharacter p) => p.actionPips != 0) == null) //if no player without 0 actionPips;
                    {
                        _gameState = EGameState.enemySetup;
                        break;
                    }
                    else if (rottenEggs.Find((EnemyCharacter e) => e.actionPips != 0) == null)
                    {
                        _gameState = EGameState.playerSetup;
                        break;
                    }
                    #endregion

                    _gameState = previousState == EGameState.setupState ? EGameState.playerSetup : previousState; // bypass gameState setter as to avoid having previous state ever be the setUpState;
                    break;
                }
            case EGameState.playerSetup:
                {
                    foreach (PlayerCharacter playerCharacter in playerCharacters) // Set action pips;
                    {
                        playerCharacter.actionPips = playerCharacter.MaximumActionPips;
                    }
                    activeCharacter = playerCharacters[0];
                    gameState = EGameState.playerTurn;
                    break;
                }
            case EGameState.playerTurn:
                {
                    if(activeCharacter.actionPips == 0)
                    {
                        SelectNextAvailablePlayerCharacter();
                        break;
                    }
                    else
                    {
                    #region Input Handling
                        #region Change Active PlayerCharacter
                        //Select Previous Character with > 0 pips
                        if (Input.GetKeyUp(KeyCode.Q))
                        {
                            SelectEarlierAvailablePlayerCharacter();
                        }
                        //Select Next Character with > 0 pips
                        if (Input.GetKeyUp(KeyCode.E))
                        {
                            SelectNextAvailablePlayerCharacter();
                        }
                        #endregion

                        //Click Release Action; Active Character has to have > 0 pips;
                        if (Input.GetMouseButtonUp(0))
                        {
                            if (destinationTile != null)
                            {
                                activeCharacterPath = PathfindingAgent.CreatePath(destinationTile);
                                PathfindingAgent.NodeReset(activeCharacterMovementRange);
                                activeCharacter.SetMovementStack(activeCharacterPath, activeCharacterMovementRange);
                                gameState = EGameState.movement;
                            }
                        }

                        #region Camera Affecting Inputs
                        if(Input.GetKey(KeyCode.DownArrow))
                        {
                            Camera.main.transform.position -= Camera.main.transform.forward;
                        }
                        if(Input.GetKey(KeyCode.UpArrow))
                        {
                            Camera.main.transform.position += Camera.main.transform.forward;
                        }
                        if (Input.GetKey(KeyCode.LeftArrow))
                        {
                            Camera.main.transform.position -= Camera.main.transform.right;
                        }
                        if (Input.GetKey(KeyCode.RightArrow))
                        {
                            Camera.main.transform.position += Camera.main.transform.right;
                        }
                        if(Input.GetKeyUp(KeyCode.Home))
                        {
                            SetupCameraPosition();
                        }
                        if(Input.GetKeyUp(KeyCode.PageUp))
                        {
                            cameraPositionIndex++;
                            if(cameraPositionIndex > cameraPositionArray.Length)
                            {
                                cameraPositionIndex = 0;
                            }
                            SetupCameraPosition();
                        }
                        if (Input.GetKeyUp(KeyCode.PageDown))
                        {
                            cameraPositionIndex--;
                            if (cameraPositionIndex < 0)
                            {
                                cameraPositionIndex = cameraPositionArray.Length - 1;
                            }
                            SetupCameraPosition();
                        }
                        #endregion

                        #endregion
                    }
                    break;
                }
            case EGameState.enemySetup:
                {
                    foreach (EnemyCharacter rottenEgg in rottenEggs) // Set action pips;
                    {
                        rottenEgg.actionPips = rottenEgg.MaximumActionPips;
                    }
                    activeCharacter = rottenEggs[0]; //set active Character to first EnemyCharacter
                    gameState = EGameState.enemyTurn;
                    break;
                }
            case EGameState.enemyTurn:
                {
                    //end of (activeCharacter as EnemyCharacter) turn check; 
                    if (activeCharacter.actionPips == 0)
                    {
                        SelectNextAvailableAICharacter();
                        break;
                    }
                    EnemyCharacter activeRottenEgg = (EnemyCharacter)activeCharacter;
                    activeRottenEgg.MakeDecision();
                    break;
                }
            case EGameState.movement:
                {
                    if(activeCharacter.MoveCharacterAlongTilePath())
                    {
                        this.activeCharacterMovementRange = this.PathfindingAgent.FindMovementRange(activeCharacter.occupiedTile, activeCharacter.MovementRange);
                        gameState = previousState;
                    }
                    //update Camera Position;
                    //Debug.LogError("Camera not being updated");
                    //UpdateCanvasRotations();
                    break;
                }
            case EGameState.beeAttack:
                break;
            case EGameState.victoryScreen:
                {
                    initialiseEndScreen();
                    break;
                }
            case EGameState.failureScreen:
                {
                    initialiseEndScreen();
                    break;
                }
            default:
                break;
        }
    }

    private void SelectNextAvailablePlayerCharacter()
    {
        int currentActivePlayerCharacterIndex = playerCharacters.FindIndex((PlayerCharacter playerCharacter) => playerCharacter == activeCharacter);
        PlayerCharacter nextCharacter = null;
        do
        {
            currentActivePlayerCharacterIndex++;
            if (currentActivePlayerCharacterIndex >= playerCharacters.Count)
            {
                currentActivePlayerCharacterIndex = 0;
            }
            nextCharacter = playerCharacters[currentActivePlayerCharacterIndex];
        } while (nextCharacter != null && nextCharacter.actionPips == 0 && nextCharacter != activeCharacter);
        if(activeCharacter == nextCharacter || nextCharacter == null)
        {
            gameState = EGameState.setupState;
        }
        else
        {
            activeCharacter = nextCharacter;
        }
    }

    private void SelectEarlierAvailablePlayerCharacter()
    {
        int currentActivePlayerCharacterIndex = playerCharacters.FindIndex((PlayerCharacter playerCharacter) => playerCharacter == activeCharacter);
        PlayerCharacter nextCharacter = null;
        do
        {
            currentActivePlayerCharacterIndex--;
            if (currentActivePlayerCharacterIndex < 0)
            {
                currentActivePlayerCharacterIndex = playerCharacters.Count - 1;
            }
            nextCharacter = playerCharacters[currentActivePlayerCharacterIndex];
        } while (nextCharacter != null && nextCharacter.actionPips == 0 && nextCharacter != activeCharacter);
        if(activeCharacter == nextCharacter || nextCharacter == null)
        {
            gameState = EGameState.setupState;
        }
        else
        {
            activeCharacter = nextCharacter;
        }
    }

    private void SelectNextAvailableAICharacter()
    {
        int nextActiveCharacterIndex = rottenEggs.FindIndex((EnemyCharacter rottenEgg) => rottenEgg == activeCharacter) + 1;
        if (nextActiveCharacterIndex < rottenEggs.Count)
        {
            activeCharacter = rottenEggs[nextActiveCharacterIndex];
        }
        else
        {
            foreach (EnemyCharacter rottenEgg in rottenEggs)
            {
                rottenEgg.actionPips = 0;
            }
            gameState = EGameState.setupState;
        }
    }

    public bool CheckAgainstMovementRange(INodeSearchable nodeToCheck)
    {
        this.activeCharacterMovementRange = PathfindingAgent.FindMovementRange(activeCharacter.occupiedTile, activeCharacter.MovementRange);
        return this.activeCharacterMovementRange.Contains(nodeToCheck);
    }

    public void UpdateCanvasRotations()
    {
        foreach (CharacterBase player in playerCharacters)
        {
            player.faceCanvasToCamera();
        }
        foreach (CharacterBase rottenEgg in rottenEggs)
        {
            rottenEgg.faceCanvasToCamera();
        }
    }

    public void initialiseEndScreen()
    {
        turnText.gameObject.SetActive(false);

        GameObject canvas = GameObject.Find("Canvas");
        GameObject eomScreen = Instantiate(endingScreenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        eomScreen.transform.SetParent(canvas.transform, false);
    }

    public void SetupCameraPosition()
    {
        //selectedPlayer = selectablePlayers[selectedIndex];

        cameraPositionArray = new Vector3[] {   new Vector3(activeCharacter.transform.position.x, activeCharacter.transform.position.y + 4.5f, activeCharacter.transform.position.z - 5),
                                                new Vector3(activeCharacter.transform.position.x - 5, activeCharacter.transform.position.y + 4.5f, activeCharacter.transform.position.z),
                                                new Vector3(activeCharacter.transform.position.x, activeCharacter.transform.position.y + 4.5f, activeCharacter.transform.position.z + 5),
                                                new Vector3(activeCharacter.transform.position.x + 5, activeCharacter.transform.position.y + 4.5f, activeCharacter.transform.position.z)   };

        Camera.main.transform.position = cameraPositionArray[cameraPositionIndex];
        UpdateCanvasRotations();
    }

    public void clearBadEggsSpottedContainer()
    {
        for (int i = 0; i < badEggSeenUI.transform.childCount; i++)
        {
            Destroy(badEggSeenUI.transform.GetChild(i).gameObject);
        }
    }

    public void addToBadEggsSpottedUI(List<KeyValuePair<CharacterBase, int>> badEggsSpotted)
    {

        foreach (KeyValuePair<CharacterBase, int> pair in badEggsSpotted)
        {
            GameObject go = Instantiate(badEggPrefab, new Vector3(badEggSeenUI.transform.position.x, badEggSeenUI.transform.position.y, badEggSeenUI.transform.position.z), Quaternion.identity);
            go.transform.SetParent(badEggSeenUI.transform, false);
            go.GetComponent<BadEggInfo>().badEgg = pair.Key.gameObject;
        }
    }

    public bool isAnyBadEggSpotted()
    {
        if (badEggSeenUI.transform.childCount > 0)
        {
            return true;
        }
        return false;
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

            bool playerHasEnteredOverwatchSightline = false;
            foreach (EnemyCharacter enemy in overwatchingEnemies)
            {
                foreach (var keyValuePair in enemy.enemiesInSight)
                {
                    if (keyValuePair.Key == caller)
                    {
                        playerHasEnteredOverwatchSightline = true;
                    }
                }
                if (playerHasEnteredOverwatchSightline)
                {
                    enemy.OverwatchAttackCharacter(caller);
                    overwatchingEnemies.Remove(enemy);
                    enemy.isOverwatching = false;
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
            bool enemyHasEnteredOverwatchSightline = false;
            foreach (PlayerCharacter player in overwatchingPlayers)
            {
                foreach (var keyValuePair in player.enemiesInSight)
                {
                    if (keyValuePair.Key == caller)
                    {
                        enemyHasEnteredOverwatchSightline = true;
                    }
                }
                if (enemyHasEnteredOverwatchSightline)
                {
                    player.OverwatchAttackCharacter(caller);
                    overwatchingPlayers.Remove(player);
                    player.isOverwatching = false;
                }
            }
        }
    }
    #endregion
}
