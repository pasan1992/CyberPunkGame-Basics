using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWaypoint : MonoBehaviour,IPoints
{
    ICyberAgent m_currentOccupent = null;


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

        Gizmos.DrawCube(transform.position + new Vector3(0, 0.3f, 0),new Vector3(0.4f,0.4f,0.4f));
    }
}
