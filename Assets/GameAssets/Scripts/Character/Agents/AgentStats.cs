using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class AgentStats
{

    public enum AGENT_NATURE { DROID,DRONE,PLAYER,HUMANOID}

    [Header("Non functional behavior Parameters")]
    [Tooltip("This governs non functional behavior of the unity Ex:- Post destory effects")]
    public AGENT_NATURE AgentNature;
}
