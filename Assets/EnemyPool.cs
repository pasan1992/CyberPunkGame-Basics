using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    // Start is called before the first frame update
    AutoDroneController[] drones;
    AutoHumanoidAgentController[] droids;
    SpawnPoint[] spawnPoints;

    private int agentCount;

    void Awake()
    {
        droids = this.GetComponentsInChildren<AutoHumanoidAgentController>();
        drones = this.GetComponentsInChildren<AutoDroneController>();
        spawnPoints = FindObjectsOfType<SpawnPoint>();

        foreach (var droid in droids)
        {
            droid.addOnDestroyEvent(OnAgentDestory);
        }

        foreach (var drone in drones)
        {
            drone.addOnDestroyEvent(OnAgentDestory);
        }
    }

    public void OnAgentDestory()
    {
        agentCount--;

        //if(agentCount == 0)
        //{
        //    foreach(AutoDroneController drone in drones)
        //    {

        //    }
        //}
    }
}
