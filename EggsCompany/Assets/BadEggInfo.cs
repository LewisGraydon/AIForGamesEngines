using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BadEggInfo : MonoBehaviour, IPointerDownHandler
{
    public GameObject badEgg;
    public Text hitChanceText = null;
    private GameObject cameraObject;
    private GameState gsmScript;

    public void OnPointerDown(PointerEventData eventData)
    {
        int i = GameObject.Find("Players").GetComponent<PlayerManager>().cameraPositionIndex;

        Vector3[] cameraPositionArray = new Vector3[] {   new Vector3(badEgg.transform.position.x, badEgg.transform.position.y + 4f, badEgg.transform.position.z - 5),
                                                new Vector3(badEgg.transform.position.x - 5, badEgg.transform.position.y + 4f, badEgg.transform.position.z),
                                                new Vector3(badEgg.transform.position.x, badEgg.transform.position.y + 4f, badEgg.transform.position.z + 5),
                                                new Vector3(badEgg.transform.position.x + 5, badEgg.transform.position.y + 4f, badEgg.transform.position.z)   };

        cameraObject.transform.position = cameraPositionArray[i];
        gsmScript.GetComponent<GameState>().badEggTargetted = badEgg.GetComponent<EnemyCharacter>();

        if(gsmScript.attackPromptUI.gameObject.activeSelf)
        {
            gsmScript.attackPromptText.text = "Fire a shot at " + gsmScript.badEggTargetted.gameObject.name;
        }
    }


    // Start is called before the first frame update
    void Awake()
    {
        gsmScript = GameObject.Find("GameStateManager").GetComponent<GameState>();
        cameraObject = GameObject.Find("Main Camera");
        hitChanceText = gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
