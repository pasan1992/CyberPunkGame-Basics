﻿using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class SecondaryWeapon : Weapon
{
    private float timeFromLastFire;
    private bool waitToFire = false;

    public void Awake()
    {
        base.Awake();
        properties.Type = InteractableProperties.InteractableType.SecondaryWeapon;
    }

    #region command

    public override void pullTrigger()
    {
        if(!weaponSafty)
        {
            base.pullTrigger();
            if (!waitToFire)
            {
                fireWeapon();
                waitToFire = true;
            }
        }

    }

    public override void releaseTrigger()
    {
        base.releaseTrigger();
    }

    public override void dropWeapon()
    {
        base.dropWeapon();
        triggerPulled = false;
    }

    protected override void playWeaponFireSound()
    {
        m_audioScource.PlayOneShot(m_soundManager.getLaserPistolAudioClip());
    }
    #endregion

    #region updates

    public override void updateWeapon()
    {
        base.updateWeapon();

        if(waitToFire)
        {
            timeFromLastFire += Time.deltaTime;

            if ( (1 / fireRate) <= timeFromLastFire)
            {
                waitToFire = false;
                timeFromLastFire = 0;
            }
        }
    }
    #endregion

    #region getters and setters

    public override WEAPONTYPE getWeaponType()
    {
        return WEAPONTYPE.secondary;
    }

    #endregion
}
