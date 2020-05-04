using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VectorChecker : MonoBehaviour
{

    public GameObject otherFuckingCube;
    public float coverThreshhold = 0.44f;

    //public Dictionary<ActionID, int> testBob = new Dictionary<ActionID, int>();

    int peter = 0;

    int _graham = 0;
    int graham
    {
        get => _graham;
        set
        {
            _graham = Mathf.Clamp(value, 0, 2);
            peter = _graham * 10;
        }
    }

    int richard
    {
        get => 6;
    }

    // Start is called before the first frame update
    void Start()
    {
        //for(ActionID i = ActionID.Move; i <= ActionID.Reload; i++)
        //{
        //    testBob.Add(i, (int)i * UnityEngine.Random.Range(0, 10));
        //}
        //Debug.Log("Unordered");
        //foreach (KeyValuePair<ActionID, int> kp in testBob)
        //{
        //    Debug.Log("Key: " + kp.Key.ToString() + ", Value: " + kp.Value);
        //}
        //testBob = (Dictionary<ActionID, int>)testBob.OrderBy(key => key.Value);
        //Debug.Log("Ordered");
        //foreach (KeyValuePair<ActionID, int> kp in testBob)
        //{
        //    Debug.Log("Key: " + kp.Key.ToString() + ", Value: " + kp.Value);
        //}
        graham += 12;
        Debug.Log("Graham: " + graham + " , peter: " + peter);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            Vector3 normaldirection = (otherFuckingCube.transform.position - this.transform.position).normalized;

            if (normaldirection.x >= coverThreshhold)
            {
                print("Should count cover from x (East?) as normalDirection.x: " + normaldirection.x);
            }
            else if (normaldirection.x <= -coverThreshhold)
            {
                print("Should count cover from -x (West?) as normalDirection.x: " + normaldirection.x);
            }
            else
            {
                print("Should Ignore cover from East or West as normalDirection.x: " + normaldirection.x);
            }
            if (normaldirection.z >= coverThreshhold)
            {
                print("Should count cover from -z (South?) as normalDirection.z: " + normaldirection.z);
            }
            else if (normaldirection.z <= -coverThreshhold)
            {
                print("Should count cover from z (North?) as normalDirection.z: " + normaldirection.z);
            }
            else
            {
                print("Should Ignore cover from North or South as normalDirection.z: " + normaldirection.z);
            }

        }

        if(Input.GetKeyUp(KeyCode.LeftArrow))
        {

            this.transform.position += Vector3.left;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            this.transform.position += Vector3.right;

        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            this.transform.position += Vector3.forward;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            this.transform.position += Vector3.back;
        }
    }


    // all testBed functions relate to understanding manipulating the action consideration list that then will be able to be repurposed on teh movmentConsiderations as well.
    void testBed()
    {
        List<ActionConsideration> conList = new List<ActionConsideration>()
        {
            new ReloadConsideration(),
            new MoveConsideration(),
            new ReloadConsideration(),
            new DefendConsideration(),
            new ReloadConsideration(),
            new ShootConsideration(),
            new OverwatchConsideration()
        };

        conList.Sort();

        foreach (ActionConsideration con in conList)
        {
            Debug.Log(con);
            //Debug.Log(con is SingleEnemyActionConsideration);
        }
    }

    public static List<ActionConsideration> actionConsiderationList = new List<ActionConsideration>
    {
        new  MoveConsideration(),
        new  ReloadConsideration(),
        new  ShootConsideration(),
        new  OverwatchConsideration(),
        new  DefendConsideration()

    };


    //void testBed2()
    //{
    //    for (int i = 0; i < actionConsiderationList.Count; i++)
    //    {
    //        actionConsiderationList[i].DANGEROUSDEBUGSETACTIONVALUE(UnityEngine.Random.Range(0, 1000));
    //    }
    //    actionConsiderationList.Sort((ActionConsideration val1, ActionConsideration val2) =>
    //    {
    //        return val1.CompareTo(val2);
    //    });
    //    foreach(ActionConsideration ac in actionConsiderationList)
    //    {
    //        Debug.Log(ac + ": " + ac.actionValue);
    //    }
    //    Debug.Log("--------------------------------------");
    //    actionConsiderationList.Sort((ActionConsideration val1, ActionConsideration val2) =>
    //    {
    //        return val2.actionValue.CompareTo(val1.actionValue);
    //    });
    //    foreach (ActionConsideration ac in actionConsiderationList)
    //    {
    //        Debug.Log(ac + ": " + ac.actionValue);
    //    }
    //}

    //void testBed3()
    //{
    //    actionConsiderationList.Sort((ActionConsideration val1, ActionConsideration val2) =>
    //    {
    //        return val1.CompareTo(val2);
    //    });
    //    for (int i = 0; i < actionConsiderationList.Count; i++)
    //    {
    //        for (int enemyCounter = 0; enemyCounter < 3; enemyCounter++)
    //        {
    //            int startI = i;
    //            Debug.Log("Enemy number: " + enemyCounter);
    //            while (i < actionConsiderationList.Count && actionConsiderationList[i] is SingleEnemyActionConsideration)
    //            {
    //                int valueToAdd = UnityEngine.Random.Range(-1000, 1000);
    //                Debug.Log(actionConsiderationList[i] + ", Enemy Number- " + enemyCounter + ": " + actionConsiderationList[i].actionValue + " + " + valueToAdd + "= " + (actionConsiderationList[i].actionValue + valueToAdd));
    //                (actionConsiderationList[i] as SingleEnemyActionConsideration).DANGEROUSDEBUGSETACTIONVALUE(actionConsiderationList[i].actionValue + valueToAdd);
    //                i++;
    //            }                
    //            i = enemyCounter == (3 - 1) ? i + 1 : startI;
    //        }
    //        Debug.Log("END of SINGLEENEMYACTIONS ----------------");
    //        if(i < actionConsiderationList.Count && actionConsiderationList[i] is NoEnemyActionConsideration)
    //        {
    //            int valueToAdd = UnityEngine.Random.Range(0, 3000);
    //            Debug.Log(actionConsiderationList[i] + ": " + actionConsiderationList[i].actionValue + " + " + valueToAdd + "= " + (actionConsiderationList[i].actionValue + valueToAdd));
    //            actionConsiderationList[i].DANGEROUSDEBUGSETACTIONVALUE(actionConsiderationList[i].actionValue + valueToAdd);
    //        }
    //    }
    //    Debug.Log("FINISHED CALCULATING: Final List Below");
    //    actionConsiderationList.Sort((ActionConsideration val1, ActionConsideration val2) =>
    //    {
    //        return val2.actionValue.CompareTo(val1.actionValue);
    //    });
    //    EnemyCharacter e = new EnemyCharacter();
    //    foreach (ActionConsideration ac in actionConsiderationList)
    //    {
    //        Debug.Log(ac + ": " + ac.actionValue);
    //        ac.Enact(e);
    //    }
    //    //CharacterBase c = new EnemyCharacter();
    //    //int randomisingNum = UnityEngine.Random.Range(0,1);
    //    //Debug.Log("randomisingNum: " + randomisingNum);
    //    //actionConsiderationList[randomisingNum].Enact(c);
    //}

    //void testBed4()
    //{
    //    EnemyCharacter e = gameObject.AddComponent<EnemyCharacter>();
    //    //ConsiderationLists.DEBUGMakeDecision(e);
    //}
}
