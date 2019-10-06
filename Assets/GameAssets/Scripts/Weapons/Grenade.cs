using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Weapon
{
    protected override void playWeaponFireSound()
    {
        throw new System.NotImplementedException();
    }

    public override WEAPONTYPE getWeaponType()
    {
        return WEAPONTYPE.grenede;
    }
}
