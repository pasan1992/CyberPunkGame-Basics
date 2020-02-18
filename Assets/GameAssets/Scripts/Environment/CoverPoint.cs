using UnityEngine;
using System.Collections.Generic;
public class CoverPoint : MonoBehaviour, IPoints
{
    ICyberAgent target;
    ICyberAgent occupent;

     SortedDictionary<float, CoverPoint> cp_map;

    public SortedDictionary<float, CoverPoint> Cp_map { get => cp_map; set => cp_map = value; }

    #region Getters and Setters
    public void Awake()
    {
        this.gameObject.AddComponent<SphereCollider>().isTrigger = true;
    }

    public void Start()
    {
        Cp_map = CoverPointsManager.getDistanceMap(this);
    }

    public void setTargetToCover(ICyberAgent target )
    {
        this.target = target;
    }

    public bool canFireToTarget(float maximumFiringDistance)
    {
      return   canFireToTarget(maximumFiringDistance,false);
    }

    public bool canFireToTarget(float maximumFiringDistance,bool useCover)
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
            string[] layerMaskNames = null;

            if(useCover)
            {
                layerMaskNames = new string[]{"FullCOverObsticles","HalfCoverObsticles"};
            }
            else
            {
                layerMaskNames =new string[] { "FullCOverObsticles"};
            }
            
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
        if(target == null)
        {
            return false;
        }
        
        RaycastHit hit;
        string[] layerMaskNames = { "HalfCoverObsticles" };
        if (Physics.Raycast(transform.position + new Vector3(0,0.5f,0), target.getCurrentPosition() - this.transform.position -new Vector3(0,0.5f,0), out hit,5, LayerMask.GetMask(layerMaskNames)))
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
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, occupent.getCurrentPosition());
        }
        else
        {
            Gizmos.color = Color.black;
        }

        Gizmos.DrawSphere(transform.position+ new Vector3(0,0.3f,0), 0.3f);
        //Gizmos.DrawCube(transform.position + new Vector3(0, 0.3f, 0), new Vector3(0.6f, 0.6f, 0.6f));
    }
    #endregion

}
