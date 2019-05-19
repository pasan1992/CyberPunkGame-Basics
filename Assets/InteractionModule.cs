using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using humanoid;

public class InteractionModule : MonoBehaviour
{
    private MovingAgent m_movingAgent;

    private void Start()
    {
        m_movingAgent = this.GetComponentInParent<MovingAgent>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ammo")
        {
           Ammo ammo = other.GetComponent<Ammo>();

            switch (ammo.m_ammoType)
            {
                case Ammo.AMMO_TYPE.Primary:
                    m_movingAgent.setPrimayWeaponAmmoCount(m_movingAgent.getPrimaryWeaponAmmoCount() + ammo.count);
                    break;
                case Ammo.AMMO_TYPE.Secondary:
                    m_movingAgent.setSecondaryWeaponAmmoCount(m_movingAgent.getSecondaryWeaponAmmoCount() + ammo.count);
                    break;
                default:
                    break;
            }

            other.gameObject.SetActive(false);
        }
    }
}
