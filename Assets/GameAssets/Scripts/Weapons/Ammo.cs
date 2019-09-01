using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo :MonoBehaviour
{
    public enum AMMO_TYPE { Primary, Secondary }
    public AMMO_TYPE m_ammoType;
    public int count;
    public bool destory = false;

    public AMMO_TYPE getAmmoType()
    {
        return m_ammoType;
    }

    public void setAmmoType(AMMO_TYPE type)
    {
        m_ammoType = type;
    }
}
