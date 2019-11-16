using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidInteractionModule
{

    public delegate void OnInteractionOver();

    private OnInteractionOver m_onInteractionOver;

    private HumanoidAnimationModule m_animationModule;
    private HumanoidMovmentModule m_movementModule;
    private AgentData m_agentData;
    private HumanoidRangedWeaponsModule m_equipmentModule;
    private bool m_interacting;
    private bool m_unCancleableInstruction;
    private Interactable m_currentInteractingObject;

    public HumanoidInteractionModule(HumanoidAnimationModule animationModule, 
    HumanoidMovmentModule movmenetModule,
    AgentData agentData,
    HumanoidRangedWeaponsModule equipmentModule,
    OnInteractionOver onInteractionOver)
    {
        m_animationModule = animationModule;
        m_movementModule = movmenetModule;
        m_agentData = agentData;
        m_equipmentModule = equipmentModule;
        m_onInteractionOver = onInteractionOver;
        m_unCancleableInstruction = false;
        m_interacting = false;
    }

    public IEnumerator interactWith(Interactable interactable,Interactable.InteractableProperties.InteractableType type)
    {
        if(!m_interacting)
        {
            m_interacting = true;
            m_currentInteractingObject = interactable;
            m_currentInteractingObject.interact();
            switch(type)
            {
                case Interactable.InteractableProperties.InteractableType.PickupInteraction:

                    float distance = Vector3.Distance(m_movementModule.getCharacterTransfrom().position,interactable.transform.position);
                    
                    if(distance>0.7f)
                    {
                        m_movementModule.LookAtObject(interactable.transform.position);
                    }

                    m_animationModule.triggerPickup();
                    
                    if(m_movementModule.isCrouched())
                    {
                        yield return onPickup(interactable,0);
                    }
                    else
                    {
                        yield return onPickup(interactable,0.3f);
                    }
                
                break;
                case Interactable.InteractableProperties.InteractableType.FastInteraction:
                break;
                case Interactable.InteractableProperties.InteractableType.TimedInteraction:
                    yield return onTimedInteraction(interactable);
                    break;
                case Interactable.InteractableProperties.InteractableType.ContinousInteraction:
                    yield return onContinousInteraction(interactable);
                break;
            }
            m_interacting = false;
            m_onInteractionOver();
        }
        else
        {
            cancleInteraction();
        }


        yield return null;
    }

    public void cancleInteraction()
    {
        if(!m_unCancleableInstruction)
        {
            m_interacting = false;
            m_animationModule.setInteraction(false,0);
            m_onInteractionOver();
            m_currentInteractingObject.stopInteraction();
        }
    }

    private IEnumerator onPickup(Interactable obj,float waitTime)
    {
        m_unCancleableInstruction = true;
        m_animationModule.setUpperAnimationLayerWeight(0.2f);
        yield return new WaitForSeconds(waitTime);
        m_animationModule.setUpperAnimationLayerWeight(1);

        if(obj.properties.Type.Equals(Interactable.InteractableProperties.InteractableType.PickupInteraction))
        {
            if(obj is RangedWeapon)
            {
                if(obj is PrimaryWeapon)
                {
                    if(!m_agentData.primaryWeapon)
                    {
                        m_agentData.primaryWeapon = obj.GetComponent<PrimaryWeapon>();
                        m_equipmentModule.equipWeapon(m_agentData.primaryWeapon);
                        obj.OnEquipAction();
                    }
                    else
                    {
                        m_agentData.inventryItems.Add(obj);
                        obj.OnPickUpAction();
                    }
                }
                else if(obj is SecondaryWeapon)
                {
                    if(!m_agentData.secondaryWeapon)
                    {
                        m_agentData.secondaryWeapon = obj.GetComponent<SecondaryWeapon>();
                        m_equipmentModule.equipWeapon(m_agentData.secondaryWeapon);
                        obj.OnEquipAction();
                    }
                    else
                    {
                        m_agentData.inventryItems.Add(obj);
                        obj.OnPickUpAction();
                    }
                }
            }
        }
         yield return new WaitForSeconds(waitTime);
         m_unCancleableInstruction = false;
    }

    private IEnumerator onContinousInteraction(Interactable interactableObj)
    {
        Vector3 intendedPosition = interactableObj.transform.position + interactableObj.properties.offset;
        Quaternion intentedRotation =  Quaternion.Euler(interactableObj.properties.rotation);
        
        Transform transform = m_movementModule.getCharacterTransfrom();
        while((Vector3.Distance(transform.position,intendedPosition) > 0.3f ||
         Mathf.Abs(intentedRotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 5f) &&
         m_interacting)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,intentedRotation,0.2f);
            transform.position = Vector3.Lerp(transform.position,intendedPosition,0.1f);
            m_animationModule.setMovment(0,0);

            yield return new WaitForSeconds(Time.deltaTime/2);
        }     

        if(m_interacting)
        {
            m_animationModule.setInteraction(true,interactableObj.properties.interactionID);

            while(m_interacting)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }            
        }  
    }

    private IEnumerator onTimedInteraction(Interactable interactableObj)
    {

        Vector3 intendedPosition = interactableObj.transform.position + interactableObj.properties.offset;
        Quaternion intentedRotation =  Quaternion.Euler(interactableObj.properties.rotation);
        
        Transform transform = m_movementModule.getCharacterTransfrom();
        while((Vector3.Distance(transform.position,intendedPosition) > 0.3f || 
        Mathf.Abs(intentedRotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 5f) &&
        m_interacting)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,intentedRotation,0.2f);
            transform.position = Vector3.Lerp(transform.position,intendedPosition,0.1f);
            m_animationModule.setMovment(0,0);

            yield return new WaitForSeconds(Time.deltaTime/2);
        }

        if(m_interacting)
        {
            m_animationModule.setInteraction(true,interactableObj.properties.interactionID);
            float waitTime = interactableObj.properties.interactionTime;
            float currentWaitedTime = 0;

            while(waitTime > currentWaitedTime && m_interacting)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                currentWaitedTime += Time.deltaTime;
            }

            if(m_interacting)
            {
                cancleInteraction();
            }        
        }
    }
}
