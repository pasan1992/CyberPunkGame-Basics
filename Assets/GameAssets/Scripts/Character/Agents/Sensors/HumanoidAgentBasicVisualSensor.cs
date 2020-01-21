using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAgentBasicVisualSensor : AgentBasicSensor
{
    HumanoidMovingAgent m_agent;
    GameEvents.BasicAgentCallback onEnemyDetection;
    GameEvents.BasicNotifactionEvent onAllClear;

    private int allClearCount = 0;
    private int maximummAllClearCount = 120;

    private  List<ICyberAgent> agentList;

    public static string[] layerMaskNames ={"FullCOverObsticles","HalfCoverObsticles"};
    
    private RaycastHit hit;

    float closestDistance;

    ICyberAgent targetAgent;

    FakeMovingAgent m_fakeAgent;

    private float VISUAL_CONE_ANGLE = 150;

    private float VISUAL_DISTANCE = 20;

    private float MINIMUM_CLOSE_SENSTIVITY_DISTANCE = 2;

    private bool normalUpdate = false;


    public HumanoidAgentBasicVisualSensor(ICyberAgent agent) : base(agent)
    {
        m_agent = (HumanoidMovingAgent)agent;
        agentList = new List<ICyberAgent>();
        m_fakeAgent = new FakeMovingAgent(Vector3.zero);
    }

    
    protected override void onSensorUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(m_agent.getCurrentPosition(), VISUAL_DISTANCE);
        closestDistance = float.MaxValue;
        ICyberAgent previousAgent = targetAgent;
        targetAgent = null;

        foreach(Collider hitCollider in hitColliders)
        {
                switch (hitCollider.tag)
                {
                    case "Chest":
                        onHumanAgentDetected(hitCollider.GetComponentInParent<HumanoidMovingAgent>());
                        onDroneDetected(hitCollider.GetComponentInParent<FlyingAgent>());
                        break;
                    case "Wall":
                        break;
                    case "Cover":
                        break;
                    case "Floor":
                       break;
                }
        }

        // Check for all clear
        if(targetAgent == null)
        {

            allClearCount++;   
                   
            if(allClearCount > maximummAllClearCount || (agentList.Count == 0 && allClearCount > maximummAllClearCount/20))
            {
                //Debug.Log("wait countdown " +allClearCount);
                //Debug.Log("Agent Count " + agentList.Count);
                allClearCount = 0;
                onAllClear();
            }
            else if(previousAgent != null)
            {
                m_fakeAgent.moveCharacter(previousAgent.getTopPosition());
                m_fakeAgent.setActualAgent(previousAgent);
                onEnemyDetection(m_fakeAgent);
            }
        }
        else
        {
            if(!agentList.Contains(targetAgent))
            {
                //Debug.Log("Added " + targetAgent.getTransfrom().name);
                agentList.Add(targetAgent);
                //Debug.Log("Agent List cound when added " + agentList.Count);
                targetAgent.setOnDestoryCallback(onEnemeyDestoryed);
            }
            
            onEnemyDetection(targetAgent);
        }
    }

    public override void forceUpdateSneosr()
    {
        VISUAL_CONE_ANGLE = 360;
        VISUAL_DISTANCE = 40;
        normalUpdate = false;
        onSensorUpdate();
        VISUAL_CONE_ANGLE = 85;
        VISUAL_DISTANCE = 20;
        normalUpdate = true;
    }
    
    public void forceGussedTargetLocation(Vector3 position)
    {
        // If there is no current target, set the given location as the new target location.
        if(targetAgent == null)
        {
            m_fakeAgent.moveCharacter(position);
            onEnemyDetection(m_fakeAgent);
        }
    }

    private void onHumanAgentDetected(HumanoidMovingAgent detectedAgent)
    {
        if(detectedAgent != null)
        {
            if(detectedAgent !=null && detectedAgent.IsFunctional() && !CommonFunctions.isAllies(detectedAgent,m_agent))
            {
                Vector3 direction = (detectedAgent.getCurrentPosition() -  m_agent.getCurrentPosition());
                float distance = direction.magnitude;
                direction = direction.normalized;

                if( Vector3.Angle(direction,m_agent.getGameObject().transform.forward)< VISUAL_CONE_ANGLE && distance < closestDistance && detectedAgent.IsFunctional() && isVisibleHumanoid(detectedAgent) )
                { 
                    closestDistance = distance;
                    targetAgent = detectedAgent;  
                }
            }           
        }
       //HumanoidMovingAgent detectedAgent =  collider.GetComponentInParent<HumanoidMovingAgent>();
    }

    private void onDroneDetected(FlyingAgent detectedAgent)
    {
        if(detectedAgent !=null)
        {
            if(detectedAgent !=null && detectedAgent.IsFunctional() && !CommonFunctions.isAllies(detectedAgent,m_agent))
            {
                Vector3 direction = (detectedAgent.getCurrentPosition() -  m_agent.getCurrentPosition());
                float distance = direction.magnitude;
                direction = direction.normalized;

                if( Vector3.Angle(direction,m_agent.getGameObject().transform.forward)< VISUAL_CONE_ANGLE && distance < closestDistance && detectedAgent.IsFunctional())
                { 
                    closestDistance = distance;
                    targetAgent = detectedAgent;  
                }
            }           
        }
    }

    private bool isVisibleHumanoid(HumanoidMovingAgent detectedAgent)
    {
        Vector3 direction =  (m_agent.getCurrentPosition() - detectedAgent.getCurrentPosition());
        float distance = direction.magnitude;
        
        if(distance > MINIMUM_CLOSE_SENSTIVITY_DISTANCE && detectedAgent.isCrouched() && !detectedAgent.isAimed())
        {
            direction = direction.normalized;
            if (Physics.Raycast(detectedAgent.getCurrentPosition() + Vector3.up*0.5f,direction, out hit, 3, LayerMask.GetMask(layerMaskNames)))
            {
                return false;
            }
        }
        return true;
    }

    public void setOnEnemyDetectionEvent(GameEvents.BasicAgentCallback callback)
    {
        onEnemyDetection = callback;
    }

    public void setOnAllClear(GameEvents.BasicNotifactionEvent onAllClear)
    {
        this.onAllClear = onAllClear;
    }

    public void onEnemeyDestoryed()
    {
        
        if(agentList.Count > 0)
        {
            //Debug.Log( "Removed "+ agentList[agentList.Count-1].getTransfrom().name);
            agentList.RemoveAt(agentList.Count-1);
        }
    }
}
