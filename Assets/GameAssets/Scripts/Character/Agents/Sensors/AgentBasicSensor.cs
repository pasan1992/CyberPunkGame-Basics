using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBasicSensor
{
    private float m_updateStepInterval = 0.25f;
    private float m_timeFromLastStep = 0;

    public float StepInterval
    {
        get {return m_updateStepInterval;}
        set {
                if(value > 0)
                {
                    m_updateStepInterval = value;
                }
            }
    }

    public AgentBasicSensor(ICyberAgent agent)
    {
    }

    public void UpdateSensor()
    {
        if(m_timeFromLastStep > m_updateStepInterval)
        {
            m_timeFromLastStep = 0;
            onSensorUpdate();
        }
        m_timeFromLastStep += Time.deltaTime;
    }   

    protected virtual void onSensorUpdate()
    {

    } 

    public virtual void forceUpdateSneosr()
    {
        onSensorUpdate();
    }
}
