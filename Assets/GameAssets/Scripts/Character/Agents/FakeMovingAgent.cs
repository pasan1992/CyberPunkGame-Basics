using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using UnityEngine.AI;

[RequireComponent(typeof(AgentParameters))]
public class FakeMovingAgent : ICyberAgent
{
    GameObject fakeObject;
    ICyberAgent m_atualAgent;

    public FakeMovingAgent(Vector3 position)
    {
        fakeObject = new GameObject();
        fakeObject.transform.position = position;
    }

    public void aimWeapon()
    {
        throw new System.NotImplementedException();
    }

    public void damageAgent(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void dodgeAttack(Vector3 dodgeDirection)
    {
        throw new System.NotImplementedException();
    }

    public void enableTranslateMovment(bool enable)
    {
        throw new System.NotImplementedException();
    }

    public AgentData GetAgentData()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 getCurrentPosition()
    {
        return fakeObject.transform.position;
    }

    public Vector3 getCurrentVelocity()
    {
        return Vector3.zero;
    }

    public AgentBasicData.AgentFaction getFaction()
    {
        throw new System.NotImplementedException();
    }

    public GameObject getGameObject()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 getMovmentDirection()
    {
        throw new System.NotImplementedException();
    }

    public float getSkill()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 getTopPosition()
    {
        return fakeObject.transform.position;
    }

    public Transform getTransfrom()
    {
        return fakeObject.transform;
    }

    public void interactWith(Interactable interactableObj, Interactable.InteractableProperties.InteractableType type)
    {
        throw new System.NotImplementedException();
    }

    public bool isAimed()
    {
       return false;
    }

    public bool isDisabled()
    {
        return false;
    }

    public bool IsFunctional()
    {
        return true;
    }

    public bool isHidden()
    {
        return true;
    }

    public bool isInteracting()
    {
        return false;
    }

    public bool isReadyToAim()
    {
        throw new System.NotImplementedException();
    }

    public void lookAtTarget()
    {
        throw new System.NotImplementedException();
    }

    public void moveCharacter(Vector3 movmentDirection)
    {
        fakeObject.transform.position = movmentDirection;
    }

    public void reactOnHit(Collider collider, Vector3 force, Vector3 point)
    {
        throw new System.NotImplementedException();
    }

    public void resetAgent()
    {
        throw new System.NotImplementedException();
    }

    public void setFaction(AgentBasicData.AgentFaction group)
    {
        throw new System.NotImplementedException();
    }

    public void setOnDestoryCallback(GameEvents.BasicNotifactionEvent callback)
    {
        throw new System.NotImplementedException();
    }

    public void setOnDisableCallback(GameEvents.BasicNotifactionEvent callback)
    {
        throw new System.NotImplementedException();
    }

    public void setOnEnableCallback(GameEvents.BasicNotifactionEvent callback)
    {
        throw new System.NotImplementedException();
    }

    public void setTargetPoint(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public void setWeponFireCapability(bool enadled)
    {
        throw new System.NotImplementedException();
    }

    public void stopAiming()
    {
        throw new System.NotImplementedException();
    }

    public void toggleHide()
    {
        throw new System.NotImplementedException();
    }

    public void weaponFireForAI()
    {
        throw new System.NotImplementedException();
    }

    public void setActualAgent(ICyberAgent actualAgent)
    {
        m_atualAgent = actualAgent;
    }

    public void updatePosition()
    {
        // if(m_atualAgent != null)
        // {
        //     fakeObject.transform.position = m_atualAgent.getCurrentPosition() + new Vector3(Random.value*0.5f,0,Random.value*0.5f);
        // }
    }

    public void setOnDamagedCallback(GameEvents.BasicNotifactionEvent callback)
    {
        Debug.LogError("Not implemented yet");
    }

    public IEnumerator waitTillUnarmed()
    {
        throw new System.NotImplementedException();
    }

    public void cancleInteraction()
    {
        throw new System.NotImplementedException();
    }
}

