using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentEventTrigger : GameEventTrigger
{
    // Start is called before the first frame update
    public ICyberAgent m_agent;

    public string m_aiming = "AIMING";
    public string m_notAiming = "NOT_AIMING";
    public string m_crouch = "CROUCH";
    public string m_stand = "STAND";
    public string m_weaponInHand = "WEAPON_IN_HAND";
    public string m_weaponHosted ="WEAPON_HOSTED";

    void Start()
    {
        if(m_agent == null)
        {
            m_agent = this.GetComponent<ICyberAgent>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_agent.isAimed())
        {
            externalFSM.Fsm.Event(m_aiming);
        }
        else
        {
            externalFSM.Fsm.Event(m_notAiming);
        }

        if(m_agent.isHidden())
        {
            externalFSM.Fsm.Event(m_crouch);
        }
        else
        {
            externalFSM.Fsm.Event(m_stand);
        }

        if(m_agent.isArmed())
        {
            externalFSM.Fsm.Event(m_weaponInHand);
        }
        else
        {
            externalFSM.Fsm.Event(m_weaponHosted);
        }
    }
}
