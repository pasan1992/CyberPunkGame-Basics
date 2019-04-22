using UnityEngine;

public class CoverPoint : MonoBehaviour, IPoints
{
    ICyberAgent target;
    bool occupided;
    string occupentsName ="";
    ICyberAgent occupent;

    public void setTargetToCover(ICyberAgent target )
    {
        this.target = target;
    }

    //public void Update()
    //{
    //    if(occupent !=null)
    //    {
    //        Vector3 tempDirection = target.getTopPosition() - this.transform.position - new Vector3(0, 2f, 0);
    //        Debug.DrawLine(transform.position + new Vector3(0, 2f, 0), tempDirection * 30, Color.green);
    //    }


    //}

    public bool canFireToTarget(float maximumFiringDistance)
    {
        bool canFire = maximumFiringDistance > distanceTo(target.getCurrentPosition());
        if (!canFire)
        {
            //Debug.Log("Distance Fail");
            return false;
        }
        else
        {
            RaycastHit hit;
            string[] layerMaskNames = { "FullCOverObsticles", "Enemy" };

           // Vector3 tempDirection = target.getCurrentPosition() - this.transform.position - new Vector3(0, 1.5f, 0);
            

            if (Physics.Raycast(transform.position+ new Vector3(0,2f,0), target.getTopPosition() - this.transform.position - new Vector3(0, 2f, 0), out hit, maximumFiringDistance, LayerMask.GetMask(layerMaskNames)))
            {
                if(hit.transform.root.name == target.getName())
                {
                    //Debug.Log("Can fire target" + hit.transform.name);
                    return true;
                }
                else
                {
                    //Debug.Log("cannot fire target, hit wall or self");
                    return false;
                }
            }
            else
            {
                //Debug.Log("Nothing to hit");
                return false;
            }
        }
    }

    public bool isSafeFromTarget()
    {
        RaycastHit hit;
        string[] layerMaskNames = { "HalfCoverObsticles", "Enmey" };
        if (Physics.Raycast(transform.position, target.getCurrentPosition() - this.transform.position, out hit,2, LayerMask.GetMask(layerMaskNames)))
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

        if(occupent != null)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.blue;
        }

        Gizmos.DrawSphere(transform.position+ new Vector3(0,0.3f,0), 0.3f);

        if(occupent !=null)
        {
            Gizmos.DrawLine(this.transform.position, occupent.getCurrentPosition());
        }

    }

    public void stPointOccupentsName(string name)
    {
        this.occupentsName = name;
    }

    public void setOccupent(ICyberAgent agent)
    {
        this.occupent = agent;
    }
}
