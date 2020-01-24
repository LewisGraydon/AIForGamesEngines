using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Filter/PhysicsLayer")]
public class PhysicsLayerFilter : ContextFilter
{
    public LayerMask mask;

    public override List<Transform> Filter(FlockAgent flockAgent, List<Transform> original)
    {
        List<Transform> filteredList = new List<Transform>();
        foreach (Transform item in original)
        {
            if(mask == (mask |(1 << item.gameObject.layer)))
            {
                filteredList.Add(item);
            }
        }
        return filteredList;
    }
}
