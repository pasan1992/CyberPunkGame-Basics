using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentItemFinder
{
    public static GameObject findNearItem(Vector3 currentPosition)
    {
        Collider[] nearObjects = Physics.OverlapSphere(currentPosition, 1);
        List<GameObject> pickableObjects = new List<GameObject>();

        foreach(Collider nearObject in nearObjects)
        {
            if(nearObject.tag == "Item")
            {
                pickableObjects.Add(nearObject.gameObject);
            }
        }
        return findTheNearst(pickableObjects,currentPosition);
    }

    private static GameObject findTheNearst(List<GameObject> pickableObjects, Vector3 currentPosition)
    {
        float minmumDistance = float.MaxValue;
        GameObject nearsetObject = null;

        foreach(GameObject pickableObject in pickableObjects)
        {
            float distance = Vector3.Distance(currentPosition,pickableObject.transform.position);
            if(minmumDistance > distance)
            {
                minmumDistance = distance;
                nearsetObject = pickableObject;
            }
        }

        return nearsetObject;
    }
}
