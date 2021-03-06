﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

namespace humanoid
{
    public class HumanoidEquipmentModule
    {
        #region protectedParameters
        protected Weapon m_currentWeapon;

        protected Weapon m_rifle;
        protected Weapon m_pistol;
        protected WeaponProp m_rifleProp;
        protected WeaponProp m_pistolProp;
        protected GameObject m_target;
        protected Recoil m_recoil;
        protected MovingAgent.CharacterMainStates m_currentState;
        protected HumanoidAnimationModule m_animationSystem;
        protected AgentController.AgentFaction m_ownersFaction;

        private bool m_inEquipingAction = false;

        private AgentParameters m_agentParameters;
        #endregion

        public HumanoidEquipmentModule(Weapon[] weapons, 
        WeaponProp[] props, 
        MovingAgent.CharacterMainStates state, 
        GameObject target, 
        Recoil recoil, 
        HumanoidAnimationModule animSystem,
        AgentParameters parameters)
        {
            m_currentState = state;
            m_target = target;
            m_recoil = recoil;
            m_animationSystem = animSystem;
            getAllWeapons(weapons, props);
            m_agentParameters = parameters;
        }


        #region updates
        public void UpdateSystem(MovingAgent.CharacterMainStates state)
        {
            if (m_currentWeapon != null)
            {
                m_currentWeapon.updateWeapon();
            }


            // On Character state change.
            switch (state)
            {
                case MovingAgent.CharacterMainStates.Aimed:

                    // Set Gun Aimed;
                    if (!m_currentState.Equals(MovingAgent.CharacterMainStates.Aimed))
                    {
                        aimCurrentEquipment(true);
                    }

                    break;
                case MovingAgent.CharacterMainStates.Armed_not_Aimed:

                    // Set Gun Aimed;
                    if (!m_currentState.Equals(MovingAgent.CharacterMainStates.Armed_not_Aimed))
                    {

                        aimCurrentEquipment(false);
                    }
                    break;
                case MovingAgent.CharacterMainStates.Idle:
                    if (!m_currentState.Equals(MovingAgent.CharacterMainStates.Idle))
                    {
                        aimCurrentEquipment(false);
                    }
                    break;
            }


            m_currentState = state;
        }
        #endregion

        #region Event handlers

        public void ReloadEnd()
        {
            if(m_currentWeapon !=null)
            {
                Debug.Log("Reload End");
                m_currentWeapon.setReloading(false);
                int totalAmmo = 0;
                m_agentParameters.weaponAmmoCount.TryGetValue(m_currentWeapon.name,out totalAmmo);
                
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

            switch (type)
            {
                case Weapon.WEAPONTYPE.primary:
                    // Select rifle as currentWeapon
                    //  Debug.Log("Primary Equip finished");
                    m_rifleProp.setVisible(false);
                    m_currentWeapon = m_rifle;
                    break;

                case Weapon.WEAPONTYPE.secondary:
                    // Select pistol as currentWeapon
                    // Debug.Log("Secondary Equip finished");
                    m_pistolProp.setVisible(false);
                    m_currentWeapon = m_pistol;
                    break;
            }

            // Set Current Weapon Properties.
            m_currentWeapon.gameObject.SetActive(true);
            m_currentWeapon.setGunTarget(m_target);
        }

        // UnEquip Animation event.
        public void UnEquipAnimationEvent()
        {
            Weapon.WEAPONTYPE type = m_currentWeapon.getWeaponType();
            m_currentWeapon.gameObject.SetActive(false);
            m_currentWeapon = null;
            m_inEquipingAction = false;

            switch (type)
            {
                case Weapon.WEAPONTYPE.primary:
                    m_rifleProp.setVisible(true);
                    // Debug.Log("Primary Unequip Finished");
                    break;

                case Weapon.WEAPONTYPE.secondary:
                    m_pistolProp.setVisible(true);
                    //Debug.Log("Secondary Unequip Finished");
                    break;
            }
        }

        public void onWeaponFire(float weight)
        {
            m_recoil.Fire(weight);
        }
        #endregion

        #region commands

        public void pullTrigger()
        {
            if (m_currentWeapon)
            {
                m_currentWeapon.pullTrigger();
            }
        }

        public void releaseTrigger()
        {
            if (m_currentWeapon)
            {
                m_currentWeapon.releaseTrigger();
            }
        }

        public void DropCurrentWeapon()
        {
            if (m_currentWeapon)
            {
                m_currentWeapon.dropWeapon();
            }
        }


        //public MovingAgent.CharacterMainStates equipCurrentEquipment()
        //{   
        //   return m_animationSystem.equipCurrentEquipment();
        //}

        //public MovingAgent.CharacterMainStates unEquipCurrentEquipment()
        //{
        //   return m_animationSystem.unEquipEquipment();
        //}

        private void aimCurrentEquipment(bool aimed)
        {
            m_animationSystem.aimEquipment(aimed);

            if (getCurrentWeapon())
            {
                getCurrentWeapon().setAimed(aimed);
            }

        }

        public MovingAgent.CharacterMainStates togglePrimary()
        {

            if (!m_inEquipingAction)
            {
                //Debug.Log("Toggle Primary Start");
                m_animationSystem.setCurretnWeapon(1);

                if (m_currentWeapon != null)
                {
                    if (m_currentWeapon.getWeaponType().Equals(Weapon.WEAPONTYPE.primary))
                    {
                        m_inEquipingAction = true;
                        return m_animationSystem.unEquipEquipment();
                    }
                    else
                    {
                        // Fast toggle
                        m_currentWeapon = m_rifle;
                        m_pistol.gameObject.SetActive(false);
                        m_rifle.gameObject.SetActive(true);
                        m_rifleProp.setVisible(false);
                        m_pistolProp.setVisible(true);
                        m_animationSystem.fastEquipCurrentEquipment();

                        if (m_currentState.Equals(MovingAgent.CharacterMainStates.Aimed))
                        {
                            m_currentWeapon.setAimed(true);
                        }

                        return m_currentState;
                    }
                }
                else
                {
                    m_inEquipingAction = true;
                    m_currentWeapon = m_rifle;
                    return m_animationSystem.equipCurrentEquipment();
                }
            }
            else
            {
                return m_currentState;
            }

        }

        public MovingAgent.CharacterMainStates toggleSecondary()
        {

            if (!m_inEquipingAction)
            {
                //Debug.Log("Toggle Secondary Start");
                m_animationSystem.setCurretnWeapon(0);

                if (m_currentWeapon != null)
                {
                    if (m_currentWeapon.getWeaponType().Equals(Weapon.WEAPONTYPE.secondary))
                    {
                        m_inEquipingAction = true;
                        return m_animationSystem.unEquipEquipment();
                    }
                    else
                    {
                        // Fast toggle
                        m_currentWeapon = m_pistol;
                        m_rifle.gameObject.SetActive(false);
                        m_rifleProp.setVisible(true);
                        m_pistolProp.setVisible(false);
                        m_pistol.gameObject.SetActive(true);
                        m_animationSystem.fastEquipCurrentEquipment();

                        if (m_currentState.Equals(MovingAgent.CharacterMainStates.Aimed))
                        {
                            m_currentWeapon.setAimed(true);
                        }

                        return m_currentState;
                    }
                }
                else
                {
                    m_inEquipingAction = true;
                    m_currentWeapon = m_pistol;
                    return m_animationSystem.equipCurrentEquipment();
                }
            }
            else
            {
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
            m_currentWeapon.setOwnerFaction(m_ownersFaction);
        }

        public void setCurretnWeaponProp(WeaponProp weaponProp)
        {
            this.m_pistolProp = weaponProp;
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

        private void getAllWeapons(Weapon[] weapons, WeaponProp[] props)
        {
            foreach (Weapon wep in weapons)
            {
                wep.setOwnerFaction(m_ownersFaction);
                wep.setAimed(false);
                wep.setGunTarget(m_target);
                wep.gameObject.SetActive(false);
                wep.addOnWeaponFireEvent(onWeaponFire);

                Weapon.WEAPONTYPE type = wep.getWeaponType();

                switch (type)
                {
                    case Weapon.WEAPONTYPE.primary:
                        m_rifle = wep;
                        break;

                    case Weapon.WEAPONTYPE.secondary:
                        m_pistol = wep;
                        break;
                }
            }

            foreach (WeaponProp prop in props)
            {
                Weapon.WEAPONTYPE type = prop.getPropType();

                switch (type)
                {
                    case Weapon.WEAPONTYPE.primary:
                        m_rifleProp = prop;
                        break;

                    case Weapon.WEAPONTYPE.secondary:
                        m_pistolProp = prop;
                        break;
                }
            }
        }

        public bool isInEquipingAction()
        {
            return m_inEquipingAction;
        }

        public void setOwnerFaction(AgentController.AgentFaction agentGroup)
        {
            this.m_ownersFaction = agentGroup;
            m_rifle.setOwnerFaction(agentGroup);
            m_pistol.setOwnerFaction(agentGroup);
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

        public void reloadCurretnWeapon()
        {
            if(m_currentWeapon !=null)
            {
                m_currentWeapon.reloadWeapon();
                m_animationSystem.triggerReload();
            }
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

        #endregion
    }
}


