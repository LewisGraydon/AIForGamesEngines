using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] allEnemies = new GameObject[7];
    private GameObject gsm;
    private GameState gsmScript;

    private bool attackToggle = false;

    // Start is called before the first frame update
    void Start()
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
        if (gsmScript.gameState == EGameState.beeAttack)
        {
            // Move to target
        }

        if (Input.GetKeyUp(KeyCode.Keypad5))
        {
            Attack();
        }
    }

    bool Attack()
    {
        List<LGFlockingAgent> flockAgents = null;
        flockAgents = allEnemies[0].GetComponent<LGFlock>().FlockAgents;
        gsmScript.gameState = EGameState.beeAttack;

        foreach (LGFlockingAgent fa in flockAgents)
        {
            if (!attackToggle)
            {
                GameObject target = ; // This will be replaced by a random enemy within the sight list I would assume.
                fa.SetObjectToFollow(target);
            }
            else
            {
                fa.SetObjectToFollow(allEnemies[0]); // The target for this will be the enemy that is attacking - it may belong in the enemy character granted.
            }
        }
        attackToggle = !attackToggle;
        return true;
    }
}
