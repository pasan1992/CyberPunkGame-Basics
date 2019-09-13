using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class AgentData : AgentBasicData
{
    [SerializeField]
    public Dictionary<string,int> weaponAmmoCount;

    public AgentData()
    {
        weaponAmmoCount = new Dictionary<string,int>();
        weaponAmmoCount.Add("Pistol",40);
        weaponAmmoCount.Add("Rifle",200);
    }
}
