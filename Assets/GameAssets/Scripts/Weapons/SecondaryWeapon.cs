﻿using UnityEngine;

public class SecondaryWeapon : Weapon
{
    private float timeFromLastFire;
    private bool waitToFire = false;
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
