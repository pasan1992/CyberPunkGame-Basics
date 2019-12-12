using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointRutine : MonoBehaviour
{
    public List<BasicWaypoint> m_wayPoints ;

    public GameObject prefab;

    private static Vector3 lineOffset = new Vector3(0,0.5f,0);

    public void Start()
    {
        prefab = Resources.Load<GameObject>("Prefab/BasicWaypoint");
        List<BasicWaypoint> m_wayPoints = new List<BasicWaypoint>();
    }
    public void createWaypoint()
    {
        if(prefab)
        {
            GameObject temp = GameObject.Instantiate(prefab,Vector3.zero,Quaternion.identity);
            temp.name = this.name + "_waypoint_"+ m_wayPoints.Count.ToString();

            int count = m_wayPoints.Count;
            if(count == 0)
            {
                temp.transform.position = this.transform.position + Vector3.one;
            }
            else
            {
                temp.transform.position = m_wayPoints[count -1].transform.position + Vector3.left;
            }

            m_wayPoints.Add(temp.GetComponent<BasicWaypoint>());
           // temp.transform.parent = this.transform;
        }
    }

    public void removeWaypoint()
    {
        BasicWaypoint temp = m_wayPoints[m_wayPoints.Count -1];
        m_wayPoints.Remove(temp);
        DestroyImmediate(temp);
    }

    void OnDrawGizmos()
    {
        BasicWaypoint previousPoint = null;
        Gizmos.color = Color.blue;
        foreach (BasicWaypoint point in m_wayPoints)
        {
            if( previousPoint != null)
            {
                Gizmos.DrawLine(previousPoint.transform.position + lineOffset, point.transform.position + lineOffset);             
                previousPoint = point;
            }
            else
            {
                previousPoint = point;
                Gizmos.DrawSphere(previousPoint.transform.position,0.5f);
            }
        }
    }
}
