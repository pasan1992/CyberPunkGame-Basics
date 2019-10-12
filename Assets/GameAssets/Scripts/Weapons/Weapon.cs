using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Interactable
{   
    
    #region Initialize
    public enum WEAPONTYPE { primary,secondary,grenede,Common};

    [System.Serializable]
    public class WeaponNonFunctionalProperties
    {
        public Vector3 magazinePositionOffset;
        public GameObject magazineObjProp;

        public Vector3 handPlacementOffset;

        public Vector3 weaponRecoilOffset;     
    }

    public WeaponNonFunctionalProperties nonFunctionalProperties;

    protected AgentBasicData.AgentFaction m_ownersFaction;
    protected bool m_isAimed = false;
    protected bool weaponSafty = false;

    protected GameObject m_target;

    #endregion
    
    #region Update
    public virtual void updateWeapon()
    {
    }
    #endregion

    #region Commands
    public virtual void dropWeapon()
    {
        
    }

    public virtual void resetWeapon()
    {

    }
    #endregion


    #region Getters and Setters
    public virtual WEAPONTYPE getWeaponType()
    {
        return WEAPONTYPE.Common;
    }

    public virtual void setWeaponSafty(bool enabled)
    {
        weaponSafty = enabled;
    }
    public virtual void setAimed(bool aimed)
    {
        m_isAimed = aimed;
    }

    public virtual void setGunTarget(GameObject target)
    {
        this.m_target = target;
    }

    public virtual void setOwnerFaction(AgentBasicData.AgentFaction owner)
    {
        m_ownersFaction = owner;
    }
    #endregion

    #region Events
    public virtual void onWeaponEquip()
    {

    }
    #endregion

    #region Utility
    #endregion

}
