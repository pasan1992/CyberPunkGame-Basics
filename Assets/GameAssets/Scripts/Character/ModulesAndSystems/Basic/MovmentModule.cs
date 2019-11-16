using UnityEngine;

public class MovmentModule
{
    public enum BASIC_MOVMENT_STATE { DIRECTIONAL_MOVMENT,AIMED_MOVMENT}

    protected BASIC_MOVMENT_STATE m_movmentType = BASIC_MOVMENT_STATE.DIRECTIONAL_MOVMENT;
    protected GameObject m_target;
    protected Transform m_characterTransform;
    protected Vector3 m_lookPosition;

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
                    Vector3 lookPosition = new Vector3(m_target.transform.position.x, this.m_characterTransform.position.y, m_target.transform.position.z);
                    m_lookPosition = Vector3.Lerp(m_lookPosition, lookPosition, 0.01f);
                    m_characterTransform.LookAt(m_lookPosition, Vector3.up);
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

    public Transform getCharacterTransfrom()
    {
        return m_characterTransform;
    }
}
