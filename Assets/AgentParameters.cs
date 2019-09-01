using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentParameters : MonoBehaviour
{
    public enum TypeOfController { droid,drone,player,humanoid}
    public TypeOfController AgentType;

    public Dictionary<string,int> weaponAmmoCount;

    void Awake()
    {
        weaponAmmoCount = new Dictionary<string,int>();
        weaponAmmoCount.Add("Pistol",40);
        weaponAmmoCount.Add("Rifle",200);
    }
}
