using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypontMovementStage : BasicMovmentStage
{
    protected BasicWaypoint[] m_wayPoints;

    public enum GuardStages {MovingToWayPoint,AtWayPoint}
    private GuardStages m_currentStage = GuardStages.AtWayPoint;
    protected int m_currentWayPointID = 0;

    public WaypontMovementStage(ICyberAgent selfAgent,NavMeshAgent navMeshAgent, BasicWaypoint[] wayPoints):base(selfAgent,navMeshAgent)
    {
        m_wayPoints = wayPoints;
    }
    public override void setTargets(ICyberAgent target)
    {
        
    }

    protected override void stepUpdate()
    {
        switch (m_currentStage)
        {
            case GuardStages.MovingToWayPoint:
            if(m_navMeshAgent.remainingDistance < 1)
            {
                m_currentStage = GuardStages.AtWayPoint;  
            }
            break;
            case GuardStages.AtWayPoint: 
            //m_navMeshAgent.SetDestination(getNextWaypoint().getPosition());
            MoveToWaypoint(getNextWaypoint());   
            m_currentStage = GuardStages.MovingToWayPoint;     
            break;
        }
    }

    protected virtual BasicWaypoint getNextWaypoint()
    {
        m_currentWayPointID++;

        if(m_currentWayPointID == m_wayPoints.Length)
        {
            m_currentWayPointID = -m_wayPoints.Length +1;
        }

        if(m_currentWayPointID < 0)
        {
            m_currentWayPointID = - m_currentWayPointID;
            return  m_wayPoints[m_currentWayPointID];
        }
        return m_wayPoints[m_currentWayPointID];
    }

    protected void MoveToWaypoint(BasicWaypoint waypoint)
    {
        m_navMeshAgent.SetDestination(waypoint.getPosition()); 
    }
}
