using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject[] selectablePlayers = new GameObject[4];
    public int selectedIndex = 0;
    public GameObject cameraObject;

    private GameObject gsm;
    private GameState gsmScript;
    private GameObject selectedPlayer = null;
    private Vector3 selectedPlayerCameraPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            selectablePlayers[i] = (transform.GetChild(i).gameObject);
        }
        selectedPlayer = selectablePlayers[selectedIndex];

        gsm = GameObject.Find("GameStateManager");
        gsmScript = gsm.GetComponent<GameState>();

        selectedPlayerCameraPosition = new Vector3(selectedPlayer.transform.position.x, selectedPlayer.transform.position.y + 4.5f, selectedPlayer.transform.position.z - 5);
        cameraObject.transform.position = selectedPlayerCameraPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (gsmScript.gameState == EGameState.playerTurn)
        {
            if (Input.GetKeyUp(KeyCode.Q))
            {
                selectedIndex -= 1;

                if (selectedIndex < 0)
                {
                    selectedIndex = 3;
                }

                selectedPlayer = selectablePlayers[selectedIndex];
                selectedPlayerCameraPosition = new Vector3(selectedPlayer.transform.position.x, selectedPlayer.transform.position.y + 4.5f, selectedPlayer.transform.position.z - 5);
                cameraObject.transform.position = selectedPlayerCameraPosition;
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                selectedIndex += 1;

                if (selectedIndex > 3)
                {
                    selectedIndex = 0;
                }

                selectedPlayer = selectablePlayers[selectedIndex];
                selectedPlayerCameraPosition = new Vector3(selectedPlayer.transform.position.x, selectedPlayer.transform.position.y + 4.5f, selectedPlayer.transform.position.z - 5);
                cameraObject.transform.position = selectedPlayerCameraPosition;
            }
         

            // Camera Movement
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                cameraObject.transform.position = new Vector3(cameraObject.transform.position.x - 0.25f, cameraObject.transform.position.y, cameraObject.transform.position.z);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                cameraObject.transform.position = new Vector3(cameraObject.transform.position.x + 0.25f, cameraObject.transform.position.y, cameraObject.transform.position.z);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y, cameraObject.transform.position.z + 0.25f);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, cameraObject.transform.position.y, cameraObject.transform.position.z - 0.25f);
            }

            if (Input.GetKeyUp(KeyCode.Home))
            {
                cameraObject.transform.position = selectedPlayerCameraPosition;
            }
        }
    }
}
