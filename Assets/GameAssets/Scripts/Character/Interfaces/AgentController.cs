
using UnityEngine;

public abstract class AgentController : MonoBehaviour
{
    
    protected void intializeAgentCallbacks(ICyberAgent cyberAgent)
    {
        cyberAgent.setOnDestoryCallback(OnAgentDestroy);
        cyberAgent.setOnDisableCallback(onAgentDisable);
        cyberAgent.setOnEnableCallback(onAgentEnable);
    }

    public delegate void agentBasicCallbackDeligate();

    public enum AgentFaction { Player,Enemy,Neutral};
    public AgentFaction m_agentFaction;
        
    public abstract void setMovableAgent(ICyberAgent agent);
    public abstract float getSkill();
    public abstract ICyberAgent getICyberAgent();
    public abstract void OnAgentDestroy();
    public abstract void onAgentDisable();
    public abstract void onAgentEnable();
}
