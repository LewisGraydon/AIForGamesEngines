using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] allEnemies = new GameObject[7];
    public List<EnemyCharacter> overwatchingEnemies = new List<EnemyCharacter>();


    private GameObject gsm;
    private GameState gsmScript;

    private Stack<EnemyCharacter> EvilDoerStack = new Stack<EnemyCharacter>();
    public EnemyCharacter activeCharacter;

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            allEnemies[i] = (transform.GetChild(i).gameObject);
        }

        gsm = GameObject.Find("GameStateManager");
        gsmScript = gsm.GetComponent<GameState>();
    }

    // Update is called once per frame
    void Update()
    {       
        if(Input.GetKeyUp(KeyCode.F1))
        {
            allEnemies[0].GetComponent<BeeCharacter>().AttackCharacter(FindObjectOfType<PlayerCharacter>());
        }

        if(gsmScript.gameState == EGameState.enemyTurn)
        {
            if (activeCharacter != null)
            {
                if (activeCharacter.actionPips != 0)
                {
                    activeCharacter.FindSightline();
                    activeCharacter.MakeDecision();
                }
                else
                {
                    if (EvilDoerStack.Count > 0)
                        activeCharacter = EvilDoerStack.Pop();
                    else
                        activeCharacter = null;
                }
            }
            else
            {
                gsmScript.gameState = EGameState.setupState;
                gsmScript.ProcessGameState();
            }
        }
    }

    public void SetUpEnemyTurn()
    {
        EnemyCharacter[] e = GameObject.FindObjectsOfType<EnemyCharacter>();
        foreach (EnemyCharacter eC in e)
        {
            EvilDoerStack.Push(eC);
        }
        if(EvilDoerStack.Count > 0)
            activeCharacter = EvilDoerStack.Pop();
    }
}
