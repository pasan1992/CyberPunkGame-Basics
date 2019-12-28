using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWaypoint : MonoBehaviour,IPoints
{
    ICyberAgent m_currentOccupent = null;
    private List<BasicWaypoint> m_connectedWayPoints;

    #region Getters and Setters
    public Vector3 getPosition()
    {
        return this.transform.position;
    }

    public bool isOccupied()
    {
        return m_currentOccupent != null;
    }

    public void setOccupent(ICyberAgent agent)
    {
        m_currentOccupent = agent;
    }

    #endregion

    void OnDrawGizmos()
    {
        if (isOccupied())
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, m_currentOccupent.getCurrentPosition());
        }
        else
        {
            Gizmos.color = Color.cyan;
        }

        Gizmos.DrawCube(transform.position + new Vector3(0, 0.8f, 0),new Vector3(0.4f,0.4f,0.4f));
        Gizmos.DrawLine(this.transform.position,this.transform.position + Vector3.up*0.8f);
    }
}
