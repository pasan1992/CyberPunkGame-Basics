using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IteractionStage : WaypontMovementStage
{
    // Start is called before the first frame update
    ICyberAgent  m_agent;

    enum IterationState {MovintToPoint,OnPoint,Interaction,InteractioOver};

    IterationState m_currentIteractionState = IterationState.MovintToPoint;


    public IteractionStage(ICyberAgent selfAgent,NavMeshAgent navMeshAgent, BasicWaypoint[] wayPoints):base(selfAgent,navMeshAgent,wayPoints)
    {
        m_agent = selfAgent;
    }
    void Start()
    {
        
    }

    public override void setTargets(ICyberAgent target)
    {
        m_agent = target;
    }

    protected override void stepUpdate()
    {
        switch (m_currentIteractionState)
        {
            case IterationState.Interaction:

                if(!m_agent.isInteracting())
                {
                    m_currentIteractionState = IterationState.InteractioOver;
                }

            break;
            case IterationState.InteractioOver:
                m_currentIteractionState = IterationState.MovintToPoint;    
                m_navMeshAgent.isStopped = false;  
                MoveToWaypoint(getNextWaypoint());            
            break;
            case IterationState.MovintToPoint:
                if(!m_navMeshAgent.pathPending && m_navMeshAgent.remainingDistance < 1.5f)
                {
                    m_navMeshAgent.isStopped = true;
                    m_navMeshAgent.velocity = Vector3.zero;
                    m_currentIteractionState = IterationState.OnPoint;  
                }
            break;
            case IterationState.OnPoint:
                Interactable interactableObject = m_wayPoints[m_currentWayPointID].GetComponent<Interactable>();

                if(interactableObject)
                {
                    m_currentIteractionState = IterationState.Interaction;
                    StartInteraction(interactableObject);
                }
                else
                {
                    m_currentIteractionState = IterationState.MovintToPoint;
                    m_navMeshAgent.isStopped = false;
                    MoveToWaypoint(getNextWaypoint());   
                }
            break;
        }
    }

    private void StartInteraction(Interactable interactableObj)
    {
        m_agent.interactWith(interactableObj);
    }

    protected override BasicWaypoint getNextWaypoint()
    {
        m_currentWayPointID++;

        if(m_currentWayPointID == m_wayPoints.Length)
        {
            m_currentWayPointID = 0;
        }
        return m_wayPoints[m_currentWayPointID];
    }
}
