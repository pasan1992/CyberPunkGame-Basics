using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder {

    private float FIRE_DISTANCE = 15;
    private float FIRE_SENSITIVITY = 0.5f;

    private List<ICyberAgent>  m_targets = new List<ICyberAgent>();
    private string m_selfName;
    private Vector3 m_selfPosition;
    private ICyberAgent m_currentTarget;
    private float m_aimedDirectionMagnitue;
    private Vector3 m_aimedPosition;
    private GameObject m_targetIndicator;

    #region Initialize
    public TargetFinder(string ownersName,Vector3 selfPosition,GameObject targetIndicator)
    {
        m_selfName = ownersName;
        m_selfPosition = selfPosition;
        m_targetIndicator = targetIndicator;

        MovingAgent[] agents = GameObject.FindObjectsOfType<MovingAgent>();

        FlyingAgent[] flyingAgents = GameObject.FindObjectsOfType<FlyingAgent>();

        foreach (MovingAgent agent in agents)
        {
            m_targets.Add(agent);
        }

        foreach (FlyingAgent agent in flyingAgents)
        {
            m_targets.Add(agent);
        }
    }
    #endregion

    #region Update

    public void updateTargetFinder(Vector3 aimDirection,Vector3 selfPosition)
    {
        m_selfPosition = selfPosition;
        m_aimedDirectionMagnitue = aimDirection.magnitude;
        Vector3 currentAimedPosition = getAimedPositionFromDirection(aimDirection);

        ICyberAgent tempAgent =  getNearestTargetAgent(currentAimedPosition);

        // Only Update the current target if the aim Position changed.
        if (currentAimedPosition != m_aimedPosition)
        {
            m_currentTarget = tempAgent;
        }

        m_aimedPosition = currentAimedPosition;
    }

    #endregion

    #region Getters and Setters

    public Vector3 getCalculatedTargetPosition()
    {
        if(m_currentTarget != null)
        {
            return getTargetPositionFromAgent();
        }
        else
        {
            return m_aimedPosition;
        }
    }

    public bool canFireAtTargetAgent()
    {     
        if (m_aimedDirectionMagnitue > FIRE_SENSITIVITY && m_currentTarget != null)
        {
           float distanceFromTarget = Vector3.Distance(m_selfPosition, m_currentTarget.getTransfrom().position);
           if(distanceFromTarget < FIRE_DISTANCE)
           {
                return true;
           }
           return false;
        }
        return false;
    }

    public void addTarget(ICyberAgent cyberAgent)
    {
        m_targets.Add(cyberAgent);
    }
    #endregion


    #region Event Handlers
    #endregion

    #region Utility

    /**
     * Get the aimed position of the weapon.
     */
    private Vector3 getAimedPositionFromDirection(Vector3 aimedDirection)
    {
        return m_selfPosition + aimedDirection.normalized * 2 + new Vector3(0, 1.24f, 0);
    }

    /**
     * Get the target agent closes to the aimed postion. ( interms of the angle)
     */
    private ICyberAgent getNearestTargetAgent(Vector3 aimedPosition)
    {

        ICyberAgent tempAgent = null;
        float minAngle = 999;

        foreach (ICyberAgent target in m_targets)
        {
            float angle = Vector3.Angle(target.getCurrentPosition() - m_selfPosition, aimedPosition - m_selfPosition);

            if (angle < minAngle && angle < 50 && target.IsFunctional() && target.getName() != m_selfName)
            {
                minAngle = angle;
                tempAgent = target;
            }
        }

        return tempAgent;
    }

    /**
     * Get target position of the current aimed agent
     */ 
    private Vector3 getTargetPositionFromAgent()
    {
        MovingAgent humanoidAgent = m_currentTarget as MovingAgent;

        if (humanoidAgent == null)
        {
            return m_currentTarget.getTopPosition();
        }
        else
        {
            if (humanoidAgent.isCrouched())
            {
                if (humanoidAgent.isAimed())
                {
                    return m_currentTarget.getTopPosition();
                }
                else
                {
                    return m_currentTarget.getCurrentPosition() + new Vector3(0, 0.6f, 0);
                }
            }
            else
            {
                return m_currentTarget.getCurrentPosition() + new Vector3(0, 1.2f, 0);
            }
        }
    }
    #endregion
}
