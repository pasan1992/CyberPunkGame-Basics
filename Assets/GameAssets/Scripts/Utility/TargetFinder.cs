using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour {

    private List<ICyberAgent>  targets = new List<ICyberAgent>();
    PlayerControllerMobile m_mobilePlayerController;
    private bool changeOccured = true;
    ICyberAgent m_currentTarget = null;
    private string ownersName;

    private void Awake()
    {
        m_mobilePlayerController = this.GetComponentInParent<PlayerControllerMobile>();
        ownersName = m_mobilePlayerController.name;

        MovingAgent[] agents = GameObject.FindObjectsOfType<MovingAgent>();

        FlyingAgent[] flyingAgents = GameObject.FindObjectsOfType<FlyingAgent>();

        foreach (MovingAgent agent in agents)
        {
            targets.Add(agent);
        }

        foreach (FlyingAgent agent in flyingAgents)
        {
            targets.Add(agent);
        }
    }

    #region Event Handlers

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.transform.tag =="Enemy")
        {
            ICyberAgent agent = other.GetComponentInParent<ICyberAgent>();
            if(agent.getName() != ownersName)
            {
                Debug.Log(agent.getName() + "entered");
                targets.Add(agent);
            }

            changeOccured = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Enemy")
        {
            targets.Remove(other.GetComponentInParent<ICyberAgent>());
            changeOccured = true;
        }
    }

    public void OnEnable()
    {
        if(m_mobilePlayerController == null)
        {
            m_mobilePlayerController = this.GetComponentInParent<PlayerControllerMobile>();
        }

        m_mobilePlayerController.setTargetFinder(this);
        changeOccured = true;
        ownersName = m_mobilePlayerController.name;
    }
    #endregion

    public ICyberAgent getCurrentTarget(Vector3 position, Vector3 targetPositon)
    {
        if(changeOccured)
        {
            ICyberAgent currentTarget = null;
            float minAngle = 999;

            foreach (ICyberAgent target in targets)
            {
                //float distance = Vector3.Distance(position, target.getCurrentPosition());
                float angle = Vector3.Angle(target.getCurrentPosition() - position, targetPositon - position);
                if (angle < minAngle && target.IsFunctional() && target.getName() != ownersName)
                {
                    minAngle = angle;
                    currentTarget = target;
                }
            }

            if (currentTarget != null)
            {
                m_currentTarget = currentTarget;
                return currentTarget;
            }
            else
            {
                m_currentTarget = null;
                return null;
            }
        }
        else
        {
            if(m_currentTarget !=null && m_currentTarget.IsFunctional())
            {
                return m_currentTarget;
            }
            else
            {
                m_currentTarget = null;
                return null;
            }

        }

    }
}
