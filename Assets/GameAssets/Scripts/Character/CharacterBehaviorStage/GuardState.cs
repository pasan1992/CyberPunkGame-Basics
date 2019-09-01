using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardState :BasicMovmentStage
{
    BasicWaypoint[] m_wayPoints;
    BasicWaypoint m_currentWayPoint;

    public enum GuardStages {MovingToWayPoint,AtWayPoint}
    private GuardStages m_currentStage = GuardStages.AtWayPoint;
    private int m_currentWayPointID = 0;

    public GuardState(ICyberAgent selfAgent,NavMeshAgent navMeshAgent, BasicWaypoint[] wayPoints):base(selfAgent,navMeshAgent)
    {
        m_wayPoints = wayPoints;
        m_currentWayPoint = wayPoints[0];
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
            m_navMeshAgent.SetDestination(getNextWaypoint().getPosition());   
            m_currentStage = GuardStages.MovingToWayPoint;     
            break;
        }
    }

    private BasicWaypoint getNextWaypoint()
    {
        m_currentWayPointID++;

        if(m_currentWayPointID == m_wayPoints.Length)
        {
            m_currentWayPointID = -m_wayPoints.Length +1;
        }

        if(m_currentWayPointID <0)
        {
            return  m_wayPoints[-m_currentWayPointID];
        }
        return m_wayPoints[m_currentWayPointID];
    }
}
