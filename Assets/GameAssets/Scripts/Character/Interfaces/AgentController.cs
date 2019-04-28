
using UnityEngine;

public abstract class AgentController : MonoBehaviour
{
    public enum AgentFaction { Player,Enemy,Neutral};
    public AgentFaction m_agentFaction;
        
    public abstract void setMovableAgent(ICyberAgent agent);
    public abstract float getSkill();
    public abstract ICyberAgent getICyberAgent();
}
