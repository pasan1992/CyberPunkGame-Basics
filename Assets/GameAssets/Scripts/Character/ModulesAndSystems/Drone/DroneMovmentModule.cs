using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovmentModule : MovmentModule
{
    // Start is called before the first frame update
    private Transform m_modelTransfrom;
    private Vector3 m_landingPosition;
    private GameEnums.DroneState m_currentDroneState;
    private Vector3 m_hoverPosition;
    private AnimationModule m_animationSystem;

    public DroneMovmentModule(GameObject target,
    Transform characterTransfrom,
    GameEnums.DroneState state,
    Transform modelTransfrom,
    AnimationModule animationSystem):base(target,characterTransfrom)
    {
        m_target = target;
        m_characterTransform = characterTransfrom;
        m_modelTransfrom = modelTransfrom;
        m_currentDroneState = state;
        m_animationSystem = animationSystem;
    }

    public override void UpdateMovment(int characterMovmentState, Vector3 movmentDirection)
    {
        Debug.Log("Drone movment module does not use update movment, use update drone movment instead");
        //base.UpdateMovment(characterMovmentState, movmentDirection);
    }

    public void UpdateMovment(int characterMovmentState, Vector3 movmentDirection,GameEnums.DroneState currentState,out GameEnums.DroneState state)
    {
        // Same state
        state = currentState;

        switch(state)
        {
            case GameEnums.DroneState.Disabled:
            break;
            case GameEnums.DroneState.Flying:
                base.UpdateMovment(characterMovmentState,movmentDirection);
            break;
            case GameEnums.DroneState.Landed:
            break;
            case GameEnums.DroneState.Landing:
                updateLanding(currentState,out state);
            break;
            case GameEnums.DroneState.Recovering:
            break;
            case GameEnums.DroneState.TakeOff:
                updateTakeOff(currentState,out state);
            break;
        }
    }

    private void updateTakeOff(GameEnums.DroneState currentState,out GameEnums.DroneState state)
    {
        state = currentState;
        if(m_currentDroneState != GameEnums.DroneState.TakeOff)
        {
            m_currentDroneState = state;
             m_animationSystem.enableAnimationSystem();
        }

        if((Vector3.Distance(m_modelTransfrom.position,m_hoverPosition) > 0.3f))
        {
            state = GameEnums.DroneState.TakeOff;
            m_modelTransfrom.position = Vector3.Lerp(m_modelTransfrom.position,m_hoverPosition,0.1f);
        }
        // Landed
        else
        {
            state = GameEnums.DroneState.Flying;
        }
    }
    private void updateLanding(GameEnums.DroneState currentState,out GameEnums.DroneState state)
    {
        state = currentState;

        if(m_currentDroneState != GameEnums.DroneState.Landing)
        {
            m_currentDroneState = state;
            m_hoverPosition = m_modelTransfrom.position;
        }

        // Still Landing
        if((Vector3.Distance(m_modelTransfrom.position,m_landingPosition) > 0.3f))
        {
            state = GameEnums.DroneState.Landing;
            m_modelTransfrom.position = Vector3.Lerp(m_modelTransfrom.position,m_landingPosition,0.1f);
        }
        // Landed
        else
        {

            state = GameEnums.DroneState.Landed;
            m_animationSystem.disableAnimationSystem();
        }

        // while((Vector3.Distance(transform.position,intendedPosition) > 0.3f || 
        // Mathf.Abs(intentedRotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 5f) &&
        // m_interacting && interactableObj.properties.enablePositionRequirment)
        // {
        //     transform.rotation = Quaternion.Lerp(transform.rotation,intentedRotation,0.2f);
        //     transform.position = Vector3.Lerp(transform.position,intendedPosition,0.1f);
        //     m_animationModule.setMovment(0,0);

        //     // To enable smooth transistion from staring positon and rotation to end position and rotation.
        //     yield return new WaitForSeconds(Time.deltaTime/2);
        // }
    }

    public void setLandPosition(Vector3 position)
    {
        m_landingPosition = position;
    }
}
