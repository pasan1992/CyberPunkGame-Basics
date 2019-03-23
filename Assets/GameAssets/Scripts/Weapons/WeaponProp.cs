using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProp : MonoBehaviour
{
    public bool PropEnabled;
    public Weapon.WEAPONTYPE m_propType;

    public void setVisible(bool state)
    {
        this.gameObject.SetActive(state);
    }

    public Weapon.WEAPONTYPE getPropType()
    {
        return m_propType;
    }
}
