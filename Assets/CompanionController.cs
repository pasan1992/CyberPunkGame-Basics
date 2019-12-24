using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionController : AgentController
{
    private ICyberAgent m_companionAgent;
    private Vector3 m_currentMovmentVector = Vector3.zero;
    private NavMeshAgent navMeshAgent;


    public void MoveToPosition(Vector3 position)
    {
        if(position != Vector3.zero && m_currentMovmentVector != position && Vector3.Distance(position,m_currentMovmentVector) > 1)
        {
            m_currentMovmentVector = position;
            navMeshAgent.SetDestination(m_currentMovmentVector);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_companionAgent = GetComponent<ICyberAgent>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        handleNavigation();
    }

    private void handleNavigation()
    {
        Vector3 velocity = navMeshAgent.desiredVelocity;

        if (true)
        {
            velocity = velocity.normalized;
        }
        else
        {
            velocity = velocity * 2.2f;
        }

        velocity = new Vector3(velocity.x, 0, velocity.z);
        m_companionAgent.moveCharacter(velocity);
    }

    private void updatePlayerInput()
    {
        
    }
    public override ICyberAgent getICyberAgent()
    {
        throw new System.NotImplementedException();
    }

    public override float getSkill()
    {
        throw new System.NotImplementedException();
    }

    public override void onAgentDisable()
    {
        throw new System.NotImplementedException();
    }

    public override void onAgentEnable()
    {
        throw new System.NotImplementedException();
    }

    public override void resetCharacher()
    {
        throw new System.NotImplementedException();
    }

    public override void setMovableAgent(ICyberAgent agent)
    {
        throw new System.NotImplementedException();
    }

    public override void setPosition(Vector3 postion)
    {
        throw new System.NotImplementedException();
    }
}
