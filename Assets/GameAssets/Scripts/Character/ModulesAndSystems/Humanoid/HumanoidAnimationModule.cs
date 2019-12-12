using UnityEngine;
using RootMotion.FinalIK;


public partial class HumanoidAnimationModule : AnimationModule
{
    // Start is called before the first frame update
    protected AimIK m_aimIK;
    protected float m_aimSpeed = 10;

    protected AgentFunctionalComponents m_functionalComponents;

    public HumanoidAnimationModule(Animator animator, AimIK m_aimIK,AgentFunctionalComponents functionalComponents, float AimSpeed) : base(animator)
    {
        this.m_aimIK = m_aimIK;
        m_aimSpeed = AimSpeed;
        m_functionalComponents = functionalComponents;
    }

    #region updates

    public void UpdateAnimationState(HumanoidMovingAgent.CharacterMainStates state)
    {
        switch (state)
        {
            case HumanoidMovingAgent.CharacterMainStates.Aimed:
                m_aimIK.solver.IKPositionWeight = Mathf.Lerp(m_aimIK.solver.IKPositionWeight, 1, Time.deltaTime * m_aimSpeed / 2);
                break;
            case HumanoidMovingAgent.CharacterMainStates.Armed_not_Aimed:
                m_aimIK.solver.IKPositionWeight = Mathf.Lerp(m_aimIK.solver.IKPositionWeight, 0, Time.deltaTime * m_aimSpeed);
                break;
            case HumanoidMovingAgent.CharacterMainStates.Idle:
                m_aimIK.solver.IKPositionWeight = Mathf.Lerp(m_aimIK.solver.IKPositionWeight, 0, Time.deltaTime * m_aimSpeed);
                break;
            case HumanoidMovingAgent.CharacterMainStates.Dodge:
                m_aimIK.solver.IKPositionWeight = Mathf.Lerp(m_aimIK.solver.IKPositionWeight, 0, Time.deltaTime * m_aimSpeed);
                break;
            default:
                m_aimIK.solver.IKPositionWeight = Mathf.Lerp(m_aimIK.solver.IKPositionWeight, 0, Time.deltaTime * m_aimSpeed);
                break;
        }
    }

    #endregion


    #region commands

    public HumanoidMovingAgent.CharacterMainStates unEquipEquipment()
    {
        m_animator.SetBool("equip", false);
        return HumanoidMovingAgent.CharacterMainStates.Idle;
    }

    HumanoidMovingAgent.CharacterMainStates toggleEquip()
    {
        bool state = isEquiped();
        state = !state;
        m_animator.SetBool("equip", state);

        if (state)
        {
            return HumanoidMovingAgent.CharacterMainStates.Armed_not_Aimed;
        }
        else
        {
            return HumanoidMovingAgent.CharacterMainStates.Idle;
        }
    }

    public HumanoidMovingAgent.CharacterMainStates equipCurrentEquipment()
    {
        m_animator.SetBool("equip", true);
        return HumanoidMovingAgent.CharacterMainStates.Armed_not_Aimed;
    }

    public HumanoidMovingAgent.CharacterMainStates fastEquipCurrentEquipment()
    {
        m_animator.SetBool("equip", true);
        return HumanoidMovingAgent.CharacterMainStates.Armed_not_Aimed;
    }

    public bool isEquiped()
    {
        return m_animator.GetBool("equip");
    }

    public override void setMovment(float forward, float side)
    {
        m_animator.SetFloat("forward", forward);
        m_animator.SetFloat("side", side);
    }

    public void aimEquipment(bool aimed)
    {
        m_animator.SetBool("aimed", aimed);
    }

    public void toggleCrouched()
    {
        bool crouched = isCrouched();
        crouched = !crouched;
        m_animator.SetBool("crouched", crouched);
    }

    public bool isProperlyAimed()
    {
        return (m_aimIK.solver.GetIKPositionWeight() > 0.3f);
    }

    public override void disableAnimationSystem()
    {
        m_animator.enabled = false;
        m_aimIK.enabled = false;
    }

    public override void enableAnimationSystem()
    {
        m_animator.enabled = true;
        m_aimIK.enabled = true;
    }

    public bool checkCurrentAnimationTag(string tagName)
    {
      return  m_animator.GetCurrentAnimatorStateInfo(2).IsTag(tagName);
    }

    public void setUpperAnimationLayerWeight(float weight)
    {
        m_animator.SetLayerWeight(2,weight);
    }
    public void setCurretnWeapon(int value)
    {
        switch (value)
        {
            case 0:
            case 1:
                m_aimIK.solver.transform = m_functionalComponents.weaponAimTransform.transform;
            break;
            case 2:
                m_aimIK.solver.transform = m_functionalComponents.lookAimTransform.transform;
            break;
        }
        m_animator.SetFloat("currentWeapon", value);
    }

    public void triggerReload()
    {
        m_animator.SetTrigger("realoadWeapon");
    }

    public void triggerDodge()
    {
        m_animator.SetTrigger("dodge");
    }

    public void triggerThrow()
    {
        m_animator.SetTrigger("throw");
    }

    public void triggerShrug()
    {
        m_animator.SetTrigger("shrug");
        //m_animator.SetTrigger("throw");
    }

    public void triggerPickup(int id)
    {
        // TODO
        if(!isCrouched())
        {
            m_animator.SetInteger("interactionID",id);
            m_animator.SetTrigger("pickup");
        }
        
    }

    #endregion

    #region gettersAndSetters
    public Animator getAnimator()
    {
        return m_animator;
    }

    public bool isCrouched()
    {
        return m_animator.GetBool("crouched");
    }




    #endregion

    #region Events
    #endregion
}


