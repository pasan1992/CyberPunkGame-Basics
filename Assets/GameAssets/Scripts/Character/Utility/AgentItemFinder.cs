using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentItemFinder
{
    public static Interactable findNearItem(Vector3 currentPosition)
    {
        Collider[] nearObjects = Physics.OverlapSphere(currentPosition, 1.4f);
        List<Interactable> pickableObjects = new List<Interactable>();

        foreach(Collider nearObject in nearObjects)
        {
            Interactable interactableObject = nearObject.GetComponent<Interactable>();

            if(interactableObject && interactableObject.properties.interactionEnabled && nearObject.tag == "Item")
            {
                pickableObjects.Add(interactableObject);
            }
        }
        return findTheNearst(pickableObjects,currentPosition);
    }

    private static Interactable findTheNearst(List<Interactable> pickableObjects, Vector3 currentPosition)
    {
        float minmumDistance = float.MaxValue;
        Interactable nearsetObject = null;

        foreach(Interactable pickableObject in pickableObjects)
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
