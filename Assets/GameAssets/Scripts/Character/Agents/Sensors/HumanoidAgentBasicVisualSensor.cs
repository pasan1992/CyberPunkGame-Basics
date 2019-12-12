using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAgentBasicVisualSensor : AgentBasicSensor
{
    HumanoidMovingAgent m_agent;
    GameEvents.BasicAgentCallback onEnemyDetection;
    GameEvents.BasicNotifactionEvent onAllClear;

    private bool noAgentsNearby = true;

    private int allClearCount = 0;
    private int maximummAllClearCount = 5;

    public HumanoidAgentBasicVisualSensor(ICyberAgent agent) : base(agent)
    {
        m_agent = (HumanoidMovingAgent)agent;
    }

    protected override void onSensorUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(m_agent.getCurrentPosition(), 15);
        noAgentsNearby = true;
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
        if(noAgentsNearby)
        {
            allClearCount++;
            if(allClearCount > maximummAllClearCount)
            {
                allClearCount = 0;
                onAllClear();
            }
        }
    }

    private void onHumanAgentDetected(Collider collider)
    {
       HumanoidMovingAgent detectedAgent =  collider.GetComponentInParent<HumanoidMovingAgent>();
       //HumanoidDamagableObject detectedAgentDamageModule = collider.GetComponentInParent<HumanoidDamagableObject>();

       if(detectedAgent !=null && detectedAgent.IsFunctional() && !CommonFunctions.isAllies(detectedAgent,m_agent))
       {
            Vector3 direction = (detectedAgent.getCurrentPosition() -  m_agent.getCurrentPosition()).normalized;

            if(Vector3.Angle(direction,m_agent.getGameObject().transform.forward)< 85)
            {
                onEnemyDetection(detectedAgent);        
            }
            noAgentsNearby = false;
       }
      

    }

    public void setOnEnemyDetectionEvent(GameEvents.BasicAgentCallback callback)
    {
        onEnemyDetection = callback;
    }

    public void setOnAllClear(GameEvents.BasicNotifactionEvent onAllClear)
    {
        this.onAllClear = onAllClear;
    }
}
