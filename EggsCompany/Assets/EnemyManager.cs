using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] allEnemies = new GameObject[7];
    private GameObject gsm;
    private GameState gsmScript;

    private List<LGFlockingAgent> flockAgents = null;
    private GameObject target = null;
    private bool damageDealt = false;
    private GameObject attackingEnemy = null;

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

        attackingEnemy = allEnemies[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (gsmScript.gameState == EGameState.beeAttack)
        {
            // Move to target
            if(AllFlockAgentsAtTarget(target))
            {
                if (!damageDealt)
                {
                    target.GetComponent<CharacterBase>().health -= 2;
                    damageDealt = true;                   
                }

                foreach (LGFlockingAgent fa in flockAgents)
                {
                    fa.SetObjectToFollow(attackingEnemy);
                }

                if (AllFlockAgentsAtTarget(attackingEnemy))
                {
                    attackingEnemy.GetComponent<EnemyCharacter>().actionPips--;
                    gsmScript.gameState = EGameState.enemyTurn;                  
                    gsmScript.ProcessGameState();
                }
            }
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

        if (Input.GetKeyUp(KeyCode.Keypad5))
        {
            Attack();
        }
    }

    void Attack()
    {     
        flockAgents = attackingEnemy.GetComponent<LGFlock>().FlockAgents;
        gsmScript.gameState = EGameState.beeAttack;
        damageDealt = false;
        target = GameObject.Find("Players").GetComponent<PlayerManager>().selectedPlayer; // This will be replaced by a random enemy within the sight list I would assume.

        foreach (LGFlockingAgent fa in flockAgents)
        {            
            fa.SetObjectToFollow(target);
        }
    }

    bool AllFlockAgentsAtTarget(GameObject tar)
    {
        foreach (LGFlockingAgent fa in flockAgents)
        {
            if (Mathf.Abs(fa.transform.position.x - tar.transform.position.x) > 0.5f || Mathf.Abs(fa.transform.position.z - tar.transform.position.z) > 0.5f)
            {
                return false;
            }
        }
        return true;
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
