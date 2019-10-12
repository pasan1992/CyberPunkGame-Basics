using UnityEngine;


public class PrimaryWeapon : RangedWeapon
{
    protected float burstFireInterval;

    #region command

    public void Awake()
    {
        base.Awake();
        properties.Type = InteractableProperties.InteractableType.PrimaryWeapon;
    }

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

        if(triggerPulled)
        {
            updateContinouseFire();
        }
        else
        {
            
        }
    }

    private void updateContinouseFire()
    {
        burstFireInterval += Time.deltaTime;

        if (burstFireInterval > (1 / fireRate))
        {
            burstFireInterval = 0;
            fireWeapon();

            if(this.isActiveAndEnabled && getAmmoCount() >0)
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
