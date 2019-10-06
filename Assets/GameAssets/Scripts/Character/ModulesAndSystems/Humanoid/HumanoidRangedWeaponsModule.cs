using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
public class HumanoidRangedWeaponsModule
{
    #region protectedParameters

    // Weapon Related properties
    protected Weapon m_currentWeapon;
    protected Weapon m_rifle;
    protected Weapon m_pistol;
    protected Weapon m_grenede;

    protected GameObject primaryHosterLocation;
    protected GameObject secondaryHosterLocation;
    protected GameObject weaponHoldLocation;

    protected AgentData m_agentData;

    protected GameObject m_target;
    protected Recoil m_recoil;
    protected HumanoidMovingAgent.CharacterMainStates m_currentState;
    protected HumanoidAnimationModule m_animationSystem;

    // Functional related properties
    private bool m_inEquipingAction = false;
    private bool m_inWeaponAction = false;

    private enum WeaponSystemSubStages {
        Armed 
        ,Equiping 
        ,UnEquiping
        ,Reloading
        ,WeaponAction
        }
    private WeaponSystemSubStages m_currentWeaponSubStage;
    
    #endregion

    public HumanoidRangedWeaponsModule( 
    WeaponProp[] props, 
    HumanoidMovingAgent.CharacterMainStates state, 
    GameObject target, 
    Recoil recoil, 
    HumanoidAnimationModule animSystem,
    AgentData agentData)
    {
        m_currentState = state;
        m_target = target;
        m_recoil = recoil;
        m_animationSystem = animSystem;
        m_agentData = agentData;
        getAllWeapons(props);
    }

    private void UpdateWeaponSubStage()
    {
        if(m_animationSystem.checkCurrentAnimationTag("Armed"))
        {
            m_currentWeaponSubStage = WeaponSystemSubStages.Armed;
        }
        else if(m_animationSystem.checkCurrentAnimationTag("Equip"))
        {
            m_currentWeaponSubStage = WeaponSystemSubStages.Equiping;
        }
        else if(m_animationSystem.checkCurrentAnimationTag("UnEquip"))
        {
            m_currentWeaponSubStage = WeaponSystemSubStages.UnEquiping;
        }
        else if(m_animationSystem.checkCurrentAnimationTag("Reload"))
        {
            m_currentWeaponSubStage = WeaponSystemSubStages.Reloading;
        }
        else if(m_animationSystem.checkCurrentAnimationTag("WeaponAction"))
        {
            m_currentWeaponSubStage = WeaponSystemSubStages.WeaponAction;
        }

        switch (m_currentWeaponSubStage)
        {
            case WeaponSystemSubStages.Armed:
                m_inWeaponAction = false;
            break;
            case WeaponSystemSubStages.Equiping:
            break;
            case WeaponSystemSubStages.Reloading:
            break;
            case WeaponSystemSubStages.UnEquiping:
            break;
            case WeaponSystemSubStages.WeaponAction:
                m_inWeaponAction = true;
            break;
        }
    }

    #region updates
    public void UpdateSystem(HumanoidMovingAgent.CharacterMainStates state)
    {
        UpdateWeaponSubStage();

        if (m_currentWeapon != null)
        {
            m_currentWeapon.updateWeapon();
        }

        // On Character state change.
        switch (state)
        {
            case HumanoidMovingAgent.CharacterMainStates.Aimed:

                // Set Gun Aimed;
                if (!m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Aimed))
                {
                    aimCurrentEquipment(true);
                }

                break;
            case HumanoidMovingAgent.CharacterMainStates.Armed_not_Aimed:

                // Set Gun Aimed;
                if (!m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Armed_not_Aimed))
                {

                    aimCurrentEquipment(false);
                }
                break;
            case HumanoidMovingAgent.CharacterMainStates.Idle:

                // Set one time only
                if (!m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Idle))
                {
                    aimCurrentEquipment(false);
                }
                break;
            default:
                if (m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Armed_not_Aimed) || m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Aimed))
                {
                    aimCurrentEquipment(false);
                }
                break;
        }


        m_currentState = state;
    }
    #endregion

    #region Event handlers

    public void OnThrow()
    {
        //m_inWeaponAction = false;
    }
    public void ReloadEnd()
    {
        if(m_currentWeapon !=null)
        {
            m_currentWeapon.setReloading(false);
            int totalAmmo = 0;
            m_agentData.weaponAmmoCount.TryGetValue(m_currentWeapon.weaponName,out totalAmmo);
            
            // Enought Ammo available
            if(totalAmmo > m_currentWeapon.m_magazineSize)
            {
                totalAmmo -=m_currentWeapon.m_magazineSize;
                //  m_agentParameters.weaponAmmoCount.(m_currentWeapon.name,totalAmmo);
                m_currentWeapon.setAmmoCount(m_currentWeapon.m_magazineSize);
                return;
            }

            // Not enough ammo
            m_currentWeapon.setAmmoCount(totalAmmo);
            totalAmmo = 0;
            return;
        }
    }

    // Equip Animation event.
    public void EquipAnimationEvent()
    {
        Weapon.WEAPONTYPE type = m_currentWeapon.getWeaponType();
        m_inEquipingAction = false;

        // switch (type)
        // {
        //     case Weapon.WEAPONTYPE.primary:
        //         // Select rifle as currentWeapon
        //         //  Debug.Log("Primary Equip finished");
        //         //m_rifleProp.setVisible(false);    
        //         m_currentWeapon = m_rifle;
        //         break;

        //     case Weapon.WEAPONTYPE.secondary:
        //         // Select pistol as currentWeapon
        //         // Debug.Log("Secondary Equip finished");
        //         //m_pistolProp.setVisible(false);
        //         m_currentWeapon = m_pistol;
        //         break;
        // }
        placeWeaponInHand(m_currentWeapon);

        // Set Current Weapon Properties.
        m_currentWeapon.gameObject.SetActive(true);
        m_currentWeapon.setGunTarget(m_target);
    }

    // UnEquip Animation event.
    public void UnEquipAnimationEvent()
    {
        Weapon.WEAPONTYPE type = m_currentWeapon.getWeaponType();
        //m_currentWeapon.gameObject.SetActive(false);
        m_currentWeapon = null;
        m_inEquipingAction = false;

        switch (type)
        {
            case Weapon.WEAPONTYPE.primary:
                
                //m_rifleProp.setVisible(true);
                //placePrimaryWeaponInHosterLocation();
                // Debug.Log("Primary Unequip Finished");
                placeWeaponinHosterLocation(m_rifle);
                break;

            case Weapon.WEAPONTYPE.secondary:
                //m_pistolProp.setVisible(true);
                placeWeaponinHosterLocation(m_pistol);
                //placeSecondaryWeaponInHosterLocation();
                //Debug.Log("Secondary Unequip Finished");
                break;
        }
    }

    public void OnWeaponFire(float weight)
    {
        m_recoil.Fire(weight);
    }
    #endregion

    #region commands

    public void pullTrigger()
    {
        if (isProperlyAimed() && m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Aimed) && m_currentWeapon)
        {
            m_currentWeapon.pullTrigger();

            if(m_currentWeapon.GetType().Equals(typeof(Grenade)))
            {
                m_inWeaponAction = true;
            }
        }
    }

    public void releaseTrigger()
    {
        if (m_currentWeapon)
        {
            m_currentWeapon.releaseTrigger();

            if(m_currentWeapon.GetType().Equals(typeof(Grenade)))
            {
                m_animationSystem.triggerThrow();
                m_inWeaponAction = true;
            }
        }
    }

    public void DropCurrentWeapon()
    {
        if (m_currentWeapon)
        {
            m_currentWeapon.dropWeapon();
        }
    }

    private void aimCurrentEquipment(bool aimed)
    {
        m_animationSystem.aimEquipment(aimed);

        if (getCurrentWeapon())
        {
            getCurrentWeapon().setAimed(aimed);
        }

    }

    public void reloadCurretnWeapon()
    {
        if(m_currentWeapon !=null && !isReloading() && !m_inWeaponAction && !m_currentWeapon.GetType().Equals(typeof(Grenade)))
        {
            m_currentWeapon.reloadWeapon();
            m_animationSystem.triggerReload();
        }
    }
    public HumanoidMovingAgent.CharacterMainStates togglePrimary()
    {
        if (!m_inEquipingAction && !isReloading() && m_rifle && !m_inWeaponAction)
        {
            m_animationSystem.setCurretnWeapon(1);

            if (m_currentWeapon != null)
            {
                // UnEquip Weapon With Animation
                if (m_currentWeapon.getWeaponType().Equals(Weapon.WEAPONTYPE.primary))
                {
                    m_inEquipingAction = true;
                    return m_animationSystem.unEquipEquipment();
                }
                // Fast toggle to weapon
                else
                {
                    // Fast toggle
                    placeWeaponinHosterLocation(m_currentWeapon);
                    m_currentWeapon = m_rifle;
                    placeWeaponInHand(m_currentWeapon);
                         
                    m_animationSystem.fastEquipCurrentEquipment();

                    if (m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Aimed))
                    {
                        m_currentWeapon.setAimed(true);
                    }

                    return m_currentState;
                }
            }
            // Equip Weapon with animation
            else
            {
                m_inEquipingAction = true;
                m_currentWeapon = m_rifle;
                return m_animationSystem.equipCurrentEquipment();
            }
        }
        // Not possible to equip weapon - in mid action or weapon not available
        else
        {
            // Weapon not avialable
            if(m_currentWeapon == null && m_rifle == null && !m_inWeaponAction)
            {
                m_animationSystem.triggerShrug();
            }
            return m_currentState;
        }

    }
    public HumanoidMovingAgent.CharacterMainStates toggleSecondary()
    {
        if (!m_inEquipingAction && !isReloading() && m_pistol && !m_inWeaponAction)
        {
            m_animationSystem.setCurretnWeapon(0);

            // Current weapon equiped
            if (m_currentWeapon != null)
            {
                // Unequip weapon with animation
                if (m_currentWeapon.getWeaponType().Equals(Weapon.WEAPONTYPE.secondary))
                {
                    m_inEquipingAction = true;
                    return m_animationSystem.unEquipEquipment();
                }
                // Fast toggle weapon
                else
                {
                    // Fast toggle
                    placeWeaponinHosterLocation(m_currentWeapon);
                    m_currentWeapon = m_pistol;
                    placeWeaponInHand(m_currentWeapon);
                    


                    m_animationSystem.fastEquipCurrentEquipment();

                    if (m_currentState.Equals(HumanoidMovingAgent.CharacterMainStates.Aimed))
                    {
                        m_currentWeapon.setAimed(true);
                    }

                    return m_currentState;
                }
            }
            // Equip weapon with animation
            else
            {
                m_inEquipingAction = true;
                m_currentWeapon = m_pistol;
                return m_animationSystem.equipCurrentEquipment();
            }
        }
        // Unable to equip weapon - no weapon or mid action
        else
        {
            // No weapon avaialble
            if(m_currentWeapon == null && m_pistol == null && !m_inWeaponAction)
            {
                m_animationSystem.triggerShrug();
            }
            return m_currentState;
        }
    }

    public HumanoidMovingAgent.CharacterMainStates toggleGrenede()
    {
       if(!m_inEquipingAction && !isReloading() && m_grenede && !m_inWeaponAction)
       {
           // Current Weapon avaialble
           if(m_currentWeapon != null)
           {
               // Unequip weapon with animation
                if (m_currentWeapon.getWeaponType().Equals(Weapon.WEAPONTYPE.grenede))
                {
                    // Since there are not equiping animation, no need to have m_inEquipingAction enable
                    //m_inEquipingAction = true;
                    placeWeaponinHosterLocation(m_currentWeapon);
                    m_currentWeapon = null;
                    return m_animationSystem.unEquipEquipment();
                }
                // Weapon fast toggle
                else
                {
                    placeWeaponinHosterLocation(m_currentWeapon);
                    m_currentWeapon = m_grenede;
                    placeWeaponInHand(m_currentWeapon);
                    m_animationSystem.setCurretnWeapon(2);
                    m_animationSystem.fastEquipCurrentEquipment();
                    return m_currentState;
                }
           }
           // Equip with animation
           else
           {
                m_animationSystem.setCurretnWeapon(2);
                m_currentWeapon = m_grenede;
                placeWeaponInHand(m_currentWeapon);
                return m_animationSystem.equipCurrentEquipment();
           }
       }
       else
       {
           // Unable to equip or unequip no weapon
            if (m_grenede == null && m_currentWeapon == null && !m_inWeaponAction)
            {
                m_animationSystem.triggerShrug();          
            }
            return m_currentState;
       }     
    } 
    #endregion

    #region Getters And Setters definition

    public bool isProperlyAimed()
    {
        return m_animationSystem.isProperlyAimed();
    }

    public void setCurrentWeapon(Weapon currentWeapon)
    {
        this.m_currentWeapon = currentWeapon;
        m_currentWeapon.setGunTarget(m_target);
        m_currentWeapon.setOwnerFaction(m_agentData.m_agentFaction);
    }

    public void setCurretnWeaponProp(WeaponProp weaponProp)
    {
        //this.m_pistolProp = weaponProp;
    }

    public Weapon getCurrentWeapon()
    {
        return m_currentWeapon;
    }

    public int getCurrentWeaponAmmoCount()
    {
        int count = 0;

        if(m_currentWeapon)
        {
            //m_agentParameters.weaponAmmoCount.TryGetValue(m_currentWeapon.name,out count);
            count =  m_currentWeapon.getAmmoCount();
        }

        return count;
    }

    public void setWeaponTarget(GameObject target)
    {
        m_currentWeapon.setGunTarget(target);
    }

    public GameObject getTarget()
    {
        return m_target;
    }

    public bool isEquiped()
    {
        return m_animationSystem.isEquiped();
    }

    private void getAllWeapons(WeaponProp[] props)
    {       
        foreach (WeaponProp prop in props)
        {
            WeaponProp.WeaponLocation type = prop.getPropType();

            switch (type)
            {
                case WeaponProp.WeaponLocation.HOSTER_PRIMARY:
                    primaryHosterLocation = prop.gameObject;
                    break;

                case WeaponProp.WeaponLocation.HOSTER_SECONDAY:
                    secondaryHosterLocation = prop.gameObject;
                    break;
                case WeaponProp.WeaponLocation.HAND:
                    weaponHoldLocation = prop.gameObject;
                    break;
            }
        }

        if(m_agentData.primaryWeapon)
        {
            m_rifle = m_agentData.primaryWeapon;
            m_rifle.setAimed(false);
            m_rifle.setOwnerFaction(m_agentData.m_agentFaction);
            m_rifle.setGunTarget(m_target);
            m_rifle.addOnWeaponFireEvent(OnWeaponFire);
            //placePrimaryWeaponInHosterLocation();
            placeWeaponinHosterLocation(m_rifle);
        }

        if(m_agentData.secondaryWeapon)
        {
            m_pistol = m_agentData.secondaryWeapon;
            m_pistol.setAimed(false);
            m_pistol.setOwnerFaction(m_agentData.m_agentFaction);
            m_pistol.setGunTarget(m_target);
            m_pistol.addOnWeaponFireEvent(OnWeaponFire);
            placeWeaponinHosterLocation(m_pistol);           
        }

        if(m_agentData.grenade)
        {
            m_grenede = m_agentData.grenade;
        }

        // foreach (Weapon wep in weapons)
        // {
        //     wep.setOwnerFaction(m_ownersFaction);
        //     wep.setAimed(false);
        //     wep.setGunTarget(m_target);
        //     //wep.gameObject.SetActive(false);
        //     wep.addOnWeaponFireEvent(OnWeaponFire);

        //     Weapon.WEAPONTYPE type = wep.getWeaponType();

        //     switch (type)
        //     {
        //         case Weapon.WEAPONTYPE.primary:
        //             m_rifle = wep;
        //             placePrimaryWeaponInHosterLocation();
        //             break;

        //         case Weapon.WEAPONTYPE.secondary:
        //             m_pistol = wep;
        //             placeSecondaryWeaponInHosterLocation();
        //             break;
        //     }
        // }
    }

    public bool isInEquipingAction()
    {
        return m_inEquipingAction;
    }

    public void setOwnerFaction(AgentBasicData.AgentFaction agentGroup)
    {
        m_agentData.m_agentFaction = agentGroup;

        if(m_rifle)
        {
            m_rifle.setOwnerFaction(agentGroup);
        }

        if(m_pistol)
        {
            m_pistol.setOwnerFaction(agentGroup);           
        }
    }

    public void resetWeapon()
    {
        m_currentWeapon.resetWeapon();
    }

    public int getPrimaryWeaponAmmoCount()
    {
        return m_rifle.getAmmoCount();
    }

    public int getSecondaryWeaponAmmoCount()
    {
        return m_pistol.getAmmoCount();
    }

    public void setPrimayWeaponAmmoCount(int count)
    {
        m_rifle.setAmmoCount(count);
    }

    public void setSecondaryWeaponAmmoCount(int count)
    {
        m_pistol.setAmmoCount(count);
    }

    public bool isReloading()
    {
        if(m_currentWeapon !=null)
        {
            return m_currentWeapon.isReloading();
        }
        else
        {
            return false;
        }
    }

    // public void placePrimaryWeaponInHosterLocation()
    // {
    //     m_rifle.transform.parent = primaryHosterLocation.transform;
    //     m_rifle.transform.localPosition = Vector3.zero;
    //     m_rifle.transform.localRotation = Quaternion.identity;
    // }

    public void placeWeaponinHosterLocation(Weapon weapon)
    {
            Transform hosteringLocation = null;
            System.Type weaponType = weapon.GetType();

            if(weaponType == typeof(PrimaryWeapon))
            {
                hosteringLocation = primaryHosterLocation.transform;
            }
            else if (weaponType == typeof(SecondaryWeapon))
            {
                hosteringLocation = secondaryHosterLocation.transform;
            }
            else if(weaponType == typeof(Grenade))
            {
                hosteringLocation = secondaryHosterLocation.transform;
            }
            
            weapon.transform.parent = hosteringLocation;
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
    }

    // public void placeSecondaryWeaponInHosterLocation()
    // {
    //     m_pistol.transform.parent = secondaryHosterLocation.transform;
    //     m_pistol.transform.localPosition = Vector3.zero;
    //     m_pistol.transform.localRotation = Quaternion.identity;
    // }

    public void placeWeaponInHand(Weapon weapon)
    {
        weapon.transform.parent = weaponHoldLocation.transform;
        weapon.transform.localPosition = Vector3.zero + weapon.handPlacementOffset;
        weapon.transform.localRotation = Quaternion.identity;
        m_recoil.handRotationOffset = weapon.weaponRecoilOffset;  
        weapon.transform.localScale = Vector3.one;     
    }

    public void equipWeapon(Weapon weapon)
    {
        bool weaponEquipable = false;
        if(weapon.GetType() == typeof(PrimaryWeapon))
        {
            if(m_rifle)
            {
                m_rifle.dropWeapon();
                m_rifle = null;
            }

            m_rifle = weapon;
            placeWeaponinHosterLocation(weapon);
            m_rifle.onWeaponEquip();
            weaponEquipable = true;
        }
        else if (weapon.GetType() == typeof(SecondaryWeapon))
        {
            if(m_pistol)
            {
                m_pistol.dropWeapon();
                m_pistol = null;
            }

            m_pistol = weapon;
            placeWeaponinHosterLocation(weapon);
            m_pistol.onWeaponEquip();
            weaponEquipable = true;
        }

        if(weaponEquipable)
        {
            weapon.setAimed(false);
            weapon.setOwnerFaction(m_agentData.m_agentFaction);
            weapon.setGunTarget(m_target);
            weapon.addOnWeaponFireEvent(OnWeaponFire);
        }
    }

    #endregion
}