using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InteractionModule : MonoBehaviour
{
    private HumanoidMovingAgent m_movingAgent;
    private AudioSource audioSocurce;

    private void Start()
    {
        m_movingAgent = this.GetComponentInParent<HumanoidMovingAgent>();
        audioSocurce = this.GetComponent<AudioSource>();
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

            audioSocurce.Play();
            if (ammo.destory)
            {
                Destroy(ammo.gameObject);
            }
            else
            {
                other.gameObject.SetActive(false);
            }

        }
    }
}
