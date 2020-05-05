using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeCharacter : EnemyCharacter
{
    private List<LGFlockingAgent> flockAgents = null;
    private bool attackCalculatedOnce = false;
    private float timeElapsedToAttack = 0.0f;
    private float timeElapsedToReturnToHive = 0.0f;

    CharacterBase target = null;

    // Start is called before the first frame update
    public override void AttackCharacter(CharacterBase otherCharacter) 
    {
        target = otherCharacter;
        flockAgents = gameObject.GetComponent<LGFlock>().FlockAgents;
        gsmScript.gameState = EGameState.beeAttack;
        attackCalculatedOnce = false;

        foreach (LGFlockingAgent fa in flockAgents)
        {
            fa.SetObjectToFollow(target.gameObject);
        }

    }
    void Update()
    {
        if (target != null)
        {
            if (gsmScript.gameState == EGameState.beeAttack)
            {
                timeElapsedToAttack += Time.deltaTime;
                // Move to target
                if (AllFlockAgentsAtTarget(target.gameObject) || timeElapsedToAttack > 2.0f)
                {
                    if (!attackCalculatedOnce)
                    {
                        base.AttackCharacter(target);
                        attackCalculatedOnce = true;
                    }

                    foreach (LGFlockingAgent fa in flockAgents)
                    {
                        fa.SetObjectToFollow(gameObject);
                    }

                    timeElapsedToReturnToHive += Time.deltaTime;

                    if (AllFlockAgentsAtTarget(gameObject) || timeElapsedToReturnToHive > 2.0f)
                    {
                        timeElapsedToAttack = 0.0f;
                        timeElapsedToReturnToHive = 0.0f;
                        target = null;
                        gsmScript.gameState = EGameState.enemyTurn;
                        gsmScript.ProcessGameState();
                    }
                }
            }
        }
    }

    public bool AllFlockAgentsAtTarget(GameObject tar)
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
}
