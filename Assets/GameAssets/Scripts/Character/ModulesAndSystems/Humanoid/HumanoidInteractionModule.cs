using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanoidInteractionModule
{

    public delegate void OnInteractionOver();

    private OnInteractionOver m_onInteractionOver;

    private HumanoidAnimationModule m_animationModule;
    private HumanoidMovmentModule m_movementModule;
    private AgentData m_agentData;
    private HumanoidRangedWeaponsModule m_equipmentModule;

    private NavMeshAgent m_navMesAgent;

    /**
     This bool variable make sure that when interaction cancled, 
     all the Coroutines started by the Humanoid Agent realated to interaction will not continue the interaction.
    */
    private bool m_interacting;
    private bool m_unCancleableInstruction;
    private Interactable m_currentInteractingObject;

    public HumanoidInteractionModule(HumanoidAnimationModule animationModule, 
    HumanoidMovmentModule movmenetModule,
    AgentData agentData,
    HumanoidRangedWeaponsModule equipmentModule,
    NavMeshAgent agent,
    OnInteractionOver onInteractionOver)
    {
        m_animationModule = animationModule;
        m_movementModule = movmenetModule;
        m_agentData = agentData;
        m_equipmentModule = equipmentModule;

        // This is the callback of the HumanoidMovingAgent to return the character state into normal state
        m_onInteractionOver = onInteractionOver;
        m_unCancleableInstruction = false;
        m_interacting = false;
        m_navMesAgent = agent;
    }

    public IEnumerator interactWith(Interactable interactable,Interactable.InteractableProperties.InteractableType type)
    {
        if(!m_interacting)
        {
            // Set all required varialbes and conditions for the interaction
            m_interacting = true;
            m_navMesAgent.enabled = false;
            m_currentInteractingObject = interactable;
            m_currentInteractingObject.interact();

            switch(type)
            {
                case Interactable.InteractableProperties.InteractableType.PickupInteraction:

                    float distance = Vector3.Distance(m_movementModule.getCharacterTransfrom().position,interactable.transform.position);
                    
                    // Make sure the character turns at the object - if the character is very close no need to turn.
                    if(distance>1f)
                    {
                        Vector3 lookPoistion = new Vector3(interactable.transform.position.x,m_movementModule.getCharacterTransfrom().position.y,interactable.transform.position.z);
                        m_movementModule.LookAtObject(lookPoistion);
                    }

                    // Check if need to bend
                    int interactionID = -1;
                    if(Mathf.Abs(interactable.transform.position.y - m_movementModule.getCharacterTransfrom().position.y) > 0.5f)
                    {
                        interactionID = -2;
                    }
                    m_animationModule.triggerPickup(interactionID);
                    
                    // Is the agent is crouched no animation is played when picking up items, Thus no wait time
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

            // Make sure the character state return to previous state.
            if(m_interacting)
            {
                // Only reach this with uncancalalbe instructions -on pick up automaticaly return to animation, no need to cancle
                m_interacting = false;
                m_onInteractionOver();
                m_navMesAgent.enabled = true;
            }

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
            m_navMesAgent.enabled = true;
        }
    }

    private IEnumerator onPickup(Interactable obj,float waitTime)
    {
        // fast interactions are uncancellable - once started need to wait until they are ended.
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

    /**
     Continous interactions will continue unless they are cancled
    */
    private IEnumerator onContinousInteraction(Interactable interactableObj)
    {
        Vector3 intendedPosition = interactableObj.transform.position + interactableObj.properties.offset;
        Quaternion intentedRotation =  Quaternion.Euler(interactableObj.properties.rotation);
        
        Transform transform = m_movementModule.getCharacterTransfrom();

        // Place agent in the intaraction position.
        while((Vector3.Distance(transform.position,intendedPosition) > 0.3f ||
         Mathf.Abs(intentedRotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 5f) &&
         m_interacting)
        {

            if(Vector3.Distance(transform.position,intendedPosition) > 0.3f)
            {
                Debug.Log("position mission");
            }
            


            if(Mathf.Abs(intentedRotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 5f)
            {
                Debug.Log("angle missing");
            }

            transform.rotation = Quaternion.Lerp(transform.rotation,intentedRotation,0.2f);
            transform.position = Vector3.Lerp(transform.position,intendedPosition,0.1f);
            m_animationModule.setMovment(0,0);

            // To enable smooth transistion from staring positon and rotation to end position and rotation.
            yield return new WaitForSeconds(Time.deltaTime/2);
        }     

        /** Make sure that agent is still in interaction mode before continuing the interaction 
            If interaction is cancled in mid process this condition will fail
         */
        if(m_interacting)
        {
            m_animationModule.setInteraction(true,interactableObj.properties.interactionID);

            // Wait until intereaction is cancled.
            while(m_interacting)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }            
        }  
    }

    /**
       Interaction will end after a certain time.
    */
    private IEnumerator onTimedInteraction(Interactable interactableObj)
    {

        Vector3 intendedPosition = interactableObj.transform.position + interactableObj.properties.offset;
        Quaternion intentedRotation =  Quaternion.Euler(interactableObj.properties.rotation);
        Transform transform = m_movementModule.getCharacterTransfrom();

        // Place agent in the interaction position
        while((Vector3.Distance(transform.position,intendedPosition) > 0.3f || 
        Mathf.Abs(intentedRotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 5f) &&
        m_interacting && interactableObj.properties.enablePositionRequirment)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,intentedRotation,0.2f);
            transform.position = Vector3.Lerp(transform.position,intendedPosition,0.1f);
            m_animationModule.setMovment(0,0);

            // To enable smooth transistion from staring positon and rotation to end position and rotation.
            yield return new WaitForSeconds(Time.deltaTime/2);
        }

        /** Make sure that agent is still in interaction mode before continuing the interaction 
            If interaction is cancled in mid process this condition will fail
         */
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

            // After every wait, need to check the if the interaction is still in progress before continuing.
            if(m_interacting)
            {
                cancleInteraction();
            }        
        }
    }
}
