using UnityEngine;

public class CoverPoint : MonoBehaviour, IPoints
{
    ICyberAgent target;
    ICyberAgent occupent;

    #region Getters and Setters
    public void setTargetToCover(ICyberAgent target )
    {
        this.target = target;
    }

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
            string[] layerMaskNames = { "FullCOverObsticles"};

            if (Physics.Raycast(transform.position + new Vector3(0, 2f, 0), target.getTopPosition() - this.transform.position - new Vector3(0, 2f, 0), out hit, maximumFiringDistance, LayerMask.GetMask(layerMaskNames)))
            {
                return false;
            }
            else
            {
                //Debug.Log("Nothing to hit");
                return true;
            }
        }
    }

    public bool isSafeFromTarget()
    {
        RaycastHit hit;
        string[] layerMaskNames = { "HalfCoverObsticles" };
        if (Physics.Raycast(transform.position, target.getCurrentPosition() - this.transform.position, out hit,1, LayerMask.GetMask(layerMaskNames)))
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
        return false;
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
        return occupent != null;
    }

    public void setOccupent(ICyberAgent agent)
    {
        if(agent == null)
        {
            Invoke("SetOccupentNull",2);
        }
        else
        {
            this.occupent = agent;
        }
    }

    private void SetOccupentNull()
    {
        this.occupent = null;
        CancelInvoke();
    }
    #endregion

    #region editor
    void OnDrawGizmos()
    {
        if(isOccupied())
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(this.transform.position, occupent.getCurrentPosition());
        }
        else
        {
            Gizmos.color = Color.gray;
        }

        Gizmos.DrawSphere(transform.position+ new Vector3(0,0.3f,0), 0.3f);
        //Gizmos.DrawCube(transform.position + new Vector3(0, 0.3f, 0), new Vector3(0.6f, 0.6f, 0.6f));
    }
    #endregion

}
