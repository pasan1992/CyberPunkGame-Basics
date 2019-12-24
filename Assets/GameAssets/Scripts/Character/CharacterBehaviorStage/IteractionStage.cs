using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IteractionStage : WaypontMovementStage
{
    // Start is called before the first frame update
    ICyberAgent  m_agent;

    enum IterationState {MovintToPoint,OnPoint,Interaction,InteractionOver};

    IterationState m_currentIteractionState = IterationState.InteractionOver;


    public IteractionStage(ICyberAgent selfAgent,NavMeshAgent navMeshAgent, BasicWaypoint[] wayPoints):base(selfAgent,navMeshAgent,wayPoints)
    {
        m_agent = selfAgent;
        m_currentIteractionState = IterationState.InteractionOver;
    }
    void Start()
    {
        
    }

    public override void setTargets(ICyberAgent target)
    {
        m_agent = target;
    }

    public override void initalizeStage()
    {
        m_currentIteractionState = IterationState.MovintToPoint;
        m_navMeshAgent.isStopped = false;
        MoveToWaypoint(getNextWaypoint());         
    }

    protected override void stepUpdate()
    {
        switch (m_currentIteractionState)
        {
            case IterationState.Interaction:

                if(!m_agent.isInteracting())
                {
                    m_currentIteractionState = IterationState.InteractionOver;
                }

            break;
            case IterationState.InteractionOver:
                 m_currentIteractionState = IterationState.MovintToPoint; 
                 m_navMeshAgent.enabled = true;   
                 m_navMeshAgent.isStopped = false;  
                MoveToWaypoint(getNextWaypoint());            
            break;
            case IterationState.MovintToPoint:

                if(!m_navMeshAgent.pathPending && 
                   m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance && 
                  (m_navMeshAgent.hasPath || m_navMeshAgent.velocity.sqrMagnitude == 0f))
                {
                    m_navMeshAgent.isStopped = true;
                    m_navMeshAgent.velocity = Vector3.zero;
                    m_currentIteractionState = IterationState.OnPoint;  
                }
                
            break;
            case IterationState.OnPoint:
                Interactable interactableObject = m_wayPoints[m_currentWayPointID].GetComponent<Interactable>();

                if(interactableObject && !interactableObject.isInteracting())
                {
                    m_currentIteractionState = IterationState.Interaction;
                    m_navMeshAgent.enabled = false;
                    StartInteraction(interactableObject,Interactable.InteractableProperties.InteractableType.TimedInteraction);
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

    private void StartInteraction(Interactable interactableObj,Interactable.InteractableProperties.InteractableType type)
    {
        m_agent.interactWith(interactableObj,type);
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
