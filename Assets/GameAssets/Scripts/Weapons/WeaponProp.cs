using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProp : MonoBehaviour
{
    public enum WeaponLocation {HOSTER_PRIMARY,HOSTER_SECONDAY,HAND}
    public WeaponLocation weaponLocationType;
    public bool PropEnabled;
    //public Weapon.WEAPONTYPE m_propType;

    public void setVisible(bool state)
    {
        this.gameObject.SetActive(state);
    }

    public WeaponLocation getPropType()
    {
        return weaponLocationType;
    }
}
