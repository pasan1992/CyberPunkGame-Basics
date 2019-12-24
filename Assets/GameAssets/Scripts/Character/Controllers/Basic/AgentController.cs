
using UnityEngine;

public abstract class AgentController : MonoBehaviour
{
    protected agentOnDestoryEventDelegate m_onDestoryEvent;

    public bool m_inUse = false;
    public float m_timeToReset = 2;

    protected void intializeAgentCallbacks(ICyberAgent cyberAgent)
    {
        cyberAgent.setOnDestoryCallback(OnAgentDestroy);
        cyberAgent.setOnDisableCallback(onAgentDisable);
        cyberAgent.setOnEnableCallback(onAgentEnable);
    }

    //public delegate void agentBasicEventDelegate();
    public delegate void agentOnDestoryEventDelegate(AgentController controller);

    // public enum AgentFaction { Player,Enemy,Neutral};
    // public AgentFaction m_agentFaction;
        
    public abstract void setMovableAgent(ICyberAgent agent);
    public abstract float getSkill();
    public abstract ICyberAgent getICyberAgent();

    public virtual void OnAgentDestroy()
    {
        if(m_onDestoryEvent !=null)
        {
            m_onDestoryEvent(this);
        }
    }
    public abstract void onAgentDisable();
    public abstract void onAgentEnable();
    public abstract void resetCharacher();
    public abstract void setPosition(Vector3 postion);

    public void addOnDestroyEvent(agentOnDestoryEventDelegate onDestoryCallback)
    {
        m_onDestoryEvent = onDestoryCallback;
    }

    public void resetControlAgent()
    {
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public bool isInUse()
    {
        return m_inUse;
    }

    public void setInUse(bool used)
    {
        m_inUse = used;
    }
}
