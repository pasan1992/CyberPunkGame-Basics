using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentParameters : MonoBehaviour
{
    //public enum TypeOfAgent { droid,drone,player,humanoid}
   // public TypeOfAgent AgentType;

    public Dictionary<string,int> weaponAmmoCount;

    void Awake()
    {
        weaponAmmoCount = new Dictionary<string,int>();
        weaponAmmoCount.Add("Pistol",40);
        weaponAmmoCount.Add("Rifle",200);
    }
}
