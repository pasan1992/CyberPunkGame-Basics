using UnityEngine;

public class PrimaryWeapon : Weapon
{
    protected float burstFireInterval;

    #region command

    public override void pullTrigger()
    {
        base.pullTrigger();
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
    #endregion

    #region updates

    public override void updateWeapon()
    {
        base.updateWeapon();

        if(triggerPulled)
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
            StartCoroutine(waitAndRecoil());
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
