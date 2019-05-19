
using UnityEngine;

public abstract class AgentController : MonoBehaviour
{
    protected agentOnDestoryEventDelegate m_onDestoryEvent;
    protected bool inUse = false;
    public float timeToReset = 2;

    protected void intializeAgentCallbacks(ICyberAgent cyberAgent)
    {
        cyberAgent.setOnDestoryCallback(OnAgentDestroy);
        cyberAgent.setOnDisableCallback(onAgentDisable);
        cyberAgent.setOnEnableCallback(onAgentEnable);
    }

    public delegate void agentBasicEventDelegate();
    public delegate void agentOnDestoryEventDelegate(AgentController controller);

    public enum AgentFaction { Player,Enemy,Neutral};
    public AgentFaction m_agentFaction;
        
    public abstract void setMovableAgent(ICyberAgent agent);
    public abstract float getSkill();
    public abstract ICyberAgent getICyberAgent();

    public virtual void OnAgentDestroy()
    {
        if(m_onDestoryEvent !=null)
        {
            m_onDestoryEvent(this);
        }
        //this.gameObject.SetActive(true);
        //this.transform.position = new Vector3(0, -11, 0);
    }
    public abstract void onAgentDisable();
    public abstract void onAgentEnable();
    public abstract void resetCharacher();

    public void addOnDestroyEvent(agentOnDestoryEventDelegate onDestoryCallback)
    {
        m_onDestoryEvent = onDestoryCallback;
    }

    public void resetControlAgent()
    {
        //this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public bool isInUse()
    {
        return inUse;
    }

    public void setInUse(bool used)
    {
        inUse = used;
    }
}
