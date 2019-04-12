using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPoint : MonoBehaviour, IPoints
{
    ICyberAgent target;
    public LayerMask mask;
    bool occupided;

    public void setTargetToCover(ICyberAgent target )
    {
        this.target = target;
    }

    public bool canFireToTarget()
    {
        return true;
    }

    public bool isSafeFromTarget()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, target.getCurrentPosition() - this.transform.position, out hit,5, mask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float distanceTo(Vector3 distanceFrom)
    {
        return Vector3.Distance(this.transform.position, distanceFrom);
    }

    public Vector3 getPosition()
    {
       return this.transform.position;
    }

    public bool isOccupied()
    {
        return occupided;
    }

    void OnDrawGizmos()
    {
        // Gizmos.DrawIcon(transform.position, "cover.png", false);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position+ new Vector3(0,0.3f,0), 0.3f);
    }
}
