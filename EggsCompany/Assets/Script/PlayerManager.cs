using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public GameObject[] selectablePlayers = new GameObject[4];
    public List<PlayerCharacter> overwatchingPlayers = new List<PlayerCharacter>();
    public int selectedIndex = 0;

    public GameObject cameraObject;

    private GameObject gsm;
    private GameState gsmScript;
    public GameObject selectedPlayer = null;
    private Vector3 selectedPlayerCameraPosition = Vector3.zero;

    public Text playerNameText;

    private int _cameraPositionIndex = 0;
    public int cameraPositionIndex
    {
        get => _cameraPositionIndex;
        set
        {
            _cameraPositionIndex = value;
        }
    }

    private Vector3[] cameraPositionArray = new Vector3[4];

    public Tile destinationTile = null;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            selectablePlayers[i] = (transform.GetChild(i).gameObject);
        }
       
        gsm = GameObject.Find("GameStateManager");
        gsmScript = gsm.GetComponent<GameState>();
     
        SetupCameraPosition();
    }

    private Stack<INodeSearchable> pathToDestination = null;
    public List<INodeSearchable> nodeSearchables = null;

    void Update()
    {
        if (gsmScript.gameState == EGameState.playerTurn)
        {


            if(Input.GetMouseButtonUp(1))
            {
                if (selectedPlayer.GetComponent<CharacterBase>().actionPips == 0)
                {
                    return;
                }

                if (destinationTile != null)
                {
                    CharacterBase tempChar = gsmScript.pathfindingAgent.CheckIfTileOccupied(destinationTile);
                    if (!nodeSearchables.Contains(destinationTile) || tempChar != null)
                    {
                        gsmScript.pathfindingAgent.NodeReset(nodeSearchables);
                        return;
                    }

                    pathToDestination = gsmScript.pathfindingAgent.CreatePath(destinationTile.GetComponent<Tile>());
                    selectedPlayer.GetComponent<CharacterBase>().SetMovementStack(pathToDestination, nodeSearchables);
                 
                    gsmScript.gameState = EGameState.movement;
                }
                gsmScript.updateCanvasRotations();
            }

            if (Input.GetKeyUp(KeyCode.Q))
            {   
                gsmScript.clearBadEggsSpottedContainer();
                gsmScript.badEggsSpottedUI.SetActive(false);

                selectedIndex -= 1;

                if (selectedIndex < 0)
                {
                    selectedIndex = 3;
                }
            
                SetupCameraPosition();

                gsmScript.badEggsSpottedUI.SetActive(true);
                gsmScript.addToBadEggsSpottedUI(selectedPlayer.GetComponent<CharacterBase>().enemiesInSight);

                if(!gsmScript.isAnyBadEggSpotted())
                {
                    gsmScript.badEggsSpottedUI.SetActive(false);
                }

                gsmScript.updateCanvasRotations();
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                gsmScript.clearBadEggsSpottedContainer();
                gsmScript.badEggsSpottedUI.SetActive(false);

                selectedIndex += 1;

                if (selectedIndex > 3)
                {
                    selectedIndex = 0;
                }
            
                SetupCameraPosition();

                gsmScript.badEggsSpottedUI.SetActive(true);
                gsmScript.addToBadEggsSpottedUI(selectedPlayer.GetComponent<CharacterBase>().enemiesInSight);

                if (!gsmScript.isAnyBadEggSpotted())
                {
                    gsmScript.badEggsSpottedUI.SetActive(false);
                }

                gsmScript.updateCanvasRotations();
            }

            if(Input.GetKeyUp(KeyCode.Alpha1))
            {
                SelectedPlayerActions(0);
            }

            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                SelectedPlayerActions(1);
            }

            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                SelectedPlayerActions(2);
            }

            if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                SelectedPlayerActions(3);
            }

            if(gsmScript.attackPromptUI.gameObject.activeSelf)
            {
                if(Input.GetKeyUp(KeyCode.Escape))
                {
                    gsmScript.attackPromptUI.gameObject.SetActive(false);
                }
            }

            #region Camera Movement
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                switch(cameraPositionIndex)
                {
                    case 0: // Left
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x - 0.25f, cameraObject.transform.position.y, cameraObject.transform.position.z);
                        break;

                    case 1: // Up
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y, cameraObject.transform.position.z + 0.25f);
                        break;

                    case 2: // Right
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x + 0.25f, cameraObject.transform.position.y, cameraObject.transform.position.z);
                        break;

                    case 3: // Down
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y, cameraObject.transform.position.z - 0.25f);                       
                        break;
                }
                gsmScript.updateCanvasRotations();
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                switch (cameraPositionIndex)
                {
                    case 0: // Right
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x + 0.25f, cameraObject.transform.position.y, cameraObject.transform.position.z);
                        break;

                    case 1: // Down
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y, cameraObject.transform.position.z - 0.25f);
                        break;

                    case 2: // Left
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x - 0.25f, cameraObject.transform.position.y, cameraObject.transform.position.z);
                        break;

                    case 3: // Up
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y, cameraObject.transform.position.z + 0.25f);                        
                        break;
                }           
                gsmScript.updateCanvasRotations();
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                switch (cameraPositionIndex)
                {
                    case 0: // Up
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y, cameraObject.transform.position.z + 0.25f);
                        break;

                    case 1: // Right
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x + 0.25f, cameraObject.transform.position.y, cameraObject.transform.position.z);
                        break;

                    case 2: // Down
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y, cameraObject.transform.position.z - 0.25f);
                        break;

                    case 3: // Left
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x - 0.25f, cameraObject.transform.position.y, cameraObject.transform.position.z);                       
                        break;
                }                
                gsmScript.updateCanvasRotations();
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                switch (cameraPositionIndex)
                {
                    case 0: // Down
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y, cameraObject.transform.position.z - 0.25f);
                        break;

                    case 1: // Left
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x - 0.25f, cameraObject.transform.position.y, cameraObject.transform.position.z);
                        break;

                    case 2: // Up
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y, cameraObject.transform.position.z + 0.25f);
                        break;

                    case 3: // Right
                        cameraObject.transform.position = new Vector3(cameraObject.transform.position.x + 0.25f, cameraObject.transform.position.y, cameraObject.transform.position.z);
                        break;
                }
                gsmScript.updateCanvasRotations();
            }

            if (Input.GetKeyUp(KeyCode.Home))
            {
                cameraObject.transform.position = selectedPlayerCameraPosition;
                gsmScript.updateCanvasRotations();
            }

            if (Input.GetKeyUp(KeyCode.PageUp))
            {
                cameraPositionIndex++;

                if (cameraPositionIndex > 3)
                {
                    cameraPositionIndex = 0;
                }

                selectedPlayerCameraPosition = cameraPositionArray[cameraPositionIndex];
                cameraObject.transform.position = selectedPlayerCameraPosition;
                cameraObject.transform.Rotate((int)cameraObject.transform.rotation.x, (int)cameraObject.transform.rotation.y + 90, (int)cameraObject.transform.rotation.z, Space.World);
                gsmScript.updateCanvasRotations();
            }

            if (Input.GetKeyUp(KeyCode.PageDown))
            {
                cameraPositionIndex--;
                
                if(cameraPositionIndex < 0)
                {
                    cameraPositionIndex = 3;
                }

                selectedPlayerCameraPosition = cameraPositionArray[cameraPositionIndex];
                cameraObject.transform.position = selectedPlayerCameraPosition;
                cameraObject.transform.Rotate((int)cameraObject.transform.rotation.x, (int)cameraObject.transform.rotation.y - 90, (int)cameraObject.transform.rotation.z, Space.World);
                gsmScript.updateCanvasRotations();
            }
            #endregion
        }
    }

    public void SetupCameraPosition()
    {
        selectedPlayer = selectablePlayers[selectedIndex];

        playerNameText.text = selectedPlayer.name;
        gsmScript.playerAmmoCountText.text = "Ammo: " + selectedPlayer.GetComponent<CharacterBase>().ammunition.ToString();

        cameraPositionArray = new Vector3[] {   new Vector3(selectedPlayer.transform.position.x, selectedPlayer.transform.position.y + 4.5f, selectedPlayer.transform.position.z - 5),
                                                new Vector3(selectedPlayer.transform.position.x - 5, selectedPlayer.transform.position.y + 4.5f, selectedPlayer.transform.position.z),
                                                new Vector3(selectedPlayer.transform.position.x, selectedPlayer.transform.position.y + 4.5f, selectedPlayer.transform.position.z + 5),
                                                new Vector3(selectedPlayer.transform.position.x + 5, selectedPlayer.transform.position.y + 4.5f, selectedPlayer.transform.position.z)   };

        selectedPlayerCameraPosition = cameraPositionArray[cameraPositionIndex];
        cameraObject.transform.position = selectedPlayerCameraPosition;
    }

    public void SelectedPlayerActions(int action)
    {
        if (selectedPlayer.GetComponent<CharacterBase>().actionPips > 0 && gsmScript.gameState == EGameState.playerTurn)
        {
            switch (action)
            {
                case 0:

                    gsmScript.attackPromptUI.SetActive(true);
                    gsmScript.attackPromptText.text = "Fire a shot at " + gsmScript.badEggTargetted.gameObject.name;
                    break;

                case 1:
                    selectedPlayer.GetComponent<CharacterBase>().EnterOverwatchStance();
                    break;

                case 2:
                    selectedPlayer.GetComponent<CharacterBase>().EnterDefenseStance();
                    break;

                case 3:
                    selectedPlayer.GetComponent<CharacterBase>().Reload();                  
                    break;

                case 4:
                    selectedPlayer.GetComponent<CharacterBase>().AttackCharacter(gsmScript.badEggTargetted);
                    gsmScript.attackPromptUI.SetActive(false);
                    break;

                default:
                    break;
            }
        }
    }
}
