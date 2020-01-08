using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAgentBasicVisualSensor : AgentBasicSensor
{
    HumanoidMovingAgent m_agent;
    GameEvents.BasicAgentCallback onEnemyDetection;
    GameEvents.BasicNotifactionEvent onAllClear;

    private int allClearCount = 0;
    private int maximummAllClearCount = 500;

    private  List<ICyberAgent> agentList;

    public static string[] layerMaskNames ={"FullCOverObsticles","HalfCoverObsticles"};
    
    private RaycastHit hit;

    float closestDistance;

    ICyberAgent targetAgent;

    FakeMovingAgent m_fakeAgent;

    private float visualConeAngle = 85;

    private float visualDistnace = 12;

    private bool normalUpdate = false;


    public HumanoidAgentBasicVisualSensor(ICyberAgent agent) : base(agent)
    {
        m_agent = (HumanoidMovingAgent)agent;
        agentList = new List<ICyberAgent>();
        m_fakeAgent = new FakeMovingAgent(Vector3.zero);
    }

    
    protected override void onSensorUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(m_agent.getCurrentPosition(), visualDistnace);
        agentList.Clear();
        closestDistance = float.MaxValue;
        ICyberAgent previousAgent = targetAgent;
        targetAgent = null;

        foreach(Collider hitCollider in hitColliders)
        {
                switch (hitCollider.tag)
                {
                    case "Chest":
                        onHumanAgentDetected(hitCollider);
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
            if(normalUpdate)
            {
                allClearCount++;
            }
            
            if(allClearCount > maximummAllClearCount || (agentList.Count == 0 && allClearCount > maximummAllClearCount/20))
            {
                Debug.Log(allClearCount);
                allClearCount = 0;
                onAllClear();
            }
            else if(previousAgent != null)
            {
                m_fakeAgent.moveCharacter(previousAgent.getTopPosition());
                onEnemyDetection(m_fakeAgent);
            }
        }
        else
        {
            if(!agentList.Contains(targetAgent))
            {
                agentList.Add(targetAgent);
                targetAgent.setOnDestoryCallback(onEnemeyDestoryed);
            }
            
            onEnemyDetection(targetAgent);
        }
    }

    public override void forceUpdateSneosr()
    {
        visualConeAngle = 360;
        visualDistnace = 20;
        normalUpdate = false;
        onSensorUpdate();
        visualConeAngle = 85;
        visualDistnace = 12;
        normalUpdate = true;
    }

    private void onHumanAgentDetected(Collider collider)
    {
       HumanoidMovingAgent detectedAgent =  collider.GetComponentInParent<HumanoidMovingAgent>();

       if(detectedAgent !=null && detectedAgent.IsFunctional() && !CommonFunctions.isAllies(detectedAgent,m_agent))
       {
            Vector3 direction = (detectedAgent.getCurrentPosition() -  m_agent.getCurrentPosition());
            float distance = direction.magnitude;
            direction = direction.normalized;

            if( Vector3.Angle(direction,m_agent.getGameObject().transform.forward)< visualConeAngle && distance < closestDistance && detectedAgent.IsFunctional() && isVisible(detectedAgent) )
            { 
                closestDistance = distance;
                targetAgent = detectedAgent;  
            }
       }
    }

    private bool isVisible(HumanoidMovingAgent detectedAgent)
    {
        Vector3 direction =  (m_agent.getCurrentPosition() - detectedAgent.getCurrentPosition()).normalized;
        if(detectedAgent.isCrouched() && !detectedAgent.isAimed())
        {
            if (Physics.Raycast(detectedAgent.getCurrentPosition() + Vector3.up*0.5f,direction, out hit, 10, LayerMask.GetMask(layerMaskNames)))
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
            agentList.RemoveAt(agentList.Count-1);
        }
    }
}
