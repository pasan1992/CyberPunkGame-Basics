using UnityEngine;
using RootMotion.FinalIK;

public class HumanoidAnimationModule : AnimationModule
{
    // Start is called before the first frame update
    protected AimIK m_aimIK;
    protected float m_aimSpeed = 10;

    public HumanoidAnimationModule(Animator animator, AimIK m_aimIK,float AimSpeed):base(animator)
    {
        this.m_aimIK = m_aimIK;
        m_aimSpeed = AimSpeed;
    }

    #region updates

    public void UpdateAnimationState(MovingAgent.CharacterMainStates state)
    {
        switch (state)
        {
            case MovingAgent.CharacterMainStates.Aimed:
                m_aimIK.solver.IKPositionWeight = Mathf.Lerp(m_aimIK.solver.IKPositionWeight, 1, Time.deltaTime * m_aimSpeed/2);
                break;
            case MovingAgent.CharacterMainStates.Armed_not_Aimed:
                m_aimIK.solver.IKPositionWeight = Mathf.Lerp(m_aimIK.solver.IKPositionWeight, 0, Time.deltaTime * m_aimSpeed);
                break;
            case MovingAgent.CharacterMainStates.Idle:
                m_aimIK.solver.IKPositionWeight = Mathf.Lerp(m_aimIK.solver.IKPositionWeight, 0, Time.deltaTime * m_aimSpeed);
                break;
        }
    }

    #endregion

    #region commands

    public MovingAgent.CharacterMainStates unEquipEquipment()
    {
        m_animator.SetBool("equip", false);
        return MovingAgent.CharacterMainStates.Idle;
    }

    public MovingAgent.CharacterMainStates toggleEquip()
    {
        bool state = isEquiped();
        state = !state;
        m_animator.SetBool("equip", state);

        if (state)
        {
            return MovingAgent.CharacterMainStates.Armed_not_Aimed;
        }
        else
        {
            return MovingAgent.CharacterMainStates.Idle;
        }
    }

    public MovingAgent.CharacterMainStates equipCurrentEquipment()
    {
        m_animator.SetBool("equip", true);
        return MovingAgent.CharacterMainStates.Armed_not_Aimed;
    }

    public MovingAgent.CharacterMainStates fastEquipCurrentEquipment()
    {
        m_animator.SetBool("equip", true);
        return MovingAgent.CharacterMainStates.Armed_not_Aimed;
    }

    public bool isEquiped()
    {
        return m_animator.GetBool("equip");
    }

    public override void setMovment(float forward,float side)
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
        return ( m_aimIK.solver.GetIKPositionWeight() > 0.3f);
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


    public void setCurretnWeapon(int value)
    {
        m_animator.SetFloat("currentWeapon", value);
    }

    public void triggerDodge()
    {
        m_animator.SetTrigger("dodge");
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
