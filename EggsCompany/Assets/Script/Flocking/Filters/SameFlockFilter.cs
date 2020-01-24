using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Filter/SameFlock")]
public class SameFlockFilter : ContextFilter
{
    public override List<Transform> Filter(FlockAgent flockAgent, List<Transform> original)
    {
        List<Transform> filteredList = new List<Transform>();
        foreach(Transform item in original)
        {
            FlockAgent itemAgent = item.GetComponent<FlockAgent>();
            if(itemAgent != null && itemAgent.GetAgentFlock == flockAgent.GetAgentFlock)
            {
                filteredList.Add(item);
            }
        }
        return filteredList;
    }
}
