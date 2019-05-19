using UnityEngine;

public class PrimaryWeapon : Weapon
{
    protected float burstFireInterval;

    #region command

    public override void pullTrigger()
    {
        if(!weaponSafty)
        {
            base.pullTrigger();
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
        m_audioScource.PlayOneShot(m_soundManager.getLaserRifalAudioClip());
    }
    #endregion

    #region updates

    public override void updateWeapon()
    {
        base.updateWeapon();

        if(triggerPulled && getAmmoCount() > 0)
        {
            updateContinouseFire();
        }
    }

    private void updateContinouseFire()
    {
        burstFireInterval += Time.deltaTime;

        if (burstFireInterval > (1 / fireRate))
        {
            burstFireInterval = 0;
            fireWeapon();

            if(this.isActiveAndEnabled)
            {
                StartCoroutine(waitAndRecoil());
            }

        }
    }
    #endregion

    #region getters and setters
    public override WEAPONTYPE getWeaponType()
    {
        return WEAPONTYPE.primary;
    }
    #endregion
}
