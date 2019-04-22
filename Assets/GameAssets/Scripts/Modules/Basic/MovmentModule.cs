using UnityEngine;

public class MovmentModule
{
    public enum BASIC_MOVMENT_STATE { DIRECTIONAL_MOVMENT,AIMED_MOVMENT}

    protected BASIC_MOVMENT_STATE m_movmentType = BASIC_MOVMENT_STATE.DIRECTIONAL_MOVMENT;
    protected GameObject m_target;
    protected Transform m_characterTransform;

    public MovmentModule(GameObject target, Transform characterTransfrom)
    {
        m_target = target;
        m_characterTransform = characterTransfrom;
    }

    public virtual void UpdateMovment(int characterMovmentState, Vector3 movmentDirection)
    {
        m_movmentType = (BASIC_MOVMENT_STATE)characterMovmentState;

        switch (m_movmentType)
        {
            case BASIC_MOVMENT_STATE.AIMED_MOVMENT:
                if (m_target != null)
                {
                    m_characterTransform.LookAt(m_target.transform.position);
                }
                break;
            case BASIC_MOVMENT_STATE.DIRECTIONAL_MOVMENT:
                break;
        }
    }

    public virtual void setTarget(GameObject target)
    {
        m_target = target;
    }
}
