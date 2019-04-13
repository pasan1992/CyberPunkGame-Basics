using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPoint : MonoBehaviour, IPoints
{
    ICyberAgent target;
    bool occupided;
    string occupentsName ="";

    public void setTargetToCover(ICyberAgent target )
    {
        this.target = target;
    }

    public bool canFireToTarget(float maximumFiringDistance)
    {
        bool canFire = maximumFiringDistance > distanceTo(target.getCurrentPosition());
        if (!canFire)
        {
            return false;
        }
        else
        {
            RaycastHit hit;
            string[] layerMaskNames = { "FullCOverObsticles", "Enemy" };
            if (Physics.Raycast(transform.position+ new Vector3(0,0.5f,0), target.getCurrentPosition() - this.transform.position, out hit, maximumFiringDistance, LayerMask.GetMask(layerMaskNames)))
            {
                if(hit.transform.root.name == target.getName())
                {
                    return true;
                }
                else
                {
                   // Debug.Log("Cannot Fire");
                    return false;
                }
            }
            else
            {
               // Debug.Log("Cannot find any");
                return false;
            }
        }
    }

    public bool isSafeFromTarget()
    {
        RaycastHit hit;
        string[] layerMaskNames = { "HalfCoverObsticles", "Enmey" };
        if (Physics.Raycast(transform.position, target.getCurrentPosition() - this.transform.position, out hit,5, LayerMask.GetMask(layerMaskNames)))
        {
            if(hit.transform.tag =="Cover" || hit.transform.tag == "Wall")
            {
                return true;
            }
            else
            {
                return false;
            }

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
        return occupentsName !="";
    }

    void OnDrawGizmos()
    {
        // Gizmos.DrawIcon(transform.position, "cover.png", false);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position+ new Vector3(0,0.3f,0), 0.3f);
    }

    public void stPointOccupentsName(string name)
    {
        this.occupentsName = name;
    }
}
