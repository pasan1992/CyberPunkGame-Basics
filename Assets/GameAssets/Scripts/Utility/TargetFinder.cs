using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour {

    private List<Collider>  targets = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag =="Enemy")
        {
            targets.Add(other);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Enemy")
        {
            targets.Remove(other);
        }
    }

    public Vector3 getCurrentTarget(Vector3 position)
    {
        Collider currentTarget =null;
        float MinDistance  =999;

        foreach( Collider target in targets)
        {
            float distance = Vector3.Distance(position , target.transform.position);
            if(distance < MinDistance)
            {
                MinDistance = distance;
                currentTarget = target;
            }
        }

        if(currentTarget !=null)
        {
            return currentTarget.transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
