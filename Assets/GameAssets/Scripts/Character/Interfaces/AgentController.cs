
using UnityEngine;

public abstract class AgentController : MonoBehaviour
{
    protected agentBasicCallbackDeligate m_onDestoryEvent;

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
    public virtual void OnAgentDestroy()
    {
        if(m_onDestoryEvent !=null)
        {
            m_onDestoryEvent();
        }
    }
    public abstract void onAgentDisable();
    public abstract void onAgentEnable();
    public abstract void resetCharacher();

    public void addOnDestroyEvent(agentBasicCallbackDeligate onDestoryCallback)
    {
        m_onDestoryEvent = onDestoryCallback;
    }
}
